using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Kyanendar.MVVM.Models;
using System.Windows.Input;
using Kyanendar.MVVM.Views;

namespace Kyanendar.MVVM.ViewModels
{
    public class KyanendarViewModel : BindableObject
    {
        private readonly Models.Kyanendar _kyanendar;
        private readonly KyanendarService _kyanendarService;
        private bool _isNewestFirst = true;
        private string _searchText = "";

        public string Title => _kyanendar.Name;
        
        public string SearchText {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Kyans));
                }
            }
        }

        public bool IsNewestFirst {
            get => _isNewestFirst;
            set
            {
                if (_isNewestFirst != value)
                {
                    _isNewestFirst = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Kyans));
                    OnPropertyChanged(nameof(NewestFirstText));
                }
            }
        }

        public ObservableCollection<Kyan> Kyans {
            get {
                IEnumerable<Kyan> filtered;

                if (string.IsNullOrWhiteSpace(_searchText)) {
                    filtered = _kyanendar.Kyans;
                } else {
                    filtered = _kyanendar.Kyans.Where(k => 
                        k.Week1.Title.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                        k.Week2.Title.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                        k.Week3.Title.Contains(_searchText, StringComparison.OrdinalIgnoreCase));
                }

                var ordered = _isNewestFirst ? filtered.Reverse() : filtered;
                return new ObservableCollection<Kyan>(ordered);
            }
        }
        
        public string NewestFirstText => _isNewestFirst ? "↑ Newest" : "↓ Oldest";

        public ICommand OpenWeekCommand { get; }
        public ICommand AddKyanCommand { get; }
        public ICommand RemoveKyanCommand { get; }
        public ICommand ToggleSortCommand { get; }

        public KyanendarViewModel(Models.Kyanendar kyanendar, KyanendarService kyanendarService)
        {
            _kyanendar = kyanendar;
            _kyanendarService = kyanendarService;

            OpenWeekCommand = new Command<Week>(async (week) => await OpenWeekView(week));
            AddKyanCommand = new Command(AddKyan);
            RemoveKyanCommand = new Command(async () => await RemoveKyan());
            ToggleSortCommand = new Command(() => IsNewestFirst = !IsNewestFirst);
        }

        private async Task OpenWeekView(Week week) {
            if (week == null) return;
            await Shell.Current.Navigation.PushAsync(new WeekView(week, this));
        }

        private void AddKyan() {
            KyanTime new_time = new KyanTime(value: _kyanendar.RootTime.Value + _kyanendar.Kyans.Count * 3);
            Kyan newKyan = new Kyan(new_time);
            _kyanendar.Kyans.Add(newKyan);

            Save();
            OnPropertyChanged(nameof(Kyans));
        }

        private async Task RemoveKyan() {
            if (_kyanendar.Kyans.Count <= 0) {
                return;
            }

            bool shouldDelete = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                "Are you sure you want to delete last kyan ?",
                "Yes",
                "No");

            if (shouldDelete) {
                _kyanendar.Kyans.RemoveAt(_kyanendar.Kyans.Count - 1);

                Save();
                OnPropertyChanged(nameof(Kyans));
            }
        }

        public void Save() => _kyanendarService.SaveKyanendar(_kyanendar);
    }
}
