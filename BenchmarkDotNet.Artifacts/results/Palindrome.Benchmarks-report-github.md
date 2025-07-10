```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4652/24H2/2024Update/HudsonValley)
Unknown processor
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method                | Mean     | Error     | StdDev    | Allocated |
|---------------------- |---------:|----------:|----------:|----------:|
| IsPalindrome_Normal   | 6.662 ns | 0.0839 ns | 0.0785 ns |         - |
| IsPalindrome_XOR      | 6.646 ns | 0.0759 ns | 0.0710 ns |         - |
| IsPalindrome_PureSpan | 6.136 ns | 0.0060 ns | 0.0050 ns |         - |
| IsPalindrome_Smth     | 3.291 ns | 0.0106 ns | 0.0099 ns |         - |
| IsPalindrome_Simd     | 3.574 ns | 0.0116 ns | 0.0108 ns |         - |
