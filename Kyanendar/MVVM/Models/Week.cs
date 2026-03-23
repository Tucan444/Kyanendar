using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kyanendar.MVVM.Models
{
    public class Week : INotifyPropertyChanged {
        private string _title = "";
        public string Content {get; set; } = "";
        public KyanTime Time { get; set; }

        public Week() {
            Time = new KyanTime();
        }

        public string Title{
            get => _title;
            set {
                if (_title != value) {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public override string ToString() {
            string normalizedContent = Content.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "(|||)");
            normalizedContent = normalizedContent.Replace(",", "{|||}");

            string normalizedTitle = Title.Replace(",", "{|||}");
            
            if (string.IsNullOrEmpty(normalizedContent)) {
                return normalizedTitle;
            }
            
            return $"{normalizedTitle}|||||{normalizedContent}";
        }
    }
}
