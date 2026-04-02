using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ChatModule.Models;
using ChatModule.Services;
using ChatModule.ViewModels;

namespace ChatModule.src.view_models
{
    public class MemberPanelViewModel : BaseViewModel
    {
        private readonly MemberPanelService _memberPanelService;
        private readonly ModerationService _moderationService;
        private readonly Guid _currentUserId;
        private Guid _conversationId;

        public ObservableCollection<Participant> Members { get; } = new();
        public ObservableCollection<User> AddMemberResults { get; } = new();

        private string _addMemberQuery = string.Empty;
        public string AddMemberQuery
        {
            get => _addMemberQuery;
            set
            {
                if (Set(ref _addMemberQuery, value))
                    _ = SearchUsersToAddAsync();
            }
        }

        private User? _selectedAddMember;
        public User? SelectedAddMember
        {
            get => _selectedAddMember;
            set => Set(ref _selectedAddMember, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => Set(ref _isLoading, value);
        }

        public RelayCommand LoadCommand { get; }
        public RelayCommand AddMemberCommand { get; }
        public RelayCommand ViewProfileCommand { get; }

        public event Action<Guid>? NavigateToProfileRequested;

        public MemberPanelViewModel(
            MemberPanelService memberPanelService,
            ModerationService moderationService,
            Guid currentUserId)
        {
            _memberPanelService = memberPanelService ?? throw new ArgumentNullException(nameof(memberPanelService));
            _moderationService = moderationService ?? throw new ArgumentNullException(nameof(moderationService));
            _currentUserId = currentUserId;

            LoadCommand = new RelayCommand(LoadAsync);
            AddMemberCommand = new RelayCommand(AddMemberAsync);
            ViewProfileCommand = new RelayCommand(ViewProfileAsync);
        }

        public async Task InitializeAsync(Guid conversationId)
        {
            _conversationId = conversationId;
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            IsLoading = true;
            try
            {
                var members = await _memberPanelService.GetMembersAsync(_conversationId);
                Members.Clear();
                foreach (var member in members)
                {
                    Members.Add(member);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SearchUsersToAddAsync()
        {
            AddMemberResults.Clear();
            SelectedAddMember = null;

            if (string.IsNullOrWhiteSpace(_addMemberQuery))
            {
                return;
            }

            var results = await _memberPanelService.SearchUsersToAddAsync(_conversationId, _addMemberQuery);
            foreach (var user in results)
            {
                AddMemberResults.Add(user);
            }
        }

        private async Task AddMemberAsync()
        {
            if (SelectedAddMember == null)
            {
                return;
            }

            await _moderationService.AddMemberAsync(_conversationId, _currentUserId, SelectedAddMember.Id);
            AddMemberQuery = string.Empty;
            AddMemberResults.Clear();
            SelectedAddMember = null;
            await LoadAsync();
        }

        private Task ViewProfileAsync()
        {
            if (SelectedAddMember == null)
            {
                return Task.CompletedTask;
            }

            NavigateToProfileRequested?.Invoke(SelectedAddMember.Id);
            return Task.CompletedTask;
        }
    }
}
