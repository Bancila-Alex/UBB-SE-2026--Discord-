using ChatModule.Repositories;
using ChatModule.Services;
using ChatModule.ViewModels;
using ChatModule.src.view_models;
using Microsoft.UI.Xaml;
using System;

namespace ChatModule
{
    public sealed partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; }

        public MainWindow()
        {
            var db = (Application.Current as App)?.DatabaseManager
                     ?? new DatabaseManager("Server=localhost\\SQLEXPRESS;Database=ChatModule;Trusted_Connection=True;TrustServerCertificate=True;");

            var userRepository = new UserRepository(db);
            var friendRepository = new FriendRepository(db);
            var conversationRepository = new ConversationRepository(db);
            var participantRepository = new ParticipantRepository(db);
            var messageRepository = new MessageRepository(db);

            var conversationListService = new ConversationListService(conversationRepository, participantRepository, messageRepository, userRepository);
            var friendRequestService = new FriendRequestService(friendRepository, userRepository, conversationRepository, participantRepository);
            var friendListService = new FriendListService(friendRepository, userRepository);
            var blockService = new BlockService(friendRepository, userRepository);
            var profileService = new ProfileService(userRepository, friendRepository);
            var directMessageService = new DirectMessageService(conversationRepository, participantRepository, friendRepository, userRepository);

            ViewModel = new MainViewModel(
                conversationListService,
                friendRequestService,
                friendListService,
                blockService,
                profileService,
                directMessageService);

            InitializeComponent();

            _ = ViewModel.InitialiseAsync(Guid.Empty, string.Empty);
        }
    }
}
