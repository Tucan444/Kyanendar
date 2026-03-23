using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kyanendar.MVVM.Models
{
    public struct KyanTime
    {
        public static Color ZERO_COLOR = Color.FromArgb("#8787e8");
        public static Color ONE_COLOR = Color.FromArgb("#5ed793");
        public static Color TWO_COLOR = Color.FromArgb("#d3eb43");

        private List<int> _digits;

        public List<int> Digits => _digits.ToList().Reverse<int>().ToList();

        public int Value {
            get {
                int result = 0;
                for (int i = 0; i < _digits.Count; i++) {
                    result += _digits[i] * (int)Math.Pow(3, i);
                }
                return result;
            }
        }

        public List<Color> ColorSegments {
            get {
                var digits = this.ToString();
                var colorList = new List<Color>();

                foreach (char digit in digits) {
                    colorList.Add(digit switch
                    {
                        '0' => ZERO_COLOR,
                        '1' => ONE_COLOR,
                        '2' => TWO_COLOR,
                        _ => Colors.Gray
                    });
                }

                return colorList;
            }
        }

        public KyanTime() {
            _digits = new List<int> { 0 };
        }

        public KyanTime(int? value = null, List<int>? digits = null) {
            if (digits != null) {
                _digits = digits.ToList().Reverse<int>().ToList();
            }
            else if (value.HasValue) {
                _digits = ConvertToBase3(value.Value);
            }
            else {
                _digits = new List<int> { 0 };
            }
        }

        public KyanTime Clone() {
            return new KyanTime(digits: _digits.ToList().Reverse<int>().ToList());
        }

        public void Increment() {
            _digits = ConvertToBase3(Value + 1);
        }

        public void Decrement() {
            if (Value <= 0) return;
            _digits = ConvertToBase3(Value - 1);
        }

        private List<int> ConvertToBase3(int value) {
            if (value == 0) return new List<int> { 0 };

            var result = new List<int>();
            while (value > 0) {
                result.Add(value % 3);
                value /= 3;
            }
            return result;
        }

        public override string ToString() {
            return string.Join("", _digits.AsEnumerable().Reverse());
        }

        public override bool Equals(object? obj) {
            if (obj is KyanTime other) {
                return Value == other.Value;
            }
            return false;
        }

        public override int GetHashCode() {
            return Value.GetHashCode();
        }
    }
}
