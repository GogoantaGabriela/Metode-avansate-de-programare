namespace MusicPlayer.Models;


public sealed record Track(
    Guid Id,
    string Title,
    string Artist,
    string Album,
    int Year,
    TimeSpan Duration,
    string FilePath)
{
    public static Track Unknown => new(
        Guid.Empty, "Unknown Title", "Unknown Artist",
        "Unknown Album", 0, TimeSpan.Zero, string.Empty);

    public string DisplayDuration =>
        Duration.TotalHours >= 1
            ? Duration.ToString(@"h\:mm\:ss")
            : Duration.ToString(@"m\:ss");

    public override string ToString() => $"{Artist} — {Title}";
}
