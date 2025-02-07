using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Running;

namespace DotNetPerfBenchmark
{
    public class ConfigWithRPlot : ManualConfig
    {
        public ConfigWithRPlot()
        {
            AddExporter(RPlotExporter.Default);
            AddDiagnoser(MemoryDiagnoser.Default);
            // Add other exporters or diagnosers as needed
        }
    }
    
    /// <summary>
    /// We use MemoryDiagnoser to get memory usage (Gen0/Gen1/Gen2 collects, allocated bytes).
    /// </summary>
    [MemoryDiagnoser]
    [Config(typeof(ConfigWithRPlot))]
    public class ZipCodeBenchmarks
    {
        /// <summary>
        /// We fix the number of zip codes to 360,000 to reflect the user's scenario.
        /// If you want multiple test sizes, you can add more values in the Params array.
        /// </summary>
        [Params(1000, 360000)]
        public int Count { get; set; }

        private List<string> zipCodes;

        /// <summary>
        /// GlobalSetup runs once per benchmark (before any iterations).
        /// We'll create a large list of zip code–like strings
        /// (e.g. "0000 AA", "0001 AA", etc.) and store it in a field.
        /// 
        /// This simulates "persisted" memory usage.
        /// </summary>
        [GlobalSetup]
        public void Setup()
        {
            zipCodes = new List<string>(Count);
            for (int i = 0; i < Count; i++)
            {
                // Simple example: "0123 AA", "0124 AA", etc.
                zipCodes.Add($"{i:D4} AA");
            }
        }

        /// <summary>
        /// Scenario 1: Create a new list of 360,000 zip codes.
        /// This shows how much time & memory it takes to *build* a big list.
        /// 
        /// The result is returned just to prevent the JIT from optimizing it away.
        /// </summary>
        [Benchmark]
        public List<string> S1_Create()
        {
            var localList = new List<string>(Count);
            for (int i = 0; i < Count; i++)
            {
                localList.Add($"{i:D4} AA");
            }
            return localList;
        }

        /// <summary>
        /// Scenario 2: Perform a string concatenation operation
        /// on each zip code from the pre-built list (zipCodes).
        /// 
        /// This simulates adding extra data or formatting them
        /// in a new list, increasing CPU and memory usage.
        /// </summary>
        [Benchmark]
        public List<string> S2_Conc()
        {
            var newList = new List<string>(Count);
            foreach (var zc in zipCodes)
            {
                // e.g., add a suffix
                newList.Add(zc + " - ExtraSuffix");
            }
            return newList;
        }

        /// <summary>
        /// Scenario 3: Perform a string concatenation operation
        /// on each zip code from the pre-built list (zipCodes).
        /// 
        /// This simulates adding extra data or formatting them
        /// in a new list, increasing CPU and memory usage.
        /// </summary>
        [Benchmark(Baseline = true)]
        public List<string> S3_Conc()
        {
            var newList = new List<string>(Count);
            foreach (var zc in zipCodes)
            {
                // e.g., add a suffix
                newList.Add("A location for x" + "(" + zc + ")");
            }
            return newList;
        }

        /// <summary>
        /// Scenario 4: Perform a string concatenation operation with a
        /// stringbuilder on each zip code from the pre-built list (zipCodes).
        /// 
        /// This simulates adding extra data or formatting them
        /// in a new list, increasing CPU and memory usage.
        /// </summary>
        [Benchmark]
        public int S4_StrOpr()
        {
            int foundCount = 0;

            // For demonstration, let's do 1,000 searches for different prefixes
            // e.g. "0000", "0001", "0002", etc.
            // Adjust how you see fit (random, or just a range).
            for (int i = 0; i < 1000; i++)
            {
                string prefix = $"{i:D4}";

                // E.g. find the first zip code that starts with "0023" or "0500", etc.
                var found = zipCodes.FirstOrDefault(zc => zc.StartsWith(prefix));

                if (found != null)
                    foundCount++;
            }

            return foundCount;
        }    
    }

    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ZipCodeBenchmarks>();
        }
    }
}
