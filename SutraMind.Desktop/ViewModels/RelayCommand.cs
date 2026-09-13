using System.Windows.Input;
using SutraMind.Application.Authorization;
using SutraMind.Domain.Enums;

namespace SutraMind.Desktop.ViewModels;

public sealed class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Func<object?, bool>? _canExecute;

    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = _ => execute();
        if (canExecute is not null)
            _canExecute = _ => canExecute();
    }

    public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    public void Execute(object? parameter) => _execute(parameter);
}

public sealed class PermissionCommand : ICommand
{
    private readonly Func<UserRole> _getRole;
    private readonly Permission _permission;
    private readonly Action _execute;
    private readonly Func<bool>? _canExecute;

    public PermissionCommand(Func<UserRole> getRole, Permission permission, Action execute, Func<bool>? canExecute = null)
    {
        _getRole = getRole;
        _permission = permission;
        _execute = execute;
        _canExecute = canExecute;
    }

    public string DisabledReason => PermissionPolicy.Allows(_getRole(), _permission)
        ? "This action is unavailable right now."
        : $"Your role ({_getRole()}) cannot perform this action.";

    public bool HasPermission => PermissionPolicy.Allows(_getRole(), _permission);

    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter) =>
        PermissionPolicy.Allows(_getRole(), _permission) && (_canExecute?.Invoke() ?? true);

    public void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
            return;
        _execute();
    }
}
