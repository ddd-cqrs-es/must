using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Permissions;
using NUnit.Framework;
using Nohros.Data;
using Nohros.Dynamics;
using Telerik.JustMock;

namespace Nohros.Common
{
  public class MapperTests
  {
    public class CrmEvent : ICrmEvent
    {
      /// <inheritdoc/>
      public int ID { get; set; }

      /// <inheritdoc/>
      public int TypeID { get; set; }

      /// <inheritdoc/>
      public int AgentID { get; set; }

      /// <inheritdoc/>
      public int ContactID { get; set; }

      /// <inheritdoc/>
      public DateTime Date { get; set; }

      /// <inheritdoc/>
      public Guid ServerID { get; set; }
    }

    public interface ICrmEvent
    {
      /// <summary>
      /// Retorna um numero que identifica o acionamento de forma unica em
      /// um servidor.
      /// </summary>
      /// <remarks>
      /// Este valor esta, geralmente, associado ao ID do registro que contem
      /// os dados do acinamento.
      /// </remarks>
      int ID { get; }

      /// <summary>
      /// Retorna um numero que identifica o tipo de acionamento realizado.
      /// </summary>
      int TypeID { get; }

      /// <summary>
      /// Retorna o codigo do agente que realizou o acionamento.
      /// </summary>
      int AgentID { get; }

      /// <summary>
      /// Retorna um numero que identifica o contato acionado de forma unica em
      /// um servidor.
      /// </summary>
      int ContactID { get; }

      /// <summary>
      /// Retorna a data em que o acionamento foi realizado.
      /// </summary>
      DateTime Date { get; }

      Guid ServerID { get; set; }
    }

    public class IgnoreMapperTest
    {
      public string Name { get; set; }
      public string Location { get; set; }
    }

    public class KeyedMapperTest
    {
      public string Name { get; set; }
    }

    public class MapMapperTest
    {
      public string Name { get; set; }
    }

    public class MapNestedMapperTest
    {
      public MapperTest Nested { get; set; }
    }

    public class MapperDerivedTest : MapperTest
    {
    }

    public class MapperTest
    {
      public string Name { get; set; }
    }

    public class NestedMapperTest
    {
      public MapperTest Nested { get; set; }
    }

    public class PostPoco
    {
      public int Id { get; set; }
      public string Text { get; set; }
      public DateTime CreationDate { get; set; }
      public TimeSpan Counter1 { get; set; }
      public int Counter2 { get; set; }
    }

    [Test]
    [ReflectionPermission(SecurityAction.Demand,
      Flags = ReflectionPermissionFlag.RestrictedMemberAccess | ReflectionPermissionFlag.MemberAccess)]
    public void ShouldMapInternalClass() {
      var builder = new SqlConnectionStringBuilder();
      builder.DataSource = "192.168.203.207";
      builder.UserID = "nohros";
      builder.Password = "Noors03";

      using (var conn = new SqlConnection(builder.ToString()))
      using (
        var cmd =
          new SqlCommand(
            "select 15 as Id, 'post' as text, Getdate() as CreationDate, 9 as Counter1, 10 as Counter2",
            conn)) {
        conn.Open();
        using (var reader = cmd.ExecuteReader()) {
          var mapper = new DataReaderMapperBuilder<PostPoco>()
            .Map<int, TimeSpan>(poco => poco.Counter1, "counter1",
              i => FromSeconds(i))
            .Build();
          //Assert.Pass("Value:" + mapper.Map(reader).Counter1);
          mapper.Map(reader);
        }
      }
       Dynamics_.AssemblyBuilder.Save("dynamics.dll");
    }

    public static TimeSpan FromSeconds(int i ) {
      return TimeSpan.FromSeconds(i);
    }

    [Test]
    public void test() {
      Func<int, TimeSpan> a = i => TimeSpan.FromSeconds(i);
      a(10);
    }

    [Test]
    public void ShouldBuildDynamicType() {
      var reader = Mock.Create<IDataReader>();
      var mapper = new DataReaderMapperBuilder<MapperTest>("MyNamespace")
        .Map("usuario_nome", "name")
        .Build()
        .Map(reader);
      Assert.That(mapper, Is.AssignableTo<DataReaderMapper<MapperTest>>());
    }

    [Test]
    public void ShouldMapMemberExpression() {
      var reader = Mock.Create<IDataReader>();
      var mapper = new DataReaderMapperBuilder<MapperTest>("MyNamespace")
        .Build()
        .Map(reader);
      Assert.That(mapper, Is.AssignableTo<DataReaderMapper<MapperTest>>());
    }

    [Test]
    public void ShouldBuildNestedDynamicType() {
      var reader = Mock.Create<IDataReader>();
      Mock
        .Arrange(() => reader.GetOrdinal(Arg.AnyString))
        .Returns(0);
      var mapper = new DataReaderMapperBuilder<NestedMapperTest>()
        .Map("usuario_nome", "name")
        .Build();
      var inner = mapper.Map(reader).Nested;
      Assert.That(inner, Is.AssignableTo<DataReaderMapper<MapperTest>>());
      Assert.That(inner, Is.AssignableTo<MapperTest>());
      Assert.That(mapper, Is.AssignableTo<DataReaderMapper<NestedMapperTest>>());
      Assert.That(mapper, Is.AssignableTo<NestedMapperTest>());
    }

    [Test]
    public void ShouldBuildDerivedInterface() {
      var mapper = new DataReaderMapperBuilder<MapperDerivedTest>()
        .Build();
      Assert.That(mapper, Is.AssignableTo<DataReaderMapper<MapperDerivedTest>>());
      Assert.That(mapper, Is.AssignableTo<MapperDerivedTest>());
      Assert.That(mapper, Is.AssignableTo<MapperTest>());
    }

    [Test]
    public void ShouldIgnoreProperty() {
      var reader = Mock.Create<IDataReader>();
      Mock
        .Arrange(() => reader.Read())
        .Returns(true);
      Mock
        .Arrange(() => reader.GetString(0))
        .Returns("name-value");

      var mapper =
        new DataReaderMapperBuilder<IgnoreMapperTest>("ShouldIgnoreProperty")
          .AutoMap()
          .Ignore("name")
          .Build();
      Assert.That(() => mapper.Map(reader).Name, Is.Null);
      //Dynamics_.AssemblyBuilder.Save("nohros.tests.dll");
    }

    [Test]
    public void ShouldMapCustomColumnToProperty() {
      var builder = new SqlConnectionStringBuilder();
      builder.DataSource = "192.168.203.207";
      builder.UserID = "nohros";
      builder.Password = "Noors03";

      using (var conn = new SqlConnection(builder.ToString()))
      using (var cmd = new SqlCommand("select 'nohros' as usuario_nome", conn)) {
        conn.Open();
        using (var reader = cmd.ExecuteReader()) {
          var mapper = new DataReaderMapperBuilder<MapMapperTest>()
            .Map("name", "usuario_nome")
            .Build();
          Assert.That(mapper.Map(reader).Name, Is.EqualTo("nohros"));
        }
      }
    }

    [Test]
    public void ShouldMapArrayOfKeyValuePairs() {
      var builder = new SqlConnectionStringBuilder();
      builder.DataSource = "itaucaj";
      builder.UserID = "nohros";
      builder.Password = "Noors03";

      using (var conn = new SqlConnection(builder.ToString()))
      using (var cmd = new SqlCommand("exec fila_get_by_id @fila_id = 1", conn)) {
        conn.Open();
        using (var reader = cmd.ExecuteReader()) {
          var mapper = new DataReaderMapperBuilder<KeyedMapperTest>()
            .Map(
              new KeyValuePair<string, string>[] {
                new KeyValuePair<string, string>("name", "usuario_nome")
              })
            .Build();
          Assert.That(mapper.Map(reader).Name, Is.EqualTo("nohros"));
        }
      }
    }

    [Test]
    public void ShoudMapToConstValues() {
      var reader = Mock.Create<IDataReader>();
      var mapper = new DataReaderMapperBuilder<IgnoreMapperTest>()
        .Map("name", new ConstStringMapType("myname"))
        .Build();
      Assert.That(mapper.Map(reader).Name, Is.EqualTo("myname"));
    }

    [Test]
    public TimeSpan name() {
      return TimeSpan.FromSeconds(10);
    }

    public interface IMapper
    {
      void Map();
    }

    [Test]
    public void dynamic() {
      AppDomain app = AppDomain.CurrentDomain;
      AssemblyName name = new AssemblyName("assemblyB");
      AssemblyBuilder assembly = app
        .DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave);
      ModuleBuilder module = assembly
        .DefineDynamicModule(assembly.GetName().Name, "b.dll");

      TypeBuilder type = module.DefineType("MyType",
        TypeAttributes.Public |
          TypeAttributes.Class |
          TypeAttributes.AutoClass |
          TypeAttributes.AutoLayout, null, new[] {typeof (IMapper)});

      MethodBuilder method = type
        .DefineMethod("Map",
          MethodAttributes.Public |
            MethodAttributes.HideBySig |
            MethodAttributes.Virtual);

      ILGenerator il = method.GetILGenerator();

      Func<int, TimeSpan> func = i => TimeSpan.FromSeconds(i);

      il.Emit(OpCodes.Ldc_I4_0);
      //il.Emit(OpCodes.Call, typeof(TimeSpan).GetMethod("FromSeconds"));
      il.EmitCall(OpCodes.Call, func.Method, null);
      il.Emit(OpCodes.Ret);

      Type t = type.CreateType();

      assembly.Save("b.dll");

      IMapper mapper = (IMapper) Activator.CreateInstance(t);
      mapper.Map();
    }
  }
}
