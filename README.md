# cmstar.Data

[![NuGet](https://img.shields.io/nuget/v/cmstar.Data.svg)](https://www.nuget.org/packages/cmstar.Data/)

简单的 ADO.net 数据访问客户端和轻量化 ORM 。
- 通用的数据访问客户端。
- 轻量化 ORM ，支持查询结果映射到对象。
- 使用普通对象或匿名对象传参。
- 支持异步（async/await）操作。

支持的 .NET 版本：
- .NET Framework 3.5 或更高版本。异步（async/await）操作需要 .NET Framework 4.5 。
- 支持 .NET Standard 2 的运行时如 .NET Core 2/3 、 .NET 5/6 。

依赖库：
- [cmstar.RapidReflection](https://www.nuget.org/packages/cmstar.RapidReflection/) To emit IL for accessing type members.


## 开始使用

### 安装

通过 Package Manager:
```
Install-Package cmstar.Data
```

或通过 dotnet-cli:
```
dotnet add package cmstar.Data
```

### 数据库入口

`IDbClient` 接口是定义了数据库访问的方法，它的默认实现是 `DbClient` 。
要创建一个 `DbClient` ，需要找到对应数据库驱动里的 `System.Data.Common.DbProviderFactory` 实现。
它在每个驱动里通常是单例的。

下面的例子分别声明了 SQLServer 和 Mysql 的客户端，使用的都是官方驱动。
```csharp
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

public static class Db
{
    // SQLServer 连接到本机的 Northwind 库。
    public static IDbClient Northwind
        => new DbClient("server=.;database=Northwind;trusted_connection=true;", SqlClientFactory.Instance);

    // Mysql 连接到本机的 mysqltest 库。
    public static IDbClient MysqlTest
        => new DbClient("server=127.0.0.1;uid=test;pwd=123456;database=mysqltest", MySqlClientFactory.Instance);
}
```

现在，可以使用 `Db.Northwind` 和 `Db.MysqlTest` 访问对应的数据库了。
类似的，可以创建访问 Oracle，Sqlite 或是其他数据库的客户端，只需要找到对应的 `DbProviderFactory` 实例即可。

#### 关于 MySql.Data 库的一个 bug

官方的 MySql.Data 驱动里，曾经在部分版本（可能很旧）中出现 `MySqlClientFactory.CreateDataAdapter` 方法返回 null 的问题，
导致 `IDbClient.GetDataTable/GetDataSet` 方法不能正常运作。我们可以通过重写次方法修复此问题：
```csharp
using MySql.Data.MySqlClient;

/// <summary>
/// 修复 MySql.Data 库可能在<see cref="DbProviderFactory.CreateDataAdapter"/>返回 null 的问题。
/// </summary>
public class FixedMySqlClientFactory : DbProviderFactoryWrapper
{
    public static readonly FixedMySqlClientFactory Instance = new FixedMySqlClientFactory();

    private FixedMySqlClientFactory() : base(MySqlClientFactory.Instance) { }

    public override DbDataAdapter CreateDataAdapter()
    {
        return base.CreateDataAdapter() ?? new MySqlDataAdapter();
    }
}
```

使用时，不直接使用 `MySqlClientFactory.Instance` 而是改用 `FixedMySqlClientFactory.Instance` 。


## 数据库操作

数据库操作分布在三部分：
- `IDbClient` 是数据库的基础操作，不包含 ORM 部分。
- [`ObjectiveExtension`](#ObjectiveExtension扩展) 定义了 `IDbClient` 的扩展方法，提供轻量化 ORM ，支持使用对象（含匿名对象）传递参数。
- [`IndexingExtension`](#IndexingExtension扩展) 定义了 `IDbClient` 的扩展方法，支持以索引的方法传递参数。



### 基础 CRUD

下面演示 `IDbClient` 的基本用法。

前文已经声明了 Northwind 数据库，它是 SQLServer 的示例库，可以从
[这里](https://github.com/Microsoft/sql-server-samples/tree/master/samples/databases/northwind-pubs) 
下载创建库、表和数据的脚本 `instnwnd.sql` 。

```csharp
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
```

#### 使用参数和调用存储过程

这里演示基于 `IDbClient` 接口方法创建和使用参数、调用存储过程。

在实际使用中，通常使用下文的 `ObjectiveExtension扩展` 或 `IndexingExtension扩展`，避免繁琐的操作。

```csharp
// 使用参数
DbParameter parameter = Db.Northwind.CreateParameter();
parameter.DbType = DbType.String;
parameter.ParameterName = "CustomerID";
parameter.Value = "ALFKI";
parameter.Direction = ParameterDirection.Input;

// 调用存储过程 CustOrderHist @CustomerID
DataSet ds = Db.Northwind.DataSet(
    "CustOrderHist", new[] { parameter }, CommandType.StoredProcedure);

// 使用DbClientParamEx中的扩展方法快速创建参数（需要 using cmstar.Data 命名空间）
DbParameter[] parameters = new[] 
{
    Db.Northwind.CreateParameter("id", DbType.Int32, 115, direction: ParameterDirection.Input),
    Db.Northwind.CreateParameter("name", DbType.String, "Ikura", 5)
};
Db.Northwind.DataSet("SELECT * FROM Products WHERE ProductName=@name OR ProductID=@id", parameters);
```

#### 使用Mapper

`IMapper<T>`接口定义了从`IDataRecord`到`T`类型的映射，可以用过实现该接口，以便从数据库读取并创建特定类型实例及实例的集合。

```csharp
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
```

利用上面的`ProductMapper`，我们可以直接从查询中创建`Product`实例了。
```csharp
// 获取一个实例
Product product = Db.Northwind.Get(
    new ProductMapper(), "SELECT * FROM Products WHERE ProductID=115");

// 获取实例的集合
IList<Product> products = Db.Northwind.List(new ProductMapper(), "SELECT * FROM Products");
```

`Mappers`类中已经定义了部分简单类型的Mapper实现，以便实现便捷的查询。
```csharp
// 使用已定义好的简单 Mapper
IList<string> productNames = Db.Northwind.List(
    Mappers.String(), "SELECT ProductName FROM Products");

IList<int> productIds = Db.Northwind.List(
    Mappers.Int32(), "SELECT ProductID FROM Products");

// 使用实现IConvertible的类型创建Mapper
IList<DateTime> orderDates = Db.Northwind.List(
    Mappers.Convertible<DateTime>(), "SELECT OrderDate FROM Orders");
```

### 使用事务

使用`CreateTransaction`方法来获取一个`ITransactionKeeper`事务容器。获取到的事务容器自身也实现了`IDbClient`，可以在其上进行各种CRUD操作。

事务的最后，别忘了`Commit`。

`ITransactionKeeper`同时也实现了`IDisposable`接口，其`Dispose`方法能够在事务没有提交时进行事务回滚（如果已经提交，则什么也不做），利用这个机制和C#的using语法，可以很方便的编写一个在出现异常时回滚的事务操作。
```csharp
using (ITransactionKeeper tran = Db.Northwind.CreateTransaction())
{
    tran.Execute("UPDATE Products SET ProductName='The Name' WHERE ProductID=115");
    tran.Execute("UPDATE Products SET ProductName='The Name2' WHERE ProductID=118");

    tran.Commit();
}
```

## ObjectiveExtension扩展

在`ObjectiveExtension`类中，定义了一套`IDbClient`的扩展方法，能够使用更快捷的方式进行数据库操作。

### .net对象传参

这些扩展方法具有与`IDbClient`中的方法很类似的签名，但能够接收一个用于存放参数信息的.net对象，以节省许多编码量（是的，和Dapper、ServiceStack.OrmLite很相似）。

通过这些扩展方法，上面使用参数的示例可以这样写了：
```csharp
DataSet ds = Db.Northwind.DataSet(
    "CustOrderHist", new { CustomerID = "ALFKI" }, CommandType.StoredProcedure);

DataTable dt = Db.Northwind.DataTable(
    "SELECT * FROM Products WHERE ProductName=@name OR ProductID=@id",
    new { name = "Ikura", id = 115 });
```

### 获取类型实例

现在不指定Mapper就可以直接进行对象查询了。
```csharp
Product product = Db.Northwind.Get<Product>("SELECT * FROM Products WHERE ProductID=115");
IList<Product> products = Db.Northwind.List<Product>("SELECT * FROM Products");
IList<DateTime> orderDates = Db.Northwind.List<DateTime>("SELECT OrderDate FROM Orders");
```

在这些方法内部，会在运行时动态生成对应的Mapper，并且生成一次以后，信息会被缓存下来，不需要每次都重新创建。当然，因为做了更多的是事情，它还是会比非扩展的原生版本慢那么一点点。

也可以使用匿名对象作为实体模板，在许多场景尤其是处理包含少量字段（但又多于1个）时尤其方便。
```csharp
var template = new { ProductID = 0, ProductName = string.Empty };
var productsByTemplate = Db.Northwind.TemplateList(template, "SELECT * FROM Products");
```

### 关于字段名称的匹配

.net对象的属性和公共字段使用Pascal命名法，但数据库规范中的字段命名法可能不一样，比如MySql的snake_case命名法；而且也有太多的数据库设计使用“意识流”了。为了解决这个命名差异问题，查询结果映射到非匿名对象字段时支持字段名称的模糊匹配，具体规则如下，越靠前的规则优先级越高：

- 查询结果的字段名称和对象字段名称完全一致；
- 大小写不敏感的匹配；例：查询结果字段`goodName`可映射到对象字段`GoodName`。
- 查询结果的字段名称移除下划线（头尾的下划线将保留）之后，再进行大小写不敏感的匹配；例：查询结果字段`good_name`可映射到对象字段`GoodName`；`_goodName`不会映射到`GoodName`，因为头尾的下划线不会被忽略。

字体匹配时，考前的规则将优先进行匹配，没有匹配到的字段再使用下一优先级的规则进行匹配。若所有规则都为命中，则对象字段将在映射中被忽略从而保持字段类型的默认值。

*注意：使用匿名对象作为模板查询时，匿名对象的字段名称需和查询结果的字段名称完全匹配，不支持模糊匹配。*


## IndexingExtension扩展

在`IndexingExtension`类中，定义了另外一套`IDbClient`的扩展方法，能够基于索引访问传入的参数。

记得`string.Format`方法吗：
```csharp
string.Format("My name is {0}, I'm {1} years old.", "John Doe", 8);
```

类似的，这些扩展方法用起来是这个样子的：
```csharp
DataTable dt = Db.Northwind.IxDataTable(
    "SELECT * FROM Products WHERE ProductName=@0 OR ProductID=@1", "Ikura", 115);

IList<Product> products = Db.Northwind.IxList<Product>(
    "SELECT * FROM Products WHERE ProductID IN (@0, @1)", 15, 16);
```

为了避免同`ObjectiveExtension`中的方法歧义，这套扩展方法均在方法名称前增加了“Ix”前缀。

通常在一个地方并不混用两套扩展。Dynamic扩展会更泛用一些，但在一些特定的场景下，使用Indexing扩展也是个好主意。还有，这套扩展方法速度会更快一些。


## AnsiString

类似 Dapper，我们使用相同的思路处理 AnsiString 的问题。有关问题可参考 [这里](https://lowleveldesign.org/2013/05/16/be-careful-with-varchars-in-dapper/)。

为了传递 AnsiString，我们有下面的几种方法：
```csharp
var db = Db.Northwind;
var sql = "SELECT @value";

// 直接传递 DbParameter 实例。
var param = db.CreateParameter();
param.ParameterName = "value";
param.Value = "non-unicode string";
param.DbType = DbType.AnsiString;
param.Size = 50;
db.Execute(sql, param);

// 也可以利用 DbClientParamEx 类中对应 CreateParameter() 扩展方法快速创建 DbParameter。
param = db.CreateParameter("value", DbType.AnsiString, "non-unicode string");
db.Execute(sql, param);

// 使用 DbString 类（没错，长得和 Dapper 一样）。
db.Execute(sql, new { value = new DbString { Value = "non-unicode string", IsAnsi = true } });
db.IxExecute("SELECT @0", new DbString { Value = "non-unicode string", IsAnsi = true });

// 利用 DbClientParamEx 类中 AnsiString() 扩展方法快速创建 DbString。
db.Execute(sql, new { value = "non-unicode string".AnsiString() });
```

显然，AnsiString() 扩展方法是使用起来最简单便捷的。


## 异步方法

.net4.5版的所有数据库操作API均有对应的异步版本，它们具有与非异步版本相同的参数表，方法末尾增加“Async”，并返回`Task`或`Task<T>`，可以在 async/await 上下文中使用：
```csharp
string productName = (string)await Db.Northwind.ScalarAsync(
    "SELECT ProductName FROM Products WHERE ProductID=115");

// Indexing 扩展方法
IList<Product> products = await Db.Northwind.IxListAsync<Product>(
    "SELECT * FROM Products WHERE ProductID IN (@0, @1)", 15, 16);
```

> 注意，由于还没有找到适当的方式，目前 DataTableAsync 和 DataSetAsync 方法实际上不是异步执行的。


## 其他语言的版本

- Golang ： [bunnier/sqlmer](https://github.com/bunnier/sqlmer)