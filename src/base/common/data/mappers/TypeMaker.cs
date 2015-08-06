using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Nohros.Dynamics;
using Nohros.Extensions;
using System.Linq;

namespace Nohros.Data
{
  public partial class DataReaderMapperBuilder<T>
  {
    /// <summary>
    /// A class that is used to implement interfaces that does not have any
    /// behaviour except for storage and retrieval of its own data. 
    /// </summary>
    class TypeMaker
    {
      readonly Type type_t_;

      /// <summary>
      /// Initialize a new instance of the <see cref="TypeMaker"/>
      /// </summary>
      public TypeMaker() {
        type_t_ = typeof (T);
      }

      void EnsureNoMethods(IEnumerable<Type> interfaces) {
        //var observed_types = new HashSet<Type>();
        //var types_to_scan = new Queue<Type>(5);

        //observed_types.Add(type_t_);
        //types_to_scan.Enqueue(type_t_);

        //while (types_to_scan.Count > 0) {
        //Type type = types_to_scan.Dequeue();
        //foreach (Type t in type.GetInterfaces()) {
        //if (observed_types.Add(t)) {
        //types_to_scan.Enqueue(t);
        //}
        //}
        //}

        // If one of the interfaces of the type's hierarchy defines a method
        // we should thrown an exception since we cannot implement it.
        foreach (Type @interface in interfaces) {
          MethodInfo[] methods = @interface.GetMethods();
          foreach (MethodInfo method in methods) {
            if (!method.IsSpecialName) {
              throw new ArgumentException(
                "The interface \"{0}\" defines a method and cannot be dynamically implemented"
                  .Fmt(type_t_));
            }
          }
        }
      }

      public Type MakeType(string impl_type_name, PropertyInfo[] properties) {
        const TypeAttributes kTypeAttributes =
          TypeAttributes.Public |
            TypeAttributes.Class |
            TypeAttributes.AutoClass |
            TypeAttributes.AutoLayout;

        var interfaces =
          new List<Type>(type_t_.GetInterfaces()) {
            type_t_
          }.ToArray();

        TypeBuilder builder =
          Dynamics_
            .ModuleBuilder
            .DefineType(impl_type_name, kTypeAttributes,
              null, interfaces);

        EnsureNoMethods(interfaces);

        foreach (PropertyInfo property in properties) {
          MakeProperty(builder, property);
        }
        return builder.CreateType();
      }

      void MakeProperty(TypeBuilder type, PropertyInfo property) {
        FieldBuilder field =
          type
            .DefineField(property.Name.ToLower() + "_", property.PropertyType,
              FieldAttributes.Private);

        PropertyBuilder p =
          type
            .DefineProperty(property.Name, property.Attributes,
              property.PropertyType, null);

        const MethodAttributes kMethodAttributes =
          MethodAttributes.Public |
            MethodAttributes.HideBySig |
            MethodAttributes.SpecialName;

        if (property.Name == "Item") {
          throw new ArgumentException(
            "The property \"{0}\" could not be automatically implemented.".Fmt(
              property.Name));
        }

        // Getter
        MethodInfo get_method = property.GetGetMethod();
        if (get_method == null) {
          throw new ArgumentException(
            "The property \"{0}\" does not have a get method.".Fmt(property.Name));
        }

        ParameterInfo[] args = get_method.GetParameters();
        Type[] args_types = Array.ConvertAll(args, a => a.ParameterType);

        MethodBuilder @get =
          type
            .DefineMethod(get_method.Name,
              get_method.Attributes
                & ~MethodAttributes.Abstract
                | MethodAttributes.Final,
              get_method.CallingConvention, get_method.ReturnType,
              args_types);

        ILGenerator get_il = @get.GetILGenerator();
        get_il.Emit(OpCodes.Ldarg_0);
        get_il.Emit(OpCodes.Ldfld, field);
        get_il.Emit(OpCodes.Ret);
        p.SetGetMethod(@get);

        // Setter
        MethodInfo set_method = property.GetSetMethod();
        MethodBuilder @set;
        if (set_method != null) {
          args = set_method.GetParameters();
          args_types = Array.ConvertAll(args, a => a.ParameterType);
          @set =
            type
              .DefineMethod(set_method.Name,
                set_method.Attributes
                  & ~MethodAttributes.Abstract
                  | MethodAttributes.Final,
                set_method.CallingConvention, set_method.ReturnType,
                args_types);
        } else {
          @set =
            type
              .DefineMethod("set_" + property.Name, kMethodAttributes,
                get_method.CallingConvention, typeof (void),
                new[] {property.PropertyType});
        }

        ILGenerator set_il = @set.GetILGenerator();
        set_il.Emit(OpCodes.Ldarg_0);
        set_il.Emit(OpCodes.Ldarg_1);
        set_il.Emit(OpCodes.Stfld, field);
        set_il.Emit(OpCodes.Ret);
        p.SetSetMethod(@set);
      }
    }
  }
}
