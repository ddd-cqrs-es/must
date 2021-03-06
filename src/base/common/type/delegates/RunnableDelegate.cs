﻿using System;

namespace Nohros
{
  /// <summary>
  /// Represents a method that returns nothing and receives no parameters.
  /// </summary>
  /// <remarks>
  /// A delegate is a type that defines a signature, that is, the return value
  /// type and parameter list types for a method. The delegate can be used to
  /// declare a variable that can refer to any method with the same signature
  /// as the delegate.
  /// <para>
  /// This delegate defines a method that does not return a value and have no
  /// arguments.
  /// </para>
  /// <para>
  /// <see cref="RunnableDelegate"/> is similar to the delegate
  /// <see cref="EventHandler{TEventArgs}"/> but does not requires a event
  /// argument do not impose an obligation to use a object to identify the
  /// method runner('sender'), which is useless in most cases.
  /// </para>
  /// </remarks>
  public delegate void RunnableDelegate();
}
