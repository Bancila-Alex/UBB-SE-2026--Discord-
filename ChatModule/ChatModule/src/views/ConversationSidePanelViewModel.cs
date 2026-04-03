using System;
using ChatModule.src.domain.Enums;
using ChatModule.src.view_models;
using ChatModule.ViewModels;

namespace ChatModule.src.views
{
    public class ConversationSidePanelViewModel : BaseViewModel
    {
        private bool _isPanelVisible = true;
        private readonly Action? _onBackRequested;

        public ConversationType ConversationType { get; }

        public string PanelTitle => ConversationType == ConversationType.Group ? "Members" : "Profile";

        public bool CanGoBack => _onBackRequested != null;

        public BaseViewModel ContentViewModel { get; }

        public bool IsPanelVisible
        {
            get => _isPanelVisible;
            private set
            {
                if (Set(ref _isPanelVisible, value))
                {
                    OnPropertyChanged(nameof(TogglePanelIcon));
                    OnPropertyChanged(nameof(PanelWidth));
                }
            }
        }

        public string TogglePanelIcon => IsPanelVisible ? "◀" : "▶";

        public double PanelWidth => IsPanelVisible ? 360 : 44;

        public RelayCommand TogglePanelCommand { get; }
        public RelayCommand BackCommand { get; }

        public ConversationSidePanelViewModel(ConversationType conversationType, BaseViewModel contentViewModel, Action? onBackRequested = null)
        {
            ConversationType = conversationType;
            ContentViewModel = contentViewModel;
            _onBackRequested = onBackRequested;
            TogglePanelCommand = new RelayCommand(TogglePanelAsync);
            BackCommand = new RelayCommand(BackAsync);
        }

        private System.Threading.Tasks.Task TogglePanelAsync()
        {
            IsPanelVisible = !IsPanelVisible;
            return System.Threading.Tasks.Task.CompletedTask;
        }

        private System.Threading.Tasks.Task BackAsync()
        {
            _onBackRequested?.Invoke();
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}
