using System;
using System.Collections.Generic;
using System.Linq;

namespace PPOE
{
    public class CybersecurityBot
    {
        private Random random = new Random();
        private UserProfile userProfile;
        private ActivityLogger logger;
        private DatabaseHelper dbHelper;

        private Dictionary<string, List<string>> responses = new Dictionary<string, List<string>>()
        {
            ["password"] = new List<string>
            {
                "🔐 Use strong, unique passwords for each account.",
                "🔐 Create passwords with 12+ characters, mix of letters, numbers, and symbols.",
                "🔐 Never share your passwords. Use a password manager.",
                "🔐 Enable two-factor authentication for extra security!"
            },
            ["phish"] = new List<string>
            {
                "🎣 Never click suspicious links in emails.",
                "🎣 Always check the sender's email address carefully.",
                "🎣 If an email creates urgency, it's likely phishing.",
                "🎣 Report phishing emails to help protect others."
            },
            ["scam"] = new List<string>
            {
                "⚠️ If it sounds too good to be true, it probably is!",
                "⚠️ Never send money to someone you've only met online.",
                "⚠️ Verify unexpected calls by contacting the organization directly.",
                "⚠️ Report scams to 0800 222 777."
            },
            ["privacy"] = new List<string>
            {
                "🛡️ Review your privacy settings on social media regularly.",
                "🛡️ Only share necessary personal information online.",
                "🛡️ Regularly review which apps have access to your accounts.",
                "🛡️ Use the principle of least privilege."
            }
        };

        private List<string> greetings = new List<string>
        {
            "Hello! I'm your Cybersecurity Assistant. How can I help you today?",
            "Welcome back! Ready to learn about staying secure online?",
            "Hi there! What would you like to learn about cybersecurity today?"
        };

        public CybersecurityBot(UserProfile profile, ActivityLogger activityLogger)
        {
            userProfile = profile;
            logger = activityLogger;
            dbHelper = new DatabaseHelper();
        }

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Please type something so I can help you!";

            string lowerInput = input.Trim().ToLower();
            logger.AddLog($"User: {input}");

            // Add Task with Reminder
            if (lowerInput.Contains("add task") || lowerInput.Contains("add:") || lowerInput.Contains("new task"))
                return AddTask(input);

            // Show Tasks
            if (lowerInput.Contains("show tasks") || lowerInput.Contains("tasks") || lowerInput.Contains("my tasks"))
                return ShowTasks();

            // Complete Task
            if (lowerInput.Contains("complete") || lowerInput.Contains("done"))
                return CompleteTask(input);

            // Delete Task
            if (lowerInput.Contains("delete") || lowerInput.Contains("remove"))
                return DeleteTask(input);

            // Set Reminder
            if (lowerInput.Contains("remind") || lowerInput.Contains("set reminder"))
                return SetReminder(input);

            // Activity Log
            if (lowerInput.Contains("show activity") || lowerInput.Contains("activity log"))
                return ShowActivityLog();

            // Memory
            if (lowerInput.Contains("what do you remember") || lowerInput.Contains("what do you know about me"))
                return GetMemorySummary();

            // Greeting
            if (IsGreeting(lowerInput))
                return GetGreeting();

            // Name
            if (lowerInput.Contains("my name is") || lowerInput.Contains("i am") || lowerInput.Contains("call me"))
            {
                string name = ExtractName(input);
                if (!string.IsNullOrEmpty(name))
                {
                    userProfile.UserName = name;
                    dbHelper.SaveUser(name);
                    return $"Nice to meet you, {name}! How can I help you with cybersecurity today?";
                }
            }

            // Help
            if (lowerInput.Contains("help") || lowerInput.Contains("what can you do"))
                return GetHelp();

            // Keywords
            foreach (var key in responses.Keys)
            {
                if (lowerInput.Contains(key))
                {
                    userProfile.FavoriteTopic = key;
                    return responses[key][random.Next(responses[key].Count)];
                }
            }

            return "I didn't quite understand that. Try asking about passwords, phishing, scams, or privacy.\nType 'help' to see what I can do!";
        }

        private string AddTask(string input)
        {
            string title = ExtractTitle(input);
            if (string.IsNullOrEmpty(title))
                return "Please tell me what task you want to add.\nExample: 'add task: Review privacy settings'";

            DateTime? reminderDate = ExtractReminderDate(input);

            string description = "";
            if (reminderDate.HasValue)
            {
                description = $"Reminder set for {reminderDate.Value:dd MMM yyyy HH:mm}";
            }

            bool success = dbHelper.AddTask(title, description, reminderDate, userProfile.UserName);
            if (success)
            {
                dbHelper.LogActivity($"Added task: {title}", userProfile.UserName);
                string response = $"✅ Task added: '{title}'";
                if (reminderDate.HasValue)
                {
                    response += $"\n🔔 Reminder set for {reminderDate.Value:dd MMM yyyy HH:mm}";
                }
                response += "\nType 'show tasks' to see all your tasks.";
                return response;
            }
            return "❌ Could not add task. Click 'DB Setup' first.";
        }

        private string ShowTasks()
        {
            var tasks = dbHelper.GetTasks(userProfile.UserName);
            if (tasks.Count == 0)
                return "📋 No tasks.\nType 'add task: [description]' to add one!";

            string response = "📋 YOUR TASKS\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n";
            int i = 1, pending = 0, completed = 0;
            int dueReminders = 0;

            foreach (var task in tasks)
            {
                if (task.IsCompleted) completed++; else pending++;

                response += $"{i}. {task.DisplayStatus} - {task.Title}\n";
                if (!string.IsNullOrEmpty(task.Description))
                    response += $"   📝 {task.Description}\n";

                if (task.ReminderDate.HasValue)
                {
                    string reminderIcon = task.IsReminderDue ? "🔴 DUE!" : "🔔";
                    response += $"   {reminderIcon} Reminder: {task.ReminderDate.Value:dd MMM yyyy HH:mm}\n";
                    if (task.IsReminderDue && !task.IsCompleted)
                    {
                        dueReminders++;
                    }
                }
                response += "\n";
                i++;
            }

            response += $"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n";
            response += $"📊 {pending} pending, {completed} completed";
            if (dueReminders > 0)
            {
                response += $"\n⚠️ {dueReminders} reminder(s) are DUE!";
            }
            response += "\n\n💡 Commands:\n";
            response += "   'complete 1' - Mark task 1 as done\n";
            response += "   'delete 2' - Delete task 2\n";
            response += "   'add task: Review privacy' - Add a task\n";
            response += "   'remind 1 in 3 days' - Set reminder for task 1";

            return response;
        }

        private string SetReminder(string input)
        {
            int taskNumber = ExtractNumber(input);
            if (taskNumber <= 0)
                return "Please specify which task to set a reminder for.\nExample: 'remind 1 in 3 days' or 'set reminder 2 tomorrow'";

            var tasks = dbHelper.GetTasks(userProfile.UserName);
            if (taskNumber > tasks.Count)
                return $"❌ Task {taskNumber} not found. You have {tasks.Count} tasks.";

            var task = tasks[taskNumber - 1];
            if (task.IsCompleted)
                return $"✅ Task '{task.Title}' is already completed! No reminder needed.";

            DateTime? reminderDate = ExtractReminderDate(input);
            if (!reminderDate.HasValue)
            {
                return "Please specify when to remind you.\nExamples:\n" +
                       "   'remind 1 tomorrow'\n" +
                       "   'remind 2 in 3 days'\n" +
                       "   'remind 3 next week'\n" +
                       "   'set reminder 1 Friday'";
            }

            string description = task.Description ?? "";
            if (!string.IsNullOrEmpty(description) && !description.Contains("Reminder set for"))
            {
                description += $" | Reminder set for {reminderDate.Value:dd MMM yyyy HH:mm}";
            }
            else
            {
                description = $"Reminder set for {reminderDate.Value:dd MMM yyyy HH:mm}";
            }

            bool success = dbHelper.UpdateTaskReminder(task.Id, reminderDate.Value, description);

            if (success)
            {
                dbHelper.LogActivity($"Set reminder for task: {task.Title}", userProfile.UserName);
                return $"🔔 Reminder set for '{task.Title}'\n📅 {reminderDate.Value:dddd, dd MMMM yyyy HH:mm}";
            }

            return "❌ Could not set reminder. Please try again.";
        }

        private string CompleteTask(string input)
        {
            int num = ExtractNumber(input);
            if (num <= 0) return "Please specify: 'complete 1'";

            var tasks = dbHelper.GetTasks(userProfile.UserName);
            if (num > tasks.Count) return $"❌ Task {num} not found.";

            var task = tasks[num - 1];
            if (task.IsCompleted) return $"✅ '{task.Title}' is already completed!";

            bool success = dbHelper.MarkTaskComplete(task.Id);
            if (success)
            {
                dbHelper.LogActivity($"Completed task: {task.Title}", userProfile.UserName);
                return $"✅ '{task.Title}' marked complete! 🎉";
            }
            return "❌ Could not complete task.";
        }

        private string DeleteTask(string input)
        {
            int num = ExtractNumber(input);
            if (num <= 0) return "Please specify: 'delete 1'";

            var tasks = dbHelper.GetTasks(userProfile.UserName);
            if (num > tasks.Count) return $"❌ Task {num} not found.";

            var task = tasks[num - 1];
            bool success = dbHelper.DeleteTask(task.Id);
            if (success)
            {
                dbHelper.LogActivity($"Deleted task: {task.Title}", userProfile.UserName);
                return $"🗑️ '{task.Title}' deleted.";
            }
            return "❌ Could not delete task.";
        }

        private DateTime? ExtractReminderDate(string input)
        {
            string lower = input.ToLower();
            DateTime now = DateTime.Now;

            // Check for "tomorrow"
            if (lower.Contains("tomorrow"))
                return now.AddDays(1).Date.AddHours(9);

            // Check for "today"
            if (lower.Contains("today"))
                return now.AddHours(2);

            // Check for "in X days/weeks"
            if (lower.Contains("in ") && (lower.Contains(" day") || lower.Contains(" week")))
            {
                string[] parts = lower.Split(' ');
                for (int i = 0; i < parts.Length; i++)
                {
                    if (int.TryParse(parts[i], out int number))
                    {
                        if (i + 1 < parts.Length)
                        {
                            if (parts[i + 1].Contains("day"))
                                return now.AddDays(number);
                            if (parts[i + 1].Contains("week"))
                                return now.AddDays(number * 7);
                            if (parts[i + 1].Contains("month"))
                                return now.AddMonths(number);
                        }
                    }
                }
            }

            // Check for specific days
            string[] days = { "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday" };
            foreach (string day in days)
            {
                if (lower.Contains(day))
                {
                    DayOfWeek targetDay = (DayOfWeek)Array.IndexOf(days, day);
                    int daysUntil = ((int)targetDay - (int)now.DayOfWeek + 7) % 7;
                    if (daysUntil == 0) daysUntil = 7;
                    return now.AddDays(daysUntil).Date.AddHours(9);
                }
            }

            // Check for "next week"
            if (lower.Contains("next week"))
                return now.AddDays(7).Date.AddHours(9);

            // Check for "weekend"
            if (lower.Contains("weekend"))
            {
                int daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)now.DayOfWeek + 7) % 7;
                if (daysUntilSaturday == 0) daysUntilSaturday = 7;
                return now.AddDays(daysUntilSaturday).Date.AddHours(10);
            }

            return null;
        }

        private string ShowActivityLog()
        {
            var logs = dbHelper.GetActivityLogs(userProfile.UserName, 10);
            if (logs.Count == 0) return "📋 No activity yet.";

            string response = "📋 RECENT ACTIVITY\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n";
            foreach (var log in logs)
                response += $"• {log}\n";
            return response;
        }

        private string GetMemorySummary()
        {
            var tasks = dbHelper.GetTasks(userProfile.UserName);
            string summary = "🧠 WHAT I REMEMBER\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n";
            summary += $"• Name: {userProfile.UserName}\n";
            summary += $"• Favorite topic: {userProfile.FavoriteTopic ?? "Not specified"}\n";

            int pending = tasks.Count(t => !t.IsCompleted);
            int completed = tasks.Count(t => t.IsCompleted);
            int dueReminders = tasks.Count(t => t.IsReminderDue && !t.IsCompleted);

            summary += $"• Tasks: {pending} pending, {completed} completed\n";
            if (dueReminders > 0)
            {
                summary += $"⚠️ {dueReminders} reminder(s) are due!";
            }
            return summary;
        }

        private bool IsGreeting(string input)
        {
            string[] g = { "hello", "hi", "hey", "good morning", "good afternoon", "good evening", "howdy" };
            return g.Any(x => input.Contains(x));
        }

        private string GetGreeting()
        {
            string g = greetings[random.Next(greetings.Count)];
            return userProfile.UserName != "User" ? $"Hi {userProfile.UserName}! {g}" : g;
        }

        private string GetHelp()
        {
            return "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n" +
                   "CYBERSECURITY BOT - HELP\n" +
                   "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n\n" +
                   "TOPICS: passwords, phishing, scams, privacy\n\n" +
                   "TASKS WITH REMINDERS:\n" +
                   "   • 'add task: Review privacy' - Add a task\n" +
                   "   • 'remind 1 tomorrow' - Set reminder for task 1\n" +
                   "   • 'remind 2 in 3 days' - Set reminder for task 2\n" +
                   "   • 'remind 3 next week' - Set reminder for task 3\n" +
                   "   • 'show tasks' - View all tasks with reminders\n" +
                   "   • 'complete 1' - Mark task 1 as done\n" +
                   "   • 'delete 2' - Delete task 2\n\n" +
                   "FEATURES:\n" +
                   "   • 'show activity' - View recent activity\n" +
                   "   • 'My name is John' - Save your name\n" +
                   "   • 'what do you remember' - Memory check\n\n" +
                   "Try: 'Tell me about passwords' or 'What is phishing?'";
        }

        private string ExtractTitle(string input)
        {
            string[] prefixes = { "add task", "add:", "new task", "add a task", "create task" };
            string title = input.Trim();
            foreach (string p in prefixes)
            {
                if (title.ToLower().StartsWith(p))
                {
                    title = title.Substring(p.Length).Trim();
                    break;
                }
            }
            string[] fillers = { "to", "for", "about", "please" };
            foreach (string f in fillers)
            {
                if (title.ToLower().StartsWith(f + " "))
                    title = title.Substring(f.Length + 1);
            }
            string[] removePhrases = { " remind", " reminder", " in ", " tomorrow", " today", " next", " with reminder" };
            foreach (string phrase in removePhrases)
            {
                int idx = title.ToLower().IndexOf(phrase);
                if (idx > 0)
                {
                    title = title.Substring(0, idx);
                    break;
                }
            }
            return string.IsNullOrEmpty(title) ? input.Trim() : title.Trim();
        }

        private int ExtractNumber(string input)
        {
            string[] words = input.Split(' ');
            foreach (string w in words)
                if (int.TryParse(w, out int n)) return n;
            return 0;
        }

        private string ExtractName(string input)
        {
            string lower = input.ToLower();
            if (lower.Contains("my name is"))
            {
                int start = lower.IndexOf("my name is") + 10;
                if (start < input.Length) return input.Substring(start).Trim().Split(' ')[0];
            }
            if (lower.Contains("i am"))
            {
                int start = lower.IndexOf("i am") + 4;
                if (start < input.Length) return input.Substring(start).Trim().Split(' ')[0];
            }
            if (lower.Contains("call me"))
            {
                int start = lower.IndexOf("call me") + 7;
                if (start < input.Length) return input.Substring(start).Trim().Split(' ')[0];
            }
            if (lower.StartsWith("i'm") || lower.StartsWith("im "))
            {
                int start = lower.StartsWith("i'm") ? 3 : 2;
                if (start < input.Length) return input.Substring(start).Trim().Split(' ')[0];
            }
            return null;
        }
    }
}