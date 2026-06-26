using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPOE
{
    public class DatabaseHelper
    {
        // YOUR MySQL CREDENTIALS
        // Username: st10459758
        // Password: DonKamzz#10
        private string connectionString = "Server=localhost;Database=cybersecurity_bot;Uid=st10459758;Pwd=DonKamzz#10;";

        public DatabaseHelper()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    Console.WriteLine("Database connection successful.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection error: {ex.Message}");
            }
        }

        public bool AddTask(string title, string description, DateTime? reminderDate, string userName)
        {
            try
            {
                if (string.IsNullOrEmpty(userName)) userName = "Anonymous";

                string query = @"INSERT INTO tasks (title, description, reminder_date, user_name) 
                                VALUES (@title, @description, @reminderDate, @userName)";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@title", title ?? "Untitled");
                        cmd.Parameters.AddWithValue("@description", description ?? "");
                        cmd.Parameters.AddWithValue("@reminderDate", reminderDate.HasValue ? (object)reminderDate.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@userName", userName);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddTask error: {ex.Message}");
                return false;
            }
        }

        public List<TaskItem> GetTasks(string userName)
        {
            List<TaskItem> tasks = new List<TaskItem>();

            try
            {
                string query = @"SELECT id, title, description, reminder_date, is_completed, created_at 
                                FROM tasks WHERE user_name = @userName 
                                ORDER BY is_completed ASC, created_at DESC";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userName", userName ?? "Anonymous");
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tasks.Add(new TaskItem
                                {
                                    Id = reader.GetInt32("id"),
                                    Title = reader.GetString("title"),
                                    Description = reader.IsDBNull(reader.GetOrdinal("description")) ? "" : reader.GetString("description"),
                                    ReminderDate = reader.IsDBNull(reader.GetOrdinal("reminder_date")) ? (DateTime?)null : reader.GetDateTime("reminder_date"),
                                    IsCompleted = reader.GetBoolean("is_completed"),
                                    CreatedAt = reader.GetDateTime("created_at"),
                                    UserName = userName
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetTasks error: {ex.Message}");
            }

            return tasks;
        }

        public bool MarkTaskComplete(int taskId)
        {
            try
            {
                string query = "UPDATE tasks SET is_completed = TRUE WHERE id = @taskId";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@taskId", taskId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MarkTaskComplete error: {ex.Message}");
                return false;
            }
        }

        public bool DeleteTask(int taskId)
        {
            try
            {
                string query = "DELETE FROM tasks WHERE id = @taskId";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@taskId", taskId);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteTask error: {ex.Message}");
                return false;
            }
        }

        public bool UpdateTaskReminder(int taskId, DateTime reminderDate, string description)
        {
            try
            {
                string query = "UPDATE tasks SET reminder_date = @reminderDate, description = @description WHERE id = @taskId";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@taskId", taskId);
                        cmd.Parameters.AddWithValue("@reminderDate", reminderDate);
                        cmd.Parameters.AddWithValue("@description", description ?? "");
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateTaskReminder error: {ex.Message}");
                return false;
            }
        }

        public List<QuizQuestion> GetQuizQuestions()
        {
            List<QuizQuestion> questions = new List<QuizQuestion>();
            try
            {
                string query = "SELECT * FROM quiz_questions";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                questions.Add(new QuizQuestion
                                {
                                    Id = reader.GetInt32("id"),
                                    Question = reader.GetString("question"),
                                    OptionA = reader.IsDBNull(reader.GetOrdinal("option_a")) ? null : reader.GetString("option_a"),
                                    OptionB = reader.IsDBNull(reader.GetOrdinal("option_b")) ? null : reader.GetString("option_b"),
                                    OptionC = reader.IsDBNull(reader.GetOrdinal("option_c")) ? null : reader.GetString("option_c"),
                                    OptionD = reader.IsDBNull(reader.GetOrdinal("option_d")) ? null : reader.GetString("option_d"),
                                    CorrectAnswer = reader.GetString("correct_answer"),
                                    Explanation = reader.IsDBNull(reader.GetOrdinal("explanation")) ? "" : reader.GetString("explanation"),
                                    Category = reader.IsDBNull(reader.GetOrdinal("category")) ? "General" : reader.GetString("category")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetQuizQuestions error: {ex.Message}");
            }
            return questions;
        }

        public bool LogActivity(string action, string userName)
        {
            try
            {
                string query = "INSERT INTO activity_logs (action, user_name) VALUES (@action, @userName)";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@action", action);
                        cmd.Parameters.AddWithValue("@userName", userName ?? "Anonymous");
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LogActivity error: {ex.Message}");
                return false;
            }
        }

        public List<string> GetActivityLogs(string userName, int count = 10)
        {
            List<string> logs = new List<string>();
            try
            {
                string query = @"SELECT action, timestamp FROM activity_logs 
                                WHERE user_name = @userName 
                                ORDER BY timestamp DESC LIMIT @count";

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userName", userName);
                        cmd.Parameters.AddWithValue("@count", count);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                logs.Add($"{reader.GetDateTime("timestamp"):HH:mm} - {reader.GetString("action")}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetActivityLogs error: {ex.Message}");
            }
            return logs;
        }

        public bool SaveUser(string name)
        {
            try
            {
                string query = "INSERT INTO users (name) VALUES (@name)";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SaveUser error: {ex.Message}");
                return false;
            }
        }
    }
}
