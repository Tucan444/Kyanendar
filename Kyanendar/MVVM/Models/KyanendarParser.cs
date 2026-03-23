using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kyanendar.MVVM.Models
{
    public class KyanendarParser {
        private readonly string _filePath;
        private readonly List<string> _lines;
        private KyanTime _rootTime;

        public KyanTime RootTime => _rootTime;
        public List<Kyan> Kyans { get; private set; }

        public KyanendarParser(string filePath) {
            _filePath = filePath;
            
            _lines = File.ReadAllLines(filePath)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => {
                    // remove numbered prefixes
                    line = Regex.Replace(line, @"^\d+\.", "");

                    return line.Trim();
                }).ToList();
                
            ParseRootTime();
            Kyans = new List<Kyan>();
            ParseKyans();
        }

        private void ParseRootTime() {
            if (_lines.Count == 0) {
                _rootTime = new KyanTime();
                return;
            }

            string firstLine = _lines[0];
            if (firstLine.All(c => c == '0' || c == '1' || c == '2')) {
                var digits = firstLine.Select(c => int.Parse(c.ToString())).ToList();
                _rootTime = new KyanTime(digits: digits);
                _lines.RemoveAt(0);
            } else {
                _rootTime = new KyanTime();
            }
        }

        private void ParseKyans() {
            for (int i = 0; i < _lines.Count; i++) {
                string line = _lines[i];
                var weekStrings = line.Split(',');
                
                if (weekStrings.Length != 3) { continue; }

                var weeks = new List<Week>();
                foreach (var weekStr in weekStrings) {
                    var week = new Week();
                    
                    var parts = weekStr.Split("|||||", StringSplitOptions.TrimEntries);
                    week.Title = parts[0].Trim().Replace("{|||}", ",");
                    week.Content = "";

                    if (parts.Length > 1) {
                        week.Content = parts[1].Trim().Replace("(|||)", "\n").Replace("{|||}", ",");
                    }
                    
                    weeks.Add(week);
                }

                var kyanTime = new KyanTime(value: _rootTime.Value + i*3);
                var kyan = new Kyan(kyanTime, weeks[0], weeks[1], weeks[2]);
                Kyans.Add(kyan);
            }
        }
    }
}
