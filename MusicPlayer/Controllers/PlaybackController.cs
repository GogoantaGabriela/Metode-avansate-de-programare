using System.ComponentModel;
using System.Runtime.CompilerServices;
using MusicPlayer.Audio;
using MusicPlayer.Models;
using MusicPlayer.Strategies;

namespace MusicPlayer.Controllers;

public sealed class PlaybackController : INotifyPropertyChanged, IDisposable
{
    private readonly AudioPlayer _player;
    private readonly Playlist    _playlist;
    private IPlaybackStrategy    _strategy;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<IPlaybackStrategy>? StrategyChanged;

    public AudioPlayer Player   => _player;
    public Playlist    Playlist => _playlist;

    public IPlaybackStrategy CurrentStrategy => _strategy;

    public PlaybackController(AudioPlayer player, Playlist playlist, IPlaybackStrategy initialStrategy)
    {
        _player   = player;
        _playlist = playlist;
        _strategy = initialStrategy;
    }


    public void LoadAndPlay(Track track)
    {
        _player.Load(track);
        _player.Play();
    }

    public void Play()
    {
        if (_player.State == PlayerState.Paused)
        {
            _player.Play();
            return;
        }

        var track = _player.CurrentTrack ?? _strategy.GetNextTrack(_playlist, null);
        if (track != null)
        {
            _player.Load(track);
            _player.Play();
        }
    }

    public void Pause()
    {
        if (_player.State == PlayerState.Playing)
            _player.Pause();
    }

    public void Stop() => _player.Stop();

    public void Next()
    {
        var next = _strategy.GetNextTrack(_playlist, _player.CurrentTrack);
        if (next != null) LoadAndPlay(next);
    }

    public void Previous()
    {
        var prev = _strategy.GetPreviousTrack(_playlist, _player.CurrentTrack);
        if (prev != null) LoadAndPlay(prev);
    }

    public void SetStrategy(IPlaybackStrategy strategy)
    {
        _strategy = strategy;
        _strategy.Reset(_playlist);
        StrategyChanged?.Invoke(this, strategy);
        OnPropertyChanged(nameof(CurrentStrategy));
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public void Dispose() => _player.Dispose();
}
