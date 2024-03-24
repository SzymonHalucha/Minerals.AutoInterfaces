namespace Minerals.AutoInterfaces.Utils
{
    public class CodeBuilder
    {
        private readonly StringBuilder _builder;
        private readonly int _indentationSize;
        private int _indentationLevel;

        public CodeBuilder(StringBuilder builder, int indentationSize = 4, int indentationLevel = 0)
        {
            _indentationLevel = indentationLevel;
            _indentationSize = indentationSize;
            _builder = builder;
        }

        public CodeBuilder(int builderStartCapacity = 1024, int indentationSize = 4, int indentationLevel = 0)
        {
            _indentationLevel = indentationLevel;
            _indentationSize = indentationSize;
            _builder = new(builderStartCapacity);
        }

        public CodeBuilder Write(string text)
        {
            Append(text);
            return this;
        }

        public CodeBuilder WriteLine(string text)
        {
            AppendLine(text);
            return this;
        }

        public CodeBuilder WriteIteration(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                AppendLine(line);
            }
            return this;
        }

        public CodeBuilder WriteIteration(IEnumerable<string> lines, Action<CodeBuilder, string, bool> iterator)
        {
            var enumerator = lines.GetEnumerator();
            var moveNext = enumerator.MoveNext();
            var current = enumerator.Current;
            while (moveNext)
            {
                moveNext = enumerator.MoveNext();
                iterator(this, current, !moveNext);
                current = enumerator.Current;
            }
            return this;
        }

        public CodeBuilder WriteIteration(IReadOnlyCollection<string> lines)
        {
            foreach (var line in lines)
            {
                AppendLine(line);
            }
            return this;
        }

        public CodeBuilder WriteIteration(IReadOnlyCollection<string> lines, Action<CodeBuilder, string, bool> iterator)
        {
            var enumerator = lines.GetEnumerator();
            var moveNext = enumerator.MoveNext();
            var current = enumerator.Current;
            while (moveNext)
            {
                moveNext = enumerator.MoveNext();
                iterator(this, current, moveNext);
                current = enumerator.Current;
            }
            return this;
        }

        public CodeBuilder OpenBlock()
        {
            AppendLine("{");
            _indentationLevel++;
            return this;
        }

        public CodeBuilder CloseBlock(bool newLine = false, bool appendSemicolon = false)
        {
            _indentationLevel--;
            if (appendSemicolon)
            {
                AppendLine("};");
            }
            else
            {
                AppendLine("}");
            }
            if (newLine)
            {
                AppendLine("");
            }
            return this;
        }

        public CodeBuilder WriteBlock(string text, bool newLine = false, bool appendSemicolon = false)
        {
            OpenBlock();
            AppendLine(text);
            CloseBlock(newLine, appendSemicolon);
            return this;
        }

        public CodeBuilder WriteBlock(Action<CodeBuilder> writer, bool newLine = false, bool appendSemicolon = false)
        {
            OpenBlock();
            writer(this);
            CloseBlock(newLine, appendSemicolon);
            return this;
        }

        public CodeBuilder? If(bool condition)
        {
            if (condition)
            {
                return this;
            }
            return null;
        }

        public override string ToString()
        {
            return _builder.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Append(string text)
        {
            AppendIndentation();
            _builder.Append(text);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AppendLine(string text)
        {
            AppendIndentation();
            _builder.AppendLine(text);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AppendIndentation()
        {
            if (_builder.Length > 0 && _builder[_builder.Length - 1].Equals('\n'))
            {
                _builder.Append(' ', _indentationSize * _indentationLevel);
            }
        }
    }
}