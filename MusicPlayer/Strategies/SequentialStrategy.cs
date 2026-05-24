using MusicPlayer.Models;

namespace MusicPlayer.Strategies;

public sealed class SequentialStrategy : IPlaybackStrategy
{
    public bool Repeat { get; set; } = false;

    public string Name => "Sequential";

    public Track? GetNextTrack(Playlist playlist, Track? currentTrack)
    {
        if (playlist.Count == 0) return null;
        if (currentTrack == null) return playlist.GetAt(0);

        int idx = playlist.IndexOf(currentTrack);
        int next = idx + 1;
        if (next >= playlist.Count)
            return Repeat ? playlist.GetAt(0) : null;
        return playlist.GetAt(next);
    }

    public Track? GetPreviousTrack(Playlist playlist, Track? currentTrack)
    {
        if (playlist.Count == 0) return null;
        if (currentTrack == null) return playlist.GetAt(playlist.Count - 1);

        int idx = playlist.IndexOf(currentTrack);
        int prev = idx - 1;
        if (prev < 0)
            return Repeat ? playlist.GetAt(playlist.Count - 1) : null;
        return playlist.GetAt(prev);
    }

    public void Reset(Playlist playlist) { /* stateless */ }
}
