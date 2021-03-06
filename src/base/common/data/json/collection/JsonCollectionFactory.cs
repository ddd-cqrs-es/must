﻿using System;
using System.Data;
using Nohros.Resources;

namespace Nohros.Data.Json
{
  /// <summary>
  /// The default implementation of the <see cref="IJsonCollectionFactory"/>
  /// interface. A <see cref="JsonCollectionFactory"/> should be used to
  /// create instances of built-in json collections.
  /// </summary>
  public class JsonCollectionFactory : IJsonCollectionFactory
  {
    /// <summary>
    /// The name of the <see cref="JsonTable"/> collection.
    /// </summary>
    public const string kJsonTableCollection = "table";

    /// <summary>
    /// The name of the <see cref="JsonObject"/> collection.
    /// </summary>
    public const string kJsonObjectCollection = "object";

    /// <summary>
    /// The name of the <see cref="JsonArray"/> collection.
    /// </summary>
    public const string kJsonArrayCollection = "array";

    /// <summary>
    /// The name of the aray of <see cref="JsonObject"/> collection.
    /// </summary>
    public const string kJsonArrayOfObjectsCollection = "arrayOfObjects";

    /// <inheritdoc/>
    public IJsonCollection CreateJsonCollection(string name) {
      switch (name) {
        case kJsonObjectCollection:
          return new JsonObject();

        case kJsonArrayCollection:
        case kJsonArrayOfObjectsCollection:
          return new JsonArray();

        case kJsonTableCollection:
          return new JsonTable();

        default:
          throw new NotSupportedException(string.Format(
            StringResources.NotSupported_CannotCreateType, name));
      }
    }

    /// <inheritdoc/>
    public IJsonCollection CreateJsonCollection(string name, IDataReader reader) {
      switch (name) {
        case kJsonObjectCollection:
          return CreateJsonObject(reader);

        case kJsonArrayCollection:
          return CreateJsonArray(reader);

        case kJsonTableCollection:
          return CreateJsonTable(reader);

        case kJsonArrayOfObjectsCollection:
          return CreateJsonArrayOfObject(reader);

        default:
          throw new NotSupportedException(string.Format(
            StringResources.NotSupported_CannotCreateType, name));
      }
    }

    /// <summary>
    /// Creates a <see cref="JsonObject"/> containing the data readed from
    /// <paramref name="reader"/>.
    /// </summary>
    /// <param name="reader">
    /// A <see cref="IDataReader"/> object that contains the data that will be
    /// used to populate a <see cref="JsonObject"/>.
    /// </param>
    /// <returns>
    /// A <see cref="JsonObject"/> object that contains the data readed from
    /// the <paramref name="reader"/>.
    /// </returns>
    /// <remarks>
    /// This method reads only the first line of th <paramref name="reader"/>
    /// and discards the rest.
    /// </remarks>
    public JsonObject CreateJsonObject(IDataReader reader) {
      JsonObject json_object = new JsonObject();
      if (reader.Read()) {
        IJsonDataField[] json_data_fields = GetJsonDataFields(reader);
        int length = json_data_fields.Length;
        do {
          for (int i = 0, j = length; i < j; i++) {
            IJsonDataField json_data_field = json_data_fields[i];
            JsonObject.JsonMember json_object_member =
              new JsonObject.JsonMember(json_data_field.Name,
                json_data_field.GetValue(reader));
            json_object.Add(json_object_member);
          }
        } while (reader.Read());
      }
      return json_object;
    }

    /// <summary>
    /// Creates a <see cref="JsonArray"/> containing the data readed from
    /// <paramref name="reader"/>.
    /// </summary>
    /// <param name="reader">
    /// A <see cref="IDataReader"/> object that contains the data that will be
    /// used to populate a <see cref="JsonArray"/>.
    /// </param>
    /// <returns>
    /// A <see cref="JsonArray"/> object that contains the data readed from
    /// the <paramref name="reader"/>.
    /// </returns>
    /// <remarks>
    /// This method reads only the first line of th <paramref name="reader"/>
    /// and discards the rest.
    /// </remarks>
    public JsonArray CreateJsonArray(IDataReader reader) {
      JsonArray json_array = new JsonArray();
      if (reader.Read()) {
        IJsonDataField[] json_data_fields = GetJsonDataFields(reader);
        do {
          int length = json_data_fields.Length;
          for (int i = 0, j = length; i < j; i++) {
            IJsonDataField json_data_field = json_data_fields[i];
            json_array.Add(json_data_field.GetValue(reader));
          }
        } while (reader.Read());
      }
      return json_array;
    }

    /// <summary>
    /// Creates a <see cref="JsonTable"/> containing the data readed from
    /// <paramref name="reader"/>.
    /// </summary>
    /// <param name="reader">
    /// A <see cref="IDataReader"/> object that contains the data that will be
    /// used to populate a <see cref="JsonTable"/>.
    /// </param>
    /// <returns>
    /// A <see cref="JsonTable"/> object that contains the data readed from
    /// the <paramref name="reader"/>.
    /// </returns>
    public JsonTable CreateJsonTable(IDataReader reader) {
      if (reader.Read()) {
        IJsonDataField[] json_data_fields = GetJsonDataFields(reader);
        int length = json_data_fields.Length;

        // Get the column names for the json table.
        string[] columns = new string[length];
        for (int i = 0, j = length; i < j; i++) {
          columns[i] = json_data_fields[i].Name;
        }

        JsonTable json_table = new JsonTable(columns);

        // Read the values into a sequence of json arrays and append it to
        // the json table.
        do {
          JsonArray json_array = new JsonArray();
          for (int i = 0, j = length; i < j; i++) {
            IJsonDataField json_data_field = json_data_fields[i];
            json_array.Add(json_data_field.GetValue(reader));
          }
          json_table.Add(json_array);
        } while (reader.Read());
        return json_table;
      }
      return new JsonTable();
    }

    /// <summary>
    /// Creates a <see cref="JsonArray"/> containing the data readed from
    /// <paramref name="reader"/>.
    /// </summary>
    /// <param name="reader">
    /// A <see cref="IDataReader"/> object that contains the data that will be
    /// used to populate a <see cref="JsonArray"/>.
    /// </param>
    /// <returns>
    /// A <see cref="JsonArray"/> object that contains the data readed from
    /// the <paramref name="reader"/>.
    /// </returns>
    /// <remarks>
    /// Each line readed from the <paramref name="reader"/> is mapped to a
    /// <see cref="JsonObject"/> object.
    /// </remarks>
    public JsonArray CreateJsonArrayOfObject(IDataReader reader) {
      JsonArray json_array = new JsonArray();
      if (reader.Read()) {
        IJsonDataField[] json_data_fields = GetJsonDataFields(reader);

        int length = json_data_fields.Length;
        do {
          JsonObject json_object = new JsonObject();
          json_array.Add(json_object);
          for (int i = 0, j = length; i < j; i++) {
            IJsonDataField json_data_field = json_data_fields[i];
            JsonObject.JsonMember json_object_member =
              new JsonObject.JsonMember(json_data_field.Name,
                json_data_field.GetValue(reader));
            json_object.Add(json_object_member);
          }
        } while (reader.Read());
      }
      return json_array;
    }

    IJsonDataField[] GetJsonDataFields(IDataReader reader) {
      int count = reader.FieldCount;
      IJsonDataField[] json_data_fields = new IJsonDataField[count];
      for (int i = 0; i < count; i++) {
        json_data_fields[i] = JsonDataFields.CreateDataField(reader.GetName(i),
          i, reader.GetFieldType(i));
      }
      return json_data_fields;
    }
  }
}
