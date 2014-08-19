using System;
using System.Collections.Generic;
using System.Data.Common;

namespace cmstar.Data.Dynamic
{
    /// <summary>
    /// Keeps the information for a <see cref="DbCommand"/> to be executed.
    /// </summary>
    internal class CommandCacheItem
    {
        /// <summary>
        /// The command text.
        /// </summary>
        public string Sql;

        /// <summary>
        /// A function returns a series of <see cref="DbParameter"/>.
        /// The function accepts an instance of <see cref="IDbClient"/> as the first parameter,
        /// the sencond is an object which keeps the values of parameters for the <see cref="DbCommand"/>.
        /// </summary>
        public Func<IDbClient, object, IEnumerable<DbParameter>> Params;

        /// <summary>
        /// The instance of <see cref="IMapper{T}"/>.
        /// </summary>
        public object Mapper;

        /// <summary>
        /// A counter specifies how many times this cahce item is reused.
        /// </summary>
        public int Reused;
    }
}
