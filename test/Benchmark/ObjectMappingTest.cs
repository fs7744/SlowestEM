using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using Chloe;
using Chloe.Query.Internals;
using Dapper;
using SlowestEM;
using System.Data;

namespace BenchmarkTest
{
    [MemoryDiagnoser, Orderer(summaryOrderPolicy: SummaryOrderPolicy.FastestToSlowest), GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), CategoriesColumn]
    public class ObjectMappingTest
    {
        [Params(1, 1000, 10000, 100000, 1000000)]
        public int RowCount { get; set; }

        [Benchmark(Baseline = true)]
        public void SetClass()
        {
            var connection = new TestDbConnection() { RowCount = RowCount };
            var dogs = new List<Dog>();
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "select ";
                using (var reader = cmd.ExecuteReader(CommandBehavior.Default))
                {
                    while (reader.Read())
                    {
                        var dog = new Dog();
                        dogs.Add(dog);
                        dog.Name = reader.GetString(0);
                        dog.Age = reader.GetInt32(1);
                        dog.Weight = reader.GetFloat(2);
                    }
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Benchmark]
        public void Dapper()
        {
            var connection = new TestDbConnection() { RowCount = RowCount };
            var dogs = connection.Query<Dog>("select * from dog").AsList();
        }

        [Benchmark, DapperAot]
        public void DapperAOT()
        {
            var connection = new TestDbConnection() { RowCount = RowCount };
            var dogs = connection.Query<Dog>("select * from dog").AsList();
        }

        [Benchmark]
        public void SourceGenerator()
        {
            var connection = new TestDbConnection() { RowCount = RowCount };
            List<Dog> dogs;
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "select ";
                using (var reader = cmd.ExecuteReader(CommandBehavior.Default))
                {
                    dogs = reader.ReadTo<Dog>().AsList();
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Benchmark]
        public void Chloe()
        {
            var connection = new TestDbConnection() { RowCount = RowCount };
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                var dogs = new InternalSqlQuery<Dog>(cmd, "select").AsList();
            }
            finally
            {
                connection.Close();
            }
        }
    }
}