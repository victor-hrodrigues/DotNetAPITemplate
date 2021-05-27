using System;
using System.Data.SqlClient;
using Template.Models;

namespace Template.Repository
{
    public class UserRepository
    {
        private SqlConnection _dbConnection;
        protected SqlConnection DbConnection
        {
            get
            {
                if (_dbConnection is null)
                    _dbConnection = new SqlConnection(Startup.ConnectionString);

                return _dbConnection;
            }
        }

        public bool ValidateCredentials(UserInfo userInfo)
        {
            try
            {
                using (DbConnection)
                {
                    DbConnection.Open();

                    SqlCommand command = new SqlCommand(@"SELECT * FROM user_entity WHERE email = @email AND password = @password");

                    command.Parameters.Add(new SqlParameter("@email", userInfo.Email));
                    command.Parameters.Add(new SqlParameter("@password", userInfo.Password));

                    SqlDataReader reader = command.ExecuteReader();
                    return reader.HasRows;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
