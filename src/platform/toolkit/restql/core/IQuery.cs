﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Nohros.RestQL
{
  /// <summary>
  /// Represents a string that contains data to be passed to a query processor
  /// in order to obtain results from a data source.
  /// </summary>
  /// <remarks>
  /// A query is composed by a plain text having or not parameters within it.
  /// A parameter is a sequence of characters with no spaces delimited by
  /// anohther string (delimiter). The default parameter delimiter is the
  /// string "$".
  /// <para>
  /// Each query should have a unique name.
  /// </para>
  /// </remarks>
  public interface IQuery
  {
    /// <summary>
    /// Gets a <see cref="IDictionary{TKey,TValue}"/> containing the options
    /// configured for the query.
    /// </summary>
    /// <remarks>
    /// Options is a collection of name/vaue pairs that contains useful
    /// informations about the query. This informations is typically used by
    /// query executors.
    /// </remarks>
    IDictionary<string, string> Options { get; }

    /// <summary>
    /// Gets the query's method.
    /// </summary>
    /// <remarks>
    /// The <see cref=" QueryMethod"/> identifies the type of operation that
    /// will be performed on the data base ( retrieve value, modify data,
    /// modify and retrieve data).
    /// </remarks>
    QueryMethod QueryMethod { get; }

    /// <summary>
    /// Gets the name of the parameters associated with the query.
    /// </summary>
    string[] Parameters { get; }

    /// <summary>
    /// Gets or sets a string that identifies the query's type.
    /// </summary>
    /// <remarks>
    /// The query's type is on of the criteria that a
    /// <see cref="IQueryExecutor"/> could use to evaluate if it can execute
    /// the query. For example the <see cref="AbstractSqlQueryExecutor"/> can execute
    /// queries of which type is "sqlquery".
    /// </remarks>
    string Type { get; }

    /// <summary>
    /// Gets a string that uniquely idenfies the query.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the text of the query.
    /// </summary>
    string QueryText { get; }
  }
}
