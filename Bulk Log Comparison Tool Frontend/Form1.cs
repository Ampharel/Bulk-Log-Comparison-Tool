using Bulk_Log_Comparison_Tool_Frontend.Bulk_Log_Comparison_Tool;
using DarkModeForms;
using Bulk_Log_Comparison_Tool_Frontend.UI;
using System.Collections.Concurrent;
using Bulk_Log_Comparison_Tool_Frontend.Utils;
using Bulk_Log_Comparison_Tool_Frontend.Properties;
using System.Drawing.Printing;

namespace Bulk_Log_Comparison_Tool_Frontend
{
    public partial class Form1 : Form
    {


        private List<string> _activePlayers = new();
        private List<string> ActivePlayers => _activePlayers;
        private UILogParser _logParser = new();
        private PlayerPanel? _playerPanel;
        private PlayerUI? _activePanel;
        private PlayerUI? _boonPanel;
        private PlayerUI? _mechanicPanel;
        private PlayerUI? _dpsPanel;
        private PlayerUI? _stealthPanel;
        private PlayerUI? _shockwavePanel;
        private PlayerUI? _summaryPanel;

        private ConcurrentQueue<string> _loadedFiles = new();

        private Font tableFont;

        public Form1()
        {
            InitializeComponent();
            _ = new DarkModeCS(this);
            Setup();
        }

        private void Setup()
        {
            _playerPanel = new(panelPlayers, _logParser.BulkLog);
            _playerPanel.PlayerSelectionChangedEvent += OnPlayerSelectionChanged;
            tabsControl.SelectedIndexChanged += (sender, e) => UpdatePanels();

            tableFont = new Font("Verdana", (float)nudFontSize.Value);
            SetupPanels();
            UpdateFonts();
            ParseCustomPhases();
            StartTimer();
        }

        private void SetupPanels()
        {
            _shockwavePanel = new ShockwaveUI(tableShockwave, tabShockwaves, _logParser, ActivePlayers);
            _stealthPanel = new StealthAnalysisUI(tableStealth, lblSelectedPhaseStealth, comboStealthPhase, tabStealth, _logParser, ActivePlayers, cbShowLate,cbAlgoritmn,btnShowAlgoritmns);
            _dpsPanel = new DpsUI(tableDps, lblSelectedPhaseDps, comboDpsPhase, tabDps, _logParser, ActivePlayers, cbCumulative, cbDefiance, cbAllTargets);
            _mechanicPanel = new MechanicsUI(tableMechanics, lblSelectedPhaseMechanics, lblSelectedMechanic, comboMechanicPhase, comboMechanicMechanics, tabMechanics, _logParser, ActivePlayers);
            _boonPanel = new BoonUI(tableBoons, lblSelectedBoonBoons, lblSelectedPhaseBoons, comboBoonPhase, comboBoonBoons, tabBoons, cbBoonTime, cbGraph, nudBoonTime,  _logParser, ActivePlayers);
            _summaryPanel = new LogSummaryUI(tabSummary, tableStealthSummary, tableShockwaveSummary, tableMechanicsSummary, tableDeaths, _logParser, comboSummaryLog, ActivePlayers);
        }

        private void StartTimer()
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(CheckQueue);
            timer.Start();
        }

        private void ParseCustomPhases()
        {
            if (!File.Exists("Settings.txt"))
            {
                File.WriteAllLines("Settings.txt", ["font=8"]);
            }
            var settings = File.ReadAllLines("Settings.txt");
            foreach (var setting in settings)
            {
                if (setting.StartsWith("font="))
                {
                    var size = setting.Split('=')[1];
                    if(int.TryParse(size, out int sizeInt))
                    {
                        nudFontSize.Value = sizeInt;
                    }
                }
            }
            if (File.Exists("CustomPhase.txt"))
            {
                var customPhases = File.ReadAllLines("CustomPhase.txt").ToList();
                foreach (var phase in customPhases)
                {
                    if (phase.StartsWith("#"))
                    {
                        continue;
                    }
                    var splitPhase = phase.Split('|');
                    if (splitPhase.Length != 3)
                    {
                        continue;
                    }
                    _logParser.AddCustomPhase(splitPhase[0], long.Parse(splitPhase[1]), long.Parse(splitPhase[2]));
                }
            }
            else
            {
                File.WriteAllLines("CustomPhase.txt", 
                    [
                        "# Adding custom phases can be done using the following syntax:",
                        "# {PhaseName}:{Description}|StartTimeInPhase|Duration",
                        "# Example:",
                        "# Primordus:before first chomp|0|14",
                        "# Lines starting with a # will be ignored"
                    ]);
            }
        }

        private void SaveSettings(string key, string value)
        {
            var settings = File.ReadAllLines("Settings.txt").ToList();
            var index = settings.FindIndex(x => x.StartsWith(key));
            if (index != -1)
            {
                settings[index] = $"{key}={value}";
            }
            else
            {
                settings.Add($"{key}={value}");
            }
            File.WriteAllLines("Settings.txt", settings);
        }

        private void CheckQueue(object? sender, EventArgs e)
        {
            bool loadedFile = false;
            while (_loadedFiles.TryDequeue(out string? file))
            {
                lbLoadedFiles.Items.Add(file);
                comboSummaryLog.Items.Add(file);
                comboSummaryLog.SelectedIndex = comboSummaryLog.Items.Count-1;
                loadedFile = true;
            }
            if (loadedFile)
            {
                _playerPanel?.Refresh();
                UpdatePanels();
            }
        }


        private void OnPlayerSelectionChanged(List<string> activePlayers)
        {
            _activePlayers = activePlayers;
            UpdatePanels();
        }

        private void UpdatePanels()
        {
            if (_activePlayers.Count == 0)
            {
                return;
            }
            if (tabsControl.SelectedTab == tabShockwaves) _shockwavePanel?.UpdateActivePlayers(ActivePlayers);
            if (tabsControl.SelectedTab == tabStealth) _stealthPanel?.UpdateActivePlayers(ActivePlayers);
            if (tabsControl.SelectedTab == tabDps) _dpsPanel?.UpdateActivePlayers(ActivePlayers);
            if (tabsControl.SelectedTab == tabMechanics) _mechanicPanel?.UpdateActivePlayers(ActivePlayers);
            if (tabsControl.SelectedTab == tabBoons) _boonPanel?.UpdateActivePlayers(ActivePlayers);
            if (tabsControl.SelectedTab == tabSummary) _summaryPanel?.UpdateActivePlayers(ActivePlayers);    
        }

        private void btnOpenLogs_Click(object? sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "zevtc files (*.zevtc)|*.zevtc";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<Task> _runningTasks = new();
                foreach (string file in openFileDialog.FileNames)
                {
                    _runningTasks.Add(Task.Run(() => LoadNewFile(file)));
                }
                Task.WhenAll(_runningTasks);
            }

        }

        private void btnDeleteSelected_Click(object? sender, EventArgs e)
        {
            lbLoadedFiles.SelectedItems.Cast<string>().ToList().ForEach(file => { lbLoadedFiles.Items.Remove(file); _logParser.RemoveLog(file); });
            _playerPanel?.Refresh();
        }

        private string _directory = "";
        private bool _running = false;

        private List<string> _oldLogs = new();

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            var openFolderDialog = new FolderBrowserDialog();
            openFolderDialog.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Guild Wars 2\\addons\\arcdps\\arcdps.cbtlogs\\");
            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                _directory = openFolderDialog.SelectedPath;
            }

            if (!_running && _directory != "")
            {
                _oldLogs.Clear();
                _oldLogs.AddRange(Directory.GetFiles(_directory, "*.zevtc"));
                Task.Run(LoadNewFilesAsync);
            }
        }

        private async void LoadNewFilesAsync()
        {
            _running = true;
            while (_running)
            {
                var files = Directory.GetFiles(_directory, "*.zevtc");
                List<Task> _runningTasks = new();
                foreach (var file in files)
                {
                    if (_oldLogs.Contains(file)) continue;
                    _oldLogs.Add(file);
                    _runningTasks.Add(Task.Run(() => LoadNewFile(file)));
                }
                await Task.WhenAll(_runningTasks);
                await (Task.Delay(1000));
            }
        }

        private void LoadNewFile(string file)
        {
            var FI = new FileInfo(file);
            if (_logParser.BulkLog.Logs.Any(x => x.GetFileName() == FI.Name))
            {
                return;
            }
            try
            {
                _logParser.AddLog(file);
                _loadedFiles.Enqueue(FI.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file {file}: {ex.Message}");
            }
        }

        private void nudFontSize_ValueChanged(object sender, EventArgs e)
        {
            SaveSettings("font", nudFontSize.Value.ToString());
            UpdateFonts();
            UpdatePanels();
        }

        private void UpdateFonts()
        {
            tableFont = new Font("Verdana", (float)nudFontSize.Value);
            IPanel.columnFont = tableFont;
            tableBoons.UpdateTableFont();
            tableDps.UpdateTableFont();
            tableMechanics.UpdateTableFont();
            tableShockwave.UpdateTableFont();
            tableStealth.UpdateTableFont();
            tableDeaths.UpdateTableFont();
            tableMechanicsSummary.UpdateTableFont();
            tableShockwaveSummary.UpdateTableFont();
            tableStealthSummary.UpdateTableFont();
        }
    }
}
