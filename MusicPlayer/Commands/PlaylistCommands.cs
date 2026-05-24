using MusicPlayer.Models;

namespace MusicPlayer.Commands;


public sealed class AddTrackCommand : IPlayerCommand
{
    private readonly Playlist _playlist;
    private readonly Track _track;

    public bool CanUndo => true;
    public string Description => $"Add \"{_track.Title}\"";

    public AddTrackCommand(Playlist playlist, Track track)
    {
        _playlist = playlist;
        _track    = track;
    }

    public void Execute() => _playlist.Add(_track);

    public void Undo() => _playlist.Remove(_track);
}


public sealed class RemoveTrackCommand : IPlayerCommand
{
    private readonly Playlist _playlist;
    private readonly Track _track;
    private int _savedIndex = -1;  

    public bool CanUndo => true;
    public string Description => $"Remove \"{_track.Title}\"";

    public RemoveTrackCommand(Playlist playlist, Track track)
    {
        _playlist = playlist;
        _track    = track;
    }

    public void Execute()
    {
        _savedIndex = _playlist.IndexOf(_track);
        _playlist.Remove(_track);
    }

    public void Undo()
    {
        if (_savedIndex >= 0)
            _playlist.Insert(_savedIndex, _track);
        else
            _playlist.Add(_track);
    }
}


public sealed class MoveTrackCommand : IPlayerCommand
{
    private readonly Playlist _playlist;
    private readonly int _oldIndex;
    private readonly int _newIndex;

    public bool CanUndo => true;
    public string Description => $"Move Track {_oldIndex + 1} → position {_newIndex + 1}";

    public MoveTrackCommand(Playlist playlist, int oldIndex, int newIndex)
    {
        _playlist = playlist;
        _oldIndex = oldIndex;
        _newIndex = newIndex;
    }

    public void Execute() => _playlist.Move(_oldIndex, _newIndex);

    public void Undo() => _playlist.Move(_newIndex, _oldIndex);
}


public sealed class ClearPlaylistCommand : IPlayerCommand
{
    private readonly Playlist _playlist;
    private IReadOnlyList<Track> _snapshot = Array.Empty<Track>(); 

    public bool CanUndo => true;
    public string Description => "Clear Playlist";

    public ClearPlaylistCommand(Playlist playlist)
        => _playlist = playlist;

    public void Execute()
    {
        _snapshot = _playlist.Snapshot();
        _playlist.Clear();
    }

    public void Undo()
    {
        _playlist.Clear();
        foreach (var track in _snapshot)
            _playlist.Add(track);
    }
}
