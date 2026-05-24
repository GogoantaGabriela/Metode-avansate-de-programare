using MusicPlayer.Models;

namespace MusicPlayer.Strategies;


public sealed class ShuffleStrategy : IPlaybackStrategy
{
    private readonly Random _rng = new();
    private List<Track> _order = new();
    private int _index = 0;

    public string Name => "Shuffle";

    public Track? GetNextTrack(Playlist playlist, Track? currentTrack)
    {
        if (playlist.Count == 0) return null;

        if (_order.Count != playlist.Count || _order.Except(playlist.Tracks).Any())
            Regenerate(playlist);

        _index++;
        if (_index >= _order.Count)
        {
            Regenerate(playlist);
            _index = 0;
        }

        return _order.ElementAtOrDefault(_index);
    }

    public Track? GetPreviousTrack(Playlist playlist, Track? currentTrack)
    {
        if (playlist.Count == 0) return null;
        if (_order.Count == 0) Regenerate(playlist);

        _index = Math.Max(0, _index - 1);
        return _order.ElementAtOrDefault(_index);
    }

    public void Reset(Playlist playlist)
    {
        Regenerate(playlist);
        _index = 0;
    }

    private void Regenerate(Playlist playlist)
    {
        _order = playlist.Tracks.OrderBy(_ => _rng.Next()).ToList();
        _index = 0;
    }
}
