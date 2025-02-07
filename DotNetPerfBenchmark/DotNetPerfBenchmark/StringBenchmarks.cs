using System.Text;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace DotNetPerfBenchmark
{
    [MemoryDiagnoser]            // Tracks memory allocation
    [MarkdownExporterAttribute.GitHub] // Exports results in GitHub-friendly Markdown
    public class StringManipulationBenchmarks
    {
        // This parameter will control the size of the input strings
        [Params(10, 100, 1000, 10_000, 100_000, 1_000_000)]
        public int InputSize;

        private string _sampleText;
        private string _wordToReplace;
        private string[] _words;
        private Regex _regex;

        [GlobalSetup]
        public void Setup()
        {
            // Create a sample text of length 'InputSize'
            // We'll just repeat 'abc' for demonstration
            _sampleText = string.Concat(Enumerable.Repeat("abc", InputSize / 3 + 1))
                                 .Substring(0, InputSize);

            _wordToReplace = "abc";
            _regex = new Regex(_wordToReplace);

            // Create an array of words to merge/concatenate
            _words = Enumerable.Repeat("abc", InputSize / 3 + 1).ToArray();
        }

        // --------------------------------------------------------------------
        // INEFFICIENT STRING OPERATIONS
        // --------------------------------------------------------------------

        [Benchmark]
        public string NaiveConcatInLoop()
        {
            // Repeated string concatenation in a loop
            var result = "";
            for (int i = 0; i < _words.Length; i++)
            {
                result += _words[i];
            }
            return result;
        }

        [Benchmark]
        public string RegexReplace()
        {
            // Using regex for repeated replacement
            // This is costly for simple repeated patterns
            return _regex.Replace(_sampleText, "XYZ");
        }

        [Benchmark]
        public string SplittingAndRejoining()
        {
            // Splitting and joining for every small manipulation
            var tokens = _sampleText.Split(new[] { _wordToReplace }, StringSplitOptions.None);
            return string.Join("XYZ", tokens);
        }

        [Benchmark]
        public string SubstringLoop()
        {
            // Repeatedly extracting substrings in a loop
            // Just for demonstration, loop over half of the string
            var length = _sampleText.Length / 2;
            var result = "";
            for (int i = 0; i < length; i++)
            {
                // Inefficient substring usage in a loop
                result += _sampleText.Substring(i, 1);
            }
            return result;
        }

        // --------------------------------------------------------------------
        // EFFICIENT BASELINE
        // --------------------------------------------------------------------

        [Benchmark(Baseline = true)]
        public string StringBuilderConcat()
        {
            // A more efficient way of building large strings
            var sb = new StringBuilder();
            for (int i = 0; i < _words.Length; i++)
            {
                sb.Append(_words[i]);
            }
            return sb.ToString();
        }
    }
}
