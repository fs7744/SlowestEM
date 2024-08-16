using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using Dapper;
using SV.Db;
using System.Data;

namespace BenchmarkTest
{
    [MemoryDiagnoser, Orderer(summaryOrderPolicy: SummaryOrderPolicy.FastestToSlowest), GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), CategoriesColumn]
    public class ScalarTest
    {
        [Params(1, 1000, 10000, 100000, 1000000)]
        public int RowCount { get; set; }

        public IDataReader DataReader { get; set; } = new TestDbConnection() { RowCount = 1 }.CreateCommand().ExecuteReader();


        //[Benchmark(Baseline = true), BenchmarkCategory("Convert")]
        //public string DBUtilsAs()
        //{
        //    return DBUtils.As<string>(DataReader.GetValue(0));
        //}

        //[Benchmark, BenchmarkCategory("Convert")]
        //public string ReadScalar()
        //{
        //    return DataReader.ReadScalar<string>();
        //}

        [Benchmark(Baseline = true)]
        public List<string> ExecuteScalar()
        {
            var dogs = new List<string>();
            var connection = new TestDbConnection() { RowCount = RowCount };
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "select ";
                using (var reader = cmd.ExecuteReader(CommandBehavior.Default))
                {
                    while (reader.Read())
                    {
                        dogs.Add(reader.GetString(0));
                    }
                }
            }
            finally
            {
                connection.Close();
            }
            return dogs;
        }

        [Benchmark]
        public List<string> ScalarFactory()
        {
            var connection = new TestDbConnection() { RowCount = RowCount };
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "select ";
                return cmd.ExecuteReader().ReadScalarEnumerable<string>().AsList();
            }
            finally
            {
                connection.Close();
            }
        }

        [Benchmark]
        public List<string> RecordFactory()
        {
            var connection = new TestDbConnection() { RowCount = RowCount };
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "select ";
                return cmd.ExecuteReader().ReadEnumerable<string>().AsList();
            }
            finally
            {
                connection.Close();
            }
        }

        [Benchmark]
        public List<string> Dapper()
        {
            var connection = new TestDbConnection() { RowCount = RowCount };
            return connection.Query<string>("select * from dog").AsList();
        }

        [Benchmark, DapperAot]
        public List<string> DapperAot()
        {
            var connection = new TestDbConnection() { RowCount = RowCount };
            return connection.Query<string>("select * from dog").AsList();
        }
    }
}