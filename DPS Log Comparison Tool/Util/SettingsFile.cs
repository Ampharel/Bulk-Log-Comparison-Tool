using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bulk_Log_Comparison_Tool.Util
{

    public class SettingsFile
    {
        private readonly string settingsName;
        private string SettingsPath
        {
            get
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Settings";
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                return $"{path}/{settingsName}";
            }
        }
        private Dictionary<string, string> _settings = new();
        private string[] descriptionLines;
        private char prefix = '#';
        private char equalSign = '=';

        public SettingsFile(string settingsName, (string, string)[] defaultSetting, string[]? descriptionLines = null, char prefixToIgnore = '#', char equalSign = '=')
        {
            this.settingsName = settingsName;
            this.equalSign = equalSign;
            this.descriptionLines = descriptionLines ?? [];
            this.prefix = prefixToIgnore;
            LoadSettings();
            foreach (var setting in defaultSetting)
            {
                if(_settings.Keys.Contains(setting.Item1))
                {
                    continue;
                }
                AddSetting(setting.Item1, setting.Item2, false);
            }
            SaveSettings();
        }

        private void LoadSettings()
        {
            if (!System.IO.File.Exists(SettingsPath))
            {
                return;
            }
            string[] lines = System.IO.File.ReadAllLines(SettingsPath);
            foreach (string line in lines)
            {
                if (line.StartsWith(prefix))
                {
                    continue;
                }
                string[] parts = line.Split(equalSign);
                if (parts.Length != 2)
                {
                    continue;
                }
                _settings[parts[0]] = parts[1];
            }
        }

        public void AddSetting(string key, string value, bool autoSave = true)
        {
            if(_settings.Keys.Contains(key) && _settings[key] == value)
            {
                
                return;
            }
            _settings[key] = value;
            if (autoSave)
            {
                SaveSettings();
            }
        }

        public string GetSetting(string key)
        {
            if (_settings.TryGetValue(key, out string value))
            {
                return value;
            }
            return "";
        }
        public (string,string)[] GetSettings() => _settings.Select(x => (x.Key, x.Value)).ToArray();

        private void SaveSettings()
        {
            var kvps = _settings.Select(x => x.Key + equalSign + x.Value);
            var description = string.Join(Environment.NewLine, descriptionLines.Select(x => prefix + x));

            try
            {
                System.IO.File.WriteAllText(SettingsPath, description + Environment.NewLine + Environment.NewLine);
                System.IO.File.AppendAllLines(SettingsPath, kvps);
            }
            catch (Exception e)
            {

            }
        }
    }
}
