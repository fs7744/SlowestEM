using BenchmarkDotNet.Running;
using BenchmarkTest;
using Dapper;

var a = new ObjectMappingTest() {  RowCount = 100};
a.DynamicRecord();
//a.DapperAOTMapping();
//a.SourceGeneratorMapping();
var summary = BenchmarkRunner.Run(typeof(Program).Assembly);