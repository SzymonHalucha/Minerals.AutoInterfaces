using System.Text;

namespace Minerals.AutoInterfaces.Utilities
{
    public sealed class CodeBuilder
    {
        private readonly StringBuilder _builder;
        private readonly int _indentSize;
        private int _indentLevel;

        public CodeBuilder(int indentLevel = 0, int indentSize = 4)
        {
            _builder = new StringBuilder(1024);
            _indentSize = indentSize;
            _indentLevel = indentLevel;
        }

        public override string ToString()
        {
            return _builder.ToString();
        }

        public CodeBuilder Clear()
        {
            _builder.Clear();
            _indentLevel = 0;
            return this;
        }

        public CodeBuilder Write(string text)
        {
            _builder.Append(text);
            return this;
        }

        public CodeBuilder Write(char character)
        {
            _builder.Append(character);
            return this;
        }

        public CodeBuilder WriteLine(string text)
        {
            _builder.AppendLine();
            _builder.Append(' ', _indentLevel * _indentSize);
            _builder.Append(text);
            return this;
        }

        public CodeBuilder WriteLine(char character)
        {
            _builder.AppendLine();
            _builder.Append(' ', _indentLevel * _indentSize);
            _builder.Append(character);
            return this;
        }

        public CodeBuilder OpenBlock(char character = '{')
        {
            _builder.AppendLine();
            _builder.Append(' ', _indentLevel * _indentSize);
            _builder.Append(character);
            _indentLevel++;
            return this;
        }

        public CodeBuilder CloseBlock(char character = '}')
        {
            _indentLevel--;
            _builder.AppendLine();
            _builder.Append(' ', _indentLevel * _indentSize);
            _builder.Append(character);
            return this;
        }

        public CodeBuilder WriteBlock(string text)
        {
            foreach (var line in text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
            {
                _builder.AppendLine();
                _builder.Append(' ', _indentLevel * _indentSize);
                _builder.Append(line);
            }
            return this;
        }

        public CodeBuilder CloseAllBlocks()
        {
            for (int i = _indentLevel - 1; i >= 0; i--)
            {
                _builder.AppendLine();
                _builder.Append(' ', i * _indentSize);
                _builder.Append('}');
            }
            _indentLevel = 0;
            return this;
        }
    }
}