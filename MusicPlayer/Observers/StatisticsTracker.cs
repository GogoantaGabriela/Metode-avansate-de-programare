using System.ComponentModel;
using MusicPlayer.Audio;
using MusicPlayer.Models;

namespace MusicPlayer.Observers;

public sealed record StatisticsSnapshot(
    TimeSpan TotalListened,
    string TopArtist,
    int Skips,
    IReadOnlyList<(Track Track, int PlayCount)> Top5);


public sealed class StatisticsTracker : IDisposable
{
    private readonly AudioPlayer _player;

    private readonly Dictionary<Track, int> _playCounts = new();
    private readonly Dictionary<string, TimeSpan> _artistMinutes = new();
    private int _skips;

    private Track?   _currentTrack;
    private DateTime _trackStartTime;

    public StatisticsTracker(AudioPlayer player)
    {
        _player = player;
        _player.PropertyChanged += OnPropertyChanged;
        _player.TrackEnded      += OnTrackEnded;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(AudioPlayer.CurrentTrack)) return;

        var now   = DateTime.Now;
        var track = _player.CurrentTrack;

        
        if (_currentTrack != null)
        {
            var elapsed = now - _trackStartTime;
            if (_player.State == PlayerState.Playing)
            {
                if (elapsed.TotalSeconds < 30)
                    _skips++;
                else
                    AccumulateArtist(_currentTrack.Artist, elapsed);
            }
        }

        _currentTrack  = track;
        _trackStartTime = now;

        if (track != null)
        {
            _playCounts.TryGetValue(track, out int c);
            _playCounts[track] = c + 1;
        }
    }

    private void OnTrackEnded(object? sender, Track track)
    {
        AccumulateArtist(track.Artist, track.Duration);
    }

    private void AccumulateArtist(string artist, TimeSpan duration)
    {
        _artistMinutes.TryGetValue(artist, out var existing);
        _artistMinutes[artist] = existing + duration;
    }

    public StatisticsSnapshot Snapshot()
    {
        var total  = _artistMinutes.Values.Aggregate(TimeSpan.Zero, (a, b) => a + b);
        var top    = _artistMinutes.OrderByDescending(kv => kv.Value).FirstOrDefault();
        var top5   = _playCounts.OrderByDescending(kv => kv.Value)
                                 .Take(5)
                                 .Select(kv => (kv.Key, kv.Value))
                                 .ToList();
        return new StatisticsSnapshot(total, top.Key ?? "—", _skips, top5);
    }

    public void Dispose()
    {
        _player.PropertyChanged -= OnPropertyChanged;
        _player.TrackEnded      -= OnTrackEnded;
    }
}
