using Chloe.Annotations;
using Chloe.Descriptors;
using Chloe.Infrastructure;
using Chloe.Mapper;
using Chloe.Mapper.Activators;
using Chloe.Mapper.Binders;
using Chloe.Query.Mapping;
using Chloe.Reflection;
using System.Collections;
using System.Data;
using System.Reflection;
using System.Threading;
using Chloe.Mapper;
using Chloe.Threading.Tasks;
using System.Collections;
using System.Data;
using System.Threading;
using Chloe.Threading.Tasks;
using System.Collections;
using System.Data;
using BoolResultTask = System.Threading.Tasks.ValueTask<bool>;
using Chloe.Data;
using System.Data.Common;

namespace Chloe.Query.Internals
{
    internal class DataReaderEnumerator : IEnumerator<IDataReader>
    {
        bool _disposed;
        bool _hasFinished;
        Func<bool, Task<IDataReader>> _dataReaderCreator;
        IDataReader _reader;
        IDataReader _current;

        public DataReaderEnumerator(Func<bool, Task<IDataReader>> dataReaderCreator)
        {
            this._dataReaderCreator = dataReaderCreator;
        }

        public IDataReader Current { get { return this._current; } }

        object IEnumerator.Current { get { return this._current; } }

        public bool MoveNext()
        {
            return this.MoveNext(false).GetResult();
        }


        public BoolResultTask MoveNextAsync()
        {
            return this.MoveNext(true);
        }

        async BoolResultTask MoveNext(bool @async)
        {
            if (this._hasFinished || this._disposed)
                return false;

            if (this._reader == null)
            {
                this._reader = await this._dataReaderCreator(@async);
            }

            bool hasData = await this._reader.Read(@async);
            if (hasData)
            {
                this._current = this._reader;
                return true;
            }
            else
            {
                this._reader.Close();
                this._current = default;
                this._hasFinished = true;
                return false;
            }
        }

        public void Dispose()
        {
            if (this._disposed)
                return;

            if (this._reader != null)
            {
                if (!this._reader.IsClosed)
                    this._reader.Close();
                this._reader.Dispose();
                this._reader = null;
            }

            this._hasFinished = true;
            this._current = default;
            this._disposed = true;
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }
    }

    internal static class ListExtension
    {
        public static void AppendRange<T>(this List<T> list, IEnumerable<T> source)
        {
            if (source is ICollection collection)
            {
                int num = list.Count + collection.Count;
                if (list.Capacity < num)
                {
                    list.Capacity = num;
                }
            }

            list.AddRange(source);
        }

        public static List<T> CloneAndAppendOne<T>(this List<T> source, T t)
        {
            List<T> list = new List<T>(source.Count + 1);
            list.AddRange(source);
            list.Add(t);
            return list;
        }

        public static List<T> Clone<T>(this List<T> source, int? capacity = null)
        {
            List<T> list = new List<T>(capacity ?? source.Count);
            list.AddRange(source);
            return list;
        }
        public static async BoolResultTask Read(this IDataReader dataReader, bool @async)
        {
            if (!@async)
            {
                return dataReader.Read();
            }

            DbDataReader dbDataReader = dataReader as DbDataReader;
            if (dbDataReader != null)
            {
                return await dbDataReader.ReadAsync();
            }

            DataReaderDecorator dataReaderDecorator = dataReader as DataReaderDecorator;
            if (dataReaderDecorator != null)
            {
                return await dataReaderDecorator.ReadAsync();
            }

            return dataReader.Read();
        }

        public static async Task<IDataReader> ExecuteReader(this IDbCommand cmd, bool @async)
        {
            if (!@async)
            {
                return cmd.ExecuteReader();
            }

            DbCommand dbCommand = cmd as DbCommand;
            if (dbCommand != null)
            {
                return await dbCommand.ExecuteReaderAsync();
            }

            DbCommandDecorator dbCommandDecorator = cmd as DbCommandDecorator;
            if (dbCommandDecorator != null)
            {
                return await dbCommandDecorator.ExecuteReaderAsync();
            }

            return cmd.ExecuteReader();
        }
    }

    class InternalSqlQuery<T> : IEnumerable<T>, IAsyncEnumerable<T>
    {
        IDbCommand command;
        string _sql;

        public InternalSqlQuery(IDbCommand command, string sql)
        {
            this.command = command;
            this._sql = sql;
        }

        public IEnumerable<T> AsIEnumerable()
        {
            return this;
        }
        public IAsyncEnumerable<T> AsIAsyncEnumerable()
        {
            return this;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new QueryEnumerator<T>(this.ExecuteReader, this.CreateObjectActivator, CancellationToken.None);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
        {
            IAsyncEnumerator<T> enumerator = this.GetEnumerator() as IAsyncEnumerator<T>;
            return enumerator;
        }

        IObjectActivator CreateObjectActivator(IDataReader dataReader)
        {
            Type type = typeof(T);

            if (type != PublicConstants.TypeOfObject && MappingTypeSystem.IsMappingType(type))
            {
                PrimitiveObjectActivatorCreator activatorCreator = new PrimitiveObjectActivatorCreator(type, 0);
                return activatorCreator.CreateObjectActivator();
            }

            return GetObjectActivator(type, dataReader);
        }

        async Task<IDataReader> ExecuteReader(bool @async)
        {
            IDataReader reader = await this.command.ExecuteReader(@async);
            return reader;
        }

        static IObjectActivator GetObjectActivator(Type type, IDataReader reader)
        {
            List<CacheInfo> caches;
            if (!ObjectActivatorCache.TryGetValue(type, out caches))
            {
                if (!Monitor.TryEnter(type))
                {
                    return CreateObjectActivator(type, reader);
                }

                try
                {
                    caches = ObjectActivatorCache.GetOrAdd(type, new List<CacheInfo>(1));
                }
                finally
                {
                    Monitor.Exit(type);
                }
            }

            CacheInfo cache = TryGetCacheInfoFromList(caches, reader);

            if (cache == null)
            {
                lock (caches)
                {
                    cache = TryGetCacheInfoFromList(caches, reader);
                    if (cache == null)
                    {
                        ComplexObjectActivator activator = CreateObjectActivator(type, reader);
                        cache = new CacheInfo(activator, reader);
                        caches.Add(cache);
                    }
                }
            }

            return cache.ObjectActivator;
        }
        static ComplexObjectActivator CreateObjectActivator(Type type, IDataReader reader)
        {
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                throw new ArgumentException(string.Format("The type of '{0}' does't define a none parameter constructor.", type.FullName));

            ConstructorDescriptor constructorDescriptor = ConstructorDescriptor.GetInstance(constructor);
            ObjectMemberMapper mapper = constructorDescriptor.GetEntityMemberMapper();
            InstanceCreator instanceCreator = constructorDescriptor.GetInstanceCreator();
            List<IMemberBinder> memberBinders = PrepareMemberBinders(type, reader, mapper);

            ComplexObjectActivator objectActivator = new ComplexObjectActivator(instanceCreator, new List<IObjectActivator>(), memberBinders, null);
            objectActivator.Prepare(reader);

            return objectActivator;
        }
        static List<IMemberBinder> PrepareMemberBinders(Type type, IDataReader reader, ObjectMemberMapper mapper)
        {
            List<IMemberBinder> memberBinders = new List<IMemberBinder>(reader.FieldCount);

            MemberInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            MemberInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetField);
            List<MemberInfo> members = new List<MemberInfo>(properties.Length + fields.Length);
            members.AppendRange(properties);
            members.AppendRange(fields);

            TypeDescriptor typeDescriptor = EntityTypeContainer.TryGetDescriptor(type);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string name = reader.GetName(i);
                MemberInfo mapMember = TryGetMapMember(members, name, typeDescriptor);

                if (mapMember == null)
                    continue;

                MRMTuple mMapperTuple = mapper.GetMappingMemberMapper(mapMember);
                if (mMapperTuple == null)
                    continue;

                PrimitiveMemberBinder memberBinder = new PrimitiveMemberBinder(mapMember, mMapperTuple, i);
                memberBinders.Add(memberBinder);
            }

            return memberBinders;
        }

        static MemberInfo TryGetMapMember(List<MemberInfo> members, string readerName, TypeDescriptor typeDescriptor)
        {
            MemberInfo mapMember = null;

            foreach (MemberInfo member in members)
            {
                string columnName = null;
                if (typeDescriptor != null)
                {
                    PrimitivePropertyDescriptor propertyDescriptor = typeDescriptor.FindPrimitivePropertyDescriptor(member);
                    if (propertyDescriptor != null)
                        columnName = propertyDescriptor.Column.Name;
                }

                if (string.IsNullOrEmpty(columnName))
                {
                    ColumnAttribute columnAttribute = member.GetCustomAttribute<ColumnAttribute>();
                    if (columnAttribute != null)
                        columnName = columnAttribute.Name;
                }

                if (string.IsNullOrEmpty(columnName))
                    continue;

                if (!string.Equals(columnName, readerName, StringComparison.OrdinalIgnoreCase))
                    continue;

                mapMember = member;
                break;
            }

            if (mapMember == null)
            {
                mapMember = members.Find(a => a.Name == readerName);
            }

            if (mapMember == null)
            {
                mapMember = members.Find(a => string.Equals(a.Name, readerName, StringComparison.OrdinalIgnoreCase));
            }

            return mapMember;
        }

        static CacheInfo TryGetCacheInfoFromList(List<CacheInfo> caches, IDataReader reader)
        {
            CacheInfo cache = null;
            for (int i = 0; i < caches.Count; i++)
            {
                var item = caches[i];
                if (item.IsTheSameFields(reader))
                {
                    cache = item;
                    break;
                }
            }

            return cache;
        }

        static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, List<CacheInfo>> ObjectActivatorCache = new System.Collections.Concurrent.ConcurrentDictionary<Type, List<CacheInfo>>();

        public class CacheInfo
        {
            ReaderFieldInfo[] _readerFields;
            ComplexObjectActivator _objectActivator;
            public CacheInfo(ComplexObjectActivator activator, IDataReader reader)
            {
                int fieldCount = reader.FieldCount;
                var readerFields = new ReaderFieldInfo[fieldCount];

                for (int i = 0; i < fieldCount; i++)
                {
                    readerFields[i] = new ReaderFieldInfo(reader.GetName(i), reader.GetFieldType(i));
                }

                this._readerFields = readerFields;
                this._objectActivator = activator;
            }

            public ComplexObjectActivator ObjectActivator { get { return this._objectActivator; } }

            public bool IsTheSameFields(IDataReader reader)
            {
                ReaderFieldInfo[] readerFields = this._readerFields;
                int fieldCount = reader.FieldCount;

                if (fieldCount != readerFields.Length)
                    return false;

                for (int i = 0; i < fieldCount; i++)
                {
                    ReaderFieldInfo readerField = readerFields[i];
                    if (reader.GetFieldType(i) != readerField.Type || reader.GetName(i) != readerField.Name)
                    {
                        return false;
                    }
                }

                return true;
            }

            class ReaderFieldInfo
            {
                string _name;
                Type _type;
                public ReaderFieldInfo(string name, Type type)
                {
                    this._name = name;
                    this._type = type;
                }

                public string Name { get { return this._name; } }
                public Type Type { get { return this._type; } }
            }
        }
    }

    internal class QueryEnumerator<T> : IEnumerator<T>
    {
        bool _disposed;
        DataReaderEnumerator _dataReaderEnumerator;
        Func<IDataReader, IObjectActivator> _objectActivatorCreator;
        IObjectActivator _objectActivator;
        CancellationToken _cancellationToken;
        T _current;

        public QueryEnumerator(Func<bool, Task<IDataReader>> dataReaderCreator, IObjectActivator objectActivator, CancellationToken cancellationToken) : this(dataReaderCreator, dataReader => objectActivator, cancellationToken)
        {

        }
        public QueryEnumerator(Func<bool, Task<IDataReader>> dataReaderCreator, Func<IDataReader, IObjectActivator> objectActivatorCreator, CancellationToken cancellationToken)
        {
            this._dataReaderEnumerator = new DataReaderEnumerator(dataReaderCreator);
            this._objectActivatorCreator = objectActivatorCreator;
            this._cancellationToken = cancellationToken;
        }

        public T Current { get { return this._current; } }
        object IEnumerator.Current { get { return this._current; } }

        public bool MoveNext()
        {
            return this.MoveNext(false).GetResult();
        }

        async BoolResultTask MoveNext(bool @async)
        {
            if (this._disposed)
                return false;

            bool hasData = @async ? await this._dataReaderEnumerator.MoveNextAsync() : this._dataReaderEnumerator.MoveNext();

            if (hasData)
            {
                if (this._objectActivator == null)
                {
                    this._objectActivator = this._objectActivatorCreator(this._dataReaderEnumerator.Current);
                }

                this._current = (T)(await this._objectActivator.CreateInstance(this._dataReaderEnumerator.Current, @async));
            }
            else
            {
                this._current = default;
            }

            return hasData;
        }

        public void Dispose()
        {
            if (this._disposed)
                return;

            this._dataReaderEnumerator.Dispose();
            this._current = default;
            this._disposed = true;
        }


        public void Reset()
        {
            throw new NotSupportedException();
        }
    }
}