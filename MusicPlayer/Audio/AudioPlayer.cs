using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using MusicPlayer.Models;
using NAudio.Wave;

namespace MusicPlayer.Audio;


public sealed class AudioPlayer : INotifyPropertyChanged, IDisposable
{
    private IWavePlayer?      _waveOut;
    private AudioFileReader?  _reader;
    private readonly DispatcherTimer _positionTimer;
    private bool _manualStop;
    private bool _disposed;


    private Track? _currentTrack;
    public Track? CurrentTrack
    {
        get => _currentTrack;
        private set { _currentTrack = value; OnPropertyChanged(); }
    }

    private PlayerState _state = PlayerState.Stopped;
    public PlayerState State
    {
        get => _state;
        private set { _state = value; OnPropertyChanged(); }
    }

    private TimeSpan _position;
    public TimeSpan Position
    {
        get => _position;
        private set { _position = value; OnPropertyChanged(); }
    }

    private TimeSpan _duration;
    public TimeSpan Duration
    {
        get => _duration;
        private set { _duration = value; OnPropertyChanged(); }
    }

    private float _volume = 0.8f;
    public float Volume
    {
        get => _volume;
        set
        {
            _volume = Math.Clamp(value, 0f, 1f);
            if (_reader != null) _reader.Volume = _volume;
            OnPropertyChanged();
        }
    }


    public event EventHandler<Track>? TrackEnded;   
    public event PropertyChangedEventHandler? PropertyChanged;


    public AudioPlayer()
    {
        _positionTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(200)
        };
        _positionTimer.Tick += OnPositionTick;
    }


    public void Load(Track track)
    {
        ArgumentNullException.ThrowIfNull(track);
        DisposeNAudio();

        CurrentTrack = track;
        State = PlayerState.Stopped;
        Position = TimeSpan.Zero;

        if (!File.Exists(track.FilePath))
        {
            Duration = track.Duration;
            return;
        }

        _reader  = new AudioFileReader(track.FilePath) { Volume = _volume };
        Duration = _reader.TotalTime;

        _waveOut = new WaveOutEvent();
        _waveOut.Init(_reader);
        _waveOut.PlaybackStopped += OnPlaybackStopped;
    }

    public void Play()
    {
        if (_waveOut == null || _reader == null) return;
        _manualStop = false;
        _waveOut.Play();
        State = PlayerState.Playing;
        _positionTimer.Start();
    }

    public void Pause()
    {
        if (_waveOut == null) return;
        _waveOut.Pause();
        State = PlayerState.Paused;
        _positionTimer.Stop();
    }

    public void Stop()
    {
        if (_waveOut == null) return;
        _manualStop = true;
        _waveOut.Stop();
        State = PlayerState.Stopped;
        _positionTimer.Stop();
        if (_reader != null) _reader.Position = 0;
        Position = TimeSpan.Zero;
    }

    public void Seek(TimeSpan position)
    {
        if (_reader == null) return;
        position = TimeSpan.FromSeconds(
            Math.Clamp(position.TotalSeconds, 0, _reader.TotalTime.TotalSeconds));
        _reader.CurrentTime = position;
        Position = position;
    }


    private void OnPositionTick(object? sender, EventArgs e)
    {
        if (_reader != null)
            Position = _reader.CurrentTime;
    }

    private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
    {
        _positionTimer.Stop();

        if (!_manualStop && _reader != null)
        {
            bool natural = _reader.Position >= _reader.Length - (_reader.WaveFormat.AverageBytesPerSecond / 5);
            if (natural)
            {
                State = PlayerState.Stopped;
                var ended = CurrentTrack;
                if (ended != null)
                    TrackEnded?.Invoke(this, ended);
                return;
            }
        }

        State = PlayerState.Stopped;
    }

    private void DisposeNAudio()
    {
        _positionTimer.Stop();
        _waveOut?.Stop();
        _waveOut?.Dispose();
        _waveOut = null;
        _reader?.Dispose();
        _reader = null;
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        DisposeNAudio();
    }
}
