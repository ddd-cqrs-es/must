using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Nohros.Data;
using Telerik.JustMock;
using Telerik.JustMock.Helpers;

namespace Nohros.Common
{
  public class MapperTests
  {
    public interface ISimpleType
    {
      string Name { get; }
    }

    public class SimpleType : ISimpleType
    {
      public string Name { get; set; }
    }

    public class NestedType
    {
      public SimpleType Nested { get; set; }
    }

    public class DerivedType : SimpleType
    {
    }

    public class IgnoreType
    {
      public string Name { get; set; }
      public string Location { get; set; }
    }

    public interface IDerivedType : ISimpleType
    {
      string Name2 { get; }
    }

    public interface IDerivedType2 : IDerivedType
    {
      string Name3 { get; }
    }

    [Test]
    public void should_map_interfaces_when_no_factory_supplied() {
      IDataReader reader = GetDataReader();
      Mock
        .Arrange(() => reader.FieldCount)
        .Returns(1);
      Mock
        .Arrange(() => reader.GetName(0))
        .Returns("name");
      Mock
        .Arrange(() => reader.GetOrdinal("name"))
        .Returns(0);
      Mock
        .Arrange(() => reader.GetString(0))
        .Returns("nohros");
      var mapper =
        new DataReaderMapperBuilder<ISimpleType>(
          "should_map_interfaces_when_no_factory_supplied")
          .Map(x => x.Name, "name")
          .Build();
      ISimpleType obj = mapper.Map(reader);
      Assert.That(obj.Name, Is.EqualTo("nohros"));
    }

    [Test]
    public void should_map_all_implemented_interfaces() {
      IDataReader reader = GetDataReader();
      Mock
        .Arrange(() => reader.FieldCount)
        .Returns(3);
      Mock
        .Arrange(() => reader.GetName(0))
        .Returns("name");
      Mock
        .Arrange(() => reader.GetName(1))
        .Returns("name1");
      Mock
        .Arrange(() => reader.GetName(2))
        .Returns("name2");
      Mock
        .Arrange(() => reader.GetOrdinal("name"))
        .Returns(0);
      Mock
        .Arrange(() => reader.GetOrdinal("name1"))
        .Returns(1);
      Mock
        .Arrange(() => reader.GetOrdinal("name2"))
        .Returns(2);
      Mock
        .Arrange(() => reader.GetString(0))
        .Returns("nohros");
      Mock
        .Arrange(() => reader.GetString(1))
        .Returns("nohros1");
      Mock
        .Arrange(() => reader.GetString(2))
        .Returns("nohros2");

      var mapper =
        new DataReaderMapperBuilder<IDerivedType2>(
          "should_map_all_implemented_interfaces")
          .Map(x => x.Name, "name")
          .Map(x => x.Name2, "name1")
          .Map(x => x.Name3, "name2")
          .Build();

      IDerivedType2 obj = mapper.Map(reader);
      Assert.That(obj.Name, Is.EqualTo("nohros"));
      Assert.That(obj.Name2, Is.EqualTo("nohros1"));
      Assert.That(obj.Name3, Is.EqualTo("nohros2"));
    }

    [Test]
    public void should_map_interfaces_when_factory_supplied() {
      IDataReader reader = GetDataReader();
      Mock
        .Arrange(() => reader.FieldCount)
        .Returns(1);
      Mock
        .Arrange(() => reader.GetName(0))
        .Returns("name");
      Mock
        .Arrange(() => reader.GetOrdinal("name"))
        .Returns(0);
      Mock
        .Arrange(() => reader.GetString(0))
        .Returns("nohros");
      var mapper =
        new DataReaderMapperBuilder<ISimpleType>(
          "should_map_interfaces_when_factory_supplied")
          .Map(x => x.Name, "name")
          .SetFactory(() => new SimpleType())
          .Build();
      ISimpleType obj = mapper.Map(reader);
      Assert.That(obj.Name, Is.EqualTo("nohros"));
      Assert.That(obj is SimpleType, Is.True);
    }

    [Test]
    public void should_not_map_ignored_property() {
      var reader = GetDataReader();
      Mock
        .Arrange(() => reader.FieldCount)
        .Returns(2);
      Mock
        .Arrange(() => reader.GetName(0))
        .Returns("name");
      Mock
        .Arrange(() => reader.GetName(1))
        .Returns("location");
      Mock
        .Arrange(() => reader.GetOrdinal("name"))
        .Returns(0);
      Mock
        .Arrange(() => reader.GetString(0))
        .Returns("nohros");

      var mapper =
        new DataReaderMapperBuilder<IgnoreType>(
          "should_not_map_ignored_property")
          .AutoMap()
          .Ignore("name")
          .Build();
      Assert.That(() => mapper.Map(reader).Name, Is.Null);
    }

    [Test]
    public void shoud_map_to_a_constant() {
      IDataReader reader = GetDataReader();
      var mapper =
        new DataReaderMapperBuilder<IgnoreType>("shoud_map_to_a_constant")
          .Map("name", TypeMaps.String("myname"))
          .Build();
      Assert.That(mapper.Map(reader).Name, Is.EqualTo("myname"));
    }

    [Test]
    public void should_be_thread_safe() {
      IDataReader reader = GetDataReader();

      var sync = new CountdownEvent(2);

      Action method = () => {
        var mapper =
          new DataReaderMapperBuilder<IgnoreType>("shoud_map_to_a_constant")
            .Map("name", TypeMaps.String("myname"))
            .Build();
        sync.Signal();
        Assert.That(mapper.Map(reader).Name, Is.EqualTo("myname"));
      };

      Action parallel = () => {
        ThreadPool.QueueUserWorkItem(state => method());
        ThreadPool.QueueUserWorkItem(state => method());
      };

      Assert.That(() => parallel(), Throws.Nothing);
      sync.Wait();
    }

    [Test]
    public void should_ignore_field_if_column_not_exists_and_optional_is_set() {
      IDataReader reader = GetDataReader();
      Mock
        .Arrange(() => reader.FieldCount)
        .Returns(1);
      Mock
        .Arrange(() => reader.GetName(0))
        .Returns("name2");
      Mock
        .Arrange(() => reader.GetOrdinal("name2"))
        .Returns(1);
      Mock
        .Arrange(() => reader.GetString(0))
        .Throws<ArgumentException>();

      var mapper =
        new DataReaderMapperBuilder<IgnoreType>(
          "should_ignore_field_if_column_not_exists_and_optional_is_set")
          .Map(x => x.Name, "nohros", true)
          .Build();

      try {
        IgnoreType type = mapper.Map(reader);
        Assert.That(type.Name, Is.Null);
      } catch {
        Assert.Fail("Should not throws any exception");
      }
    }

    [Test]
    public void should_map_field_if_column_exists_and_optional_is_set() {
      IDataReader reader = GetDataReader();
      Mock
        .Arrange(() => reader.FieldCount)
        .Returns(1);
      Mock
        .Arrange(() => reader.GetName(0))
        .Returns("name");
      Mock
        .Arrange(() => reader.GetOrdinal("name"))
        .Returns(0);
      Mock
        .Arrange(() => reader.GetString(0))
        .Returns("nohros");

      var mapper =
        new DataReaderMapperBuilder<IgnoreType>(
          "should_map_field_if_column_exists_and_optional_is_set")
          .Map(x => x.Name, "name", true)
          .Build();

      IgnoreType type = mapper.Map(reader);
      Assert.That(type.Name, Is.EqualTo("nohros"));
    }

    [Test]
    public void should_apply_transformations_in_order() {
      IDataReader reader = GetDataReader();
      Mock
        .Arrange(() => reader.FieldCount)
        .Returns(1);
      Mock
        .Arrange(() => reader.GetName(0))
        .Returns("name");
      Mock
        .Arrange(() => reader.GetOrdinal("name"))
        .Returns(0);
      Mock
        .Arrange(() => reader.GetString(0))
        .Returns("nohros");

      var mapper =
        new DataReaderMapperBuilder<SimpleType>(
          "should_apply_transformation_in_order")
          .Map(x => x.Name, "name")
          .Transform((r, t) => t.Name += "_1")
          .Transform((r, t) => t.Name += "_2")
          .Build();

      SimpleType type = mapper.Map(reader);
      Assert.That(type.Name, Is.EqualTo("nohros_1_2"));
    }

    [Test]
    public void should_apply_transformation_before_post_action() {
      IDataReader reader = GetDataReader();
      Mock
        .Arrange(() => reader.FieldCount)
        .Returns(1);
      Mock
        .Arrange(() => reader.GetName(0))
        .Returns("name");
      Mock
        .Arrange(() => reader.GetOrdinal("name"))
        .Returns(0);
      Mock
        .Arrange(() => reader.GetString(0))
        .Returns("nohros");

      var mapper =
        new DataReaderMapperBuilder<SimpleType>(
          "should_apply_transformation_before_post_action")
          .Map(x => x.Name, "name")
          .Transform((r, t) => t.Name += "_1")
          .Transform((r, t) => t.Name += "_2")
          .Build();

      SimpleType type = mapper.Map(reader, t => t.Name += "_3");
      Assert.That(type.Name, Is.EqualTo("nohros_1_2_3"));
    }

    [Test]
    public void
      should_not_ignore_field_if_column_not_exists_and_optional_is_not_set() {
      IDataReader reader = GetDataReader();
      Mock
        .Arrange(() => reader.FieldCount)
        .Returns(1);
      Mock
        .Arrange(() => reader.GetName(0))
        .Returns("name2");
      Mock
        .Arrange(() => reader.GetOrdinal("name2"))
        .Returns(1);
      Mock
        .Arrange(() => reader.GetString(0))
        .Throws<ArgumentException>();

      var mapper =
        new DataReaderMapperBuilder<IgnoreType>(
          "should_not_ignore_field_if_column_not_exists_and_optional_is_not_set")
          .Map(x => x.Name, "nohros")
          .Build();
      Assert.That(() => mapper.Map(reader).Name, Throws.InstanceOf<Exception>());
    }

    IDataReader GetDataReader() {
      var reader = Mock.Create<IDataReader>(Behavior.Loose);
      Mock
        .Arrange(() => reader.Read())
        .Returns(true);
      return reader;
    }
  }
}
