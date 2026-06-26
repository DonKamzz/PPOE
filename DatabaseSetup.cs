using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPOE
{
    public static class DatabaseSetup
    {
        // YOUR MySQL CREDENTIALS
        // Username: st10459758
        // Password: DonKamzz#10
        private static string rootConnection = "Server=localhost;Uid=st10459758;Pwd=DonKamzz#10;";
        private static string dbConnection = "Server=localhost;Database=cybersecurity_bot;Uid=st10459758;Pwd=DonKamzz#10;";

        public static string CreateDatabase()
        {
            try
            {
                string result = "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n";
                result += "  DATABASE SETUP\n";
                result += "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n";

                result += TestConnection();
                result += "\n" + CreateDatabaseIfNotExists();
                result += CreateTables();
                result += InsertQuizQuestions();
                result += InsertSampleTasks();

                result += "\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n";
                result += "✅ SETUP COMPLETE!\n";
                result += "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n";
                result += "• 15 Quiz Questions\n";
                result += "• 3 Sample Tasks\n";
                result += "• 4 Tables Created\n";

                return result;
            }
            catch (Exception ex)
            {
                return $"❌ Error: {ex.Message}\n\nTroubleshooting:\n1. Start MySQL service\n2. Check your username: st10459758\n3. Check your password: DonKamzz#10";
            }
        }

        private static string TestConnection()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(rootConnection))
                {
                    conn.Open();
                    return "✅ Connected to MySQL Server\n";
                }
            }
            catch (Exception ex)
            {
                return $"❌ Connection failed: {ex.Message}\n";
            }
        }

        private static string CreateDatabaseIfNotExists()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(rootConnection))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand("CREATE DATABASE IF NOT EXISTS cybersecurity_bot", conn))
                    {
                        cmd.ExecuteNonQuery();
                        return "✅ Database created or already exists\n";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"❌ Error: {ex.Message}\n";
            }
        }

        private static string CreateTables()
        {
            try
            {
                string query = @"
                    CREATE TABLE IF NOT EXISTS tasks (
                        id INT PRIMARY KEY AUTO_INCREMENT,
                        title VARCHAR(255) NOT NULL,
                        description TEXT,
                        reminder_date DATETIME,
                        is_completed BOOLEAN DEFAULT FALSE,
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        user_name VARCHAR(100)
                    );
                    CREATE TABLE IF NOT EXISTS quiz_questions (
                        id INT PRIMARY KEY AUTO_INCREMENT,
                        question TEXT NOT NULL,
                        option_a VARCHAR(255),
                        option_b VARCHAR(255),
                        option_c VARCHAR(255),
                        option_d VARCHAR(255),
                        correct_answer VARCHAR(50),
                        explanation TEXT,
                        category VARCHAR(50)
                    );
                    CREATE TABLE IF NOT EXISTS activity_logs (
                        id INT PRIMARY KEY AUTO_INCREMENT,
                        action VARCHAR(500) NOT NULL,
                        timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        user_name VARCHAR(100)
                    );
                    CREATE TABLE IF NOT EXISTS users (
                        id INT PRIMARY KEY AUTO_INCREMENT,
                        name VARCHAR(100) NOT NULL,
                        session_start TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                    );";

                using (MySqlConnection conn = new MySqlConnection(dbConnection))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                        return "✅ Tables created successfully\n";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"❌ Error creating tables: {ex.Message}\n";
            }
        }

        private static string InsertQuizQuestions()
        {
            try
            {
                string query = @"
                    INSERT IGNORE INTO quiz_questions (question, option_a, option_b, option_c, option_d, correct_answer, explanation, category) VALUES
                    ('1. What should you do if you receive an email asking for your password?', 'Reply with your password', 'Delete the email', 'Report as phishing and delete', 'Forward to friends', 'C', 'Never share passwords via email.', 'Phishing'),
                    ('2. Which of the following is a strong password?', '123456', 'Password123', 'MyBday$1990', 'G7#kLp$2qW!', 'D', 'Strong passwords contain mixed characters.', 'Password'),
                    ('3. What does HTTPS indicate in a URL?', 'The site is fast', 'The site is secure', 'The site is free', 'The site is new', 'B', 'HTTPS means encrypted connection.', 'Safe Browsing'),
                    ('4. True or False: Public Wi-Fi is safe for online banking.', 'True', 'False', NULL, NULL, 'B', 'Public Wi-Fi is not secure.', 'Safe Browsing'),
                    ('5. What is phishing?', 'A type of virus', 'A scam to steal personal information', 'A password manager', 'A security software', 'B', 'Phishing tricks you into revealing information.', 'Phishing'),
                    ('6. How often should you update your passwords?', 'Never', 'Once a year', 'Every 3-6 months', 'Only when hacked', 'C', 'Regular updates reduce risk.', 'Password'),
                    ('7. What is social engineering?', 'Building websites', 'Psychological manipulation of people', 'Writing code', 'Network security', 'B', 'Social engineering manipulates people.', 'Scams'),
                    ('8. True or False: It is safe to use the same password for multiple accounts.', 'True', 'False', NULL, NULL, 'B', 'Same password puts all accounts at risk.', 'Password'),
                    ('9. What is malware?', 'A security feature', 'Software designed to damage computers', 'A type of password', 'A safe browsing tool', 'B', 'Malware is malicious software.', 'Malware'),
                    ('10. How can you spot a phishing email?', 'Check sender address', 'Look for spelling mistakes', 'Hover over links', 'All of the above', 'D', 'All are signs of phishing.', 'Phishing'),
                    ('11. What is 2FA?', 'Two-Factor Authentication', 'Second File Access', 'Two-Form Approval', 'Second Factor Approval', 'A', '2FA adds an extra security layer.', 'Password'),
                    ('12. What should you do if you think your account is hacked?', 'Ignore it', 'Change password immediately', 'Tell friends', 'Delete your account', 'B', 'Immediately change your password.', 'Scams'),
                    ('13. True or False: All websites with a padlock icon are completely safe.', 'True', 'False', NULL, NULL, 'B', 'Padlock indicates encryption, not safety.', 'Safe Browsing'),
                    ('14. What type of scam involves someone pretending to be a trusted person?', 'Phishing', 'Impersonation', 'Malware', 'Denial of Service', 'B', 'Impersonation scams pretend to be trusted.', 'Scams'),
                    ('15. How can you protect your privacy online?', 'Share everything', 'Use strong passwords', 'Limit personal information shared', 'Ignore privacy settings', 'C', 'Limiting what you share protects privacy.', 'Privacy');";

                using (MySqlConnection conn = new MySqlConnection(dbConnection))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        int rows = cmd.ExecuteNonQuery();
                        return $"✅ {rows} quiz questions inserted\n";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"❌ Error inserting quiz questions: {ex.Message}\n";
            }
        }

        private static string InsertSampleTasks()
        {
            try
            {
                string query = @"
                    INSERT IGNORE INTO tasks (title, description, reminder_date, user_name) VALUES
                    ('Enable Two-Factor Authentication', 'Enable 2FA on all your important accounts.', DATE_ADD(NOW(), INTERVAL 3 DAY), 'Demo'),
                    ('Review Privacy Settings', 'Review privacy settings on social media platforms.', DATE_ADD(NOW(), INTERVAL 5 DAY), 'Demo'),
                    ('Update Passwords', 'Update passwords for all accounts with strong unique passwords.', DATE_ADD(NOW(), INTERVAL 7 DAY), 'Demo');";

                using (MySqlConnection conn = new MySqlConnection(dbConnection))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        int rows = cmd.ExecuteNonQuery();
                        return $"✅ {rows} sample tasks inserted\n";
                    }
                }
            }
            catch (Exception ex)
            {
                return $"❌ Error inserting sample tasks: {ex.Message}\n";
            }
        }
    }
}
