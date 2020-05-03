using MiniCover.HitServices;
using MiniCover.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace MiniCover.Reports
{
    public static class YamlReport
    {
        private static TextWriter _yamlReport;

        public static void Execute(InstrumentationResult result, FileInfo output, float threshold)
        {
            var contexts =  HitContext.TryReadFromDirectory(result.HitsPath);
            var hits = new HitsInfo(contexts);
            var files = result.GetSourceFiles();

            output.Directory.Create();
            using (_yamlReport = output.CreateText())
            {
                WriteGroupHits(contexts, c => c.AssemblyName, classes =>
                    WriteGroupHits(classes, a => a.ClassName,
                        WriteMethods, "classes", "class", "  "
                    ), "testAssemblies", "assembly", "");
                _yamlReport.WriteLine("");
                _yamlReport.WriteLine("sourceFiles:");
                foreach (var file in files)
                {
                    _yamlReport.WriteLine($"- file: {file.Key}");
                    WriteGroup(file.Value.Sequences, s => s.Method.Class, methods =>
                        WriteGroup(methods, c => c.Method.Name,
                            WriteSequences, "methods", "method", "    "
                        ), "classes", "class", "  ");
                }
            }
        }

        private static void WriteSequences(List<InstrumentedSequence> sequences)
        {
            _yamlReport.WriteLine("      hits:");
            foreach (var sequence in sequences)
            {
                _yamlReport.WriteLine($"      - hit: {sequence.HitId}");
                _yamlReport.WriteLine($"        from: {sequence.StartLine}@{sequence.StartColumn}");
                _yamlReport.WriteLine($"        to: {sequence.EndLine}@{sequence.EndColumn}");

                var branches = sequence.Conditions.SelectMany(c => c.Branches, (c, b) => b.HitId).ToList();
                if (branches.Any()) {
                    _yamlReport.WriteLine($"        branchHits: [{string.Join(", ", branches)}]");
                }
            }
        }

        private static void WriteMethods(List<HitContext> methods)
        {
            _yamlReport.WriteLine("    methods:");
            foreach (var mth in methods)
            {
                _yamlReport.WriteLine($"    - method: {mth.MethodName}");
                WriteHits("      ", mth.Hits);
            }
        }

        private static void WriteGroup<T>(
            IEnumerable<T> contexts,
            Func<T, string> groupBy,
            Action<List<T>> action,
            string heading, string label, string prefix)
        {
            _yamlReport.WriteLine($"{prefix}{heading}:");
            foreach (var group in contexts.GroupBy(groupBy))
            {
                _yamlReport.WriteLine($"{prefix}- {label}: {group.Key}");
                var items = group.ToList();
                if (!items.Any()) return;
                action(items);
            }
        }

        private static void WriteGroupHits(
            IEnumerable<HitContext> contexts,
            Func<HitContext, string> groupBy,
            Action<List<HitContext>> action,
            string heading, string label, string prefix)
        => WriteGroup(contexts, groupBy, items => {
                WriteHits($"  {prefix}", CommonHits(items));
                action(items);
            }, heading, label, prefix);

        private static IDictionary<int, int> CommonHits(List<HitContext> classes)
        {
            var commonIds = new HashSet<int>(classes[0].Hits.Keys);
            classes.ForEach(c => commonIds.IntersectWith(c.Hits.Keys));
            return commonIds.ToDictionary(k => k,
                k => classes
                    .Where(c => c.Hits.ContainsKey(k))
                    .Min(c => c.Hits[k]));
        }

        private static void WriteHits(string prefix, IDictionary<int, int> hits)
        {
            if (!hits.Any()) return;
            _yamlReport.WriteLine($"{prefix}hits:");
            foreach (var hit in hits.OrderBy(h => h.Key))
            {
                _yamlReport.WriteLine($"{prefix}  {hit.Key}: {hit.Value}");
            }
        }
    }
}