using System.Data;
using System.Data.SqlClient;
using ChiDaram.Common.Classes;

namespace ChiDaram.Data.DataService
{
    public class BaseDataService
    {
        protected readonly ConnectionStrings ConnectionStrings;
        public BaseDataService(ConnectionStrings connectionStrings)
        {
            ConnectionStrings = connectionStrings;
        }

        protected IDbConnection GetChiDaramDataBaseDbConnection
        {
            get
            {
                var sqlConnection = new SqlConnection(ConnectionStrings.ChiDaramDataBase);
                sqlConnection.Open();
                return sqlConnection;
            }
        }
    }
}
