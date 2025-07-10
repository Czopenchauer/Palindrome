```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4652/24H2/2024Update/HudsonValley)
Unknown processor
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI


```
| Method                | Mean      | Error     | StdDev    | Allocated |
|---------------------- |----------:|----------:|----------:|----------:|
| IsPalindrome_Normal   | 60.485 ns | 0.4232 ns | 0.3959 ns |         - |
| IsPalindrome_XOR      | 60.620 ns | 0.2707 ns | 0.2532 ns |         - |
| IsPalindrome_PureSpan |  5.949 ns | 0.0137 ns | 0.0128 ns |         - |
| IsPalindrome_Smth     | 13.682 ns | 0.0633 ns | 0.0561 ns |         - |
| IsPalindrome_Simd     | 13.334 ns | 0.1539 ns | 0.1439 ns |         - |
