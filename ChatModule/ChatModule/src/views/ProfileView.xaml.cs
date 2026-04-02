using ChatModule.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace ChatModule.src.views
{
    public sealed partial class ProfileView : UserControl
    {
        public ProfileViewModel? ViewModel { get; private set; }

        public ProfileView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public ProfileView(ProfileViewModel viewModel)
            : this()
        {
            ViewModel = viewModel;
            DataContext = viewModel;
        }

        private void OnLoaded(object sender, global::Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            if (ViewModel == null && DataContext is ProfileViewModel vm)
            {
                ViewModel = vm;
                Bindings.Update();
            }
        }
    }
}
