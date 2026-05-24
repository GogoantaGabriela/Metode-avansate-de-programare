using MusicPlayer.Audio;
using MusicPlayer.Controllers;
using MusicPlayer.Models;

namespace MusicPlayer.Observers;

public sealed class AutoNextHandler : IDisposable
{
    private readonly AudioPlayer _player;
    private readonly PlaybackController _controller;

    public AutoNextHandler(AudioPlayer player, PlaybackController controller)
    {
        _player     = player;
        _controller = controller;
        _player.TrackEnded += OnTrackEnded;
    }

    private void OnTrackEnded(object? sender, Track track)
    {
        var next = _controller.CurrentStrategy.GetNextTrack(_controller.Playlist, track);
        if (next != null)
            _controller.LoadAndPlay(next);
    }

    public void Dispose()
    {
        _player.TrackEnded -= OnTrackEnded;
    }
}
