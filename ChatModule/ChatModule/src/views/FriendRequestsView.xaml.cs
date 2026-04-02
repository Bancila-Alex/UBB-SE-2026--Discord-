using ChatModule.src.view_models;
using Microsoft.UI.Xaml.Controls;

namespace ChatModule.src.views
{
    public sealed partial class FriendRequestsView : UserControl
    {
        public FriendRequestsViewModel? ViewModel { get; private set; }

        public FriendRequestsView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public FriendRequestsView(FriendRequestsViewModel viewModel)
            : this()
        {
            ViewModel = viewModel;
            DataContext = viewModel;
        }

        private void OnLoaded(object sender, global::Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (ViewModel == null && DataContext is FriendRequestsViewModel vm)
            {
                ViewModel = vm;
                Bindings.Update();
            }
        }
    }
}
