using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace cmstar.Data
{
    class Program
    {
        static void Main()
        {
            BasicDemo();
            TransactionDemo();
            DynamicExtensionDemo();
            IndexingExtensionDemo();
            AsyncDemo().Wait();

            Console.WriteLine("Done!!");
            Console.ReadKey();
        }

        private static async Task AsyncDemo()
        {
            Action<string> log = msg =>
            {
                Console.WriteLine($"[AsyncDemo workder {Thread.CurrentThread.ManagedThreadId}] {msg}");
            };

            log("Calling DataTableAsync ...");
            DataTable dt = await Db.Northwind.DataTableAsync(
                "SELECT ProductID, ProductName, UnitPrice FROM Products WHERE ProductName=@name OR ProductID=@id",
                new { name = "Ikura", id = 115 });
            var row = dt.Rows[0];
            log($"row0 - ProductName: {row[1]}, UnitPrice: {row[2]}.");

            log("Calling IxGetAsync ...");
            Product product = await Db.Northwind.IxGetAsync<Product>("SELECT * FROM Products WHERE ProductID=@0", 1);
            log($"ProductID: {product.ProductID}, ProductName: {product.ProductName}.");

            log("Calling ListAsync ...");
            IList<DateTime> orderDates = await Db.Northwind.ListAsync<DateTime>("SELECT OrderDate FROM Orders");
            log($"Length: {orderDates.Count}, first: {(orderDates.Count > 0 ? orderDates[0].ToString() : "<nil>")}");

            log("Calling TemplateListAsync ...");
            var template = new { ProductID = 0, ProductName = string.Empty };
            var products = await Db.Northwind.TemplateListAsync(template, "SELECT * FROM Products");
            log($"Length: {products.Count}, first: {(products.Count > 0 ? products[0].ProductName : "")}.");

            log("Testing async transaction ...");
            const int id = 6;
            using (var tran = Db.Northwind.CreateTransaction())
            {

                log($"Before update: {await tran.IxScalarAsync("SELECT UnitsInStock FROM Products WHERE ProductID=7", id)}");

                await tran.IxExecuteAsync(
                    "UPDATE Products SET UnitsInStock=UnitsInStock-1 WHERE ProductID=@0 AND UnitsInStock>0", 7);

                log($"After update: {await tran.IxScalarAsync("SELECT UnitsInStock FROM Products WHERE ProductID=7", id)}");

                // 递归事务
                using (var tran2 = tran.CreateTransaction())
                {
                    await tran2.IxExecuteAsync(
                        "UPDATE Products SET UnitsInStock=UnitsInStock-1 WHERE ProductID=@0 AND UnitsInStock>0", 7);

                    tran2.Commit();
                }

                log($"After 2nd update: {await tran.IxScalarAsync("SELECT UnitsInStock FROM Products WHERE ProductID=7", id)}");

                // 不提交，连同递归的事务中的提交内容一起回滚
            }
            log($"After rollback: {await Db.Northwind.IxScalarAsync("SELECT UnitsInStock FROM Products WHERE ProductID=7", id)}");
        }

        // ReSharper disable UnusedVariable
        private static void DynamicExtensionDemo()
        {
            DataSet ds = Db.Northwind.DataSet(
                "CustOrderHist", new { CustomerID = "ALFKI" }, CommandType.StoredProcedure);

            DataTable dt = Db.Northwind.DataTable(
                "SELECT * FROM Products WHERE ProductName=@name OR ProductID=@id",
                new { name = "Ikura", id = 115 });

            // 获取实例的集合
            Product product = Db.Northwind.Get<Product>("SELECT * FROM Products WHERE ProductID=115");
            IList<Product> products = Db.Northwind.List<Product>("SELECT * FROM Products");
            IList<DateTime> orderDates = Db.Northwind.List<DateTime>("SELECT OrderDate FROM Orders");

            // 使用匿名对象作为模板
            var template = new { ProductID = 0, ProductName = string.Empty };
            var productsByTemplate = Db.Northwind.TemplateList(template, "SELECT * FROM Products");
        }

        private static void IndexingExtensionDemo()
        {
            DataTable dt = Db.Northwind.IxDataTable(
                "SELECT * FROM Products WHERE ProductName=@0 OR ProductID=@1", "Ikura", 115);

            IList<Product> products = Db.Northwind.IxList<Product>(
                "SELECT * FROM Products WHERE ProductID IN (@0, @1)", 15, 16);
        }

        private static void TransactionDemo()
        {
            using (ITransactionKeeper tran = Db.Northwind.CreateTransaction())
            {
                Db.Northwind.Execute("UPDATE Products SET ProductName='The Name' WHERE ProductID=115");
                Db.Northwind.Execute("UPDATE Products SET ProductName='The Name2' WHERE ProductID=118");

                tran.Commit();
            }
        }

        private static void BasicDemo()
        {
            // 查询
            string productName = (string)Db.Northwind.Scalar("SELECT ProductName FROM Products WHERE ProductID=115");
            DataTable productTable = Db.Northwind.DataTable("SELECT * FROM Products");

            // 更新
            int affectedRows = Db.Northwind.Execute("UPDATE Products SET ProductName='The Name' WHERE ProductID=115");

            // 在没有命中一行的时候抛出异常
            int expectedSize = 0;
            Db.Northwind.SizedExecute(expectedSize, "UPDATE Products SET ProductName='The Name' WHERE ProductID=115");

            // 获取一行
            IDataRecord record = Db.Northwind.GetRow("SELECT ProductName, SupplierID FROM Products WHERE ProductID=1");
            int supplierId = Convert.ToInt32(record["SupplierID"]);

            // 获取一行，仅获取元素值
            object[] itemArray = Db.Northwind.ItemArray("SELECT ProductName, SupplierID FROM Products WHERE ProductID=1");
            supplierId = Convert.ToInt32(itemArray[1]);

            // 在不用在意资源释放的情况下使用DataReader，利用了foreach的机制，在循环结束后DataReader会自动关闭
            IEnumerable<IDataRecord> rows = Db.Northwind.Rows("SELECT ProductName, SupplierID FROM Products WHERE ProductID IN (1, 2, 3)");
            foreach (IDataRecord row in rows)
            {
                Console.WriteLine(row["ProductName"]);
            }

            // 使用参数
            DbParameter parameter = Db.Northwind.CreateParameter();
            parameter.DbType = DbType.String;
            parameter.ParameterName = "CustomerID";
            parameter.Value = "ALFKI";
            parameter.Direction = ParameterDirection.Input;
            // 调用存储过程 CustOrderHist @CustomerID
            DataSet ds = Db.Northwind.DataSet("CustOrderHist", new[] { parameter }, CommandType.StoredProcedure);

            // 使用DbClientParamEx中的扩展方法快速创建参数（需要using Data命名空间）
            DbParameter[] parameters =
            {
                Db.Northwind.CreateParameter("id", DbType.Int32, 115, direction: ParameterDirection.Input),
                Db.Northwind.CreateParameter("name", DbType.String, "Ikura", 5)
            };
            Db.Northwind.DataSet("SELECT * FROM Products WHERE ProductName=@name OR ProductID=@id", parameters);

            // 获取一个实例
            Product product = Db.Northwind.Get(new ProductMapper(), "SELECT * FROM Products WHERE ProductID=115");

            // 获取实例的集合
            IList<Product> products = Db.Northwind.List(new ProductMapper(), "SELECT * FROM Products");

            // 使用已定义好的简单Mapper
            IList<string> productNames = Db.Northwind.List(Mappers.String(), "SELECT ProductName FROM Products");
            IList<int> productIds = Db.Northwind.List(Mappers.Int32(), "SELECT ProductID FROM Products");

            // 使用实现IConvertible的类型创建Mapper
            IList<DateTime> orderDates = Db.Northwind.List(Mappers.Convertible<DateTime>(), "SELECT OrderDate FROM Orders");
        }
    }

    public class Product
    {
        public int ProductID;
        public string ProductName;
    }

    public class ProductMapper : IMapper<Product>
    {
        public Product MapRow(IDataRecord record, int rowNum)
        {
            var product = new Product();
            product.ProductID = Convert.ToInt32(record["ProductID"]);
            product.ProductName = record["ProductName"].ToString();
            return product;
        }
    }
}
