using ChatModule.src.view_models;
using Microsoft.UI.Xaml.Controls;

namespace ChatModule.src.views
{
    public sealed partial class ChatView : UserControl
    {
        public ChatViewModel ViewModel { get; }

        public ChatView(ChatViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }
    }
}
