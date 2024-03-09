using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace propojeniSDS
{
    public class PripojeniKDS
    {
        private string connectionS;
        private static PripojeniKDS pripojeni;
        public static PripojeniKDS Pripojeni 
        {  
            get 
            { 
                if(pripojeni == null)
                {
                    pripojeni = new PripojeniKDS();
                }
                return pripojeni; 
            } 
        }

        /// <summary>
        ///     Metoda pro připojení k databázi. Data pro připojeí se nachází v konfiguračním souboru App.config
        /// </summary>
        private PripojeniKDS()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder();

            connectionStringBuilder.DataSource = ConfigurationManager.AppSettings["Server"];
            connectionStringBuilder.InitialCatalog = ConfigurationManager.AppSettings["Database"];
            connectionStringBuilder.UserID = ConfigurationManager.AppSettings["User"];
            connectionStringBuilder.Password = ConfigurationManager.AppSettings["Password"];

            connectionS = connectionStringBuilder.ConnectionString;
        }
        /// <summary>
        ///     Metoda vracející connection string
        /// </summary>
        /// <returns></returns>
        public string GetConnectionString()
        {
            return connectionS;
        }

        /// <summary>
        ///     Metoda pro posílání query požadavkků (využáván pro select query)
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        #region Queries Methods
        public int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionS))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        return command.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return 0;
        }


        /// <summary>
        ///   Metoda pro posílání query požadavků (využáván pro insert, ... query)
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionS))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return null;
        }
        /// <summary>
        ///       Metoda pro posílání qeery požadavků 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteScalarInt(string query, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionS))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        object result = command.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int intValue))
                        {
                            return intValue;
                        }

                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return 0;
        }


        #endregion
    }
}
