using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace PPOE
{
    public class CyberChatMessage
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public bool IsUserMessage { get; set; }

        public HorizontalAlignment MsgAlignment => IsUserMessage ? HorizontalAlignment.Right : HorizontalAlignment.Left;

        public Brush MsgBackground => IsUserMessage ?
            new SolidColorBrush(Color.FromRgb(124, 58, 237)) : // Purple (#7c3aed)
            new SolidColorBrush(Color.FromRgb(18, 18, 42));    // Dark (#12122a)

        public Brush MsgForeground => IsUserMessage ?
            new SolidColorBrush(Colors.White) :
            new SolidColorBrush(Color.FromRgb(226, 232, 240)); // Light gray

        public DropShadowEffect MsgEffect => IsUserMessage ?
            new DropShadowEffect { Color = Color.FromRgb(124, 58, 237), BlurRadius = 10, ShadowDepth = 0, Opacity = 0.3 } :
            null;
    }
}