﻿using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace MiniCover.Extensions
{
    public static class AssemblyDefinitionExtensions
    {
        public static IList<Document> GetAllDocuments(this AssemblyDefinition assemblyDefinition)
        {
            return assemblyDefinition
                .MainModule.GetTypes()
                .SelectMany(t => t.GetAllDocuments())
                .Distinct()
                .ToArray();
        }

        public static bool IsExcludedFromCodeCoverage(this AssemblyDefinition assemblyDefinition)
        {
            return assemblyDefinition.HasExcludeFromCodeCoverageAttribute();
        }
    }
}
