using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using NeuralNet_WPF_UI.Models; 

namespace NeuralNet_WPF_UI.DB
{
    internal class DBMethods
    {
        public static void AddResult(string strategyName, double lossResult, double accuracy, double learningRate)
        {
            using (var connection = DBConfig.GetConnection())
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
                connection.Open();
                string query = "INSERT INTO training_results (strategy_name, loss_result, accuracy, learning_rate, timestamp) " +
                               "VALUES (@strat, @loss, @acc, @lr, @time)";

                using (var cmd = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@strat", strategyName);
                    cmd.Parameters.AddWithValue("@loss", lossResult);
                    cmd.Parameters.AddWithValue("@acc", accuracy);
                    cmd.Parameters.AddWithValue("@lr", learningRate);
                    cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<TrainingRecord> GetResults()
        {
            List<TrainingRecord> list = new List<TrainingRecord>();
            try
            {
                using (var connection = DBConfig.GetConnection())
                {
                    
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }

                    connection.Open();

                    string query = "SELECT id, strategy_name, loss_result, accuracy, learning_rate, timestamp FROM training_results ORDER BY id DESC";

                    using (var cmd = new MySqlCommand(query, (MySqlConnection)connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new TrainingRecord
                            {
                                id = Convert.ToInt32(reader["id"]),
                                Strategy = reader["strategy_name"].ToString(),
                                Result = Convert.ToDouble(reader["loss_result"]),
                                Accuracy = Convert.ToDouble(reader["accuracy"]),
                                LearningRate = Convert.ToDouble(reader["learning_rate"]),
                                Timestamp = reader["timestamp"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a beolvasásnál: {ex.Message}");
            }
            return list;
        }

        
        public static void DeleteResultByid(int id)
        {
            try
            {
                using (var connection = DBConfig.GetConnection())
                {
                    if (connection.State != System.Data.ConnectionState.Open)
                        connection.Open();

                    
                    string query = "DELETE FROM training_results WHERE id = @id";

                    using (var cmd = new MySqlCommand(query, (MySqlConnection)connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba az id alapú törlés során: {ex.Message}", "Adatbázis hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        public static void UpdateResultById(int id, string newStrategy, double newLoss, double newAccuracy, double learningRate)
        {
            using (var connection = DBConfig.GetConnection())
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    connection.Open();
                }

                string query = "UPDATE training_results " +
                               "SET strategy_name = @newStrat, loss_result = @newLoss, accuracy = @newAcc, learning_rate = @lr, timestamp = @time " +
                               "WHERE id = @id";

                using (var cmd = new MySqlCommand(query, (MySqlConnection)connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@newStrat", newStrategy);
                    cmd.Parameters.AddWithValue("@newLoss", newLoss);
                    cmd.Parameters.AddWithValue("@newAcc", newAccuracy);
                    cmd.Parameters.AddWithValue("@lr", learningRate);
                    cmd.Parameters.AddWithValue("@time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}