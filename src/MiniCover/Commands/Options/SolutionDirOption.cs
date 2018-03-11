﻿using Microsoft.Extensions.CommandLineUtils;
using System;
using System.IO;

namespace MiniCover.Commands.Options
{
    public class SolutionDirOption : IMiniCoverOption<DirectoryInfo>
    {
        private const string DefaultValue = "./";
        private bool _invalidated;
        private CommandOption _option;
        private DirectoryInfo _value;
        public string Description => $"Solution directory [default: {DefaultValue}]";
        public string OptionTemplate => "--solutiondir";
        public CommandOptionType Type => CommandOptionType.SingleValue;

        public DirectoryInfo Value
        {
            get
            {
                if (_invalidated) return _value;
                throw new MemberAccessException("Option should be invalidate before Value access");
            }

            set => _value = value;
        }

        public void Initialize(CommandLineApplication commandContext)
        {
            _option = commandContext.Option(OptionTemplate, Description, Type);
        }

        public void Invalidate()
        {
            var solutionPath = _option.Value() ?? DefaultValue;
            if (!Directory.Exists(solutionPath))
            {
                throw new ArgumentException($"Solution directory does not exist '{solutionPath}'");
            }

            _invalidated = true;
            _value = new DirectoryInfo(solutionPath);
        }
    }
}