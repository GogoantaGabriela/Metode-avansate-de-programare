using MusicPlayer.Models;

namespace MusicPlayer.Strategies;

public sealed class SmartShuffleStrategy : IPlaybackStrategy
{
    private readonly int _historySize;
    private readonly Queue<Track> _history = new();
    private readonly Random _rng = new();

    public string Name => "Smart Shuffle";

    public SmartShuffleStrategy(int historySize = 5)
    {
        _historySize = historySize;
    }

    public Track? GetNextTrack(Playlist playlist, Track? currentTrack)
    {
        if (playlist.Count == 0) return null;

        int window = Math.Min(_historySize, playlist.Count - 1);

        while (_history.Count > window)
            _history.Dequeue();

        var candidates = playlist.Tracks.Except(_history).ToList();
        if (candidates.Count == 0)
        {
            _history.Clear();
            candidates = playlist.Tracks.ToList();
        }

        var next = candidates[_rng.Next(candidates.Count)];

        if (currentTrack != null)
            _history.Enqueue(currentTrack);

        return next;
    }

    public Track? GetPreviousTrack(Playlist playlist, Track? currentTrack)
    {
        if (_history.Count > 0)
        {
            var last = _history.Last();
            var list = _history.ToList();
            list.RemoveAt(list.Count - 1);
            _history.Clear();
            foreach (var t in list) _history.Enqueue(t);
            return last;
        }

        return playlist.Count > 0 ? playlist.GetAt(_rng.Next(playlist.Count)) : null;
    }

    public void Reset(Playlist playlist)
    {
        _history.Clear();
    }
}
