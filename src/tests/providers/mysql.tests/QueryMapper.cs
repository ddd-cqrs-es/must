using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using Nohros.Data;
using Nohros.Data.MySql;
using Nohros.Dynamics;

namespace mysql.tests
{
  public class LogIn
  {
    public LogIn()
    {
      TypeId = 1000;
    }

    public long Id { get; set; }
    public int TypeId { get; set; }
    public DateTime Date { get; set; }
    public int AgentId { get; set; }
    public DateTime EndDate { get; set; }
    public string Queue { get; set; }
    public bool IsLoggedOut { get; set; }
  }

  public class QueryMapper
  {
    public static IDataReaderMapper<LogIn> LogInDto() {
      return new DataReaderMapperBuilder<LogIn>()
        .Map(x => x.AgentId, "agente")
        .Map(x => x.Date, "dtini")
        .Map(x => x.EndDate, "dtfim")
        .Map(x => x.Id, "id", typeof (int))
        .Map(x => x.Queue, "filas")
        .Map(x => x.TypeId, "tipo_id")
        .Map(x => x.IsLoggedOut, "loggedout", typeof (bool))
        .Build();
    }

    [Test]
    public void MySql() {
      List<LogIn> events = GetLogInsWhichIdIsBetween(12, 22).ToList();
      foreach (var log_in in events) {
        Console.WriteLine(log_in.Id);
      }
    }

    public IEnumerable<LogIn> GetLogInsWhichIdIsBetween(long begin, long end) {
      const string kExecute = @"
select id
  ,agente
  ,dtini
  ,dtfim
  ,filas
  ,1000 as tipo_id
  ,dtfim >= dtini as loggedout
from login
where id between @begin and @end
order by id asc
";
      var mysql_connection_provider =
        new MySqlConnectionProvider(
          "Data Source=192.168.203.207;Initial Catalog=acaconb1;User ID=callflex;Password=$callflex$;");

      var mysql_executor =
        new MySqlQueryExecutor(mysql_connection_provider);

      /*using (
        var conn =
          new MySqlConnection(
            "Data Source=192.168.203.207;Initial Catalog=acaconb1;User ID=callflex;Password=$callflex$;")
        )
      using (var builder = new CommandBuilder(conn)) {
        IDbCommand cmd =
          builder
            .AddParameter("@begin", begin)
            .AddParameter("@end", end)
            .SetText(kExecute)
            .Build();
        conn.Open();
        using (IDataReader reader = cmd.ExecuteReader()) {
          int[] ordinals = new int[] {
            reader.GetOrdinal("id"), reader.GetOrdinal("tipo_id"),
            reader.GetOrdinal("dtini"), reader.GetOrdinal("agente"),
            reader.GetOrdinal("dtfim"), reader.GetOrdinal("filas"),
            reader.GetOrdinal("loggedout")
          };
          //return Map(reader);
        }
        conn.Close();
      }*/
      using (IQueryMapper<LogIn> mapper =
        mysql_executor
          .ExecuteQuery(kExecute,
            LogInDto,
            builder =>
              builder
                .AddParameter("@begin", begin)
                .AddParameter("@end", end))) {
                  IEnumerable<LogIn> logins = mapper.Map(false);
        //IEnumerable<LogIn> logins = Map(((QueryMapper<LogIn>) mapper).Reader);
        return logins;
      }
      //return Enumerable.Empty<LogIn>();
    }

    public IEnumerable<LogIn> Map(IDataReader reader) {
      var list = new List<LogIn>();
      while (reader.Read()) {
        list.Add(MapInternal(reader));
      }
      return list;
    }

    public LogIn MapInternal(IDataReader reader1) {
      int[] ordinals = new int[] {
        reader1.GetOrdinal("id"), reader1.GetOrdinal("tipo_id"),
        reader1.GetOrdinal("dtini"), reader1.GetOrdinal("agente"),
        reader1.GetOrdinal("dtfim"), reader1.GetOrdinal("filas"),
        reader1.GetOrdinal("loggedout")
      };
      var @in = new LogIn();
      @in.Id = reader1.GetInt32(ordinals[0]);
      @in.TypeId = reader1.GetInt32(ordinals[1]);
      @in.Date = reader1.GetDateTime(ordinals[2]);
      @in.AgentId = reader1.GetInt32(ordinals[3]);
      @in.EndDate = reader1.GetDateTime(ordinals[4]);
      @in.Queue = reader1.GetString(ordinals[5]);
      @in.IsLoggedOut = reader1.GetBoolean(ordinals[6]);
      return @in;
    }

 

  }
}
