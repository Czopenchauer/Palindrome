```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4652/24H2/2024Update/HudsonValley)
Unknown processor
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method                | Mean     | Error     | StdDev    | Allocated |
|---------------------- |---------:|----------:|----------:|----------:|
| IsPalindrome_Normal   | 6.611 ns | 0.0971 ns | 0.0909 ns |         - |
| IsPalindrome_XOR      | 6.655 ns | 0.0840 ns | 0.0786 ns |         - |
| IsPalindrome_PureSpan | 6.130 ns | 0.0059 ns | 0.0049 ns |         - |
| IsPalindrome_Smth     | 3.325 ns | 0.0162 ns | 0.0152 ns |         - |
| IsPalindrome_Simd     |       NA |        NA |        NA |        NA |

Benchmarks with issues:
  Benchmarks.IsPalindrome_Simd: DefaultJob
