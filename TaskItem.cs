using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPOE
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }

        public string DisplayStatus => IsCompleted ? "✅ Completed" : "⏳ Pending";
        public string DisplayIcon => IsCompleted ? "✅" : "⏳";

        public string ReminderDisplay => ReminderDate.HasValue
            ? $" {ReminderDate.Value:dd MMM yyyy HH:mm}"
            : "No reminder set";

        public bool IsReminderDue => ReminderDate.HasValue && ReminderDate.Value <= DateTime.Now;
    }
}
