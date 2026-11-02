using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NeuralNet_WPF_UI.DB
{
    internal static class DBConfig
    {
        private static readonly string connectionString = "server=localhost;port=3306;username=root;password=;database=neuralnetdb;";

        public static DbConnection GetConnection()
        {
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
                Console.WriteLine("Connection successful!");
                return connection;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySQL error: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Általános hiba: {ex.Message}");
                return null;
            }
        }
    }
}
