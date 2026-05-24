using System.IO;
using NAudio.Wave;
using MusicPlayer.Models;

namespace MusicPlayer.Audio;

public static class Mp3MetadataReader
{
    public static Track ReadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("Audio file not found.", filePath);

        var ext      = Path.GetExtension(filePath).ToLowerInvariant();
        var title    = Path.GetFileNameWithoutExtension(filePath);
        var artist   = "Unknown Artist";
        var album    = "Unknown Album";
        int year     = 0;
        var duration = TimeSpan.Zero;

        try
        {
            if (ext == ".mp3")
            {
                using var reader = new Mp3FileReader(filePath);
                duration = reader.TotalTime;

                
                if (reader.Id3v1Tag is { } v1Bytes && v1Bytes.Length >= 128)
                {
                    var enc = System.Text.Encoding.Latin1;
                    var t  = enc.GetString(v1Bytes, 3,  30).TrimEnd('\0').Trim();
                    var a  = enc.GetString(v1Bytes, 33, 30).TrimEnd('\0').Trim();
                    var al = enc.GetString(v1Bytes, 63, 30).TrimEnd('\0').Trim();
                    var yr = enc.GetString(v1Bytes, 93,  4).TrimEnd('\0').Trim();
                    if (!string.IsNullOrWhiteSpace(t))  title  = t;
                    if (!string.IsNullOrWhiteSpace(a))  artist = a;
                    if (!string.IsNullOrWhiteSpace(al)) album  = al;
                    if (int.TryParse(yr, out int y))    year   = y;
                }

                if (reader.Id3v2Tag is { } v2raw)
                {
                    var frames = ParseId3v2Frames(v2raw.RawData);
                    if (frames.TryGetValue("TIT2", out var t)  && !string.IsNullOrWhiteSpace(t)) title  = t;
                    if (frames.TryGetValue("TPE1", out var a)  && !string.IsNullOrWhiteSpace(a)) artist = a;
                    if (frames.TryGetValue("TALB", out var al) && !string.IsNullOrWhiteSpace(al)) album = al;
                    var yr = frames.GetValueOrDefault("TDRC") ?? frames.GetValueOrDefault("TYER");
                    if (yr != null && yr.Length >= 4 && int.TryParse(yr[..4], out int y)) year = y;
                }
            }
            else if (ext == ".wav")
            {
                using var reader = new WaveFileReader(filePath);
                duration = reader.TotalTime;
            }
        }
        catch { /* fallback to filename */ }

        if (string.IsNullOrWhiteSpace(title))  title  = Path.GetFileNameWithoutExtension(filePath);
        if (string.IsNullOrWhiteSpace(artist)) artist = "Unknown Artist";
        if (string.IsNullOrWhiteSpace(album))  album  = "Unknown Album";

        return new Track(Guid.NewGuid(), title, artist, album, year, duration, filePath);
    }

    
    private static Dictionary<string, string> ParseId3v2Frames(byte[] raw)
    {
        var result = new Dictionary<string, string>();
        try
        {
            if (raw == null || raw.Length < 10) return result;
            if (raw[0] != 'I' || raw[1] != 'D' || raw[2] != '3') return result;

            int version  = raw[3];
            int tagSize  = ((raw[6] & 0x7F) << 21) | ((raw[7] & 0x7F) << 14)
                         | ((raw[8] & 0x7F) << 7)  |  (raw[9] & 0x7F);

            int pos = 10;
            int end = Math.Min(10 + tagSize, raw.Length);

            while (pos + 10 <= end)
            {
                string frameId = System.Text.Encoding.ASCII.GetString(raw, pos, 4);
                if (frameId == "\0\0\0\0") break;

                int frameSize = version >= 4
                    ? ((raw[pos+4] & 0x7F) << 21) | ((raw[pos+5] & 0x7F) << 14)
                      | ((raw[pos+6] & 0x7F) << 7) | (raw[pos+7] & 0x7F)
                    : (raw[pos+4] << 24) | (raw[pos+5] << 16) | (raw[pos+6] << 8) | raw[pos+7];

                pos += 10;
                if (frameSize <= 0 || pos + frameSize > end) break;

                if (frameId[0] == 'T' && frameSize >= 1)
                {
                    byte encoding = raw[pos];
                    string text = encoding switch
                    {
                        0 => System.Text.Encoding.Latin1.GetString(raw, pos + 1, frameSize - 1),
                        1 => System.Text.Encoding.Unicode.GetString(raw, pos + 1, frameSize - 1),
                        2 => System.Text.Encoding.BigEndianUnicode.GetString(raw, pos + 1, frameSize - 1),
                        3 => System.Text.Encoding.UTF8.GetString(raw, pos + 1, frameSize - 1),
                        _ => System.Text.Encoding.Latin1.GetString(raw, pos + 1, frameSize - 1)
                    };
                    result[frameId] = text.TrimEnd('\0').Trim();
                }

                pos += frameSize;
            }
        }
        catch { }
        return result;
    }
}
