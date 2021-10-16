using System;
using System.Data;
using MySql.Data.MySqlClient;
using MySql.Connect.NetCore.Exceptions;

namespace MySql.Connect.NetCore
{
    /// <summary>
    /// This class can be inherited from classes used for database access. Is the base class for the MySql database connection.
    /// </summary>
    public class BaseConnection : IDisposable
    {
        private MySqlConnection Connection { get; set; }
        private MySqlTransaction Transaction { get; set; }
        private MySqlCommand Command { get; set; }

        /// <summary>
        /// This method opens a new database connection. If some connection is open, that's autommatically closed and opens a new connection.
        /// </summary>
        protected void Connect(string connectionString)
        {
            if (!String.IsNullOrEmpty(connectionString))
            {
                if (this.IsConnectionOpened())
                {
                    this.Connection.Close();
                }

                this.Connection = new MySqlConnection(connectionString);
                this.Connection.Open();
            }
            else
            {
                throw new ArgumentNullException("The connection string is null or empty.");
            }
        }

        /// <summary>
        /// This method closes all the opened connections.
        /// </summary>
        protected void Disconnect()
        {
            if (this.IsConnectionOpened())
            {
                this.Connection.Close();
                this.Connection = null;
            }
        }

        /// <summary>
        /// This method creates a new MySql transaction.
        /// </summary>
        protected void BeginTransaction()
        {
            if (this.IsConnectionOpened())
            {
                this.Transaction = this.Connection.BeginTransaction();
            }
            else
            {
                throw new MySqlOpenedConnectionException("There are no opened connections.");
            }
        }

        /// <summary>
        /// This method Commit the current MySql transaction.
        /// </summary>
        protected void Commit()
        {
            if (this.IsConnectionOpened())
            {
                if (this.Transaction != null)
                {
                    this.Transaction.Commit();
                    this.Transaction = null;
                }
                else
                {
                    throw new MySqlOpenedTransactionException("There are no opened transactions.");
                }
            }
            else
            {
                throw new MySqlOpenedConnectionException("There are no opened connections.");
            }
        }

        /// <summary>
        /// This method Rolls Back the current MySql transaction.
        /// </summary>
        protected void Rollback()
        {
            if (this.IsConnectionOpened())
            {
                if (this.Transaction != null)
                {
                    this.Transaction.Rollback();
                    this.Transaction = null;
                }
                else
                {
                    throw new MySqlOpenedTransactionException("There are no opened transactions.");
                }
            }
            else
            {
                throw new MySqlOpenedConnectionException("There are no opened connections.");
            }
        }

        /// <summary>
        /// This method check if exist any MySqlConnections opened.
        /// </summary>
        private bool IsConnectionOpened()
        {
            if (this.Connection != null && this.Connection.State == ConnectionState.Open)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// This method executes a sql query.
        /// </summary>
        protected void ExecuteSQL(string sql)
        {
            if (this.IsConnectionOpened())
            {
                this.Command = this.Connection.CreateCommand();
                this.Command.Connection = this.Connection;
                this.Command.Transaction = this.Transaction;
                this.Command.CommandText = sql;
                this.Command.ExecuteNonQuery();
            }
            else
            {
                throw new MySqlOpenedConnectionException("There are no opened connections.");
            }
        }

        /// <summary>
        /// This method executes a sql query and returns a MySqlDataReader with the values that the query returns.
        /// </summary>
        protected MySqlDataReader ExecuteReader(string sql)
        {
            if (this.IsConnectionOpened())
            {
                this.Command = this.Connection.CreateCommand();
                this.Command.Connection = this.Connection;
                this.Command.Transaction = this.Transaction;
                this.Command.CommandText = sql;
                return this.Command.ExecuteReader();
            }
            else
            {
                throw new MySqlOpenedConnectionException("There are no opened connections.");
            }
        }

        /// <summary>
        /// This method executes a sql query and returns the last id inserted on the current table.
        /// </summary>
        protected long ExecuteSQLReturnId(string sql)
        {
            if (this.IsConnectionOpened())
            {
                this.Command = this.Connection.CreateCommand();
                this.Command.Connection = this.Connection;
                this.Command.Transaction = this.Transaction;
                this.Command.CommandText = $"{sql}; SELECT LAST_INSERT_ID() as id";

                var reader = this.Command.ExecuteReader();
                var id = 0L;

                while (reader.Read())
                {
                    id = this.ReturnConvertedValue<long>(reader["id"]);
                }

                return id;
            }
            else
            {
                throw new MySqlOpenedConnectionException("There are no opened connections.");
            }
        }

        /// <summary>
        /// This method returns the converted value to the generic type passed on <T>.
        /// </summary>
        protected T ReturnConvertedValue<T>(object value) where T : struct
        {
            if (Convert.IsDBNull(value))
            {
                return default;
            }

            var type = typeof(T);

            if (type.IsEnum)
            {
                return (T)(object)Convert.ToInt32(value);
            }

            return (T)Convert.ChangeType(value, type);
        }

        public void Dispose()
        {

        }
    }
}
