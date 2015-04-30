using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using Nohros.Data;
using Telerik.JustMock;

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

    public interface INestedType : ISimpleType
    {
      string Name2 { get; }
    }

    public interface INestedType2 : INestedType
    {
      string Name3 { get; }
    }

    [Test]
    public void should_map_interfaces_when_no_factory_supplied() {
      IDataReader reader = GetDataReader();
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
    public void should_map_nested_interfaces() {
      IDataReader reader = GetDataReader();
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
        new DataReaderMapperBuilder<INestedType2>(
          "should_map_nested_interfaces")
          .Map(x => x.Name, "name")
          .Map(x => x.Name2, "name1")
          .Map(x => x.Name3, "name2")
          .Build();

      INestedType2 obj = mapper.Map(reader);
      Assert.That(obj.Name, Is.EqualTo("nohros"));
      Assert.That(obj.Name2, Is.EqualTo("nohros1"));
      Assert.That(obj.Name3, Is.EqualTo("nohros2"));
    }

    [Test]
    public void should_map_interfaces_when_factory_supplied() {
      IDataReader reader = GetDataReader();
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
    public void should_map_array_of_key_value_pairs() {
      IDataReader reader = GetDataReader();
      Mock
        .Arrange(() => reader.GetOrdinal("usuario_nome"))
        .Returns(0);
      Mock
        .Arrange(() => reader.GetString(0))
        .Returns("nohros");
      var mapper =
        new DataReaderMapperBuilder<SimpleType>(
          "should_map_array_of_key_value_pairs")
          .Map(new[] {new KeyValuePair<string, string>("name", "usuario_nome")})
          .Build();
      Assert.That(mapper.Map(reader).Name, Is.EqualTo("nohros"));
    }

    [Test]
    public void shoud_map_to_a_constant() {
      IDataReader reader = GetDataReader();
      var mapper =
        new DataReaderMapperBuilder<IgnoreType>("shoud_map_to_a_constant")
          .Map("name", new ConstStringMapType("myname"))
          .Build();
      Assert.That(mapper.Map(reader).Name, Is.EqualTo("myname"));
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
