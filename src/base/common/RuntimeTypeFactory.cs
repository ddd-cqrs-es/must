using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Nohros
{
  /// <summary>
  /// A collection of factory methods used to dynamically create instances of
  /// other classes.
  /// </summary>
  /// <remarks>
  /// This class is basically a simplification of the <see cref="Activator"/>
  /// class.
  /// </remarks>
  /// <typeparam name="T">
  /// The type to be created. It is usually a interface or a derived class of
  /// the type defined by a <see cref="IRuntimeType"/> object. The type that
  /// is loaded is the type defined by the <see cref="IRuntimeType"/> object,
  /// which is casted to <typeparamref name="T"/> before it is loaded.
  /// </typeparam>
  public sealed class RuntimeTypeFactory<T> where T : class
  {
    // create a new object instance using a public or non-public
    // constructor.
    const BindingFlags kFlags =
      BindingFlags.CreateInstance | BindingFlags.Public |
        BindingFlags.Instance | BindingFlags.NonPublic;

    /// <summary>
    /// Creates a new instance of the type defined by the
    /// <paramref name="runtime_type"/> object.
    /// </summary>
    /// <param name="runtime_type">
    /// A <see cref="IRuntimeType"/> object containing  information about
    /// the type to be instantiated.
    /// </param>
    /// <param name="args">
    /// An array of arguments that match the parameters of the constructor to
    /// invoke. If args is an empty array or null, the constructor that takes
    /// no parameters(the default constructor) is invoked.
    /// </param>
    /// <returns>
    /// An instance of the type defined by the <paramref name="runtime_type"/>
    /// object casted to <typeparamref name="T"/> or <c>null</c> if the class
    /// could not be loaded or casted to <typeparamref name="T"/>
    /// </returns>
    /// <remarks>
    /// A exception is never raised by this method. If a exception is raised
    /// by the object constructor it will be catched and <c>null</c> will be
    /// returned. If you need to know about the exception, use the method
    /// <see cref="CreateInstance(IRuntimeType, object[])"/> instead.
    /// </remarks>
    /// <seealso cref="CreateInstance(IRuntimeType, object[])"/>
    /// <seealso cref="IRuntimeType"/>
    public static T CreateInstanceNoException(IRuntimeType runtime_type,
      params object[] args) {
      return CreateInstance(runtime_type, false, false, args);
    }

    /// <summary>
    /// Creates a new instance of the type defined by the
    /// <paramref name="type"/> object.
    /// </summary>
    /// <param name="type">
    /// A <see cref="Type"/> object containing  information about
    /// the type to be instantiated.
    /// </param>
    /// <param name="args">
    /// An array of arguments that match the parameters of the constructor to
    /// invoke. If args is an empty array or null, the constructor that takes
    /// no parameters(the default constructor) is invoked.
    /// </param>
    /// <returns>
    /// An instance of the type defined by the <paramref name="type"/>
    /// object casted to <typeparamref name="T"/> or <c>null</c> if the class
    /// could not be loaded or casted to <typeparamref name="T"/>
    /// </returns>
    /// <remarks>
    /// A exception is never raised by this method. If a exception is raised
    /// by the object constructor it will be catched and <c>null</c> will be
    /// returned. If you need to know about the exception, use the method
    /// <see cref="CreateInstance(Nohros.IRuntimeType,object[])"/> instead.
    /// </remarks>
    /// <seealso cref="CreateInstance(Nohros.IRuntimeType,object[])"/>
    /// <seealso cref="IRuntimeType"/>
    public static T CreateInstanceNoException(Type type, params object[] args) {
      return CreateInstance(type, false, false, args);
    }

    /// <summary>
    /// Creates a new instance of the type defined by the
    /// <paramref name="runtime_type"/> object and the specified
    /// arguments, falling back to the default class constructor.
    /// </summary>
    /// <param name="runtime_type">
    /// A <see cref="IRuntimeType"/> object containing  information about
    /// the type to be instantiated.
    /// </param>
    /// <param name="args">
    /// An array of arguments that match the parameters of the constructor to
    /// invoke. If args is an empty array or null, the constructor that takes
    /// no parameters(the default constructor) is invoked.
    /// </param>
    /// <returns>
    /// An instance of the type defined by the <paramref name="runtime_type"/>
    /// object casted to <typeparamref name="T"/>.
    /// </returns>
    /// <remarks>
    /// If a constructor that match the specified array of arguments is not
    /// found, this method try to create an instance of the type defined by
    /// the <paramref name="runtime_type"/> object using the constructor that
    /// takes no parameters(the default constructor).
    /// </remarks>
    /// <seealso cref="CreateInstance(Nohros.IRuntimeType,object[])"/>
    /// <seealso cref="IRuntimeType"/>
    public static T CreateInstanceFallback(IRuntimeType runtime_type,
      params object[] args) {
      return CreateInstance(runtime_type, true, true, args);
    }

    /// <summary>
    /// Creates a new instance of the type defined by the
    /// <paramref name="type"/> object and the specified
    /// arguments, falling back to the default class constructor.
    /// </summary>
    /// <param name="type">
    /// A <see cref="Type"/> object containing  information about
    /// the type to be instantiated.
    /// </param>
    /// <param name="args">
    /// An array of arguments that match the parameters of the constructor to
    /// invoke. If args is an empty array or null, the constructor that takes
    /// no parameters(the default constructor) is invoked.
    /// </param>
    /// <returns>
    /// An instance of the type defined by the <paramref name="type"/>
    /// object casted to <typeparamref name="T"/>.
    /// </returns>
    /// <remarks>
    /// If a constructor that match the specified array of arguments is not
    /// found, this method try to create an instance of the type defined by
    /// the <paramref name="type"/> object using the constructor that
    /// takes no parameters(the default constructor).
    /// </remarks>
    /// <seealso cref="CreateInstance(Type,object[])"/>
    /// <seealso cref="IRuntimeType"/>
    public static T CreateInstanceFallback(Type type, params object[] args) {
      return CreateInstance(type, true, true, args);
    }

    /// <summary>
    /// Creates a new instance of the type defined by the
    /// <paramref name="runtime_type"/> object and the specified arguments.
    /// </summary>
    /// <param name="runtime_type">
    /// A <see cref="IRuntimeType"/> object containing  information about
    /// the type to be instantiated.
    /// </param>
    /// <param name="args">
    /// An array of arguments that match the parameters of the constructor to
    /// invoke. If args is an empty array or null, the constructor that takes
    /// no parameters(the default constructor) is invoked.
    /// </param>
    /// <returns>
    /// An instance of the type defined by the <paramref name="runtime_type"/>
    /// object casted to <typeparamref name="T"/>.
    /// </returns>
    public static T CreateInstance(IRuntimeType runtime_type,
      params object[] args) {
      return CreateInstance(runtime_type, false, true, args);
    }

    /// <summary>
    /// Creates a new instance of the type defined by the
    /// <paramref name="type"/> object and the specified arguments.
    /// </summary>
    /// <param name="type">
    /// A <see cref="Type"/> object containing  information about
    /// the type to be instantiated.
    /// </param>
    /// <param name="args">
    /// An array of arguments that match the parameters of the constructor to
    /// invoke. If args is an empty array or null, the constructor that takes
    /// no parameters(the default constructor) is invoked.
    /// </param>
    /// <returns>
    /// An instance of the type defined by the <paramref name="type"/>
    /// object casted to <typeparamref name="T"/>.
    /// </returns>
    public static T CreateInstance(Type type, params object[] args) {
      return CreateInstance(type, false, true, args);
    }

    internal static T CreateInstance(IRuntimeType runtime_type, bool fallback,
      bool throw_exceptions, params object[] args) {
      Type type = RuntimeType.GetSystemType(runtime_type);
      if (type == null) {
        if (!throw_exceptions) {
          return default(T);
        }
        throw new TypeLoadException(GetTypeNotFoundMessage(runtime_type.Type));
      }
      return CreateInstance(type, fallback, throw_exceptions, args);
    }

    internal static T CreateInstance(Type type, bool fallback,
      bool throw_exceptions, params object[] args) {
      string error = string.Empty;
      T new_obj = null;
      try {
        // If no constructors arguments was specified do not waste time
        // searching for a matching constructor and try to instantiate
        // the class using the parameterless constructor.
        if (args.Length == 0) {
          new_obj = Activator.CreateInstance(type, true) as T;
        } else {
          KeyValuePair<ConstructorInfo, ParameterInfo[]>[] list =
            GetOrderedConstructors(type, kFlags, args.Length);
          if (list.Length > 0) {
            KeyValuePair<ConstructorInfo, object[]> ctor;
            if (TryGetMatchingConstructor(list, args, out ctor)) {
              new_obj = ctor.Key.Invoke(kFlags, null, ctor.Value,
                CultureInfo.CurrentCulture) as T;
            }
          }
        }

        // A null |new_obj| means that a matching constructor was not found.
        // Fallback to the parameterless constructor if desired.
        if (new_obj == null && fallback) {
          new_obj = Activator.CreateInstance(type, true) as T;
        }
      } catch (MissingMethodException) {
        // A missing method exception means that a parameterless constructor
        // was not found. We can raise the TyeLoadException already since
        // fallback will only try to instantiate the class using the
        // parameterless constructor.
        if (!throw_exceptions) {
          return default(T);
        }
        throw new TypeLoadException(
          GetDefaultConstructorNotFoundMessage(type.FullName));
      }

      if (new_obj == null && throw_exceptions) {
        throw new TypeLoadException(
          GetMatchingConstructorNotFoundMessage(type.FullName, fallback,
            args));
      }
      return new_obj;
    }

    static string GetTypeNotFoundMessage(string type) {
      return
        string.Format(Resources.Resources.TypeLoad_CreateInstance, type)
          + Resources.Resources.TypeLoad_TypeNotFound;
    }

    static bool TryGetMatchingConstructor(
      KeyValuePair<ConstructorInfo, ParameterInfo[]>[] ctors, object[] args,
      out KeyValuePair<ConstructorInfo, object[]> matched_ctor) {
      // Copy the arguments into a array that we can modify.
      object[] mutable_args = new object[args.Length];
      Array.Copy(args, mutable_args, args.Length);

      // This list will hold the arguments of the matched constructor in
      // the same order as the related parameters.
      List<object> ordered_args = new List<object>(args.Length);
      for (int i = 0, j = ctors.Length; i < j; i++) {
        ParameterInfo[] parms = ctors[i].Value;
        if (parms.Length > args.Length) {
          continue;
        }
        for (int k = 0, m = parms.Length; k < m; k++) {
          int pos = GetArgumentOfType(mutable_args, parms[k].ParameterType);
          if (pos == -1) {
            // The current constructor does not match, reinitialize the
            // auxiliar arrays and restart the matching process.
            ordered_args.Clear();
            Array.Copy(args, mutable_args, args.Length);
            break;
          }
          ordered_args.Add(mutable_args[pos]);
          mutable_args[pos] = null;
        }
        if (ordered_args.Count > 0) {
          matched_ctor =
            new KeyValuePair<ConstructorInfo, object[]>(ctors[i].Key,
              ordered_args.ToArray());
          return true;
        }
      }
      matched_ctor = default(KeyValuePair<ConstructorInfo, object[]>);
      return false;
    }

    static int GetArgumentOfType(object[] args, Type type) {
      for (int i = 0, j = args.Length; i < j; i++) {
        object arg = args[i];
        if (type.IsInstanceOfType(arg)) {
          return i;
        }
      }
      return -1;
    }

    static KeyValuePair<ConstructorInfo, ParameterInfo[]>[]
      GetOrderedConstructors(Type type, BindingFlags flags, int count) {
      List<KeyValuePair<ConstructorInfo, ParameterInfo[]>> list =
        new List<KeyValuePair<ConstructorInfo, ParameterInfo[]>>();
      ConstructorInfo[] ctors = type.GetConstructors(flags);
      foreach (ConstructorInfo ctor in ctors) {
        ParameterInfo[] parms = ctor.GetParameters();
        list.Add(new KeyValuePair<ConstructorInfo, ParameterInfo[]>(ctor,
          parms));
      }

      // Sort the constructors using the following rule ordering:
      //   . constructors with the same number of parameters of the
      //     specified arguments.
      list.Sort(
        delegate(KeyValuePair<ConstructorInfo, ParameterInfo[]> x,
          KeyValuePair<ConstructorInfo, ParameterInfo[]> y) {
          if (x.Value.Length == y.Value.Length) {
            return 0;
          }

          if (x.Value.Length == count) {
            return -1;
          }

          if (y.Value.Length == count) {
            return 1;
          }

          return (x.Value.Length > y.Value.Length) ? -1 : 1;
        });
      return list.ToArray();
    }

    static string GetDefaultConstructorNotFoundMessage(string type) {
      return
        string.Format(Resources.Resources.TypeLoad_CreateInstance, type)
          + Resources.Resources.TypeLoad_DefaultConstructorNotFound;
    }

    static string GetMatchingConstructorNotFoundMessage(string type,
      bool fallback, params object[] args) {
      string signature = string.Empty;
      foreach (var o in args) {
        signature += o.GetType() + ",";
      }
      signature = signature.Substring(0, signature.Length - 1);
      string error =
        string.Format(Resources.Resources.TypeLoad_CreateInstance, type)
          + string.Format(Resources.Resources.TypeLoad_MissingConstructor,
            signature);
      if (fallback) {
        error += Resources.Resources.TypeLoad_DefaultConstructorNotFound;
      }
      return error;
    }
  }
}
