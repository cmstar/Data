using System.Data.Common;

#if !NETSTANDARD
using System.Security;
using System.Security.Permissions;
#endif

namespace cmstar.Data
{
    /// <summary>
    /// 用于重写目标<see cref="DbProviderFactory"/>的方法。
    /// 当忙目标<see cref="DbProviderFactory"/>不允许继承时，使用此类型以实现重写目的。
    /// </summary>
    public abstract class DbProviderFactoryWrapper : DbProviderFactory
    {
        private readonly DbProviderFactory _underlyingProvider;

        /// <summary>
        /// 创建类型的新实例。
        /// </summary>
        protected DbProviderFactoryWrapper(DbProviderFactory underlyingProvider)
        {
            ArgAssert.NotNull(underlyingProvider, nameof(underlyingProvider));
            _underlyingProvider = underlyingProvider;
        }

        /// <inheritdoc />
        public override bool CanCreateDataSourceEnumerator => _underlyingProvider.CanCreateDataSourceEnumerator;

        /// <inheritdoc />
        public override DbCommand CreateCommand()
        {
            return _underlyingProvider.CreateCommand();
        }

        /// <inheritdoc />
        public override DbCommandBuilder CreateCommandBuilder()
        {
            return _underlyingProvider.CreateCommandBuilder();
        }

        /// <inheritdoc />
        public override DbConnection CreateConnection()
        {
            return _underlyingProvider.CreateConnection();
        }

        /// <inheritdoc />
        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return _underlyingProvider.CreateConnectionStringBuilder();
        }

        /// <inheritdoc />
        public override DbDataAdapter CreateDataAdapter()
        {
            return _underlyingProvider.CreateDataAdapter();
        }

        /// <inheritdoc />
        public override DbDataSourceEnumerator CreateDataSourceEnumerator()
        {
            return _underlyingProvider.CreateDataSourceEnumerator();
        }

        /// <inheritdoc />
        public override DbParameter CreateParameter()
        {
            return _underlyingProvider.CreateParameter();
        }

#if !NETSTANDARD
        /// <inheritdoc />
        public override CodeAccessPermission CreatePermission(PermissionState state)
        {
            return _underlyingProvider.CreatePermission(state);
        }
#endif

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return _underlyingProvider.Equals(obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _underlyingProvider.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _underlyingProvider.ToString();
        }
    }
}
