# Test For Reader to Class


```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3880/23H2/2023Update/SunValley3)
13th Gen Intel Core i9-13900KF, 1 CPU, 32 logical and 24 physical cores
.NET SDK 9.0.100-preview.6.24328.19
  [Host]     : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2


```
| Method                      | Categories | Mean        | Error     | StdDev    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|---------------------------- |----------- |------------:|----------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
| SetClassFirst               | 1          |    260.7 ns |   5.11 ns |   6.64 ns |  1.00 |    0.00 | 0.0148 | 0.0143 |     280 B |        1.00 |
| SourceGeneratorMappingFirst | 1          |    269.4 ns |   5.28 ns |   8.52 ns |  1.04 |    0.04 | 0.0176 | 0.0172 |     336 B |        1.20 |
| DapperMappingFirst          | 1          |    505.7 ns |   6.01 ns |   5.62 ns |  1.94 |    0.06 | 0.0219 |      - |     416 B |        1.49 |
|                             |            |             |           |           |       |         |        |        |           |             |
| SetClass                    | 1000       |  4,572.1 ns |  85.22 ns |  79.72 ns |  1.00 |    0.00 | 3.0136 | 0.9995 |   56840 B |        1.00 |
| SourceGeneratorMapping      | 1000       | 12,363.1 ns | 230.43 ns | 215.55 ns |  2.71 |    0.08 | 3.0212 | 0.9918 |   56896 B |        1.00 |
| DapperMapping               | 1000       | 28,226.3 ns | 266.36 ns | 207.96 ns |  6.20 |    0.12 | 5.5847 | 0.9155 |  105120 B |        1.85 |
