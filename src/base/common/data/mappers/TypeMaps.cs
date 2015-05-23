using Nohros.Data;

namespace Nohros.Data
{
  /// <summary>
  /// Factory for <see cref="ITypeMap"/> class that maps a field to a
  /// constant value.
  /// </summary>
  public sealed class TypeMaps
  {
    /// <summary>
    /// Returns a instance of the <see cref="ITypeMap"/> class that maps the
    /// given <paramref name="value"/> to a class field.
    /// </summary>
    /// <param name="value">
    /// The <see cref="bool"/> value to be mapped.
    /// </param>
    public static ITypeMap Boolean(bool value) {
      return new TypeMap(value, TypeMapType.Boolean);
    }

    /// <summary>
    /// Returns a instance of the <see cref="ITypeMap"/> class that maps the
    /// given <paramref name="value"/> to a class field.
    /// </summary>
    /// <param name="value">
    /// The <see cref="byte"/> value to be mapped.
    /// </param>
    public static ITypeMap Byte(byte value) {
      return new TypeMap(value, TypeMapType.Byte);
    }

    /// <summary>
    /// Returns a instance of the <see cref="ITypeMap"/> class that maps the
    /// given <paramref name="value"/> to a class field.
    /// </summary>
    /// <param name="value">
    /// The <see cref="char"/> value to be mapped.
    /// </param>
    public static ITypeMap Char(char value) {
      return new TypeMap(value, TypeMapType.Char);
    }

    /// <summary>
    /// Returns a instance of the <see cref="ITypeMap"/> class that maps the
    /// given <paramref name="value"/> to a class field.
    /// </summary>
    /// <param name="value">
    /// The <see cref="int"/> value to be mapped.
    /// </param>
    public static ITypeMap Integer(int value) {
      return new TypeMap(value, TypeMapType.Int);
    }

    /// <summary>
    /// Returns a instance of the <see cref="ITypeMap"/> class that maps the
    /// given <paramref name="value"/> to a class field.
    /// </summary>
    /// <param name="value">
    /// The <see cref="long"/> value to be mapped.
    /// </param>
    public static ITypeMap Long(long value) {
      return new TypeMap(value, TypeMapType.Long);
    }

    /// <summary>
    /// Returns a instance of the <see cref="ITypeMap"/> class that maps the
    /// given <paramref name="value"/> to a class field.
    /// </summary>
    /// <param name="value">
    /// The <see cref="decimal"/> value to be mapped.
    /// </param>
    public static ITypeMap Decimal(decimal value) {
      return new TypeMap(value, TypeMapType.Decimal);
    }

    /// <summary>
    /// Returns a instance of the <see cref="ITypeMap"/> class that maps the
    /// given <paramref name="value"/> to a class field.
    /// </summary>
    /// <param name="value">
    /// The <see cref="double"/> value to be mapped.
    /// </param>
    public static ITypeMap Double(double value) {
      return new TypeMap(value, TypeMapType.Double);
    }

    /// <summary>
    /// Returns a instance of the <see cref="ITypeMap"/> class that maps the
    /// given <paramref name="value"/> to a class field.
    /// </summary>
    /// <param name="value">
    /// The <see cref="float"/> value to be mapped.
    /// </param>
    public static ITypeMap Float(float value) {
      return new TypeMap(value, TypeMapType.Float);
    }

    /// <summary>
    /// Returns a instance of the <see cref="ITypeMap"/> class that maps the
    /// given <paramref name="value"/> to a class field.
    /// </summary>
    /// <param name="value">
    /// The <see cref="short"/> value to be mapped.
    /// </param>
    public static ITypeMap Short(short value) {
      return new TypeMap(value, TypeMapType.Short);
    }

    /// <summary>
    /// Returns a instance of the <see cref="ITypeMap"/> class that maps the
    /// given <paramref name="value"/> to a class field.
    /// </summary>
    /// <param name="value">
    /// The <see cref="string"/> value to be mapped.
    /// </param>
    public static ITypeMap String(string value) {
      return new TypeMap(value, TypeMapType.ConstString);
    }
  }
}
