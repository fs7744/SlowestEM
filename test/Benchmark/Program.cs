﻿using BenchmarkDotNet.Running;
using BenchmarkTest;
using Dapper;
[module: DapperAot(true)]
var a = new ObjectMappingTest() {  RowCount = 100};
//a.SetClass();
//a.DapperAOTMapping();
//a.SourceGeneratorMapping();
var summary = BenchmarkRunner.Run(typeof(Program).Assembly);