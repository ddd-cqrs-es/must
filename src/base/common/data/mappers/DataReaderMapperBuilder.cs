using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using Nohros.Dynamics;
using PropertyAttributes = System.Reflection.PropertyAttributes;
using ConstantMap =
  System.Collections.Generic.KeyValuePair
    <Nohros.Data.ITypeMap, System.Reflection.PropertyInfo>;
using System.Linq.Expressions;

namespace Nohros.Data
{
  public class DataReaderMapperBuilder<T>
  {
    class ValueMap
    {
      public ValueMap(string key, PropertyInfo value, Type raw_type) {
        Key = key;
        Value = value;
        RawType = raw_type;
      }

      public string Key { get; set; }
      public PropertyInfo Value { get; set; }
      public Type RawType { get; set; }
    }

    class OrdinalMap
    {
      public OrdinalMap(int key, PropertyInfo value, Type raw_type) {
        Key = key;
        Value = value;
        RawType = raw_type;
      }

      public int Key { get; set; }
      public PropertyInfo Value { get; set; }
      public Type RawType { get; set; }
    }

    class MappingResult
    {
      public FieldBuilder OrdinalsField { get; set; }
      public OrdinalMap[] OrdinalsMapping { get; set; }
      public ValueMap[] ValueMappings { get; set; }
      public ConstantMap[] ConstantMappings { get; set; }
      public FieldInfo LoaderField { get; set; }
    }

    protected delegate void PreCreateTypeEventHandler(TypeBuilder builder);

    readonly IDictionary<string, ITypeMap> mappings_;
    readonly Type type_t_;
    readonly string type_t_type_name_;
    bool auto_map_;
    CallableDelegate<T> factory_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="DataReaderMapperBuilder{T}"/> class that uses the namespace
    /// of the type <typeparamref name="T"/> as the class name prefix.
    /// </summary>
    public DataReaderMapperBuilder()
      : this(typeof (T), typeof (T).Namespace) {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="DataReaderMapperBuilder{T}"/> class using the specified
    /// class name prefix.
    /// </summary>
    public DataReaderMapperBuilder(string prefix) : this(typeof (T), prefix) {
      if (prefix == null) {
        throw new ArgumentNullException("prefix");
      }
    }

    DataReaderMapperBuilder(Type type_t, string prefix) {
      if (type_t == null) {
        throw new ArgumentNullException("type_t");
      }
      mappings_ = new Dictionary<string, ITypeMap>(
        StringComparer.OrdinalIgnoreCase);
      type_t_ = type_t;
      type_t_type_name_ = prefix;
      auto_map_ = false;
    }
    #endregion

    /// <summary>
    /// Builds a dynamic <see cref="DataReaderMapper{T}"/> for the type
    /// <typeparamref source="T"/> using the specified
    /// properties.
    /// </summary>
    /// <param name="mapping">
    /// An <see cref="KeyValuePair{TKey,TValue}"/> containing the mapping
    /// between source columns and destination properties.
    /// </param>
    /// <remarks>
    /// The <see cref="KeyValuePair{TKey,TValue}.Value"/> is used as the source
    /// column name and the <see cref="KeyValuePair{TKey,TValue}.Key"/> is
    /// used as the destination property name.
    /// </remarks>
    public DataReaderMapperBuilder<T> Map(
      IEnumerable<KeyValuePair<string, string>> mapping) {
      foreach (KeyValuePair<string, string> map in mapping) {
        Map(map.Key, map.Value);
      }
      return this;
    }

    /// <summary>
    /// Builds a dynamic <see cref="DataReaderMapper{T}"/> for the type
    /// <typeparamref source="T"/> using the specified
    /// properties.
    /// </summary>
    /// <param name="mapping">
    /// An <see cref="KeyValuePair{TKey,TValue}"/> containing the mapping
    /// between source columns and destination properties.
    /// </param>
    /// <remarks>
    /// The <see cref="KeyValuePair{TKey,TValue}.Value"/> is used as the source
    /// column name and the <see cref="KeyValuePair{TKey,TValue}.Key"/> is
    /// used as the destination property name.
    /// </remarks>
    public DataReaderMapperBuilder<T> Map(
      IEnumerable<KeyValuePair<string, ITypeMap>> mapping) {
      foreach (KeyValuePair<string, ITypeMap> map in mapping) {
        Map(map.Key, map.Value);
      }
      return this;
    }

    /// <summary>
    /// Maps the source column <paramref source="source"/> to the interface
    /// property <paramref source="destination"/>.
    /// </summary>
    /// <param name="source">
    /// The source of the source column.
    /// </param>
    /// <param name="destination">
    /// The source of the property that will be mapped to the column
    /// <paramref name="source"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DataReaderMapperBuilder{T}"/> that builds an object of type
    /// <typeparamref source="T"/> and mapping the column <paramref source="source"/>
    /// to the property named <paramref source="destination"/>.
    /// </returns>
    public DataReaderMapperBuilder<T> Map(string destination, string source) {
      return Map(destination, source, null);
    }

    /// <summary>
    /// Maps the source column <paramref source="source"/> to the interface
    /// property <paramref source="destination"/>.
    /// </summary>
    /// <param name="source">
    /// The source of the source column.
    /// </param>
    /// <param name="destination">
    /// The source of the property that will be mapped to the column
    /// <paramref name="source"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DataReaderMapperBuilder{T}"/> that builds an object of type
    /// <typeparamref source="T"/> and mapping the column <paramref source="source"/>
    /// to the property named <paramref source="destination"/>.
    /// </returns>
    public DataReaderMapperBuilder<T> Map<TMap>(string destination,
      string source) {
      return Map(destination, source, typeof (TMap));
    }

    /// <summary>
    /// Maps the source column <paramref source="source"/> to the interface
    /// property <paramref source="destination"/>.
    /// </summary>
    /// <param name="source">
    /// The source of the source column.
    /// </param>
    /// <param name="destination">
    /// The source of the property that will be mapped to the column
    /// <paramref name="source"/>.
    /// </param>
    /// <param name="type">
    /// The type of source column.
    /// </param>
    /// <returns>
    /// A <see cref="DataReaderMapperBuilder{T}"/> that builds an object of type
    /// <typeparamref source="T"/> and mapping the column <paramref source="source"/>
    /// to the property named <paramref source="destination"/>.
    /// </returns>
    public DataReaderMapperBuilder<T> Map(string destination, string source,
      Type type) {
      if (source == null) {
        return Map(destination, new IgnoreMapType());
      }
      return Map(destination, new StringTypeMap(source) {
        RawType = type
      });
    }

    /// <summary>
    /// Maps the constant value <see cref="value"/> to the interface
    /// property <paramref source="destination"/>.
    /// </summary>
    /// <param name="value">
    /// The value that should be returned when by the interface property
    /// <paramref name="destination"/>.
    /// </param>
    /// <param name="destination">
    /// The source of the property that will be mapped to the value
    /// <paramref name="value"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DataReaderMapperBuilder{T}"/> that builds an object of type
    /// <typeparamref source="T"/> and mapping the constant value
    /// <paramref source="value"/> to the property named
    /// <paramref source="destination"/>.
    /// </returns>
    public DataReaderMapperBuilder<T> Map(string destination, int value) {
      return Map(destination, new IntMapType(value));
    }

    /// <summary>
    /// Maps the constant value <see cref="value"/> to the interface
    /// property <paramref source="destination"/>.
    /// </summary>
    /// <param name="value">
    /// The value that should be returned when by the interface property
    /// <paramref name="destination"/>.
    /// </param>
    /// <param name="destination">
    /// The source of the property that will be mapped to the value
    /// <paramref name="value"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DataReaderMapperBuilder{T}"/> that builds an object of type
    /// <typeparamref source="T"/> and mapping the constant value
    /// <paramref source="value"/> to the property named
    /// <paramref source="destination"/>.
    /// </returns>
    public DataReaderMapperBuilder<T> Map(string destination, short value) {
      return Map(destination, new ShortMapType(value));
    }

    /// <summary>
    /// Maps the constant value <see cref="value"/> to the interface
    /// property <paramref source="destination"/>.
    /// </summary>
    /// <param name="value">
    /// The value that should be returned when by the interface property
    /// <paramref name="destination"/>.
    /// </param>
    /// <param name="destination">
    /// The source of the property that will be mapped to the value
    /// <paramref name="value"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DataReaderMapperBuilder{T}"/> that builds an object of type
    /// <typeparamref source="T"/> and mapping the constant value
    /// <paramref source="value"/> to the property named
    /// <paramref source="destination"/>.
    /// </returns>
    public DataReaderMapperBuilder<T> Map(string destination, long value) {
      return Map(destination, new LongMapType(value));
    }

    /// <summary>
    /// Maps the constant value <see cref="value"/> to the interface
    /// property <paramref source="destination"/>.
    /// </summary>
    /// <param name="value">
    /// The value that should be returned when by the interface property
    /// <paramref name="destination"/>.
    /// </param>
    /// <param name="destination">
    /// The source of the property that will be mapped to the value
    /// <paramref name="value"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DataReaderMapperBuilder{T}"/> that builds an object of type
    /// <typeparamref source="T"/> and mapping the constant value
    /// <paramref source="value"/> to the property named
    /// <paramref source="destination"/>.
    /// </returns>
    public DataReaderMapperBuilder<T> Map(string destination, float value) {
      return Map(destination, new FloatMapType(value));
    }

    /// <summary>
    /// Maps the constant value <see cref="value"/> to the interface
    /// property <paramref source="destination"/>.
    /// </summary>
    /// <param name="value">
    /// The value that should be returned when by the interface property
    /// <paramref name="destination"/>.
    /// </param>
    /// <param name="destination">
    /// The source of the property that will be mapped to the value
    /// <paramref name="value"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DataReaderMapperBuilder{T}"/> that builds an object of type
    /// <typeparamref source="T"/> and mapping the constant value
    /// <paramref source="value"/> to the property named
    /// <paramref source="destination"/>.
    /// </returns>
    public DataReaderMapperBuilder<T> Map(string destination, decimal value) {
      return Map(destination, new DecimalMapType(value));
    }

    /// <summary>
    /// Maps the constant <see cref="value"/> to the interface property
    /// <paramref source="destination"/>.
    /// </summary>
    /// <param name="value">
    /// The value that should be returned when by the interface property
    /// <paramref name="destination"/>.
    /// </param>
    /// <param name="destination">
    /// The source of the property that will be mapped to the value
    /// <paramref name="value"/>.
    /// </param>
    /// <returns>
    /// A <see cref="DataReaderMapperBuilder{T}"/> that builds an object of type
    /// <typeparamref source="T"/> and mapping the constant value
    /// <paramref source="value"/> to the property named
    /// <paramref source="destination"/>.
    /// </returns>
    public DataReaderMapperBuilder<T> Map(string destination, ITypeMap value) {
      mappings_[destination] = value;
      return this;
    }

    /// <summary>
    /// Automatically maps the properties defined by the type
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// The properties that has no mapping defined will be mapped to the
    /// field that has the same name as the mapped property.
    /// </remarks>
    public DataReaderMapperBuilder<T> AutoMap() {
      auto_map_ = true;
      return this;
    }

    public DataReaderMapperBuilder<T> Map<TProperty>(
      Expression<Func<T, TProperty>> expression, string source, Type type) {
      MemberExpression member;
      if (expression.Body is UnaryExpression) {
        member = ((UnaryExpression) expression.Body).Operand as MemberExpression;
      } else {
        member = expression.Body as MemberExpression;
      }

      if (member == null) {
        throw new ArgumentException("[member] should be a class property");
      }
      return Map(member.Member.Name, source, type);
    }

    public DataReaderMapperBuilder<T> Map<TProperty>(
      Expression<Func<T, TProperty>> expression, string source) {
      return Map(expression, source, null);
    }

    /// <summary>
    /// Defines the factory that shoud be used to create an instance of the
    /// <typeparamref name="T"/> class.
    /// </summary>
    /// <param name="factory">
    /// A <see cref="CallableDelegate{T}"/> that should be used to create an
    /// instance of the <typeparamref name="T"/> class.
    /// </param>
    /// <remarks>
    /// If the class <typeparamref name="T"/> does not has a constructor that
    /// receives no parameters, the <see cref="SetFactory"/> method should be
    /// called to define the factory that should be used to create an instance
    /// of the <typeparamref name="T"/> class, falling to define this will
    /// causes the <see cref="Build"/> method to throw an exception.
    /// </remarks>
    public DataReaderMapperBuilder<T> SetFactory(CallableDelegate<T> factory) {
      factory_ = factory;
      return this;
    }

    /// <summary>
    /// Builds a dynamic type that implements the
    /// <see cref="IDataReaderMapper{T}"/> interface.
    /// </summary>
    /// <returns>
    /// A instance of the dynamically created class.
    /// </returns>
    /// <remarks>
    /// <see cref="Build"/> will create a dynamic type that implements the
    /// <see cref="IDataReaderMapper{T}"/> for the type
    /// <typeparamref source="T"/> only if the type does not exists already. If
    /// the type already exists <see cref="Build"/> will only create an
    /// instance of that class.
    /// <para>
    /// If the dynamic class already exists the mapping defined for the first
    /// build will be used.
    /// </para>
    /// <para>
    /// If you need to map the <typeparamref name="T"/> using distinct ways,
    /// you should create a <see cref="DataReaderMapperBuilder{T}"/> that
    /// uses distinct prefixes.
    /// </para>
    /// </remarks>
    /// <seealso cref="DataReaderMapperBuilder{T}"/>
    public IDataReaderMapper<T> Build() {
      DataReaderMapper<T> mapper = (DataReaderMapper<T>)
        Activator.CreateInstance(GetDynamicType(type_t_type_name_));
      if (factory_ != null) {
        mapper.loader_ = factory_;
      }
      return mapper;
    }

    /// <summary>
    /// Gets the dynamic type for <typeparamref name="T"/>.
    /// </summary>
    /// <param name="type">
    /// When this method returns contains the type that was dynamically
    /// created for the type <typeparamref name="T"/>, or <c>null</c> is
    /// a dynamic type for <typeparamref name="T"/> does not exists.
    /// </param>
    /// <returns></returns>
    static bool TryGetDynamicType(out Type type) {
      var t = typeof (T);
      return TryGetDynamicType(t.Namespace, out type);
    }

    /// <summary>
    /// Gets the dynamic type for <typeparamref name="T"/>.
    /// </summary>
    /// <param name="type">
    /// When this method returns contains the type that was dynamically
    /// created for the type <typeparamref name="T"/>, or <c>null</c> is
    /// a dynamic type for <typeparamref name="T"/> does not exists.
    /// </param>
    /// <returns></returns>
    static bool TryGetDynamicType(string prefix, out Type type) {
      string dynamic_type_name = Dynamics_.GetDynamicTypeName(prefix,
        typeof (T));
      type = Dynamics_.ModuleBuilder.GetType(dynamic_type_name);
      return type != null;
    }

    /// <summary>
    /// Gets the dynamic type for <typeparamref source="T"/>.
    /// </summary>
    /// <returns>
    /// The dynamic type for <typeparamref source="T"/>.
    /// </returns>
    /// <remarks>
    /// If the dynamic type does not already exists, it will be created.
    /// </remarks>
    Type GetDynamicType() {
      return GetDynamicType(type_t_.Namespace);
    }

    /// <summary>
    /// Gets the dynamic type for <typeparamref source="T"/>.
    /// </summary>
    /// <returns>
    /// The dynamic type for <typeparamref source="T"/>.
    /// </returns>
    /// <remarks>
    /// If the dynamic type does not already exists, it will be created.
    /// </remarks>
    Type GetDynamicType(string prefix) {
      string dynamic_type_name =
        Dynamics_.GetDynamicTypeName(prefix, type_t_);
      return Dynamics_.ModuleBuilder
                      .GetType(dynamic_type_name) ??
        MakeDynamicType(dynamic_type_name);
    }

    /// <summary>
    /// Ignores a destination property, by not including it in the map.
    /// </summary>
    /// <param name="destination">
    /// The name of the destination property to be ignored.
    /// </param>
    /// <remarks>
    /// Ignored properties thrown an NotImplemented exception when an attempt
    /// to access is performed.
    /// </remarks>
    public DataReaderMapperBuilder<T> Ignore(string destination) {
      mappings_.Add(destination, new IgnoreMapType());
      return this;
    }

    /// <summary>
    /// Generates the dynamic type for <typeparamref source="T"/>.
    /// </summary>
    /// <returns>
    /// The dynamic type for <typeparamref source="T"/>.
    /// </returns>
    /// <remarks>
    /// The type is dynamically created and added to the current
    /// <see cref="AppDomain"/>.
    /// </remarks>
    Type MakeDynamicType(string dynamic_type_name) {
      TypeBuilder builder;
      try {
        builder = Dynamics_.ModuleBuilder.DefineType(
          dynamic_type_name,
          TypeAttributes.Public |
            TypeAttributes.Class |
            TypeAttributes.AutoClass |
            TypeAttributes.AutoLayout,
          typeof (DataReaderMapper<T>),
          new Type[] {
            typeof (IDataReaderMapper<T>)
          });
      } catch (ArgumentException) {
        // Check if the type was created by another thread.
        Type type = Dynamics_.ModuleBuilder.GetType(dynamic_type_name);
        if (type == null) {
          throw;
        }
        return type;
      }

      PropertyInfo[] properties = GetProperties();

      MappingResult result = new MappingResult();

      // Get the mappings for the properties that return value types.
      GetMappings(properties, result);

      EmitConstructor(builder, result);
      EmitGetOrdinals(builder, result);
      EmitNewT(builder, result);
      EmitMapMethod(builder, result);

      OnPreCreateType(builder);

      return builder.CreateType();
    }

    void OnPreCreateType(TypeBuilder builder) {
      Listeners.SafeInvoke(PreCreateType,
        delegate(PreCreateTypeEventHandler handler) { handler(builder); });
    }

    /// <summary>
    /// Raised before the dynamic type is created.
    /// </summary>
    protected event PreCreateTypeEventHandler PreCreateType;

    /// <summary>
    /// Emit the constuctor code.
    /// </summary>
    void EmitConstructor(TypeBuilder type, MappingResult result) {
      ConstructorBuilder constructor =
        type.DefineConstructor(MethodAttributes.Public |
          MethodAttributes.HideBySig |
          MethodAttributes.SpecialName |
          MethodAttributes.RTSpecialName,
          CallingConventions.Standard, Type.EmptyTypes);

      // Calls the constructor of the base class DataReaderMapper
      ConstructorInfo data_reader_mapper_ctor = typeof (DataReaderMapper<T>)
        .GetConstructor(
          BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.Instance,
          null, Type.EmptyTypes, null);
      ILGenerator il = constructor.GetILGenerator();
      il.Emit(OpCodes.Ldarg_0); // load "this" pointer
      il.Emit(OpCodes.Call, data_reader_mapper_ctor); // call default ctor

      // Create the array that will store the column ordinals.
      result.OrdinalsField = type
        .DefineField("ordinals_", typeof (int[]), FieldAttributes.Private);

      result.LoaderField = typeof (DataReaderMapper<T>)
        .GetField("loader_", BindingFlags.Public |
          BindingFlags.NonPublic |
          BindingFlags.Instance);

      // return from the constructor
      il.Emit(OpCodes.Ret);
    }

    void EmitNewT(TypeBuilder type, MappingResult result) {
      MethodBuilder builder = type
        .DefineMethod("NewT",
          MethodAttributes.Assembly | MethodAttributes.HideBySig |
            MethodAttributes.Virtual, type_t_, Type.EmptyTypes);

      ILGenerator il = builder.GetILGenerator();

      // Calls the default constructor of the type T
      ConstructorInfo t_constructor = type_t_
        .GetConstructor(
          BindingFlags.Public |
            BindingFlags.NonPublic | BindingFlags.Instance,
          null, Type.EmptyTypes, null);

      if (t_constructor == null) {
        throw new MissingMethodException(type_t_.Name, "ctor()");
      }

      LocalBuilder local_t = il.DeclareLocal(type_t_);
      il.Emit(OpCodes.Newobj, t_constructor);
      il.Emit(OpCodes.Stloc_0, local_t);
      il.Emit(OpCodes.Ldloc_0);
      il.Emit(OpCodes.Ret);
    }

    void EmitGetOrdinals(TypeBuilder type, MappingResult result) {
      MethodBuilder builder = type
        .DefineMethod("GetOrdinals",
          MethodAttributes.Assembly | MethodAttributes.HideBySig |
            MethodAttributes.Virtual, typeof (void),
          new Type[] {typeof (IDataReader)});

      ILGenerator il = builder.GetILGenerator();

      // If there is nothing to be mapped, map nothing.
      if (result.ValueMappings.Length == 0) {
        // Create an empty array to prevent the NoResultException to be throw
        // by the Map method.
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldc_I4_0);
        il.Emit(OpCodes.Newarr, typeof (int));
        il.Emit(OpCodes.Stfld, result.OrdinalsField);

        result.OrdinalsMapping = new OrdinalMap[0];
      } else {
        // check if the ordinals is not already got.
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, result.OrdinalsField);
        il.Emit(OpCodes.Ldnull);
        il.Emit(OpCodes.Ceq);

        Label label = il.DefineLabel();
        il.Emit(OpCodes.Brtrue_S, label);
        il.Emit(OpCodes.Ret);
        il.MarkLabel(label);

        // get the columns ordinals, using a try/catch block to prevent a
        // InvalidOperationException to be throw when no recordset is returned.
        il.BeginExceptionBlock();

        result.OrdinalsMapping = EmitOrdinals(il, result);

        //il.Emit(OpCodes.Leave, exit_try_label);
        il.BeginCatchBlock(typeof (InvalidOperationException));

        // remove the exception from the stack
        il.Emit(OpCodes.Pop);

        // set [ordinals_] to null
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldnull);
        il.Emit(OpCodes.Stfld, result.OrdinalsField);
        il.EndExceptionBlock();
      }

      /*MethodInfo method_info =
          typeof (DataReaderMapper<T>).GetMethod("Initialize",
            BindingFlags.NonPublic | BindingFlags.Public |
              BindingFlags.Instance,
            null, new Type[] {typeof (IDataReader)}, null);*/

      il.Emit(OpCodes.Ret);
    }

    void EmitMapMethod(TypeBuilder type, MappingResult result) {
      MethodBuilder builder = type
        .DefineMethod("MapInternal",
          MethodAttributes.Public | MethodAttributes.HideBySig |
            MethodAttributes.Virtual, typeof (T),
          new Type[] {typeof (IDataReader)});

      ILGenerator il = builder.GetILGenerator();

      // Create a new instance of the T using the associated class loader and
      // stores in a local variable.
      il.DeclareLocal(type_t_);
      MethodInfo callable = typeof (CallableDelegate<T>).GetMethod("Invoke");
      il.Emit(OpCodes.Ldarg_0);
      il.Emit(OpCodes.Ldfld, result.LoaderField);
      il.Emit(OpCodes.Callvirt, callable);
      il.Emit(OpCodes.Stloc_0);

      // Set the value of the properties of the newly created T object.
      OrdinalMap[] fields = result.OrdinalsMapping;
      for (int i = 0, j = fields.Length; i < j; i++) {
        OrdinalMap field = fields[i];
        int ordinal = field.Key;
        PropertyInfo property = field.Value;

        MethodInfo get_x_method =
          Dynamics_.GetDataReaderMethod(
            Dynamics_.GetDataReaderMethodName(field.RawType ??
              property.PropertyType));

        // Get the set method of the current property. If the property does
        // not have a set method ignores it.
        MethodInfo set_x_property = property.GetSetMethod(true);
        if (set_x_property == null) {
          continue;
        }

        il.Emit(OpCodes.Ldloc_0);
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, result.OrdinalsField);
        EmitLoad(il, ordinal);
        il.Emit(OpCodes.Ldelem_I4);
        il.Emit(OpCodes.Callvirt, get_x_method);
        il.Emit(OpCodes.Callvirt, set_x_property);
      }

      ConstantMap[] constant_maps = result.ConstantMappings;
      for (int i = 0, j = constant_maps.Length; i < j; i++) {
        ITypeMap map = constant_maps[i].Key;
        PropertyInfo property = constant_maps[i].Value;
        if (map.MapType != TypeMapType.Ignore) {
          // Get the set method of the current property. If the property does
          // not have a set method ignores it.
          MethodInfo set_x_property = property.GetSetMethod(true);
          if (set_x_property == null) {
            continue;
          }

          il.Emit(OpCodes.Ldloc_0);
          EmitLoad(il, map);
          il.Emit(OpCodes.Callvirt, set_x_property);
        }
      }

      // load the local T and return.
      il.Emit(OpCodes.Ldloc_0);
      il.Emit(OpCodes.Ret);
    }

    // TODO: // optmize the load operation for small types.
    void EmitLoad(ILGenerator il, ITypeMap map) {
      switch (map.MapType) {
        case TypeMapType.Int:
        case TypeMapType.Short:
          EmitLoadInt(il, (int) map.Value);
          break;

        case TypeMapType.Long:
          il.Emit(OpCodes.Ldc_I8, (long) map.Value);
          break;

        case TypeMapType.Boolean:
          if ((bool) map.Value) {
            il.Emit(OpCodes.Ldc_I4_1);
          }
          il.Emit(OpCodes.Ldc_I4_0);
          break;

        case TypeMapType.Byte:
          il.Emit(OpCodes.Ldc_I4, (byte) map.Value);
          break;

        case TypeMapType.Char:
          il.Emit(OpCodes.Ldc_I4, (char) map.Value);
          break;

        case TypeMapType.Double:
          il.Emit(OpCodes.Ldc_R8, (double) map.Value);
          break;

        case TypeMapType.Float:
          il.Emit(OpCodes.Ldc_R4, (float) map.Value);
          break;

        case TypeMapType.ConstString:
          il.Emit(OpCodes.Ldstr, (string) map.Value);
          break;

          // TODO: Find out the IL operation to use to load decimals
        case TypeMapType.Decimal:
          throw new NotImplementedException();
      }
    }

    void EmitLoad(ILGenerator il, int value) {
      if (value > -1 && value < 9) {
        switch (value) {
          case 0:
            il.Emit(OpCodes.Ldc_I4_0);
            break;

          case 1:
            il.Emit(OpCodes.Ldc_I4_1);
            break;

          case 2:
            il.Emit(OpCodes.Ldc_I4_2);
            break;

          case 3:
            il.Emit(OpCodes.Ldc_I4_3);
            break;

          case 4:
            il.Emit(OpCodes.Ldc_I4_4);
            break;

          case 5:
            il.Emit(OpCodes.Ldc_I4_5);
            break;

          case 6:
            il.Emit(OpCodes.Ldc_I4_6);
            break;

          case 7:
            il.Emit(OpCodes.Ldc_I4_7);
            break;

          case 8:
            il.Emit(OpCodes.Ldc_I4_8);
            break;
        }
      } else if (value > -128 && value < 128) {
        il.Emit(OpCodes.Ldc_I4_S, value);
      } else {
        il.Emit(OpCodes.Ldc_I4, value);
      }
    }

    void EmitLoadInt(ILGenerator il, int value) {
      if (value < 8 && value > -1) {
        switch (value) {
          case 0:
            il.Emit(OpCodes.Ldc_I4_0);
            break;
          case 1:
            il.Emit(OpCodes.Ldc_I4_1);
            break;
          case 2:
            il.Emit(OpCodes.Ldc_I4_2);
            break;
          case 3:
            il.Emit(OpCodes.Ldc_I4_3);
            break;
          case 4:
            il.Emit(OpCodes.Ldc_I4_4);
            break;
          case 5:
            il.Emit(OpCodes.Ldc_I4_5);
            break;
          case 6:
            il.Emit(OpCodes.Ldc_I4_6);
            break;
          case 7:
            il.Emit(OpCodes.Ldc_I4_7);
            break;
          case 8:
            il.Emit(OpCodes.Ldc_I4_8);
            break;
        }
      }
      il.Emit(OpCodes.Ldc_I4, value);
    }

    OrdinalMap[] EmitOrdinals(ILGenerator il, MappingResult result) {
      ValueMap[] fields = result.ValueMappings;
      OrdinalMap[] ordinals_mapping = new OrdinalMap[fields.Length];

      if (fields.Length > 0) {
        MethodInfo get_ordinal_method =
          Dynamics_.GetDataReaderMethod("GetOrdinal");

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldc_I4, fields.Length);
        il.Emit(OpCodes.Newarr, typeof (int));
        il.Emit(OpCodes.Stfld, result.OrdinalsField);

        for (int i = 0, j = fields.Length; i < j; i++) {
          il.Emit(OpCodes.Ldarg_0);
          il.Emit(OpCodes.Ldfld, result.OrdinalsField);
          il.Emit(OpCodes.Ldc_I4, i);
          il.Emit(OpCodes.Ldarg_1);
          il.Emit(OpCodes.Ldstr, fields[i].Key);
          il.Emit(OpCodes.Callvirt, get_ordinal_method);
          il.Emit(OpCodes.Stelem_I4);

          ordinals_mapping[i] =
            new OrdinalMap(i, fields[i].Value, fields[i].RawType);
        }
      }
      return ordinals_mapping;
    }

    string GetMemberName(string name) {
      return name.ToLower() + "_";
    }

    bool IsReferenceType(PropertyInfo property) {
      Type type = property.PropertyType;
      if (type.IsValueType || type.Name == "String") {
        return false;
      }
      return true;
    }

    PropertyInfo[] GetProperties() {
      Type[] interfaces = type_t_.GetInterfaces();
      if (interfaces.Length == 0) {
        return type_t_.GetProperties();
      }

      // The interfaces that was already considered.
      List<Type> considered = new List<Type>(5);
      Queue<Type> queue = new Queue<Type>(5);
      List<PropertyInfo> properties = new List<PropertyInfo>(5*6);
      considered.Add(type_t_);
      queue.Enqueue(type_t_);
      while (queue.Count > 0) {
        Type type = queue.Dequeue();
        foreach (Type t in type.GetInterfaces()) {
          if (!considered.Contains(t)) {
            considered.Add(t);
            queue.Enqueue(t);
          }
        }
        properties.AddRange(type.GetProperties());
      }
      return properties.ToArray();
    }

    void GetMappings(PropertyInfo[] properties, MappingResult result) {
      List<ValueMap> value_mappings =
        new List<ValueMap>(properties.Length);
      List<ConstantMap> const_mappings =
        new List<ConstantMap>(properties.Length);
      for (int i = 0, j = properties.Length; i < j; i++) {
        PropertyInfo property = properties[i];
        ITypeMap mapping;

        // If a custom  map was not defined, maps to the name of the property
        // if auto map is enabled, ignoring if not.
        if (!mappings_.TryGetValue(property.Name, out mapping)) {
          mapping = auto_map_
            ? (ITypeMap) new StringTypeMap(property.Name)
            : new IgnoreMapType(property.Name);
        }

        if (mapping.MapType == TypeMapType.Ignore || IsReferenceType(property)) {
          mapping = new IgnoreMapType(GetDefaultValue(property.PropertyType));
          const_mappings.Add(new ConstantMap(mapping, property));
        } else if (mapping.MapType == TypeMapType.String) {
          var map = mapping as StringTypeMap;
          value_mappings.Add(
            new ValueMap((string) map.Value, property, map.RawType));
        } else {
          const_mappings.Add(new ConstantMap(mapping, property));
        }
      }
      result.ValueMappings = value_mappings.ToArray();
      result.ConstantMappings = const_mappings.ToArray();
    }

    object GetDefaultValue(Type type) {
      if (type.IsValueType) {
        return Activator.CreateInstance(type);
      }
      return null;
    }
  }
}
