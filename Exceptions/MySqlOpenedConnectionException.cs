using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySql.Connect.NetCore.Exceptions
{
    public class MySqlOpenedConnectionException : Exception
    {
        public MySqlOpenedConnectionException() : base()
        {

        }

        public MySqlOpenedConnectionException(string message) : base(message)
        {

        }

        public MySqlOpenedConnectionException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
