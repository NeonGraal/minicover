using System.Threading.Tasks;
using MiniCover.CommandLine.Options;
using MiniCover.Reports;
using MiniCover.Utils;

namespace MiniCover.CommandLine.Commands
{
    class YamlReportCommand : BaseCommand
    {
        private const string _name = "yamlreport";
        private const string _description = "Write an Yaml-formatted report to file";

        private readonly CoverageLoadedFileOption _coverageLoadedFileOption;
        private readonly YamlOutputOption _yamlOutputOption;
        private readonly ThresholdOption _thresholdOption;

        public YamlReportCommand(
            WorkingDirectoryOption workingDirectoryOption,
            CoverageLoadedFileOption coverageLoadedFileOption,
            YamlOutputOption yamlOutputOption,
            ThresholdOption thresholdOption)
        : base(_name, _description)
        {
            _coverageLoadedFileOption = coverageLoadedFileOption;
            _thresholdOption = thresholdOption;
            _yamlOutputOption = yamlOutputOption;

            Options = new IOption[]
            {
                workingDirectoryOption,
                _coverageLoadedFileOption,
                _thresholdOption,
                _yamlOutputOption
            };
        }

        protected override Task<int> Execute()
        {
            YamlReport.Execute(_coverageLoadedFileOption.Result, _yamlOutputOption.Value, _thresholdOption.Value);
            var result = CalcUtils.IsHigherThanThreshold(_coverageLoadedFileOption.Result, _thresholdOption.Value);
            return Task.FromResult(result);
        }
    }
}
