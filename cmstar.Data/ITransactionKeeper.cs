using System;

namespace cmstar.Data
{
    /// <summary>
    /// 定义数据库事务容器。
    /// </summary>
    public interface ITransactionKeeper : IDisposable, IDbClient
    {
        /// <summary>
        /// 提交事务。
        /// </summary>
        void Commit();

        /// <summary>
        /// 回滚事务。
        /// </summary>
        void Rollback();
    }
}
