namespace MiniCover.CommandLine.Options
{
    class YamlOutputOption : FileOption
    {
        private const string _defaultValue = "./coverage.yaml";
        private const string _template = "--output";
        private static readonly string _description = $"Output file for Yaml report [default: {_defaultValue}]";

        public YamlOutputOption()
            : base(_template, _description)
        {
        }

        protected override string GetDefaultValue()
        {
            return _defaultValue;
        }
    }
}