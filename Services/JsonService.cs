using System.Text.Json;

namespace CakeProject.Services;

public static class JsonService
{
    private static readonly string _folder = FileSystem.AppDataDirectory;
    private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    public static async Task SimpanAsync<T>(string namaFile, T data)
    {
        try
        {
            string path = Path.Combine(_folder, namaFile);
            string json = JsonSerializer.Serialize(data, _options);
            await File.WriteAllTextAsync(path, json);
            System.Diagnostics.Debug.WriteLine($"SAVED OK: {path}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SAVE ERROR: {ex.Message}");
            // Tampilkan error supaya kelihatan
            await Application.Current.MainPage.DisplayAlert("Save Error", ex.Message, "OK");
        }
    }

    public static async Task<T?> MuatAsync<T>(string namaFile)
    {
        string path = Path.Combine(_folder, namaFile);
        if (!File.Exists(path)) return default;
        string json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<T>(json);
    }

    public static bool FileAda(string namaFile)
    {
        string path = Path.Combine(_folder, namaFile);
        return File.Exists(path);
    }

    public static void Hapus(string namaFile)
    {
        string path = Path.Combine(_folder, namaFile);
        if (File.Exists(path)) File.Delete(path);
    }
}