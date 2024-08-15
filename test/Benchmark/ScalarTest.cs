using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using Dapper;
using SV.Db;

namespace BenchmarkTest
{
    [MemoryDiagnoser, Orderer(summaryOrderPolicy: SummaryOrderPolicy.FastestToSlowest), GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), CategoriesColumn]
    public class ScalarTest
    {
        [Params(1)]
        public int RowCount { get; set; }

        [Benchmark(Baseline = true)]
        public string ExecuteScalar()
        {
            var connection = new TestDbConnection() { RowCount = RowCount };
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "select ";
                return DBUtils.As<string>(cmd.ExecuteScalar());
            }
            finally
            {
                connection.Close();
            }
        }

        [Benchmark]
        public string ScalarFactory()
        {
            var connection = new TestDbConnection() { RowCount = RowCount };
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "select ";
                return cmd.ExecuteReader().ReadScalar<string>();
            }
            finally
            {
                connection.Close();
            }
        }

        [Benchmark]
        public string Dapper()
        {
            var connection = new TestDbConnection() { RowCount = RowCount };
            return connection.ExecuteScalar<string>("select * from dog");
        }

        [Benchmark, DapperAot]
        public string DapperAot()
        {
            var connection = new TestDbConnection() { RowCount = RowCount };
            return connection.ExecuteScalar<string>("select * from dog");
        }
    }
}