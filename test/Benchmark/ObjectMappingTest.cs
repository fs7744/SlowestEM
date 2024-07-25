using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using Dapper;
using SlowestEM;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BenchmarkTest
{
    public class Dog
    {
        public int? Age { get; set; }
        public string Name { get; set; }
        public float? Weight { get; set; }
    }

    public class Cat<T>
    {
        public int? Age { get; set; }
        public T Name { get; set; }
        public float? Weight { get; set; }
    }

    public class CatAccessors<T>
    {
        //[UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        //public static extern Cat<T> Ctor();

        //[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_Name")]
        //public static extern void SetName(Cat<T> c, T n);

        //[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_Age")]
        //public static extern void SetAge(Cat<T> c, int? n);

        //[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_Weight")]
        //public static extern void SetWeight(Cat<T> c, float? n);

        public static IEnumerable<Cat<T>> Read(IDataReader reader)
        {
            var s = new Action<Cat<T>>[reader.FieldCount];
            for (int i = 0; i < s.Length; i++)
            {
                switch (reader.GetName(i).ToLower())
                {
                    case "name":
                        {
                            var j = i;
                            s[i] = d => d.Name = (T)reader.GetValue(j);
                            //s[i] = d => SetName(d, reader.GetString(j));
                        }

                        break;

                    case "age":
                        {
                            var j = i;
                            s[i] = d => d.Age = reader.GetInt32(j);
                            // s[i] = d => SetAge(d, reader.GetInt32(j));
                        }

                        break;

                    case "weight":
                        {
                            var j = i;
                            s[i] = d => d.Weight = reader.GetFloat(j);
                            //s[i] = d => SetWeight(d, reader.GetFloat(j));
                        }

                        break;

                    default:
                        break;
                }
            }

            while (reader.Read())
            {
                //var dog = DogAccessors.Ctor();
                var dog = new Cat<T>();
                foreach (var item in s)
                {
                    item(dog);
                }
                yield return dog;
            }
        }
    }

    public class CatString : Cat<string> { }

    public class DogAccessors
    {
        [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
        public static extern Dog Ctor();

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_Name")]
        public static extern void SetName(Dog c, string n);

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_Age")]
        public static extern void SetAge(Dog c, int? n);

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "set_Weight")]
        public static extern void SetWeight(Dog c, float? n);

        public static IEnumerable<Dog> Read(IDataReader reader)
        {
            var s = new Action<Dog>[reader.FieldCount];
            for (int i = 0; i < s.Length; i++)
            {
                switch (reader.GetName(i).ToLower())
                {
                    case "name":
                        {
                            var j = i;
                            s[i] = d => d.Name = reader.GetString(j);
                            //s[i] = d => SetName(d, reader.GetString(j));
                        }
                        
                        break;

                    case "age":
                        {
                            var j = i;
                            s[i] = d => d.Age = reader.GetInt32(j);
                           // s[i] = d => SetAge(d, reader.GetInt32(j));
                        }
                        
                        break;

                    case "weight":
                        {
                            var j = i;
                            s[i] = d => d.Weight = reader.GetFloat(j);
                            //s[i] = d => SetWeight(d, reader.GetFloat(j));
                        }
                        
                        break;

                    default:
                        break;
                }
            }

            while (reader.Read())
            {
                //var dog = DogAccessors.Ctor();
                var dog = new Dog();
                foreach (var item in s)
                {
                    item(dog);
                }
                yield return dog;
            }
        }
    }

    public class TestDbConnection : IDbConnection
    {
        public string ConnectionString { get; set; }

        public int ConnectionTimeout => throw new NotImplementedException();

        public string Database => throw new NotImplementedException();

        public ConnectionState State { get; set; }

        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
        }

        public void ChangeDatabase(string databaseName)
        {
        }

        public void Close()
        {
        }

        public IDbCommand CreateCommand()
        {
           return new TestDbCommand();
        }

        public void Dispose()
        {
        }

        public void Open()
        {
        }
    }

    public class TestDataParameterCollection : IDataParameterCollection
    {
        public object this[string parameterName] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public object? this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public int Add(object? value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
        }

        public bool Contains(string parameterName)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object? value)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(string parameterName)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object? value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object? value)
        {
        }

        public void Remove(object? value)
        {
        }

        public void RemoveAt(string parameterName)
        {
        }

        public void RemoveAt(int index)
        {
        }
    }

    public class TestDbCommand : IDbCommand
    {
        public string CommandText { get ; set ; }
        public int CommandTimeout { get; set; }
        public CommandType CommandType { get; set; }
        public IDbConnection? Connection { get; set; }

        public IDataParameterCollection Parameters { get; } = new TestDataParameterCollection();

        public IDbTransaction? Transaction { get; set; }
        public UpdateRowSource UpdatedRowSource { get; set; }

        public void Cancel()
        {
        }

        public IDbDataParameter CreateParameter()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public int ExecuteNonQuery()
        {
            throw new NotImplementedException();
        }

        public IDataReader ExecuteReader()
        {
            throw new NotImplementedException();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return new TestDbDataReader();
        }

        public object? ExecuteScalar()
        {
            throw new NotImplementedException();
        }

        public void Prepare()
        {
        }
    }

    public class TestDbDataReader : DbDataReader
    {
        private int calls = 0;
        public override object this[int ordinal] 
        {
            get
            {
                switch (ordinal)
                {
                    case 0:
                        return "XX";
                    case 1:
                        return 2;
                    case 2:
                        return 3.3f;
                    default:
                        return null;
                }
            }
        
        }

        public override object this[string name] => throw new NotImplementedException();

        public override int Depth => throw new NotImplementedException();

        public override int FieldCount => 3;

        public override bool HasRows => throw new NotImplementedException();

        public override bool IsClosed => throw new NotImplementedException();

        public override int RecordsAffected => throw new NotImplementedException();

        public override bool GetBoolean(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override byte GetByte(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
        {
            throw new NotImplementedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetDateTime(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override decimal GetDecimal(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override double GetDouble(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)]
        public override Type GetFieldType(int ordinal)
        {
            switch (ordinal)
            {
                case 0:
                    return typeof(string);
                case 1:
                    return typeof(int);
                case 2:
                    return typeof(float);
                default:
                    return null;
            }
        }

        public override float GetFloat(int ordinal)
        {
            switch (ordinal)
            {
                case 2:
                    return 3.3f;
                default:
                    return 0;
            }
        }

        public override Guid GetGuid(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override short GetInt16(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override int GetInt32(int ordinal)
        {
            switch (ordinal)
            {
                case 1:
                    return 2;
                default:
                    return 0;
            }
        }

        public override long GetInt64(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override string GetName(int ordinal)
        {
            switch (ordinal)
            {
                case 0:
                    return "Name";
                case 1:
                    return "Age";
                case 2:
                    return "Weight";
                default:
                    return null;
            }
        }

        public override int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public override string GetString(int ordinal)
        {
            switch (ordinal)
            {
                case 0:
                    return "XX";
                default:
                    return null;
            }
        }

        public override object GetValue(int ordinal)
        {
            switch (ordinal)
            {
                case 0:
                    return "XX";
                case 1:
                    return 2;
                case 2:
                    return 3.3f;
                default:
                    return null;
            }
        }

        public override int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public override bool IsDBNull(int ordinal)
        {
            throw new NotImplementedException();
        }

        public override bool NextResult()
        {
            return false;
        }

        public override bool Read()
        {
            calls++;
            return calls <= 1000;
        }
    }

    [MemoryDiagnoser, Orderer(summaryOrderPolicy: SummaryOrderPolicy.FastestToSlowest), GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory), CategoriesColumn]
    public class ObjectMappingTest
    {
        private IDbConnection connection;

        public ObjectMappingTest()
        {
            DBExtensions.GenericTypeDefinitionReaderCache[typeof(Cat<>)] = ts =>
            {
                var t = typeof(CatAccessors<>).MakeGenericType(ts);
                return t.GetMethod("Read").CreateDelegate<Func<IDataReader, object>>();
            };
            DBExtensions.ReaderCache[typeof(Dog)] = DogAccessors.Read;
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

        [Benchmark(Baseline = true), BenchmarkCategory("1000")]
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

        [Benchmark, BenchmarkCategory("1000")]
        public void DapperMapping()
        {
            var dogs = connection.Query<Dog>("select ").ToList();
        }

        [Benchmark, BenchmarkCategory("1000")]
        public void UnsafeAccessorMapping()
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
        public void UnsafeAccessorMappingFirst()
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

        [Benchmark(Baseline = true), BenchmarkCategory("GenericType-1")]
        public void GenericTypeSetClassFirst()
        {
            Cat<string> dog;
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "select ";
                using (var reader = cmd.ExecuteReader(CommandBehavior.Default))
                {
                    if (reader.Read())
                    {
                        dog = new Cat<string>();
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

        [Benchmark, BenchmarkCategory("GenericType-1")]
        public void GenericTypeDapperMappingFirst()
        {
            var dogs = connection.QueryFirst<Cat<string>>("select ");
        }

        [Benchmark, BenchmarkCategory("GenericType-1")]
        public void GenericTypeUnsafeAccessorMappingFirst()
        {
            Cat<string> cat;
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "select ";
                using (var reader = cmd.ExecuteReader(CommandBehavior.Default))
                {
                    cat = reader.ReadTo<Cat<string>>().FirstOrDefault();
                }
            }
            finally
            {
                connection.Close();
            }
        }

        [Benchmark(Baseline = true), BenchmarkCategory("GenericType-1000")]
        public void GenericTypeSetClass()
        {
            List<Cat<string>> dogs = new List<Cat<string>>();
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "select ";
                using (var reader = cmd.ExecuteReader(CommandBehavior.Default))
                {
                    while (reader.Read())
                    {
                        var dog = new Cat<string>();
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

        [Benchmark, BenchmarkCategory("GenericType-1000")]
        public void GenericTypeDapperMapping()
        {
            var dogs = connection.Query<Cat<string>>("select ").ToList();
        }

        [Benchmark, BenchmarkCategory("GenericType-1000")]
        public void GenericTypeUnsafeAccessorMapping()
        {
            List<Cat<string>> cat;
            try
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = "select ";
                using (var reader = cmd.ExecuteReader(CommandBehavior.Default))
                {
                    cat = reader.ReadTo<Cat<string>>().ToList();
                }
            }
            finally
            {
                connection.Close();
            }
        }
    }
}