using System;
using MySql.Data.MySqlClient;

namespace Nohros.Data.MySql.Extensions
{
  public static class MySqlExceptionExtensions
  {
    public static ProviderException AsProviderException(
      this MySqlException exception) {
      switch (exception.Number) {
        case 1169: // ER_DUP_UNIQUE
          return new UniqueConstraintViolationException(exception);
      }
      return new ProviderException(exception);
    }
  }
}
