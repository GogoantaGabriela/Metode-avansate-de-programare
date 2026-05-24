using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MusicPlayer.Commands;


public sealed class CommandHistory : INotifyPropertyChanged
{
    private const int MaxHistory = 50;

    private readonly Stack<IPlayerCommand> _undoStack = new();
    private readonly Stack<IPlayerCommand> _redoStack = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;

    public IReadOnlyList<IPlayerCommand> UndoHistory =>
        _undoStack.Take(10).ToList();

    public void Execute(IPlayerCommand command)
    {
        command.Execute();

        if (command.CanUndo)
        {
            _redoStack.Clear();

            _undoStack.Push(command);

            if (_undoStack.Count > MaxHistory)
            {
                var items = _undoStack.ToArray();           
                _undoStack.Clear();
                foreach (var item in items.Take(MaxHistory).Reverse())
                    _undoStack.Push(item);
            }
        }

        Notify();
    }

    public void Undo()
    {
        if (_undoStack.Count == 0) return;
        var cmd = _undoStack.Pop();
        cmd.Undo();
        _redoStack.Push(cmd);
        Notify();
    }

    public void Redo()
    {
        if (_redoStack.Count == 0) return;
        var cmd = _redoStack.Pop();
        cmd.Execute();
        _undoStack.Push(cmd);
        Notify();
    }

    private void Notify()
    {
        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
        OnPropertyChanged(nameof(UndoHistory));
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
