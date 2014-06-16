using System.Data;

namespace cmstar.Data
{
    /// <summary>
    /// 定义<see cref="IDataRecord"/>到指定类型的映射。
    /// </summary>
    /// <typeparam name="T">目标类型。</typeparam>
    public interface IMapper<out T>
    {
        /// <summary>
        /// 从<see cref="IDataRecord"/>获取目标类型的数据。
        /// </summary>
        /// <param name="record"><see cref="IDataRecord"/>的实例。</param>
        /// <param name="rowNum"><see cref="IDataRecord"/>所在的行号。</param>
        /// <returns>目标类型。</returns>
        T MapRow(IDataRecord record, int rowNum);
    }
}
