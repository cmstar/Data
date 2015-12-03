简单的Ado.net数据访问客户端。


## 数据库访问入口

### 获取IDbClient

在开始之前，先添加一个数据库访问入口。当然，也可以使用任何你喜欢的方式来创建IDbClient（的实现类）实例。

    public static class Db
    {
        private static readonly Dictionary<string, IDbClient> KnownClients
            = new Dictionary<string, IDbClient>();

        public static IDbClient Northwind
        {
            get 
            {
                return GetClient("Northwind", "server=.;database=Northwind;trusted_connection=true;");
            }
        }

        private static IDbClient GetClient(string name, string connectionString)
        {
            IDbClient client;
            if (KnownClients.TryGetValue(name, out client))
                return client;

            lock (KnownClients)
            {
                if (KnownClients.TryGetValue(name, out client))
                    return client;

                // 创建IDbClient的实例
                client = new SqlDbClient(connectionString);
                KnownClients.Add(name, client);
            }

            return client;
        }
    }

现在，可以使用`Db.Northwind`来访问SQLServer的Northwind示例数据库了。

### 访问其他数据库

如果要访问MySql，可以用几行代码实现一个面向MySql的`IDbClient`实现。下面以使用 MySql.Data.dll 作为MySql .net客户端提供器为例。

    /// <summary>
    /// Mysql数据库访问客户端。
    /// </summary>
    public class MysqlDbClient : AbstractDbClient
    {
        private readonly string _connectionString;

        /// <summary>
        /// 使用指定的数据库类型和连接字符串初始化<see cref="SqlDbClient"/>的新实例。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        public MysqlDbClient(string connectionString)
        {
            ArgAssert.NotNullOrEmptyOrWhitespace(connectionString, "connectionString");
            _connectionString = connectionString;
        }

        /// <summary>
        /// 获取当前实例所使用的数据库连接字符串。
        /// </summary>
        public override string ConnectionString
        {
            get { return _connectionString; }
        }

        /// <summary>
        /// 获取当前实例所使用的<see cref="DbProviderFactory"/>实例。
        /// </summary>
        protected override DbProviderFactory Factory
        {
            get { return MySql.Data.MySqlClient.MySqlClientFactory.Instance; }
        }
    }

现在可以创建MySql的访问客户端了：

	IDbClient client = new MysqlDbClient("server=.;database=MySqlDb;uid=user;pwd=password");

类似的，可以创建访问Oracle，Sqlite或是其他数据库的客户端，只需要找到对应的`DbProviderFactory`实例即可。

## 基本数据库操作

### 基础CRUD

    // 查询
    string productName = (string)Db.Northwind.Scalar(
        "SELECT ProductName FROM Products WHERE ProductID=115");
    
    DataTable productTable = Db.Northwind.DataTable("SELECT * FROM Products");

    // 更新
    int affectedRows = Db.Northwind.Execute(
        "UPDATE Products SET ProductName='The Name' WHERE ProductID=115");

    // 在没有命中一行的时候抛出异常
    int expectedSize = 1;
    Db.Northwind.SizedExecute(
        expectedSize, "UPDATE Products SET ProductName='The Name' WHERE ProductID=115");

    // 获取一行
    IDataRecord record = Db.Northwind.GetRow(
        "SELECT ProductName, SupplierID FROM Products WHERE ProductID=115");

    int supplierId = Convert.ToInt32(record["SupplierID"]);

    // 获取一行，仅获取元素值
    object[] itemArray = Db.Northwind.ItemArray(
        "SELECT ProductName, SupplierID FROM Products WHERE ProductID=1");

    supplierId = Convert.ToInt32(itemArray[1]);

    // 在不用在意资源释放的情况下使用DataReader，利用了foreach的机制，在循环结束后DataReader会自动关闭
    IEnumerable<IDataRecord> rows = Db.Northwind.Rows(
        "SELECT ProductName, SupplierID FROM Products WHERE ProductID IN (1, 2, 3)");
    foreach (IDataRecord row in rows)
    {
        Console.WriteLine(row["ProductName"]);
    }

### 使用参数和调用存储过程

    // 使用参数
    DbParameter parameter = Db.Northwind.CreateParameter();
    parameter.DbType = DbType.String;
    parameter.ParameterName = "CustomerID";
    parameter.Value = "ALFKI";
    parameter.Direction = ParameterDirection.Input;
    
    // 调用存储过程 CustOrderHist @CustomerID
    DataSet ds = Db.Northwind.DataSet(
        "CustOrderHist", new[] { parameter }, CommandType.StoredProcedure);

    // 使用DbClientParamEx中的扩展方法快速创建参数（需要using Data命名空间）
    DbParameter[] parameters = new[] 
    {
        Db.Northwind.CreateParameter("id", DbType.Int32, 115, direction: ParameterDirection.Input),
        Db.Northwind.CreateParameter("name", DbType.String, "Ikura", 5)
    };
    Db.Northwind.DataSet("SELECT * FROM Products WHERE ProductName=@name OR ProductID=@id", parameters);

### 使用Mapper

`IMapper<T>`接口定义了从`IDataRecord`到`T`类型的映射，可以用过实现该接口，以便从数据库读取并创建特定类型实例及实例的集合。

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

利用上面的`ProductMapper`，我们可以直接从查询中创建`Product`实例了。

    // 获取一个实例
    Product product = Db.Northwind.Get(
        new ProductMapper(), "SELECT * FROM Products WHERE ProductID=115");

    // 获取实例的集合
    IList<Product> products = Db.Northwind.List(new ProductMapper(), "SELECT * FROM Products");

`Mappers`类中已经定义了部分简单类型的Mapper实现，以便实现便捷的查询。

	// 使用已定义好的简单Mapper
	IList<string> productNames = Db.Northwind.List(
        Mappers.String(), "SELECT ProductName FROM Products");

	IList<int> productIds = Db.Northwind.List(
        Mappers.Int32(), "SELECT ProductID FROM Products");
	
	// 使用实现IConvertible的类型创建Mapper
	IList<DateTime> orderDates = Db.Northwind.List(
        Mappers.Convertible<DateTime>(), "SELECT OrderDate FROM Orders");

### 使用事务

使用`CreateTransaction`方法来获取一个`ITransactionKeeper`事务容器。获取到的事务容器自身也实现了`IDbClient`，可以在其上进行各种CRUD操作。

事务的最后，别忘了`Commit`。

`ITransactionKeeper`同时也实现了`IDisposable`接口，其`Dispose`方法能够在事务没有提交时进行事务回滚（如果已经提交，则什么也不做），利用这个机制和C#的using语法，可以很方便的编写一个在出现异常时回滚的事务操作。

    using (ITransactionKeeper tran = Db.Northwind.CreateTransaction())
    {
        tran.Execute("UPDATE Products SET ProductName='The Name' WHERE ProductID=115");
        tran.Execute("UPDATE Products SET ProductName='The Name2' WHERE ProductID=118");
                
        tran.Commit();
    }


## ObjectiveExtension扩展方法

在`ObjectiveExtension`类中，定义了一套`IDbClient`的扩展方法，能够使用更快捷的方式进行数据库操作。

### .net对象传参

这些扩展方法具有与`IDbClient`中的方法很类似的签名，但能够接收一个用于存放参数信息的.net对象，以节省许多编码量（是的，和Dapper、ServiceStack.OrmLite很相似）。

通过这些扩展方法，上面使用参数的示例可以这样写了：

    DataSet ds = Db.Northwind.DataSet(
        "CustOrderHist", new { CustomerID = "ALFKI" }, CommandType.StoredProcedure);

    DataTable dt = Db.Northwind.DataTable(
        "SELECT * FROM Products WHERE ProductName=@name OR ProductID=@id",
        new { name = "Ikura", id = 115 });

### 获取类型实例

现在不指定Mapper就可以直接进行对象查询了。

    Product product = Db.Northwind.Get<Product>("SELECT * FROM Products WHERE ProductID=115");
    IList<Product> products = Db.Northwind.List<Product>("SELECT * FROM Products");
    IList<DateTime> orderDates = Db.Northwind.List<DateTime>("SELECT OrderDate FROM Orders");

在这些方法内部，会在运行时动态生成对应的Mapper，并且生成一次以后，信息会被缓存下来，不需要每次都重新创建。当然，因为做了更多的是事情，它还是会比非扩展的原生版本慢那么一点点。

也可以使用匿名对象作为实体模板，在许多场景尤其是处理包含少量字段（但又多于1个）时尤其方便。

    var template = new { ProductID = 0, ProductName = string.Empty };
    var productsByTemplate = Db.Northwind.TemplateList(template, "SELECT * FROM Products");

### 关于字段名称的匹配

.net对象的属性和公共字段使用Pascal命名法，但数据库规范中的字段命名法可能不一样，比如MySql的snake_case命名法；而且也有太多的数据库设计使用“意识流”了。为了解决这个命名差异问题，查询结果映射到非匿名对象字段时支持字段名称的模糊匹配，具体规则如下，越靠前的规则优先级越高：

- 查询结果的字段名称和对象字段名称完全一致；
- 大小写不敏感的匹配；例：查询结果字段`goodName`可映射到对象字段`GoodName`。
- 查询结果的字段名称移除下划线（头尾的下划线将保留）之后，再进行大小写不敏感的匹配；例：查询结果字段`good_name`可映射到对象字段`GoodName`；`_goodName`不会映射到`GoodName`，因为头尾的下划线不会被忽略。

字体匹配时，考前的规则将优先进行匹配，没有匹配到的字段再使用下一优先级的规则进行匹配。若所有规则都为命中，则对象字段将在映射中被忽略从而保持字段类型的默认值。

*注意：使用匿名对象作为模板查询时，匿名对象的字段名称需和查询结果的字段名称完全匹配，不支持模糊匹配。*


## IndexingExtension扩展方法

在`IndexingExtension`类中，定义了另外一套`IDbClient`的扩展方法，能够基于索引访问传入的参数。

记得`string.Format`方法吗：

    string.Format("My name is {0}, I'm {1} years old.", "John Doe", 8);

类似的，这些扩展方法用起来是这个样子的：

    DataTable dt = Db.Northwind.IxDataTable(
        "SELECT * FROM Products WHERE ProductName=@0 OR ProductID=@1", "Ikura", 115);

    IList<Product> products = Db.Northwind.IxList<Product>(
        "SELECT * FROM Products WHERE ProductID IN (@0, @1)", 15, 16);

为了避免同`ObjectiveExtension`中的方法歧义，这套扩展方法均在方法名称前增加了“Ix”前缀。

通常在一个地方并不混用两套扩展。Dynamic扩展会更泛用一些，但在一些特定的场景下，使用Indexing扩展也是个好主意。还有，这套扩展方法速度会更快一些。