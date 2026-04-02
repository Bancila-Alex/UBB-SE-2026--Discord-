using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ChatModule.Models;
using ChatModule.Services;
using ChatModule.ViewModels;

namespace ChatModule.src.view_models
{
    public class FriendRequestsViewModel : BaseViewModel
    {
        private readonly FriendRequestService _friendRequestService;
        private readonly Guid _currentUserId;

        public ObservableCollection<User> IncomingRequests { get; } = new();

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value);
        }

        public event Action? NavigateBackRequested;

        public RelayCommand LoadCommand { get; }
        public RelayCommand<Guid> AcceptCommand { get; }
        public RelayCommand<Guid> DeclineCommand { get; }
        public RelayCommand BackCommand { get; }

        public FriendRequestsViewModel(FriendRequestService friendRequestService, Guid currentUserId)
        {
            _friendRequestService = friendRequestService ?? throw new ArgumentNullException(nameof(friendRequestService));
            _currentUserId = currentUserId;

            LoadCommand = new RelayCommand(LoadAsync);
            AcceptCommand = new RelayCommand<Guid>(AcceptAsync);
            DeclineCommand = new RelayCommand<Guid>(DeclineAsync);
            BackCommand = new RelayCommand(BackAsync);
        }

        private async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                var requests = await _friendRequestService.GetIncomingRequestsAsync(_currentUserId);
                IncomingRequests.Clear();
                foreach (var user in requests)
                {
                    IncomingRequests.Add(user);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task AcceptAsync(Guid requesterId)
        {
            await _friendRequestService.AcceptRequestAsync(_currentUserId, requesterId);
            await LoadAsync();
        }

        private async Task DeclineAsync(Guid requesterId)
        {
            await _friendRequestService.DeclineRequestAsync(_currentUserId, requesterId);
            await LoadAsync();
        }

        private Task BackAsync()
        {
            NavigateBackRequested?.Invoke();
            return Task.CompletedTask;
        }
    }
}
