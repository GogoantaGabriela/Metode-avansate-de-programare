using MusicPlayer.Audio;
using MusicPlayer.Controllers;

namespace MusicPlayer.Commands;


public sealed class PlayCommand : IPlayerCommand
{
    private readonly PlaybackController _controller;
    public bool CanUndo => false;
    public string Description => "Play";

    public PlayCommand(PlaybackController controller)
        => _controller = controller;

    public void Execute() => _controller.Play();
    public void Undo() { }
}


public sealed class PauseCommand : IPlayerCommand
{
    private readonly PlaybackController _controller;
    public bool CanUndo => false;
    public string Description => "Pause";

    public PauseCommand(PlaybackController controller)
        => _controller = controller;

    public void Execute() => _controller.Pause();
    public void Undo() { }
}


public sealed class NextCommand : IPlayerCommand
{
    private readonly PlaybackController _controller;
    public bool CanUndo => false;
    public string Description => "Next Track";

    public NextCommand(PlaybackController controller)
        => _controller = controller;

    public void Execute() => _controller.Next();
    public void Undo() { }
}


public sealed class PreviousCommand : IPlayerCommand
{
    private readonly PlaybackController _controller;
    public bool CanUndo => false;
    public string Description => "Previous Track";

    public PreviousCommand(PlaybackController controller)
        => _controller = controller;

    public void Execute() => _controller.Previous();
    public void Undo() { }
}
