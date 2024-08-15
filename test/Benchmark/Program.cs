using BenchmarkDotNet.Running;
using BenchmarkTest;
using Dapper;
using SlowestEM.Generator;

StringHashing1.NormalizedHash("");
var a = new ScalarTest() {  RowCount = 100};
var aa = a.ExecuteScalar();
var a1 = a.Dapper();
var a2 = a.DapperAot();
var a3 = a.ScalarFactory();
//a.DapperAOTMapping();
//a.SourceGeneratorMapping();
var summary = BenchmarkRunner.Run<ScalarTest>();