using BenchmarkDotNet.Running;
using BenchmarkTest;
using Dapper;
using SlowestEM.Generator;

StringHashing1.NormalizedHash("");
var a = new ObjectMappingTest() {  RowCount = 100};
a.DapperDynamic();
//a.DapperAOTMapping();
//a.SourceGeneratorMapping();
var summary = BenchmarkRunner.Run(typeof(Program).Assembly);