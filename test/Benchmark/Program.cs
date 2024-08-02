using BenchmarkDotNet.Running;
using BenchmarkTest;
using Dapper;
[module: DapperAot(true)]
//var a = new ObjectMappingTest();
//a.SetClass();
//a.DapperMapping();
//a.SourceGeneratorMapping();
var b = new ParamMappingTest();
b.DapperSeParam();
b.SeParam();
b.SourceGeneratorSeParam();
var summary = BenchmarkRunner.Run(typeof(Program).Assembly);