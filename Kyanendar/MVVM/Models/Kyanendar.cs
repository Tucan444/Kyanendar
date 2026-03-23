using System.Collections.Generic;
using System.IO;

namespace Kyanendar.MVVM.Models;

public class Kyanendar {
    public string Name { get; }
    public string FilePath { get; }
    public KyanTime RootTime { get; }
    public List<Kyan> Kyans { get; }

    public Kyanendar(string name, string filePath) {
        if (!File.Exists(filePath)) {
            throw new FileNotFoundException($"Kyanendar file not found: {filePath}");
        }

        Name = name;
        FilePath = filePath;
        
        var parser = new KyanendarParser(filePath);
        RootTime = parser.RootTime;
        Kyans = parser.Kyans.ToList();
    }

    public override string ToString() {
        string kyanendar_string = $"{RootTime}\n";

        foreach (Kyan kyan in Kyans) {
            kyanendar_string += $"{kyan}\n";
        }

        return kyanendar_string;
    }
} 