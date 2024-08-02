using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using Dapper;
using SlowestEM;
using System.Data;

namespace BenchmarkTest
{
    [MemoryDiagnoser, Orderer(summaryOrderPolicy: SummaryOrderPolicy.FastestToSlowest), GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), CategoriesColumn]
    public class ParamMappingTest
    {
        private IDbConnection connection;
        Dog dog = new Dog() { Age = 66, Name = "dddd", Weight = 6.34f };

        public ParamMappingTest()
        {
            connection = new TestDbConnection();
        }

        [Benchmark(Baseline = true), BenchmarkCategory("1")]
        public void SeParam()
        {

            try
            {
                connection.Open();
                var command = connection.CreateCommand();
                var p = command.CreateParameter();
                p.ParameterName = "Age";
                p.DbType = DbType.Int32;
                p.Direction = ParameterDirection.Input;
                p.Value = dog.Age.HasValue ? dog.Age.Value : null;
                command.Parameters.Add(p);

                p = command.CreateParameter();
                p.ParameterName = "Name";
                p.DbType = DbType.String;
                p.Direction = ParameterDirection.Input;
                p.Value = dog.Name;
                command.Parameters.Add(p);

                p = command.CreateParameter();
                p.ParameterName = "Weight";
                p.DbType = DbType.Single;
                p.Direction = ParameterDirection.Input;
                p.Value = dog.Weight.HasValue ? dog.Weight.Value : null;
                command.Parameters.Add(p);
                command.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();
            }
        }

        [Benchmark, BenchmarkCategory("1")]
        public void SourceGeneratorSeParam()
        {
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CreateParams(dog);
                cmd.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();
            }

        }

        [Benchmark, BenchmarkCategory("1")]
        public void DapperSeParam()
        {
            connection.Execute("select * from Posts where Age = @Age  AND  Name = @Name AND  Weight = @Weight", dog);
        }

        [Benchmark(Baseline = true), BenchmarkCategory("1000")]
        public void SeParam1000()
        {

            for (int i = 0; i < 1000; i++)
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    var p = command.CreateParameter();
                    p.ParameterName = "Age";
                    p.DbType = DbType.Int32;
                    p.Direction = ParameterDirection.Input;
                    p.Value = dog.Age.HasValue ? dog.Age.Value : null;
                    command.Parameters.Add(p);

                    p = command.CreateParameter();
                    p.ParameterName = "Name";
                    p.DbType = DbType.String;
                    p.Direction = ParameterDirection.Input;
                    p.Value = dog.Name;
                    command.Parameters.Add(p);

                    p = command.CreateParameter();
                    p.ParameterName = "Weight";
                    p.DbType = DbType.Single;
                    p.Direction = ParameterDirection.Input;
                    p.Value = dog.Weight.HasValue ? dog.Weight.Value : null;
                    command.Parameters.Add(p);
                    command.ExecuteNonQuery();
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        [Benchmark, BenchmarkCategory("1000")]
        public void SourceGeneratorSeParam1000()
        {
            for (int i = 0; i < 1000; i++)
            {
                try
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CreateParams(dog);
                    cmd.ExecuteNonQuery();
                }
                finally
                {
                    connection.Close();
                }
            }

        }

        [Benchmark, BenchmarkCategory("1000")]
        public void DapperSeParam1000()
        {

            for (int i = 0; i < 1000; i++)
            {
                connection.Execute("select * from Posts where Age = @Age  AND  Name = @Name AND  Weight = @Weight", dog);
            }
        }
    }
}