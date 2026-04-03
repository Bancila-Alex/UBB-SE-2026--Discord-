using System.Text.RegularExpressions;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;

namespace ChatModule.src.views
{
    public static class MentionTextHelper
    {
        private static readonly Regex MentionPattern = new Regex(@"@\w+", RegexOptions.Compiled);

        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
            "Text",
            typeof(string),
            typeof(MentionTextHelper),
            new PropertyMetadata(null, OnTextChanged));

        public static void SetText(TextBlock element, string? value) =>
            element.SetValue(TextProperty, value);

        public static string? GetText(TextBlock element) =>
            (string?)element.GetValue(TextProperty);

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBlock tb)
                return;

            var text = e.NewValue as string ?? string.Empty;
            tb.Text = text;
            tb.TextHighlighters.Clear();

            var matches = MentionPattern.Matches(text);
            if (matches.Count == 0)
                return;

            var highlighter = new TextHighlighter
            {
                Background = new SolidColorBrush(ColorHelper.FromArgb(210, 92, 76, 196)),
                Foreground = new SolidColorBrush(Colors.White)
            };

            foreach (Match match in matches)
            {
                highlighter.Ranges.Add(new TextRange
                {
                    StartIndex = match.Index,
                    Length = match.Length
                });
            }

            tb.TextHighlighters.Add(highlighter);
        }
    }
}
