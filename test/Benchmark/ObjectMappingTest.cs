using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using Dapper;
using SlowestEM;
using System.Data;

namespace BenchmarkTest
{
    [MemoryDiagnoser, Orderer(summaryOrderPolicy: SummaryOrderPolicy.FastestToSlowest), GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), CategoriesColumn]
    public class ObjectMappingTest
    {
        private IDbConnection connection;

        public ObjectMappingTest()
        {
            //DBExtensions.ReaderCache[typeof(Dog)] = DogAccessors.Read;
            connection = new TestDbConnection();
            //var m = new Mock<IDbConnection>();
            //connection = m.Object;
            //var cmd = new Mock<IDbCommand>();
            //m.Setup(i => i.CreateCommand()).Returns(cmd.Object);
            //cmd.SetupGet(i => i.Parameters).Returns(() =>
            //{
            //    var m = new Mock<IDataParameterCollection>();
            //    return m.Object;
            //});
            //cmd.Setup(i => i.ExecuteReader(It.IsAny<CommandBehavior>())).Returns(() =>
            //{
            //    var reader = new Mock<DbDataReader>();
            //    var calls = 0;
            //    reader.Setup(j => j.Read())
            //        .Callback(() => calls++)
            //        .Returns(() => calls <= 1000);


            //    reader.SetupGet(j => j.FieldCount).Returns(3);
            //    reader.Setup(j => j.GetName(0)).Returns("Name");
            //    reader.Setup(j => j.GetFieldType(0)).Returns(typeof(string));
            //    reader.Setup(j => j.GetString(0)).Returns("XX");
            //    reader.Setup(j => j[0]).Returns("XX");
            //    reader.Setup(j => j.GetName(1)).Returns("Age");
            //    reader.Setup(j => j.GetFieldType(1)).Returns(typeof(int));
            //    reader.Setup(j => j.GetInt32(1)).Returns(2);
            //    reader.Setup(j => j[1]).Returns(2);
            //    reader.Setup(j => j.GetName(2)).Returns("Weight");
            //    reader.Setup(j => j.GetFieldType(2)).Returns(typeof(float));
            //    reader.Setup(j => j.GetFloat(2)).Returns(3.3f);
            //    reader.Setup(j => j[2]).Returns(3.3f);
            //    return reader.Object;
            //});
        }

        [Benchmark(Baseline = true), BenchmarkCategory("10000")]
        public void SetClass()
        {
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

        [Benchmark, BenchmarkCategory("10000")]
        public void DapperMapping()
        {
            var dogs = connection.Query<Dog>("select ").AsList();
        }

        [Benchmark, BenchmarkCategory("10000")]
        public void SourceGeneratorMapping()
        {
            List<Dog> dogs;
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "select ";
                using (var reader = cmd.ExecuteReader(CommandBehavior.Default))
                {
                    dogs = reader.ReadTo<Dog>().ToList();
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Benchmark(Baseline = true), BenchmarkCategory("1")]
        public void SetClassFirst()
        {
            Dog dog;
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "select ";
                using (var reader = cmd.ExecuteReader(CommandBehavior.Default))
                {
                    if (reader.Read())
                    {
                        dog = new Dog();
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

        [Benchmark, BenchmarkCategory("1")]
        public void DapperMappingFirst()
        {
            var dogs = connection.QueryFirst<Dog>("select ");
        }

        [Benchmark, BenchmarkCategory("1")]
        public void SourceGeneratorMappingFirst()
        {
            Dog dog;
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "select ";
                using (var reader = cmd.ExecuteReader(CommandBehavior.Default))
                {
                    dog = reader.ReadTo<Dog>().FirstOrDefault();
                }
            }
            finally
            {
                connection.Close();
            }
        }

        //[Benchmark(Baseline = true), BenchmarkCategory("GenericType-1")]
        //public void GenericTypeSetClassFirst()
        //{
        //    Cat<string> dog;
        //    try
        //    {
        //        connection.Open();
        //        var cmd = connection.CreateCommand();
        //        cmd.CommandText = "select ";
        //        using (var reader = cmd.ExecuteReader(CommandBehavior.Default))
        //        {
        //            if (reader.Read())
        //            {
        //                dog = new Cat<string>();
        //                dog.Name = reader.GetString(0);
        //                dog.Age = reader.GetInt32(1);
        //                dog.Weight = reader.GetFloat(2);
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        connection.Close();
        //    }
        //}

        //[Benchmark, BenchmarkCategory("GenericType-1")]
        //public void GenericTypeDapperMappingFirst()
        //{
        //    var dogs = connection.QueryFirst<Cat<string>>("select ");
        //}

        //[Benchmark, BenchmarkCategory("GenericType-1")]
        //public void GenericTypeSourceGeneratorMappingFirst()
        //{
        //    Cat<string> cat;
        //    try
        //    {
        //        connection.Open();
        //        var cmd = connection.CreateCommand();
        //        cmd.CommandText = "select ";
        //        using (var reader = cmd.ExecuteReader(CommandBehavior.Default))
        //        {
        //            cat = reader.ReadTo<Cat<string>>().FirstOrDefault();
        //        }
        //    }
        //    finally
        //    {
        //        connection.Close();
        //    }
        //}

        //[Benchmark(Baseline = true), BenchmarkCategory("GenericType-1000")]
        //public void GenericTypeSetClass()
        //{
        //    List<Cat<string>> dogs = new List<Cat<string>>();
        //    try
        //    {
        //        connection.Open();
        //        var cmd = connection.CreateCommand();
        //        cmd.CommandText = "select ";
        //        using (var reader = cmd.ExecuteReader(CommandBehavior.Default))
        //        {
        //            while (reader.Read())
        //            {
        //                var dog = new Cat<string>();
        //                dogs.Add(dog);
        //                dog.Name = reader.GetString(0);
        //                dog.Age = reader.GetInt32(1);
        //                dog.Weight = reader.GetFloat(2);
        //            }
        //        }
        //    }
        //    finally
        //    {
        //        connection.Close();
        //    }
        //}

        //[Benchmark, BenchmarkCategory("GenericType-1000")]
        //public void GenericTypeDapperMapping()
        //{
        //    var dogs = connection.Query<Cat<string>>("select ").ToList();
        //}

        //[Benchmark, BenchmarkCategory("GenericType-1000")]
        //public void GenericTypeSourceGeneratorMapping()
        //{
        //    List<Cat<string>> cat;
        //    try
        //    {
        //        connection.Open();
        //        var cmd = connection.CreateCommand();
        //        cmd.CommandText = "select ";
        //        using (var reader = cmd.ExecuteReader(CommandBehavior.Default))
        //        {
        //            cat = reader.ReadTo<Cat<string>>().ToList();
        //        }
        //    }
        //    finally
        //    {
        //        connection.Close();
        //    }
        //}
    }
}