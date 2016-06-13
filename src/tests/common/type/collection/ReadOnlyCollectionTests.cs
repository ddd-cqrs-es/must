using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Nohros.Collections
{
  public class ReadOnlyCollectionTests
  {
    [Test]
    public void should_use_original_list_as_underlying_collection() {
      var list = new List<int> {5, 6, 7, 10};
      var read_only = list.AsReadOnlyList();

      Assert.That(read_only[0], Is.EqualTo(5));

      list.Add(25);
      Assert.That(read_only[4], Is.EqualTo(25));
    }

    [Test]
    public void should_use_original_array_as_underlying_collection() {
      var list = new List<int> {5, 6, 7, 10}.ToArray();
      var read_only = list.AsReadOnlyList();

      Assert.That(read_only[0], Is.EqualTo(5));

      list[0] = 25;
      Assert.That(read_only[0], Is.EqualTo(25));
    }

    [Test]
    public void should_use_original_readonly_as_underlying_collection() {
      var list = new List<int> {5, 6, 7, 10};
      var sys_read_only = list.AsReadOnly();
      var read_only = sys_read_only.AsReadOnlyList();

      Assert.That(read_only[0], Is.EqualTo(5));

      list[0] = 25;
      Assert.That(read_only[0], Is.EqualTo(25));
    }

    [Test]
    public void should_create_new_collection_if_ilist_is_not_implemented() {
      var list = new List<int> { 5, 6, 7, 10 };
      var read_only = list.Select(item => item).AsReadOnlyList();

      Assert.That(read_only[0], Is.EqualTo(5));

      list[0] = 25;
      Assert.That(read_only[0], Is.EqualTo(5));
    }
  }
}
