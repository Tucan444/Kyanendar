using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kyanendar.MVVM.Models
{
    public class Kyan {
        public bool IsPadding { get; } = true;
        public KyanTime Time { get; }
        public string TimeString { 
            get {
                string kyanTime = Time.ToString()[..^1];
                if (string.IsNullOrEmpty(kyanTime)) { kyanTime = "0"; }

                return kyanTime;
            }
        }
        public List<Color> ColorSegments {
            get {
                List<Color> colorSegments = Time.ColorSegments.SkipLast(1).ToList();

                while (colorSegments.Count < 5) {
                    colorSegments = new List<Color> {KyanTime.ZERO_COLOR}.Concat(colorSegments).ToList();
                }

                return colorSegments;
            }
        }

        public Week Week1 { get; }
        public Week Week2 { get; }
        public Week Week3 { get; }

        public Kyan(KyanTime? time = null, Week? week1 = null, Week? week2 = null, Week? week3 = null)
        {
            Time = time ?? new KyanTime();
            
            Week1 = week1 ?? new Week();
            Week2 = week2 ?? new Week();
            Week3 = week3 ?? new Week();

            Week1.Time = new KyanTime(value: Time.Value * 3);
            Week2.Time = new KyanTime(value: Time.Value * 3 + 1);
            Week3.Time = new KyanTime(value: Time.Value * 3 + 2);
        }

        public override string ToString() {
            return $"{Week1},{Week2},{Week3}";
        }
    }
}
