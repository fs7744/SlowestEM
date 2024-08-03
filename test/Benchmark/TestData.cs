using Dapper;
using SlowestEM;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace BenchmarkTest
{
    public class Dog
    {
        public int? Age { get; set; }
        public string Name { get; set; }
        public float? Weight { get; set; }
    }

    public class TestDbConnection : DbConnection
    {
        public int RowCount { get; set; }
        public override string ConnectionString { get; set; }

        public override int ConnectionTimeout => throw new NotImplementedException();

        public override string Database => throw new NotImplementedException();

        public override ConnectionState State { get; }

        public override string DataSource => throw new NotImplementedException();

        public override string ServerVersion => throw new NotImplementedException();

        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
        }

        public override void ChangeDatabase(string databaseName)
        {
        }

        public override void Close()
        {
        }

        public IDbCommand CreateCommand()
        {
            return new TestDbCommand() { RowCount = RowCount };
        }

        public void Dispose()
        {
        }

        public override void Open()
        {
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            throw new NotImplementedException();
        }

        protected override DbCommand CreateDbCommand()
        {
            return new TestDbCommand() { RowCount = RowCount };
        }
    }

    public class TestDataParameterCollection : DbParameterCollection
    {
        public object this[string parameterName] { get => throw new NotImplementedException(); set => Add(value); }
        public object? this[int index] { get => throw new NotImplementedException(); set => Add(value); }

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public bool IsSynchronized => throw new NotImplementedException();

        public override int Count => throw new NotImplementedException();

        public override object SyncRoot => throw new NotImplementedException();

        private List<object> list = new List<object>();
        

        public override int Add(object value)
        {
            return 0;
        }

        public override void AddRange(Array values)
        {
            throw new NotImplementedException();
        }

        public override void Clear()
        {
        }

        public override bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public override bool Contains(string value)
        {
            throw new NotImplementedException();
        }

        public override void CopyTo(Array array, int index)
        {
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        protected override DbParameter GetParameter(int index)
        {
            throw new NotImplementedException();
        }

        protected override DbParameter GetParameter(string parameterName)
        {
            throw new NotImplementedException();
        }

        public override int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public override int IndexOf(string parameterName)
        {
            throw new NotImplementedException();
        }

        public override void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public override void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public override void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public override void RemoveAt(string parameterName)
        {
            throw new NotImplementedException();
        }

        protected override void SetParameter(int index, DbParameter value)
        {
            throw new NotImplementedException();
        }

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            throw new NotImplementedException();
        }
    }

    public class TestDataParameter : DbParameter
    {
        public byte Precision { get; set; }
        public byte Scale { get; set; }
        public override int Size { get; set; }
        public override DbType DbType { get; set; }
        public override ParameterDirection Direction { get; set; }
        public override bool IsNullable { get; set; }
        public override string ParameterName { get; set; }
        public override string SourceColumn { get; set; }
        public DataRowVersion SourceVersion { get; set; }
        public override object? Value { get; set; }
        public override bool SourceColumnNullMapping { get; set; }

        public override void ResetDbType()
        {
        }
    }

    public class TestDbCommand : DbCommand
    {
        public int RowCount { get; set; }
        public override string CommandText { get; set; }
        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; }
        public IDbConnection? Connection { get; set; }

        public IDataParameterCollection Parameters { get; } = new TestDataParameterCollection();

        public IDbTransaction? Transaction { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }
        public override bool DesignTimeVisible { get; set; }
        protected override DbConnection? DbConnection { get; set; }

        protected override DbParameterCollection DbParameterCollection { get; } = new TestDataParameterCollection();

        protected override DbTransaction? DbTransaction { get; set; }


        public override void Cancel()
        {
        }

        public IDbDataParameter CreateParameter()
        {
           return new TestDataParameter();
        }

        public void Dispose()
        {
        }

        public override int ExecuteNonQuery()
        {
            return 0;
        }

        public IDataReader ExecuteReader()
        {
            throw new NotImplementedException();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return new TestDbDataReader() { RowCount = RowCount };
        }


        public override object? ExecuteScalar()
        {
            throw new NotImplementedException();
        }

        public override void Prepare()
        {
        }

        protected override DbParameter CreateDbParameter()
        {
            return new TestDataParameter();
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return new TestDbDataReader() { RowCount = RowCount };
        }
    }

    public class TestDbDataReader : DbDataReader
    {
        public int RowCount { get; set; }
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

        public override bool IsClosed => true;

        public override int RecordsAffected => 0;

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
            return false;
        }

        public override bool NextResult()
        {
            return false;
        }

        public override bool Read()
        {
            calls++;
            return calls <= RowCount;
        }
    }
}