using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;

namespace PadZahr
{
    static class LanguageManager
    {
        private static Dictionary<string, string> _strings = new Dictionary<string, string>();
        public static string CurrentLanguage { get; private set; } = "en";

        public static void Load(string langCode)
        {
            CurrentLanguage = langCode;

            string path = Path.Combine(
                Application.StartupPath,
                "Languages",
                $"{langCode}.json"
            );

            if (!File.Exists(path))
                return;

            string json = File.ReadAllText(path);
            _strings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }

        public static string T(string key)
        {
            return _strings != null && _strings.ContainsKey(key)
                ? _strings[key]
                : $"[{key}]";
        }
        
    }
}
