using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Kyanendar.MVVM.Models;

namespace Kyanendar.MVVM.ViewModels
{
    class WeekViewModel : BindableObject {
        private readonly Models.Week _week;

        public string Title {
            get {
                return _week.Title;
            } set {
                if (_week.Title != value) {
                    _week.Title = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Content {
            get {
                return _week.Content;
            } set {
                if (_week.Content != value) {
                    _week.Content = value;
                    OnPropertyChanged();
                }
            }
        }

        public WeekViewModel(Models.Week week)
        {
            _week = week;
        }
    }
}
