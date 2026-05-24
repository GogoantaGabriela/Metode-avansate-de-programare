using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MusicPlayer.Models;


public sealed class Playlist
{
    private readonly ObservableCollection<Track> _tracks = new();

    public IReadOnlyList<Track> Tracks => _tracks;

    public int Count => _tracks.Count;

    public event NotifyCollectionChangedEventHandler? CollectionChanged
    {
        add    => _tracks.CollectionChanged += value;
        remove => _tracks.CollectionChanged -= value;
    }

    public ReadOnlyObservableCollection<Track> ObservableTracks { get; }

    public Playlist()
    {
        ObservableTracks = new ReadOnlyObservableCollection<Track>(_tracks);
    }

    public void Add(Track track)
    {
        ArgumentNullException.ThrowIfNull(track);
        _tracks.Add(track);
    }

    public void Remove(Track track)
    {
        _tracks.Remove(track);
    }

    public void RemoveAt(int index)
    {
        if (index >= 0 && index < _tracks.Count)
            _tracks.RemoveAt(index);
    }

    public void Insert(int index, Track track)
    {
        ArgumentNullException.ThrowIfNull(track);
        index = Math.Clamp(index, 0, _tracks.Count);
        _tracks.Insert(index, track);
    }

    public void Move(int oldIndex, int newIndex)
    {
        if (oldIndex < 0 || oldIndex >= _tracks.Count) return;
        newIndex = Math.Clamp(newIndex, 0, _tracks.Count - 1);
        _tracks.Move(oldIndex, newIndex);
    }

    public void Clear()
    {
        _tracks.Clear();
    }

    public int IndexOf(Track track) => _tracks.IndexOf(track);

    public Track? GetAt(int index) =>
        index >= 0 && index < _tracks.Count ? _tracks[index] : null;

    public IReadOnlyList<Track> Snapshot() => _tracks.ToList();
}
