using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySql.Connect.NetCore.Exceptions
{
    public class MySqlOpenedTransactionException : Exception
    {
        public MySqlOpenedTransactionException() : base()
        {

        }

        public MySqlOpenedTransactionException(string message) : base(message)
        {

        }

        public MySqlOpenedTransactionException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
