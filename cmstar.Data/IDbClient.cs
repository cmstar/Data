using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace cmstar.Data
{
    /// <summary>
    /// �������ݿ���ʿͻ��ˡ�
    /// </summary>
    public interface IDbClient
    {
        /// <summary>
        /// ��ȡ��ǰʵ����ʹ�õ����ݿ������ַ�����
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// ��ȡ��ѯ�ĵ�һ�е�һ�е�ֵ��
        /// </summary>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeOut">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns>��ѯ����ĵ�һ�е�һ�е�ֵ������ѯ�������Ϊ0������<c>null</c>��</returns>
        /// <exception cref="ArgumentNullException">��<paramref name="sql"/>Ϊ<c>null</c>��</exception>
        /// <exception cref="ArgumentException">��<paramref name="sql"/>����Ϊ0��</exception>
        object Scalar(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

        /// <summary>
        /// ִ�зǲ�ѯSQL��䣬��������Ӱ���������
        /// </summary>
        /// <param name="sql">�ǲ�ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeOut">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns>SQL��Ӱ���������</returns>
        /// <exception cref="ArgumentNullException">��<paramref name="sql"/>Ϊ<c>null</c>��</exception>
        /// <exception cref="ArgumentException">��<paramref name="sql"/>����Ϊ0��</exception>
        int Execute(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

        /// <summary>
        /// ִ�зǲ�ѯSQL��䣬��������Ӱ�����������Ӱ��ĺ�������ȷ���׳��쳣��
        /// </summary>
        /// <param name="expectedSize">�����Ե�Ӱ��������</param>
        /// <param name="sql">�ǲ�ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeOut">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <exception cref="IncorrectResultSizeException">��Ӱ�����������ȷ��</exception>
        void SizedExecute(int expectedSize,
            string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

        /// <summary>
        /// ���ز�ѯ����Ӧ��ѯ�����<see cref="System.Data.DataTable"/>��
        /// </summary>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeOut">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns>��ʾ��ѯ�����<see cref="System.Data.DataTable"/>��</returns>
        /// <exception cref="ArgumentNullException">��<paramref name="sql"/>Ϊ<c>null</c>��</exception>
        /// <exception cref="ArgumentException">��<paramref name="sql"/>����Ϊ0��</exception>
        DataTable DataTable(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

        /// <summary>
        /// ���ز�ѯ����Ӧ��ѯ�����<see cref="System.Data.DataSet"/>��
        /// </summary>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeOut">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns>��ʾ��ѯ�����<see cref="System.Data.DataSet"/>��</returns>
        /// <exception cref="ArgumentNullException">��<paramref name="sql"/>Ϊ<c>null</c>��</exception>
        /// <exception cref="ArgumentException">��<paramref name="sql"/>����Ϊ0��</exception>
        DataSet DataSet(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

        /// <summary>
        /// �жϸ����Ĳ�ѯ�Ľ���Ƿ����ٰ���1�С�
        /// </summary>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeOut">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns>����ѯ������ٰ���1�У�����<c>true</c>�����򷵻�<c>false</c>��</returns>
        /// <exception cref="ArgumentNullException">��<paramref name="sql"/>Ϊ<c>null</c>��</exception>
        /// <exception cref="ArgumentException">��<paramref name="sql"/>����Ϊ0��</exception>
        bool Exists(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

        /// <summary>
        /// ��ȡ��ѯ����ĵ�һ�м�¼��
        /// ����ѯ���е�����Ϊ0������null��
        /// </summary>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeOut">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns><see cref="IDataRecord"/>��ʵ�֣�������ѯ�ĵ�һ�м�¼��</returns>
        /// <exception cref="ArgumentNullException">��<paramref name="sql"/>Ϊ<c>null</c>��</exception>
        /// <exception cref="ArgumentException">��<paramref name="sql"/>����Ϊ0��</exception>
        /// <remarks>
        /// ������<see cref="DbCommand.ExecuteReader()"/>���÷����˷���ִ����Ϻ󽫲����������ݿ����ӣ�
        /// Ҳ����Ҫ����<see cref="IDisposable.Dispose"/>��
        /// </remarks>
        IDataRecord GetRow(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

        /// <summary>
        /// ��ȡ��ѯ����ĵ�һ�м�¼����������ʽ���ؼ�¼�ڸ��е�ֵ��
        /// ����Ԫ��˳������˳��һ�¡�����ѯ���е�����Ϊ0������null��
        /// </summary>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeOut">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns>�����˸��е�ֵ�����顣</returns>
        /// <exception cref="ArgumentNullException">��<paramref name="sql"/>Ϊ<c>null</c>��</exception>
        /// <exception cref="ArgumentException">��<paramref name="sql"/>����Ϊ0��</exception>
        object[] ItemArray(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

        /// <summary>
        /// ʹ��<see cref="IMapper{T}"/>��ѯָ������
        /// �����������ļ�¼�����ڣ�����Ŀ�����͵�Ĭ��ֵ��������������Ϊ<c>null</c>����
        /// </summary>
        /// <typeparam name="T">��ѯ��Ŀ�����͡�</typeparam>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeOut">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <param name="mapper"><see cref="IMapper{T}"/>��ʵ����</param>
        /// <returns>Ŀ�����͵�ʵ����</returns>
        T Get<T>(IMapper<T> mapper, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

        /// <summary>
        /// ʹ��<see cref="IMapper{T}"/>��ѯָ������
        /// SQL���еļ�¼����Ϊ1�У������׳��쳣��
        /// </summary>
        /// <typeparam name="T">��ѯ��Ŀ�����͡�</typeparam>
        /// <param name="mapper"><see cref="IMapper{T}"/>��ʵ����</param>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeOut">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns>Ŀ�����͵�ʵ����</returns>
        /// <exception cref="IncorrectResultSizeException">��SQL���еļ�¼������Ϊ 1��</exception>
        T ForceGet<T>(IMapper<T> mapper,
            string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

        /// <summary>
        /// ʹ��<see cref="IMapper{T}"/>��ѯָ������ļ��ϡ�
        /// </summary>
        /// <typeparam name="T">��ѯ��Ŀ�����͡�</typeparam>
        /// <param name="mapper"><see cref="IMapper{T}"/>��ʵ����</param>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeOut">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns>Ŀ�����͵�ʵ���ļ��ϡ�����ѯ���е�����Ϊ0�����ؿռ��ϡ�</returns>
        IList<T> List<T>(IMapper<T> mapper,
            string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

        /// <summary>
        /// ��ȡ��ѯ����������С�
        /// </summary>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeOut">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns>��ѯ����������С�</returns>
        IEnumerable<IDataRecord> Rows(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeOut = 0);

        /// <summary>
        /// ��������������
        /// </summary>
        /// <returns><see cref="ITransactionKeeper"/>��</returns>
        ITransactionKeeper CreateTransaction();

        /// <summary>
        /// ����һ���µ�SQL����ʵ����
        /// </summary>
        /// <returns><see cref="DbParameter"/>��ʵ����</returns>
        DbParameter CreateParameter();
    }
}