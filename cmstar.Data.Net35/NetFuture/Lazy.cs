using System;

namespace cmstar.Data.NetFuture
{
    /// <summary>
    /// A System.Lazy{T} implementation for .net 3.5.
    /// </summary>
    public sealed class Lazy<T>
    {
        private object _block = new object();
        private Func<T> _constructor;
        private T _value;

        public Lazy(Func<T> constructor)
        {
            if (constructor == null)
                throw new ArgumentNullException(nameof(constructor));

            _constructor = constructor;
        }

        public bool IsValueCreated { get; private set; }

        public T Value
        {
            get
            {
                // use a double-lock hint to improve performance
                if (IsValueCreated)
                    return _value;

                lock (_block)
                {
                    if (IsValueCreated)
                        return _value;

                    _value = _constructor();
                    IsValueCreated = true;
                }

                // release the objects not needed any more
                _constructor = null;
                _block = null;

                return _value;
            }
        }
    }
}
