using MusicPlayer.Models;

namespace MusicPlayer.Strategies;

public sealed class RepeatOneStrategy : IPlaybackStrategy
{
    public string Name => "Repeat One";

    public Track? GetNextTrack(Playlist playlist, Track? currentTrack) => currentTrack;

    public Track? GetPreviousTrack(Playlist playlist, Track? currentTrack) => currentTrack;

    public void Reset(Playlist playlist) { }
}
