using SqlSugar;

namespace RewardAutoMate;

/// <summary>
/// 定义队列数据实体类
/// </summary>
public class QueueItem
{
    [SugarColumn(IsPrimaryKey = true)]
    public DateTime Date { get; set; }
    public uint Count { get; set; }
}

/// <summary>
/// 从数据库中读取和写入队列数据
/// </summary>
public static class QueueService
{
    private static readonly SqlSugarClient _db;

    static QueueService()
    {
        _db = new SqlSugarClient(new ConnectionConfig()
        {
            ConnectionString = "DataSource=HeatMapData.db",
            DbType = DbType.Sqlite,
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute
        });
        _db.DbMaintenance.CreateDatabase();             // 创建数据库，如果已存在，则没有任何影响
        _db.CodeFirst.InitTables(typeof(QueueItem));    // 创建表
    }

    /// <summary>
    /// 从队列中读取前 365 个数据
    /// </summary>
    /// <returns></returns>
    public static List<QueueItem> DequeueTop365()
    {
        return _db.Queryable<QueueItem>().OrderBy(it => it.Date).Take(365).ToList();
    }

    /// <summary>
    /// 根据 DateTime 修改数据库中的 Count
    /// </summary>
    /// <param name="_date_time"></param>
    /// <param name="_count"></param>
    public static void UpdateItem(QueueItem _item)
    {
        if (_db.Queryable<QueueItem>().InSingle(_item.Date) is null)
        {
            _db.Insertable(_item).ExecuteCommand();
        }
        else
        {
            _db.Updateable<QueueItem>()
               .SetColumns(it => it.Count == _item.Count)
               .Where(it => it.Date == _item.Date)
               .ExecuteCommand();
        }
    }

    public static QueueItem GetTodayCount()
    {
        return _db.Queryable<QueueItem>().InSingle(DateTime.Today)
                ?? new QueueItem() { Date = DateTime.Today, Count = 0 };
    }
}
