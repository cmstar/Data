using System;

namespace cmstar.Data
{
    /// <summary>
    /// A sample class for DB entries.
    /// You can register your own clients here.
    /// </summary>
    public static class Db
    {
        private readonly static Lazy<IDbClient> DefaultProvider
            = new Lazy<IDbClient>(() => new SqlDbClient("server=.;database=Northwind;trusted_connection=true;"));

        public static IDbClient Default
        {
            get { return DefaultProvider.Value; }
        }

#if NET35
        /// <summary>
        /// A System.Lazy{T} implementation for .net 3.5.
        /// </summary>
        internal sealed class Lazy<T>
        {
            private object _block = new object();
            private Func<T> _constructor;
            private bool _isCreated;
            private T _value;

            public Lazy(Func<T> constructor)
            {
                if (constructor == null)
                    throw new ArgumentNullException("constructor");

                _constructor = constructor;
            }

            public T Value
            {
                get
                {
                    // use a double-lock hint to improve performance
                    if (_isCreated)
                        return _value;

                    lock (_block)
                    {
                        if (_isCreated)
                            return _value;

                        _value = _constructor();
                        _isCreated = true;
                    }

                    // release the objects not needed any more
                    _constructor = null;
                    _block = null;

                    return _value;
                }
            }

            public bool IsValueCreated
            {
                get
                {
                    lock (_block)
                    {
                        return _isCreated;
                    }
                }
            }
        }
#endif
    }
}
