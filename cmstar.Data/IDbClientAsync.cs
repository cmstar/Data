using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace cmstar.Data
{
    /// <summary>
    /// ����������ݿ���첽������
    /// </summary>
    public interface IDbClientAsync : IDbClient
    {
        /// <summary>
        /// ��ȡ��ѯ�ĵ�һ�е�һ�е�ֵ��
        /// ����һ���첽������
        /// </summary>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeout">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns>��ѯ����ĵ�һ�е�һ�е�ֵ������ѯ�������Ϊ0������<c>null</c>��</returns>
        /// <exception cref="ArgumentNullException">��<paramref name="sql"/>Ϊ<c>null</c>��</exception>
        /// <exception cref="ArgumentException">��<paramref name="sql"/>����Ϊ0��</exception>
        Task<object> ScalarAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0);

        /// <summary>
        /// ִ�зǲ�ѯSQL��䣬��������Ӱ���������
        /// ����һ���첽������
        /// </summary>
        /// <param name="sql">�ǲ�ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeout">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns>SQL��Ӱ���������</returns>
        /// <exception cref="ArgumentNullException">��<paramref name="sql"/>Ϊ<c>null</c>��</exception>
        /// <exception cref="ArgumentException">��<paramref name="sql"/>����Ϊ0��</exception>
        Task<int> ExecuteAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0);

        /// <summary>
        /// ִ�зǲ�ѯSQL��䣬��������Ӱ�����������Ӱ��ĺ�������ȷ���׳��쳣��
        /// ����һ���첽������
        /// </summary>
        /// <param name="expectedSize">�����Ե�Ӱ��������</param>
        /// <param name="sql">�ǲ�ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeout">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <exception cref="IncorrectResultSizeException">��Ӱ�����������ȷ��</exception>
        Task SizedExecuteAsync(int expectedSize,
            string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0);

        /// <summary>
        /// ���ز�ѯ����Ӧ��ѯ�����<see cref="System.Data.DataTable"/>��
        /// ����һ���첽������
        /// </summary>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeout">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns>��ʾ��ѯ�����<see cref="System.Data.DataTable"/>��</returns>
        /// <exception cref="ArgumentNullException">��<paramref name="sql"/>Ϊ<c>null</c>��</exception>
        /// <exception cref="ArgumentException">��<paramref name="sql"/>����Ϊ0��</exception>
        Task<DataTable> DataTableAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0);

        /// <summary>
        /// ���ز�ѯ����Ӧ��ѯ�����<see cref="System.Data.DataSet"/>��
        /// ����һ���첽������
        /// </summary>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeout">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns>��ʾ��ѯ�����<see cref="System.Data.DataSet"/>��</returns>
        /// <exception cref="ArgumentNullException">��<paramref name="sql"/>Ϊ<c>null</c>��</exception>
        /// <exception cref="ArgumentException">��<paramref name="sql"/>����Ϊ0��</exception>
        Task<DataSet> DataSetAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0);

        /// <summary>
        /// �жϸ����Ĳ�ѯ�Ľ���Ƿ����ٰ���1�С�
        /// ����һ���첽������
        /// </summary>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeout">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns>����ѯ������ٰ���1�У�����<c>true</c>�����򷵻�<c>false</c>��</returns>
        /// <exception cref="ArgumentNullException">��<paramref name="sql"/>Ϊ<c>null</c>��</exception>
        /// <exception cref="ArgumentException">��<paramref name="sql"/>����Ϊ0��</exception>
        Task<bool> ExistsAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0);

        /// <summary>
        /// ��ȡ��ѯ����ĵ�һ�м�¼��
        /// ����ѯ���е�����Ϊ0������null��
        /// ����һ���첽������
        /// </summary>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeout">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns><see cref="IDataRecord"/>��ʵ�֣�������ѯ�ĵ�һ�м�¼��</returns>
        /// <exception cref="ArgumentNullException">��<paramref name="sql"/>Ϊ<c>null</c>��</exception>
        /// <exception cref="ArgumentException">��<paramref name="sql"/>����Ϊ0��</exception>
        /// <remarks>
        /// ������<see cref="DbCommand.ExecuteReaderAsync()"/>���÷����˷���ִ����Ϻ󽫲����������ݿ����ӣ�
        /// Ҳ����Ҫ����<see cref="IDisposable.Dispose"/>��
        /// </remarks>
        Task<IDataRecord> GetRowAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0);

        /// <summary>
        /// ��ȡ��ѯ����ĵ�һ�м�¼����������ʽ���ؼ�¼�ڸ��е�ֵ��
        /// ����Ԫ��˳������˳��һ�¡�����ѯ���е�����Ϊ0������null��
        /// ����һ���첽������
        /// </summary>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeout">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns>�����˸��е�ֵ�����顣</returns>
        /// <exception cref="ArgumentNullException">��<paramref name="sql"/>Ϊ<c>null</c>��</exception>
        /// <exception cref="ArgumentException">��<paramref name="sql"/>����Ϊ0��</exception>
        Task<object[]> ItemArrayAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0);

        /// <summary>
        /// ʹ��<see cref="IMapper{T}"/>��ѯָ������
        /// �����������ļ�¼�����ڣ�����Ŀ�����͵�Ĭ��ֵ��������������Ϊ<c>null</c>����
        /// ����һ���첽������
        /// </summary>
        /// <typeparam name="T">��ѯ��Ŀ�����͡�</typeparam>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeout">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <param name="mapper"><see cref="IMapper{T}"/>��ʵ����</param>
        /// <returns>Ŀ�����͵�ʵ����</returns>
        Task<T> GetAsync<T>(IMapper<T> mapper, string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0);

        /// <summary>
        /// ʹ��<see cref="IMapper{T}"/>��ѯָ������
        /// SQL���еļ�¼����Ϊ1�У������׳��쳣��
        /// ����һ���첽������
        /// </summary>
        /// <typeparam name="T">��ѯ��Ŀ�����͡�</typeparam>
        /// <param name="mapper"><see cref="IMapper{T}"/>��ʵ����</param>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeout">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns>Ŀ�����͵�ʵ����</returns>
        /// <exception cref="IncorrectResultSizeException">��SQL���еļ�¼������Ϊ 1��</exception>
        Task<T> ForceGetAsync<T>(IMapper<T> mapper,
            string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0);

        /// <summary>
        /// ʹ��<see cref="IMapper{T}"/>��ѯָ������ļ��ϡ�
        /// ����һ���첽������
        /// </summary>
        /// <typeparam name="T">��ѯ��Ŀ�����͡�</typeparam>
        /// <param name="mapper"><see cref="IMapper{T}"/>��ʵ����</param>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeout">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns>Ŀ�����͵�ʵ���ļ��ϡ�����ѯ���е�����Ϊ0�����ؿռ��ϡ�</returns>
        Task<IList<T>> ListAsync<T>(IMapper<T> mapper,
            string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0);

        /// <summary>
        /// ��ȡ��ѯ����������С�
        /// ����һ���첽������
        /// </summary>
        /// <param name="sql">��ѯSQL��</param>
        /// <param name="parameters">�������С������л�null��ʾû�в�����</param>
        /// <param name="commandType">��������͡�</param>
        /// <param name="timeout">����ĳ�ʱʱ�䣬��λ���롣0Ϊ��ָ����</param>
        /// <returns>��ѯ����������С�</returns>
        Task<IEnumerable<IDataRecord>> RowsAsync(string sql, IEnumerable<DbParameter> parameters = null,
            CommandType commandType = CommandType.Text, int timeout = 0);
    }
}