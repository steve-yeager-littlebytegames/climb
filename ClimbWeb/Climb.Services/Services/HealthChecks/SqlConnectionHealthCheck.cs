using System.Data.Common;
using System.Data.SqlClient;

namespace Climb.Services.HealthChecks
{
    public class SqlConnectionHealthCheck : DbConnectionHealthCheck
    {
        public SqlConnectionHealthCheck(string connectionString)
            : base(connectionString)
        {
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}