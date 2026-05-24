using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using MusicPlayer.Audio;
using MusicPlayer.Commands;
using MusicPlayer.Controllers;
using MusicPlayer.Models;
using MusicPlayer.Observers;
using MusicPlayer.Strategies;

namespace MusicPlayer.ViewModels;

public sealed class MainWindowViewModel : INotifyPropertyChanged, IDisposable
{
    private readonly AudioPlayer        _player;
    private readonly Playlist           _playlist;
    private readonly PlaybackController _controller;
    private readonly CommandHistory     _history;
    private readonly StatisticsTracker  _stats;
    private readonly PlaybackLogger     _logger;
    private readonly AutoNextHandler    _autoNext;

    public  IPlaybackStrategy SequentialStrategy  { get; } = new SequentialStrategy();
    public  IPlaybackStrategy ShuffleStrategy     { get; } = new ShuffleStrategy();
    public  IPlaybackStrategy SmartShuffleStrategy{ get; } = new SmartShuffleStrategy();
    public  IPlaybackStrategy RepeatOneStrategy   { get; } = new RepeatOneStrategy();

    public event PropertyChangedEventHandler? PropertyChanged;

    public ReadOnlyObservableCollection<Track> Tracks => _playlist.ObservableTracks;
    public string PlaylistHeader => $"LIBRARY · {_playlist.Count} tracks";

    public Track? CurrentTrack  => _player.CurrentTrack;
    public Guid?  CurrentTrackId => _player.CurrentTrack?.Id;
    public PlayerState State    => _player.State;
    public bool IsPlaying       => _player.State == PlayerState.Playing;
    public bool IsPaused        => _player.State == PlayerState.Paused;

    public TimeSpan Position    => _player.Position;
    public TimeSpan Duration    => _player.Duration;
    public double PositionSeconds
    {
        get => _player.Position.TotalSeconds;
        set => _player.Seek(TimeSpan.FromSeconds(value));
    }
    public double DurationSeconds => _player.Duration.TotalSeconds;

    public string PositionLabel => FormatTime(_player.Position);
    public string DurationLabel => FormatTime(_player.Duration);

    public double Volume
    {
        get => _player.Volume * 100.0;
        set => _player.Volume = (float)(value / 100.0);
    }

    private readonly ObservableCollection<Track> _recentlyPlayed = new();
    public ObservableCollection<Track> RecentlyPlayed => _recentlyPlayed;
    private Guid _lastRecentId = Guid.Empty;

    private void AddToRecentlyPlayed(Track track)
    {
        if (track.Id == _lastRecentId) return;
        _lastRecentId = track.Id;

        var existing = _recentlyPlayed.FirstOrDefault(t => t.Id == track.Id);
        if (existing != null) _recentlyPlayed.Remove(existing);
        _recentlyPlayed.Insert(0, track);
        while (_recentlyPlayed.Count > 8)
            _recentlyPlayed.RemoveAt(_recentlyPlayed.Count - 1);
    }

    private IPlaybackStrategy _activeStrategy;
    public IPlaybackStrategy ActiveStrategy
    {
        get => _activeStrategy;
        set
        {
            if (_activeStrategy == value) return;
            var cmd = new ChangeStrategyCommand(_controller, value);
            _history.Execute(cmd);
        }
    }

    public bool IsSequential   => _activeStrategy is SequentialStrategy;
    public bool IsShuffle      => _activeStrategy is ShuffleStrategy;
    public bool IsSmartShuffle => _activeStrategy is SmartShuffleStrategy;
    public bool IsRepeatOne    => _activeStrategy is RepeatOneStrategy;

    public bool CanUndo => _history.CanUndo;
    public bool CanRedo => _history.CanRedo;
    public IReadOnlyList<IPlayerCommand> UndoHistory => _history.UndoHistory;

    private StatisticsSnapshot _snapshot = new(TimeSpan.Zero, "—", 0, Array.Empty<(Track, int)>());

    public string TotalListenedLabel =>
        $"{(int)_snapshot.TotalListened.TotalHours}h {_snapshot.TotalListened.Minutes}m";
    public string TopArtistLabel  => _snapshot.TopArtist;
    public string SkipsLabel      => _snapshot.Skips.ToString();

    public ICommand PlayPauseCommand     { get; }
    public ICommand StopCommand          { get; }
    public ICommand NextCommand          { get; }
    public ICommand PreviousCommand      { get; }
    public ICommand UndoCommand          { get; }
    public ICommand RedoCommand          { get; }
    public ICommand AddFilesCommand      { get; }
    public ICommand RemoveTrackCommand   { get; }
    public ICommand ClearPlaylistCommand { get; }
    public ICommand SelectTrackCommand   { get; }
    public ICommand SetStrategyCommand   { get; }

    public MainWindowViewModel()
    {
        _player   = new AudioPlayer();
        _playlist = new Playlist();
        _activeStrategy = SequentialStrategy;
        _controller = new PlaybackController(_player, _playlist, _activeStrategy);
        _history  = new CommandHistory();
        _stats    = new StatisticsTracker(_player);
        _logger   = new PlaybackLogger(_player, _controller);
        _autoNext = new AutoNextHandler(_player, _controller);

        _player.PropertyChanged     += OnPlayerPropertyChanged;
        _controller.StrategyChanged += OnStrategyChanged;
        _history.PropertyChanged    += OnHistoryPropertyChanged;
        _playlist.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(PlaylistHeader));
            OnPropertyChanged(nameof(Tracks));
            Application.Current?.Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested);
        };

        PlayPauseCommand = new RelayCommand(
            () => { if (IsPlaying) _history.Execute(new PauseCommand(_controller));
                    else           _history.Execute(new PlayCommand(_controller)); },
            () => _playlist.Count > 0 || _player.CurrentTrack != null);

        StopCommand = new RelayCommand(
            () => _controller.Stop(),
            () => _player.State != PlayerState.Stopped);

        NextCommand = new RelayCommand(
            () => _history.Execute(new NextCommand(_controller)),
            () => _playlist.Count > 0);

        PreviousCommand = new RelayCommand(
            () => _history.Execute(new PreviousCommand(_controller)),
            () => _playlist.Count > 0);

        UndoCommand = new RelayCommand(() => _history.Undo(), () => _history.CanUndo);
        RedoCommand = new RelayCommand(() => _history.Redo(), () => _history.CanRedo);

        AddFilesCommand = new RelayCommand(_ => AddFiles());

        RemoveTrackCommand = new RelayCommand<Track>(track =>
        {
            if (track != null)
                _history.Execute(new RemoveTrackCommand(_playlist, track));
        });

        ClearPlaylistCommand = new RelayCommand(
            () => _history.Execute(new ClearPlaylistCommand(_playlist)),
            () => _playlist.Count > 0);

        SelectTrackCommand = new RelayCommand<Track>(track =>
        {
            if (track != null) _controller.LoadAndPlay(track);
        });

        SetStrategyCommand = new RelayCommand<IPlaybackStrategy>(strategy =>
        {
            if (strategy != null && strategy != _activeStrategy)
                _history.Execute(new ChangeStrategyCommand(_controller, strategy));
        });
    }

    private void AddFiles()
    {
        var dlg = new OpenFileDialog
        {
            Title       = "Add audio files",
            Filter      = "Audio files (*.mp3;*.wav)|*.mp3;*.wav|All files (*.*)|*.*",
            Multiselect = true
        };
        if (dlg.ShowDialog() != true) return;

        foreach (var path in dlg.FileNames)
        {
            try
            {
                var track = Mp3MetadataReader.ReadFromFile(path);
                _history.Execute(new AddTrackCommand(_playlist, track));
            }
            catch { /* Skip unreadable files */ }
        }
    }

    private void OnPlayerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(AudioPlayer.State):
                OnPropertyChanged(nameof(State));
                OnPropertyChanged(nameof(IsPlaying));
                OnPropertyChanged(nameof(IsPaused));
                if (_player.State == PlayerState.Playing && _player.CurrentTrack != null)
                    AddToRecentlyPlayed(_player.CurrentTrack);
                Application.Current?.Dispatcher.InvokeAsync(CommandManager.InvalidateRequerySuggested);
                break;
            case nameof(AudioPlayer.CurrentTrack):
                OnPropertyChanged(nameof(CurrentTrack));
                OnPropertyChanged(nameof(CurrentTrackId));
                RefreshStats();
                break;
            case nameof(AudioPlayer.Position):
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(PositionSeconds));
                OnPropertyChanged(nameof(PositionLabel));
                break;
            case nameof(AudioPlayer.Duration):
                OnPropertyChanged(nameof(Duration));
                OnPropertyChanged(nameof(DurationSeconds));
                OnPropertyChanged(nameof(DurationLabel));
                break;
            case nameof(AudioPlayer.Volume):
                OnPropertyChanged(nameof(Volume));
                break;
        }
    }

    private void OnStrategyChanged(object? sender, IPlaybackStrategy strategy)
    {
        _activeStrategy = strategy;
        OnPropertyChanged(nameof(ActiveStrategy));
        OnPropertyChanged(nameof(IsSequential));
        OnPropertyChanged(nameof(IsShuffle));
        OnPropertyChanged(nameof(IsSmartShuffle));
        OnPropertyChanged(nameof(IsRepeatOne));
    }

    private void OnHistoryPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
        OnPropertyChanged(nameof(UndoHistory));
    }

    private void RefreshStats()
    {
        _snapshot = _stats.Snapshot();
        OnPropertyChanged(nameof(TotalListenedLabel));
        OnPropertyChanged(nameof(TopArtistLabel));
        OnPropertyChanged(nameof(SkipsLabel));
    }

    private static string FormatTime(TimeSpan t) =>
        t.TotalHours >= 1 ? t.ToString(@"h\:mm\:ss") : t.ToString(@"m\:ss");

    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public void Dispose()
    {
        _player.PropertyChanged     -= OnPlayerPropertyChanged;
        _controller.StrategyChanged -= OnStrategyChanged;
        _logger.Dispose();
        _stats.Dispose();
        _autoNext.Dispose();
        _controller.Dispose();
    }
}

public sealed class RelayCommand<T> : ICommand
{
    private readonly Action<T?> _execute;
    private readonly Func<T?, bool>? _canExecute;

    public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
    {
        _execute    = execute;
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add    => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    public bool CanExecute(object? parameter)
        => _canExecute?.Invoke(parameter is T t ? t : default) ?? true;

    public void Execute(object? parameter)
        => _execute(parameter is T t ? t : default);
}
