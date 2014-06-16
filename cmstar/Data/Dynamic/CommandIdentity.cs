using System;
using System.Data;

namespace cmstar.Data.Dynamic
{
    /// <summary>
    /// Identity of a cached command.
    /// </summary>
    internal class CommandIdentity : IEquatable<CommandIdentity>
    {
        private readonly CommandType? _commandType;
        private readonly string _connectionString;
        private readonly string _sql;
        private readonly int _hashCode;
        private readonly Type _parametersType;
        private readonly Type _resultType;

        public CommandIdentity(string connectionString, string sql,
            CommandType? commandType, Type parametersType, Type resultType)
        {
            _sql = sql;
            _commandType = commandType;
            _connectionString = connectionString;
            _parametersType = parametersType;
            _resultType = resultType;

            unchecked
            {
                _hashCode = 17; // we *know* we are using this in a dictionary, so pre-compute this
                _hashCode = _hashCode * 23 + commandType.GetHashCode();
                _hashCode = _hashCode * 23 + (sql == null ? 0 : sql.GetHashCode());
                _hashCode = _hashCode * 23 + (resultType == null ? 0 : resultType.GetHashCode());
                _hashCode = _hashCode * 23 + (parametersType == null ? 0 : parametersType.GetHashCode());
                _hashCode = _hashCode * 23 + (connectionString == null ? 0 : _connectionString.GetHashCode());
            }
        }

        public bool Equals(CommandIdentity other)
        {
            return other != null
                && _sql == other._sql
                && _commandType == other._commandType
                && _parametersType == other._parametersType
                && _connectionString == other._connectionString
                && _resultType == other._resultType;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CommandIdentity);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
