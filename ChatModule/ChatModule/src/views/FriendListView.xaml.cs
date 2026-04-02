using ChatModule.src.view_models;
using Microsoft.UI.Xaml.Controls;

namespace ChatModule.src.views
{
    public sealed partial class FriendListView : UserControl
    {
        public FriendListViewModel? ViewModel { get; private set; }

        public FriendListView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public FriendListView(FriendListViewModel viewModel)
            : this()
        {
            ViewModel = viewModel;
            DataContext = viewModel;
        }

        private void OnLoaded(object sender, global::Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (ViewModel == null && DataContext is FriendListViewModel vm)
            {
                ViewModel = vm;
                Bindings.Update();
            }

            ViewModel?.LoadCommand.Execute(null);
        }
    }
}
