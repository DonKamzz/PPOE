using System.Collections.Generic;

namespace PPOE
{
    public class UserProfile
    {
        public string UserName { get; set; } = "User";
        public string FavoriteTopic { get; set; } = string.Empty;
        public string CurrentTopic { get; set; } = string.Empty;
        public string LastTipGiven { get; set; } = string.Empty;
        public Dictionary<string, string> Preferences { get; set; } = new Dictionary<string, string>();
    }
}