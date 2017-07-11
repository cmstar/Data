using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace cmstar.Data
{
    internal static class DbClientHelper
    {
        /// <summary>
        /// 从<see cref="IDataRecord"/>序列（一般是<see cref="DbDataReader"/>）中读取1行。
        /// 若序列仅有一行，返回true；否则返回false。
        /// <paramref name="rows"/>读取后将被关闭（Dispose）。
        /// </summary>
        /// <param name="rows">结果集。</param>
        /// <param name="result">读取到的行，方法返回false时为null。</param>
        /// <param name="rowCount">
        /// 读取到的行数，只有三个值：
        /// 0 - 没有读取到记录；
        /// 1 - 读取到了唯一的一行；
        /// 2 - 读取到了第二行，此时终止读取过程并返回false。
        /// </param>
        /// <returns>若序列仅有一行，返回true；否则返回false。</returns>
        public static bool TryReadUniqueRow(IEnumerable<IDataRecord> rows, out IDataRecord result, out int rowCount)
        {
            result = null;
            rowCount = 0;

            using (var enumerator = rows.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    rowCount++;

                    // rows使用 DataReader，迭代完后，reader就关掉了，
                    // 为了使这行记录还可以使用，需要将reader的数据保存下来。
                    result = new SingleRowKeeper(enumerator.Current);
                }

                // 只能有一行！
                if (!enumerator.MoveNext())
                    return true;

                // 多于1行，返回false
                rowCount++;
                result = null;
                return false;
            }
        }

        /// <summary>
        /// <see cref="IDataReader"/>中读取1行。
        /// 若序列仅有一行，返回true；否则返回false。
        /// <paramref name="reader"/>读取后 *不会* 被关闭（Close）。
        /// </summary>
        /// <param name="reader">结果集。</param>
        /// <param name="result">读取到的行，方法返回false时为null。</param>
        /// <param name="rowCount">
        /// 读取到的行数，只有三个值：
        /// 0 - 没有读取到记录；
        /// 1 - 读取到了唯一的一行；
        /// 2 - 读取到了第二行，此时终止读取过程并返回false。
        /// </param>
        /// <returns>若序列仅有一行，返回true；否则返回false。</returns>
        public static bool TryReadUniqueRow(IDataReader reader, out IDataRecord result, out int rowCount)
        {
            result = null;
            rowCount = 0;

            if (reader.Read())
            {
                rowCount++;

                // 持久化此记录
                result = new SingleRowKeeper(reader);
            }

            // 只能有一行！
            if (!reader.Read())
                return true;

            // 多于1行，返回false
            rowCount++;
            result = null;
            return false;
        }

        /// <summary>
        /// <see cref="IDataReader"/>中读取1行，将其映射到指定类型的对象。
        /// 若序列仅有一行，返回true；否则返回false。
        /// <paramref name="reader"/>读取后 *不会* 被关闭（Close）。
        /// </summary>
        /// <param name="reader">结果集。</param>
        /// <param name="mapper"><see cref="IMapper{T}"/>的实例。</param>
        /// <param name="result">读取到的行，方法返回false时为null。</param>
        /// <param name="rowCount">
        /// 读取到的行数，只有三个值：
        /// 0 - 没有读取到记录；
        /// 1 - 读取到了唯一的一行；
        /// 2 - 读取到了第二行，此时终止读取过程并返回false。
        /// </param>
        /// <returns>若序列仅有一行，返回true；否则返回false。</returns>
        public static bool TryMapUniqueRow<T>(IDataReader reader, IMapper<T> mapper, out T result, out int rowCount)
        {
            result = default(T);
            rowCount = 0;

            if (reader.Read())
            {
                rowCount++;
                result = mapper.MapRow(reader, 1);
            }

            // 只能有一行！
            if (!reader.Read())
                return true;

            // 多于1行，返回false
            rowCount++;
            result = default(T);
            return false;
        }
    }
}
