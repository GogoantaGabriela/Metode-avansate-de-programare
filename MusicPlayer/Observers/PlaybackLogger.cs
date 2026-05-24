using System.IO;
using System.ComponentModel;
using MusicPlayer.Audio;
using MusicPlayer.Controllers;
using MusicPlayer.Models;
using MusicPlayer.Strategies;

namespace MusicPlayer.Observers;

public sealed class PlaybackLogger : IDisposable
{
    private readonly AudioPlayer _player;
    private readonly PlaybackController _controller;
    private readonly string _logPath;
    private PlayerState _lastState = PlayerState.Stopped;

    public PlaybackLogger(AudioPlayer player, PlaybackController controller,
        string logPath = "playback_log.txt")
    {
        _player     = player;
        _controller = controller;
        _logPath    = logPath;

        _player.PropertyChanged    += OnPlayerPropertyChanged;
        _player.TrackEnded         += OnTrackEnded;
        _controller.StrategyChanged += OnStrategyChanged;
    }

    private void OnPlayerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AudioPlayer.State))
        {
            var newState = _player.State;
            if (newState == PlayerState.Playing && _lastState != PlayerState.Playing)
                Log($"TrackStarted: {_player.CurrentTrack}");
            _lastState = newState;
        }
    }

    private void OnTrackEnded(object? sender, Track track)
        => Log($"TrackEnded (natural): {track}");

    private void OnStrategyChanged(object? sender, IPlaybackStrategy strategy)
        => Log($"StrategyChanged: {strategy.Name}");

    private void Log(string message)
    {
        try
        {
            File.AppendAllText(_logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
        }
        catch { /* Non-critical; don't crash the app on log failure */ }
    }

    public void Dispose()
    {
        _player.PropertyChanged    -= OnPlayerPropertyChanged;
        _player.TrackEnded         -= OnTrackEnded;
        _controller.StrategyChanged -= OnStrategyChanged;
    }
}
