using System;
using System.Collections.Generic;
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
        [Params(360000)]
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
        public List<string> CreateZipCodeList()
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
        public List<string> ConcatZipCodes()
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
        /// Scenario 3: Another manipulation, e.g. substring operation.
        /// This simulates scanning or partially transforming each string,
        /// which can add CPU overhead but not as much extra memory 
        /// (since we aren't storing new strings in a list).
        /// </summary>
        [Benchmark]
        public long SubstringZipCodes()
        {
            long totalLength = 0;
            foreach (var zc in zipCodes)
            {
                // Just an example: if length is >3, we do a substring(0,3).
                // Accumulate the total length for minimal overhead.
                if (zc.Length > 3)
                {
                    string partial = zc.Substring(0, 3);
                    totalLength += partial.Length;
                }
            }
            return totalLength;
        }
        
        /// <summary>
        /// Scenario 3: Perform a string concatenation operation
        /// on each zip code from the pre-built list (zipCodes).
        /// 
        /// This simulates adding extra data or formatting them
        /// in a new list, increasing CPU and memory usage.
        /// </summary>
        [Benchmark]
        public List<string> HeavyConcatZipCodes()
        {
            var newList = new List<string>(Count);
            foreach (var zc in zipCodes)
            {
                // e.g., add a suffix
                newList.Add("A location for x" + "(" + zc + ")");
            }
            return newList;
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
