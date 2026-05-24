using MusicPlayer.Controllers;
using MusicPlayer.Strategies;

namespace MusicPlayer.Commands;

public sealed class ChangeStrategyCommand : IPlayerCommand
{
    private readonly PlaybackController _controller;
    private readonly IPlaybackStrategy _newStrategy;
    private IPlaybackStrategy? _previousStrategy;

    public bool CanUndo => true;
    public string Description =>
        $"Strategy {_previousStrategy?.Name ?? "?"} → {_newStrategy.Name}";

    public ChangeStrategyCommand(PlaybackController controller, IPlaybackStrategy newStrategy)
    {
        _controller  = controller;
        _newStrategy = newStrategy;
    }

    public void Execute()
    {
        _previousStrategy = _controller.CurrentStrategy;
        _controller.SetStrategy(_newStrategy);
    }

    public void Undo()
    {
        if (_previousStrategy != null)
            _controller.SetStrategy(_previousStrategy);
    }
}
