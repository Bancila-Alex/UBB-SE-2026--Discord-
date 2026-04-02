using ChatModule.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace ChatModule.src.views
{
    public sealed partial class ConversationListView : UserControl
    {
        public ConversationListViewModel? ViewModel { get; private set; }

        public ConversationListView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public ConversationListView(ConversationListViewModel viewModel)
            : this()
        {
            ViewModel = viewModel;
            DataContext = viewModel;
        }

        private void OnLoaded(object sender, global::Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (ViewModel == null && DataContext is ConversationListViewModel vm)
            {
                ViewModel = vm;
                Bindings.Update();
            }

            if (ViewModel != null)
            {
                ViewModel.LoadCommand.Execute(null);
            }
        }
    }
}
