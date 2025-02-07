using System.Collections.Concurrent;
using BenchmarkDotNet.Attributes;

namespace DotNetPerfBenchmark
{
    [MemoryDiagnoser]
    [MarkdownExporterAttribute.GitHub] 
    public class ConcurrentVsCollectionBenchmarks
    {
        [Params(10, 100, 1000, 10_000, 100_000, 1_000_000)]
        public int N;

        private ConcurrentDictionary<int, int> _concurrentDictionary;
        private Dictionary<int, int> _dictionary;
        private object _lockObject;

        private List<int> _list;
        private ConcurrentBag<int> _concurrentBag;

        [GlobalSetup]
        public void Setup()
        {
            _concurrentDictionary = new ConcurrentDictionary<int, int>();
            _dictionary = new Dictionary<int, int>();
            _lockObject = new object();

            _list = new List<int>();
            _concurrentBag = new ConcurrentBag<int>();

            for (int i = 0; i < N; i++)
            {
                _concurrentDictionary[i] = i;
                _dictionary[i] = i;

                _list.Add(i);
                _concurrentBag.Add(i);
            }
        }

        // --------------------------------------------------------------------
        // 1) ConcurrentDictionary
        // --------------------------------------------------------------------

        [Benchmark]
        public void ConcurrentDictionary_Read()
        {
            for (int i = 0; i < N; i++)
            {
                _concurrentDictionary.TryGetValue(i, out _);
            }
        }

        [Benchmark]
        public void ConcurrentDictionary_Write()
        {
            for (int i = 0; i < N; i++)
            {
                _concurrentDictionary[i] = i * 2;
            }
        }

        // --------------------------------------------------------------------
        // 2) Dictionary + lock
        // --------------------------------------------------------------------

        [Benchmark]
        public void Dictionary_Read_WithLock()
        {
            for (int i = 0; i < N; i++)
            {
                lock (_lockObject)
                {
                    _dictionary.TryGetValue(i, out _);
                }
            }
        }

        [Benchmark]
        public void Dictionary_Write_WithLock()
        {
            for (int i = 0; i < N; i++)
            {
                lock (_lockObject)
                {
                    _dictionary[i] = i * 2;
                }
            }
        }

        // --------------------------------------------------------------------
        // 3) List
        // --------------------------------------------------------------------

        [Benchmark]
        public void List_Contains()
        {
            // O(N) per call, repeated N times => O(N^2) total for each test
            for (int i = 0; i < N; i++)
            {
                _list.Contains(i);
            }
        }

        [Benchmark]
        public void List_Add()
        {
            // Add N items at the end
            // (Re-initialize a new list each iteration to avoid size explosion)
            var tempList = new List<int>();
            for (int i = 0; i < N; i++)
            {
                tempList.Add(i);
            }
        }

        // --------------------------------------------------------------------
        // 4) ConcurrentBag
        // --------------------------------------------------------------------

        [Benchmark]
        public void ConcurrentBag_Enumeration()
        {
            // Enumerate all items
            foreach (var item in _concurrentBag)
            {
                // do nothing, just enumerating
            }
        }

        [Benchmark]
        public void ConcurrentBag_Add()
        {
            var tempBag = new ConcurrentBag<int>();
            for (int i = 0; i < N; i++)
            {
                tempBag.Add(i);
            }
        }
    }
}
