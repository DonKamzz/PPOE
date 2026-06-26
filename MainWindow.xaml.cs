using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PPOE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CybersecurityBot chatbot;
        private UserProfile userProfile;
        private ActivityLogger logger;
        private ObservableCollection<CyberChatMessage> messages = new ObservableCollection<CyberChatMessage>();
        private SoundPlayer voiceGreeting;
        private bool waitingForName = true;

        public MainWindow()
        {
            InitializeComponent();
            LoadLogoImage();
            PlayVoiceGreeting();
            InitializeChatbot();
            lstConversation.ItemsSource = messages;
            AskForUserName();
            txtStatus.Text = "Online - Ready";
        }

        // ========== WINDOW CONTROL METHODS ==========

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                MaximizeWindow_Click(sender, e);
            }
            else
            {
                this.DragMove();
            }
        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // ========== EXISTING METHODS ==========

        private void AskForUserName()
        {
            AddBotMessage("Welcome to CyberShield! 🛡️");
            AddBotMessage("I'm your Security Assistant, here to help you stay safe online.");
            AddBotMessage("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            AddBotMessage("Before we begin, what is your name?");
            AddBotMessage("(Please enter your name to continue)");
        }

        private void LoadLogoImage()
        {
            try
            {
                string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                string projectDir = Directory.GetParent(exeDir).Parent.Parent.FullName;

                string[] paths = {
                    Path.Combine(exeDir, "lock.png"),
                    Path.Combine(exeDir, "logo.png"),
                    Path.Combine(exeDir, "Assets", "lock.png"),
                    Path.Combine(exeDir, "Assets", "logo.png"),
                    Path.Combine(projectDir, "lock.png"),
                    Path.Combine(projectDir, "logo.png"),
                    Path.Combine(projectDir, "Assets", "lock.png"),
                    Path.Combine(projectDir, "Assets", "logo.png")
                };

                foreach (string path in paths)
                {
                    if (File.Exists(path))
                    {
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(path);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        imgLogo.Source = bitmap;
                        break;
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Logo error: {ex.Message}"); }
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                string[] paths = {
                    Path.Combine(exeDir, "sound.wav"),
                    Path.Combine(exeDir, "Assets", "sound.wav")
                };

                foreach (string path in paths)
                {
                    if (File.Exists(path))
                    {
                        voiceGreeting = new SoundPlayer(path);
                        voiceGreeting.Play();
                        break;
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Sound error: {ex.Message}"); }
        }

        private void InitializeChatbot()
        {
            logger = new ActivityLogger();
            userProfile = new UserProfile();
            chatbot = new CybersecurityBot(userProfile, logger);
            logger.AddLog("Chatbot initialized");
        }

        private void AddUserMessage(string message)
        {
            messages.Add(new CyberChatMessage
            {
                Message = message,
                Timestamp = DateTime.Now,
                IsUserMessage = true
            });
            ScrollToBottom();
        }

        private void AddBotMessage(string message)
        {
            messages.Add(new CyberChatMessage
            {
                Message = message,
                Timestamp = DateTime.Now,
                IsUserMessage = false
            });
            ScrollToBottom();
        }

        private void ScrollToBottom()
        {
            if (lstConversation != null && lstConversation.Items.Count > 0)
                lstConversation.ScrollIntoView(lstConversation.Items[lstConversation.Items.Count - 1]);
        }

        private void ProcessUserName(string input)
        {
            string name = null;
            string lower = input.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                AddBotMessage("Please tell me your name to continue.");
                return;
            }

            if (lower.Contains("my name is"))
            {
                int start = lower.IndexOf("my name is") + 10;
                if (start < input.Length)
                    name = input.Substring(start).Trim().Split(' ')[0];
            }
            else if (lower.Contains("i am") && !lower.Contains("i am interested"))
            {
                int start = lower.IndexOf("i am") + 4;
                if (start < input.Length)
                    name = input.Substring(start).Trim().Split(' ')[0];
            }
            else if (lower.StartsWith("i'm") || lower.StartsWith("im "))
            {
                int start = lower.StartsWith("i'm") ? 3 : 2;
                if (start < input.Length)
                    name = input.Substring(start).Trim().Split(' ')[0];
            }
            else
            {
                string[] words = input.Trim().Split(' ');
                if (words.Length == 1 && words[0].Length >= 2)
                    name = words[0];
            }

            if (!string.IsNullOrEmpty(name) && name.Length >= 2)
            {
                name = char.ToUpper(name[0]) + name.Substring(1).ToLower();
                userProfile.UserName = name;
                waitingForName = false;
                logger.AddLog($"User name set to: {name}");

                txtUserName.Text = $"👤 {name}";
                txtStatus.Text = $"Online - Chatting with {name}";

                AddBotMessage($"Nice to meet you, {name}! 👋");
                AddBotMessage("I'll remember your name throughout our conversation.");
                AddBotMessage("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                AddBotMessage("Now, let's talk about cybersecurity!");
                AddBotMessage("");
                AddBotMessage("What would you like to learn about today?");
                AddBotMessage("");
                AddBotMessage("Try asking me about:");
                AddBotMessage("  🔐 Passwords and how to create strong ones");
                AddBotMessage("  🎣 Phishing scams and how to spot them");
                AddBotMessage("  🛡️ Online privacy and protection");
                AddBotMessage("  ⚠️ Common scams in South Africa");
                AddBotMessage("");
                AddBotMessage("Or type 'help' to see all topics I can help with.");
                AddBotMessage("");
                AddBotMessage("📋 Click 'Tasks' to manage your cybersecurity tasks");
                AddBotMessage("🎯 Click 'Quiz' to test your cybersecurity knowledge");
                AddBotMessage("⚡ Click 'DB Setup' to setup the database once");
            }
            else
            {
                AddBotMessage("I didn't catch your name properly.");
                AddBotMessage("Please tell me your name using one of these formats:");
                AddBotMessage("  - My name is John");
                AddBotMessage("  - I am Sarah");
                AddBotMessage("  - I'm Thabo");
                AddBotMessage("  - Or just type your name");
                AddBotMessage("");
                AddBotMessage("What is your name?");
            }
        }

        private void SendMessage()
        {
            string input = txtUserInput.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            AddUserMessage(input);

            if (waitingForName)
                ProcessUserName(input);
            else
                AddBotMessage(chatbot.GetResponse(input));

            txtUserInput.Clear();
            txtUserInput.Focus();
            txtStatus.Text = $"Online - Last message: {DateTime.Now:HH:mm:ss}";
        }

        private void btnSend_Click(object sender, RoutedEventArgs e) => SendMessage();

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            messages.Clear();
            waitingForName = true;
            AskForUserName();
            txtUserName.Text = "";
            logger.AddLog("Conversation cleared");
        }

        private void btnTasks_Click(object sender, RoutedEventArgs e)
        {
            if (waitingForName)
            {
                AddBotMessage("Please tell me your name first!");
                return;
            }
            try
            {
                AddBotMessage(chatbot.GetResponse("show tasks"));
                AddBotMessage("");
                AddBotMessage("💡 To add a task: 'add task: [description]'");
                AddBotMessage("💡 To set reminder: 'remind 1 tomorrow'");
                AddBotMessage("💡 To complete: 'complete 1'");
                AddBotMessage("💡 To delete: 'delete 1'");
                logger.AddLog($"Tasks viewed by {userProfile.UserName}");
            }
            catch (Exception ex)
            {
                AddBotMessage($"❌ Error: {ex.Message}");
                AddBotMessage("Click 'DB Setup' first.");
            }
        }

        private void btnQuiz_Click(object sender, RoutedEventArgs e)
        {
            if (waitingForName)
            {
                AddBotMessage("Please tell me your name first!");
                return;
            }
            try
            {
                QuizWindow quiz = new QuizWindow(userProfile.UserName);
                quiz.Owner = this;
                quiz.ShowDialog();
                logger.AddLog($"Quiz opened by {userProfile.UserName}");
            }
            catch (Exception ex)
            {
                AddBotMessage($"❌ Error: {ex.Message}");
                AddBotMessage("Click 'DB Setup' first.");
            }
        }

        private void btnSetupDB_Click(object sender, RoutedEventArgs e)
        {
            AddBotMessage("⚡ Setting up database...");
            try
            {
                string result = DatabaseSetup.CreateDatabase();
                foreach (string line in result.Split('\n'))
                    if (!string.IsNullOrWhiteSpace(line)) AddBotMessage(line.Trim());
                txtStatus.Text = "Online - Database ready";
                logger.AddLog("Database setup completed");
            }
            catch (Exception ex)
            {
                AddBotMessage($"❌ Error: {ex.Message}");
                AddBotMessage("Troubleshooting:");
                AddBotMessage("1. Start MySQL service");
                AddBotMessage("2. Check connection string");
                txtStatus.Text = "Online - Database error";
            }
        }

        private void txtUserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) SendMessage();
        }
    }
}
