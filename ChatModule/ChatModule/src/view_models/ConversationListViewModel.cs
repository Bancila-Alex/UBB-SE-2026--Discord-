using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatModule.Services;
using ChatModule.src.domain;
using ChatModule.ViewModels;

namespace ChatModule.ViewModels
{
    public class ConversationListViewModel : BaseViewModel
    {
        private readonly ConversationListService _convListService;
        private readonly Guid _currentUserId;

        public ObservableCollection<Conversation> Conversations { get; } = new();

        private Conversation? _selectedConversation;
        public Conversation? SelectedConversation
        {
            get => _selectedConversation;
            set => Set(ref _selectedConversation, value);
        }

        private string _searchQuery = string.Empty;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (Set(ref _searchQuery, value))
                    _ = SearchAsync();
            }
        }

        private string _activeTab = "All";
        public string ActiveTab
        {
            get => _activeTab;
            set => Set(ref _activeTab, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value);
        }

        public ICommand             LoadCommand      { get; }
        public RelayCommand<string> SwitchTabCommand { get; }

        public ConversationListViewModel(ConversationListService convListService, Guid currentUserId)
        {
            _convListService = convListService;
            _currentUserId   = currentUserId;
            LoadCommand      = new RelayCommand(LoadTabAsync);
            SwitchTabCommand = new RelayCommand<string>(SwitchTabAsync);
        }

        private async Task LoadTabAsync()
        {
            IsLoading = true;
            try
            {
                var results = _activeTab switch
                {
                    "DMs"        => await _convListService.GetDmsAsync(_currentUserId),
                    "Groups"     => await _convListService.GetGroupsAsync(_currentUserId),
                    "Unread"     => await _convListService.GetUnreadAsync(_currentUserId),
                    "Favourites" => await _convListService.GetFavouritesAsync(_currentUserId),
                    _            => await _convListService.GetAllAsync(_currentUserId),
                };

                Conversations.Clear();
                foreach (var c in results)
                    Conversations.Add(c);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(_searchQuery))
            {
                await LoadTabAsync();
                return;
            }

            IsLoading = true;
            try
            {
                var results = await _convListService.SearchAsync(_currentUserId, _searchQuery);
                Conversations.Clear();
                foreach (var c in results)
                    Conversations.Add(c);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SwitchTabAsync(string tab)
        {
            ActiveTab = tab;
            _searchQuery = string.Empty;
            OnPropertyChanged(nameof(SearchQuery));
            await LoadTabAsync();
        }
    }
}
