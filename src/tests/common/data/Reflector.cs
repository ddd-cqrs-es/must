using System.Data;

namespace Nohros.Common
{
  public class Reflector<T>
  {
    public void MyMethod2(T type) {
    }
  }

  class Reflector2
  {
    public void Reflect(MapperTests.ICompositeType type) {
      var reflector = new Reflector<MapperTests.ISimpleType>();
      reflector.MyMethod2(type);
    } 
  }
}
