using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace Kyanendar.MVVM.Models;

public class KyanendarService
{
    private readonly string _storagePath;
    private ObservableCollection<Kyanendar> _kyanendars;
    public ObservableCollection<Kyanendar> Kyanendars => _kyanendars;

    public KyanendarService() {
        _storagePath = Path.Combine(FileSystem.AppDataDirectory, "Kyanendars");
        _kyanendars = new ObservableCollection<Kyanendar>();
        
        if (!Directory.Exists(_storagePath)) {
            Directory.CreateDirectory(_storagePath);
        }
        
        LoadKyanendars();
    }

    private void LoadKyanendars() {
        var files = Directory.GetFiles(_storagePath, "*.txt");
        
        var loadTasks = files.Select(
            file => Task.Run(() => {
                var name = Path.GetFileNameWithoutExtension(file);
                return new Kyanendar(name, file);
        })).ToList();

        Task.WhenAll(loadTasks).ContinueWith(t => {
            if (t.Status == TaskStatus.RanToCompletion) {
                foreach (var kyanendar in t.Result) {
                    MainThread.BeginInvokeOnMainThread(() => {
                        _kyanendars.Add(kyanendar);
                    });
                }
            }
        }).Wait();
    }

    public bool KyanendarExists(string name) {
        return _kyanendars.Any(k => k.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public void CreateBlankKyanendar(string name) {
        if (KyanendarExists(name)) {
            throw new InvalidOperationException($"A Kyanendar with name '{name}' already exists.");
        }

        var filePath = Path.Combine(_storagePath, $"{name}.txt");
        File.WriteAllText(filePath, string.Empty);
        
        var kyanendarEntry = new Kyanendar(name, filePath);
        _kyanendars.Add(kyanendarEntry);
    }

    public void AddKyanendar(string filePath) {
        var name = Path.GetFileNameWithoutExtension(filePath);
        
        if (KyanendarExists(name)) {
            throw new InvalidOperationException($"A Kyanendar with name '{name}' already exists.");
        }

        var destinationPath = Path.Combine(_storagePath, Path.GetFileName(filePath));
        File.Copy(filePath, destinationPath, true);
        
        var kyanendarEntry = new Kyanendar(name, destinationPath);
        _kyanendars.Add(kyanendarEntry);
    }

    public void DeleteKyanendar(Kyanendar kyanendar) {
        if (File.Exists(kyanendar.FilePath)) {
            File.Delete(kyanendar.FilePath);
            _kyanendars.Remove(kyanendar);
        }
    }

    public void SaveKyanendar(Kyanendar kyanendar) {
        try {
            string kyanendarString = kyanendar.ToString();
            File.WriteAllText(kyanendar.FilePath, kyanendarString);
        } catch (Exception ex) {
            throw new InvalidOperationException($"Failed to save Kyanendar '{kyanendar.Name}': {ex.Message}");
        }
    }
} 