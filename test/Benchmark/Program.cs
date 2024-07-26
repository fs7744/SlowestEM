using BenchmarkDotNet.Running;
using BenchmarkTest;
var a = new ObjectMappingTest();
//a.GenericTypeSourceGeneratorMappingFirst();
//a.SetClass();
//a.DapperMapping();
a.SourceGeneratorMapping();
var summary = BenchmarkRunner.Run(typeof(Program).Assembly);