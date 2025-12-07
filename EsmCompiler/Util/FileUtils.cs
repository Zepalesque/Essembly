using System.Numerics;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace EsmCompiler.Util;

public readonly record struct FileCoord(int Line, int Column) : IComparisonOperators<FileCoord, FileCoord, bool> {
    public static implicit operator FileCoord((int line, int column) tuple)
        => new(tuple.line, tuple.column);

    public static bool operator >(FileCoord left, FileCoord right) =>  left.Line > right.Line || !(left.Line < right.Line) && left.Column >  right.Column;

    public static bool operator >=(FileCoord left, FileCoord right) => left.Line > right.Line || !(left.Line < right.Line) && left.Column >= right.Column;

    public static bool operator <(FileCoord left, FileCoord right) =>  left.Line < right.Line || !(left.Line > right.Line) && left.Column <  right.Column;

    public static bool operator <=(FileCoord left, FileCoord right) => left.Line < right.Line || !(left.Line > right.Line) && left.Column <= right.Column;

    
    
    
    public static FileCoord Min(FileCoord left, FileCoord right) => left < right ? left : right;
    public static FileCoord Max(FileCoord left, FileCoord right) => left > right ? left : right;

}

public readonly record struct FilePos(FileCoord Start, FileCoord End, FileData File): IFilePosHolder {
    public static FilePos FromFileIndices(int from, int to, FileData file) {
        FileCoord start = file.Memory.AsSpan().ByIndex(from), end = file.Memory.AsSpan().ByIndex(to);

        return new(start, end, file);
    }

    public FilePos this[Range r] {
        get {
            
            FileSpan span = File.Memory.AsSpan();

            int startIndex = span.ByCoord(Start);
            int endIndex = span.ByCoord(End);

            var  offsLeng = r.GetOffsetAndLength(endIndex - startIndex + 1);

            int newStart = startIndex + offsLeng.Offset, newEnd = newStart + offsLeng.Length - 1;

            if (newStart == startIndex && newEnd == endIndex) return this;

            return FromFileIndices(newStart, newEnd, File);
        }
    }

    public FilePos To(IFilePosHolder? other) => other == null || other.Pos == this ? this : To(other.Pos);
    public FilePos To(FilePos other) => new(FileCoord.Min(Start, other.Pos.Start), FileCoord.Max(End, other.Pos.End), File);
    public FilePos To(IToken? other) => other == null ? this : To(other.InFile(File));
    public FilePos To(ITerminalNode? other) => other == null ? this : To(other.InFile(File));
    public FilePos To(ParserRuleContext? other) => other == null ? this : To(other.InFile(File));

    // waow basedbasedbasedbased
    public override string ToString() =>
        Start.Line == End.Line
            ? Start.Column == End.Column
                ? $"{File.Name} @ ({Start.Line + 1}, {Start.Column + 1})"
                : $"{File.Name} @ ({Start.Line + 1}, {Start.Column + 1}..={End.Column + 1})"
            : Start.Column == End.Column
                ? $"{File.Name} @ ({Start.Line + 1}..={End.Line + 1}, {Start.Column + 1})"
                : $"{File.Name} @ ({Start.Line + 1}, {Start.Column + 1})..=({End.Line + 1}, {End.Column + 1})";

    public FilePos Pos => this;
}

public readonly record struct FileData(string Name, FileMemory Memory);

public readonly struct FileMemory {
    // Contents of the file
    // Array of the indices of all the line breaks within the contents
    readonly int[] _lineBreaks;
    
    public string Contents { get; }

    public FileMemory(string contents) {
        this.Contents = contents;
        
        // Construct the line break array
        List<int> building = [];
        for (int i = 0; i < contents.Length; i++) {
            char c = contents[i];
            if (c == '\n') building.Add(i);
        }
        
        _lineBreaks = building.ToArray();
    }

    public int LineCount => _lineBreaks.Length + 1;
    
    // stack-allocated version :3
    public FileSpan AsSpan() => new(Contents.AsSpan(), _lineBreaks.AsSpan());
}

// ref struct moment
public readonly ref struct FileSpan {
    readonly ReadOnlySpan<char> _contents;
    readonly ReadOnlySpan<int> _lineBreaks;

    public int LineCount => _lineBreaks.Length + 1;

    internal FileSpan(ReadOnlySpan<char> contents, ReadOnlySpan<int> lineBreaks) {
        _contents = contents;
        _lineBreaks = lineBreaks;
    }

    // Get the contents of a given line

    public ReadOnlySpan<char> this[int i] {
        get {
            // Get the position of the start of the line. For an explanation of why this works, see the second
            //  large comment of code in the ByIndex method.
            int start = StartIndex(i);
            // If the input line is the last one, then the end of the line is at the end of the file.
            int end = i >= _lineBreaks.Length ? _contents.Length : _lineBreaks[i];
            // windows \r\n moment :skull:
            if (_contents[end - 1] == '\r') end--;
            // return a sub-span
            return _contents[start..end];
        }
    }

    // Construct a file coordinate by the index within the string.

    public FileCoord ByIndex(int index) {
        
        // Binary search for the closest line break to the input index
        int lineIndex = _lineBreaks.BinarySearch(index);
        // As per the definition of the binary search function in C#, if the exact search result is not found,
        //  then it will return the bitwise complement (~) to the index of the *next* value within the array,
        //  or the total length of the array if no such value exists. As the first line break (which starts
        //  line 1, zero based) is at index zero. Therefore, any index on line zero will return the bitwise
        //  complement of zero itself. The line break itself will return zero. Therefore, this *is* the line
        //  number for the given index. This is one half of the coord construction complete.
        int line = lineIndex < 0 ? ~lineIndex : lineIndex;
        
        // We now find the index of the first character of the line. As previously described, the index of the
        //  binary search (inverted if negative) IS the line number, as the line break array is shifted by one.
        //  This is easily solved, as we can subtract one from the index, keeping in mind that we don't want to
        //  attempt to get element -1 of the array. In the event that we are searching for line zero's start,
        //  we know it will be at index zero anyway. In other cases, we also have to add one, as the line break
        //  is the character BEFORE the start of the line.
        int lineStart = StartIndex(line);
        // Subtract the line's start index from the input index to get the zero-based column number of the
        //  character at said index.
        int column = index - lineStart;

        // Finally, we construct the coordinate!
        return new(line, column);
    }

    public int ByCoord(FileCoord coord) {
        var l = coord.Line;
        var c = coord.Column;
        var start = StartIndex(l);
        return start + c;
    }

    int StartIndex(int line) => line == 0 ? 0 : _lineBreaks[line - 1] + 1;
}


public interface IFilePosHolder {
    public FilePos Pos { get; }

    public FilePos To(IFilePosHolder? other);
    public FilePos To(IToken? other);
    public FilePos To(ITerminalNode? other);
    public FilePos To(ParserRuleContext? other);
}

// simple lil extension for easily constructing a file pos from a token
public static class TokenExtensions {

    // silly c# 14 stuffs :3
    extension(IToken self) {
        public FilePos InFile(FileData file)
            => FilePos.FromFileIndices(self.StartIndex, self.StopIndex, file);
        public FilePos To(IToken other, FileData file) => self.InFile(file).To(other.InFile(file));
        public FilePos To(ITerminalNode other, FileData file) => self.InFile(file).To(other.InFile(file));
        public FilePos To(ParserRuleContext other, FileData file) => self.InFile(file).To(other.InFile(file));
        public FilePos To(IFilePosHolder holder) => self.InFile(holder.Pos.File).To(holder.Pos);
    }
    
    extension(ITerminalNode self) {
        public FilePos InFile(FileData file) => self.Symbol.InFile(file);

        public FilePos To(ITerminalNode other, FileData file) => self.InFile(file).To(other.InFile(file));
        public FilePos To(IToken other, FileData file) => self.InFile(file).To(other);
        public FilePos To(ParserRuleContext other, FileData file) => self.InFile(file).To(other.InFile(file));
        public FilePos To(IFilePosHolder other) => self.InFile(other.Pos.File).To(other.Pos);
    }
    
    extension(ParserRuleContext self) {
        public FilePos InFile(FileData file) => self.Start.To(self.Stop, file);

        public FilePos To(ITerminalNode other, FileData file) => self.InFile(file).To(other.InFile(file));
        public FilePos To(IToken other, FileData file) => self.InFile(file).To(other);
        public FilePos To(ParserRuleContext other, FileData file) => self.InFile(file).To(other.InFile(file));
        public FilePos To(IFilePosHolder other) => self.InFile(other.Pos.File).To(other.Pos);
    }
}