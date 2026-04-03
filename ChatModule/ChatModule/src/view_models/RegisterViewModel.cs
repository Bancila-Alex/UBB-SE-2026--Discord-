using ChatModule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChatModule.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    private readonly AuthService _authService;

    private string _username = string.Empty;
    public string Username
    {
        get => _username;
        set => Set(ref _username, value);
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => Set(ref _email, value);
    }

    private string _password = string.Empty;
    public string Password
    {
        get => _password;
        set => Set(ref _password, value);
    }

    private string _phone = string.Empty;
    public string Phone
    {
        get => _phone;
        set => Set(ref _phone, value);
    }

    private DateTime? _birthday;
    public DateTime? Birthday
    {
        get => _birthday;
        set => Set(ref _birthday, value);
    }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => Set(ref _errorMessage, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => Set(ref _isLoading, value);
    }

    public RelayCommand RegisterCommand { get; }
    public RelayCommand BackToLoginCommand { get; }

    public event Action? NavigateToLoginRequested;
    public event Func<Guid, string, Task>? RegisterSucceeded;

    public RegisterViewModel(AuthService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        RegisterCommand = new RelayCommand(RegisterAsync, () => !IsLoading);
        BackToLoginCommand = new RelayCommand(OnBackToLogin);
    }

    private async Task RegisterAsync()
    {
        if (string.IsNullOrWhiteSpace(Username) || Username.Trim().Length < 5 || Username.Trim().Length > 16)
        {
            ErrorMessage = "Username must be between 5 and 16 characters.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Password) || Password.Length < 8 || Password.Length > 32)
        {
            ErrorMessage = "Password must be 8-32 chars and include uppercase, number, and special character.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Invalid email format.";
            return;
        }

        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var user = await _authService.RegisterAsync(
                Username,
                Email,
                Password,
                Phone,
                Birthday,
                null // avatarUrl
            );

            if (RegisterSucceeded != null)
            {
                await RegisterSucceeded(user.Id, user.Username);
            }
            else
            {
                NavigateToLoginRequested?.Invoke();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private Task OnBackToLogin()
    {
        NavigateToLoginRequested?.Invoke();
        return Task.CompletedTask;
    }
}
