using BenchmarkDotNet.Running;
using BenchmarkTest;
//var a = new ObjectMappingTest();
//a.SetClass();
//a.DapperMapping();
//a.UnsafeAccessorMapping();
var summary = BenchmarkRunner.Run(typeof(Program).Assembly);