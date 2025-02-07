```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.2894)
AMD Ryzen 7 7800X3D, 1 CPU, 16 logical and 8 physical cores
.NET SDK 9.0.101
  [Host]     : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 9.0.0 (9.0.24.52809), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method    | Count  | Mean          | Error       | StdDev      | Ratio  | RatioSD | Gen0     | Gen1     | Gen2     | Allocated   | Alloc Ratio |
|---------- |------- |--------------:|------------:|------------:|-------:|--------:|---------:|---------:|---------:|------------:|------------:|
| **S1_Create** | **1000**   |     **25.988 μs** |   **0.1944 μs** |   **0.1723 μs** |   **3.67** |    **0.18** |   **0.9460** |   **0.1526** |        **-** |    **46.93 KB** |        **0.60** |
| S2_Conc   | 1000   |      7.076 μs |   0.1415 μs |   0.2285 μs |   1.00 |    0.06 |   1.4343 |   0.2823 |        - |    70.37 KB |        0.90 |
| S3_Conc   | 1000   |      7.095 μs |   0.1402 μs |   0.3645 μs |   1.00 |    0.07 |   1.5945 |   0.3510 |        - |    78.18 KB |        1.00 |
| S4_StrOpr | 1000   |  4,867.772 μs |  11.8428 μs |   9.8892 μs | 687.80 |   34.09 |        - |        - |        - |   117.19 KB |        1.50 |
|           |        |               |             |             |        |         |          |          |          |             |             |
| **S1_Create** | **360000** | **18,789.667 μs** | **249.4719 μs** | **233.3562 μs** |   **1.40** |    **0.03** | **468.7500** | **437.5000** | **187.5000** | **16875.12 KB** |       **0.547** |
| S2_Conc   | 360000 | 13,145.008 μs | 255.6216 μs | 341.2475 μs |   0.98 |    0.03 | 781.2500 | 765.6250 | 281.2500 | 28047.04 KB |       0.909 |
| S3_Conc   | 360000 | 13,426.806 μs | 222.4290 μs | 197.1775 μs |   1.00 |    0.02 | 828.1250 | 812.5000 | 265.6250 | 30859.53 KB |       1.000 |
| S4_StrOpr | 360000 |  4,935.739 μs |  39.7147 μs |  35.2060 μs |   0.37 |    0.01 |        - |        - |        - |   117.19 KB |       0.004 |
