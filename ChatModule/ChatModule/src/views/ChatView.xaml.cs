using ChatModule.src.view_models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Specialized;
using System.Linq;

namespace ChatModule.src.views
{
    public sealed partial class ChatView : UserControl
    {
        public ChatViewModel ViewModel { get; }

        public ChatView(ChatViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            ViewModel.RequestEmojiAsync = RequestEmojiAsync;

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            ViewModel.ScrollToMessageRequested += OnScrollToMessageRequested;
            ViewModel.ReadReceiptDetailsRequested += OnReadReceiptDetailsRequested;
            ViewModel.Messages.CollectionChanged += OnMessagesCollectionChanged;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            AttachSearchPanel();
            _ = ViewModel.MarkConversationAsReadAsync();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.ScrollToMessageRequested -= OnScrollToMessageRequested;
            ViewModel.ReadReceiptDetailsRequested -= OnReadReceiptDetailsRequested;
            ViewModel.Messages.CollectionChanged -= OnMessagesCollectionChanged;
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;
        }

        private void OnSearchPanelLoaded(object sender, RoutedEventArgs e)
        {
            AttachSearchPanel();
        }

        private void AttachSearchPanel()
        {
            if (SearchPanelHost.Content != null)
            {
                return;
            }

            SearchPanelHost.Content = new MessageSearchPanel(ViewModel.MessageSearch);
        }

        public void SetSidePanel(UserControl panel)
        {
            SidePanelHost.Content = panel;
        }

        private void OnMessagesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (ViewModel.Messages.Count > 0 && e?.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is ChatModule.Models.Message newMessage && !newMessage.IsMine)
                    {
                        _ = ViewModel.MarkVisibleMessagesAsReadAsync(newMessage.Id);
                    }
                }
            }
        }

        private void OnScrollToMessageRequested(Guid messageId)
        {
            var target = ViewModel.Messages.FirstOrDefault(m => m.Id == messageId);
            if (target != null)
            {
                MessagesList.ScrollIntoView(target);
            }
        }

        private async void OnReadReceiptTapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is TextBlock { Tag: Guid messageId })
            {
                await ViewModel.ShowReadReceiptDetailsAsync(messageId);
            }
        }

        private async void OnReadReceiptDetailsRequested(string body)
        {
            if (XamlRoot == null)
            {
                return;
            }

            var dialog = new ContentDialog
            {
                Title = "Seen By",
                Content = body,
                CloseButtonText = "Close",
                XamlRoot = XamlRoot
            };

            _ = await dialog.ShowAsync();
        }

        private async System.Threading.Tasks.Task<string?> RequestEmojiAsync()
        {
            if (XamlRoot == null)
            {
                return null;
            }

            var emojiBox = new TextBox
            {
                PlaceholderText = "Emoji",
                Text = "👍"
            };

            var dialog = new ContentDialog
            {
                Title = "React",
                Content = emojiBox,
                PrimaryButtonText = "Apply",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = XamlRoot
            };

            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Primary)
            {
                return null;
            }

            var emoji = emojiBox.Text?.Trim();
            return string.IsNullOrWhiteSpace(emoji) ? null : emoji;
        }
    }
}
