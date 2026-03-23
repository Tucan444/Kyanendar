using System.Collections.ObjectModel;
using System.Windows.Input;
using Kyanendar.MVVM.Models;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using System.Diagnostics;
using Kyanendar.MVVM.Views;

namespace Kyanendar.MVVM.ViewModels;

public class MenuViewModel : BindableObject
{
    private readonly KyanendarService _kyanendarService;
    public ICommand AddBlankCommand { get; }
    public ICommand ImportCommand { get; }
    public ICommand DownloadCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand ShowPathsCommand { get; }
    public ICommand OpenKyanendarCommand { get; }
    public ObservableCollection<Models.Kyanendar> Kyanendars => _kyanendarService.Kyanendars;


    public MenuViewModel(KyanendarService kyanendarService)
    {
        _kyanendarService = kyanendarService;
        AddBlankCommand = new Command(async () => await AddBlankKyanendar());
        ImportCommand = new Command(async () => await ImportKyanendar());
        DownloadCommand = new Command<Models.Kyanendar>(async (kyanendar) => await DownloadKyanendar(kyanendar));
        DeleteCommand = new Command<Models.Kyanendar>(async (kyanendar) => await DeleteKyanendar(kyanendar));
        ShowPathsCommand = new Command(ShowPaths);
        OpenKyanendarCommand = new Command<Models.Kyanendar>(async (kyanendar) => await OpenKyanendar(kyanendar));
    }

    private void ShowPaths() {
        try {
            var path = FileSystem.AppDataDirectory;
            if (OperatingSystem.IsWindows())
            {
                Process.Start("explorer.exe", path);
            }
            else
            {
                Application.Current.MainPage.DisplayAlert("App Data Path", path, "OK");
            }
        }
        catch (Exception ex)
        {
            Application.Current.MainPage.DisplayAlert("Error", $"Could not open folder: {ex.Message}", "OK");
        }
    }

    private async Task AddBlankKyanendar() {
        string name = await Application.Current.MainPage.DisplayPromptAsync(
            "New Kyanendar", "Enter name for new Kyanendar:");

        if (!string.IsNullOrWhiteSpace(name)) {
            try {
                _kyanendarService.CreateBlankKyanendar(name);
            }
            catch (InvalidOperationException ex) {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    private async Task ImportKyanendar() {
        var filePath = await PickKyanendarFile();

        if (!string.IsNullOrEmpty(filePath)) {
            try {
                _kyanendarService.AddKyanendar(filePath);
            }
            catch (InvalidOperationException ex) {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    private async Task DownloadKyanendar(Models.Kyanendar kyanendar) {
        if (kyanendar != null) {
            var content = File.ReadAllText(kyanendar.FilePath);
            await SaveKyanendarFile(content, kyanendar.Name);
        }
    }

    private async Task<string?> PickKyanendarFile() {
        try {
            var result = await FilePicker.PickAsync(new PickOptions {

                PickerTitle = "Pick a Kyanendar file",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".txt" } },
                    { DevicePlatform.Android, new[] { "text/plain" } }
                })
            });

            if (result != null) { return result.FullPath; }
            
        } catch (Exception ex) {
            await Application.Current.MainPage.DisplayAlert("Error", "Failed to pick file: " + ex.Message, "OK");
        }
        return null;
    }

    private async Task SaveKyanendarFile(string content, string fileName) {
        try {
            string tempPath = Path.Combine(FileSystem.CacheDirectory, $"{fileName}.txt");

            File.WriteAllText(tempPath, content);

            var shareRequest = new ShareFileRequest {
                Title = $"Share {fileName}",
                File = new ShareFile(tempPath)
            };

            await Share.RequestAsync(shareRequest);
            await Application.Current.MainPage.DisplayAlert("Success", "File shared successfully!", "OK");

        }
        catch (Exception ex) {
            await Application.Current.MainPage.DisplayAlert("Error", "Failed to share file: " + ex.Message, "OK");
        }
    }

    private async Task DeleteKyanendar(Models.Kyanendar kyanendar) {
        if (kyanendar == null) return;

        bool shouldDelete = await Application.Current.MainPage.DisplayAlert(
            "Confirm Delete",
            $"Are you sure you want to delete \"{kyanendar.Name}\" ?",
            "Yes",
            "No");

        if (shouldDelete) {
            _kyanendarService.DeleteKyanendar(kyanendar);
        }
    }

    private async Task OpenKyanendar(Models.Kyanendar kyanendar) {
        if (kyanendar == null) return;
        await Shell.Current.Navigation.PushAsync(new KyanendarView(kyanendar, _kyanendarService));
    }
} 