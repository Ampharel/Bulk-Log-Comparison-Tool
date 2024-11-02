using Bulk_Log_Comparison_Tool_Frontend.Bulk_Log_Comparison_Tool;
using DarkModeForms;
using Bulk_Log_Comparison_Tool_Frontend.UI;
using System.Collections.Concurrent;
using Bulk_Log_Comparison_Tool_Frontend.Utils;
using Bulk_Log_Comparison_Tool_Frontend.Properties;
using System.Drawing.Printing;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using Bulk_Log_Comparison_Tool.Util;

namespace Bulk_Log_Comparison_Tool_Frontend
{
    public partial class Form1 : Form
    {


        private List<string> _activePlayers = new();
        private List<string> ActivePlayers => _activePlayers;
        private UILogParser _logParser = new();
        private PlayerPanel? _playerPanel;
        private PlayerUI? _boonPanel;
        private PlayerUI? _mechanicPanel;
        private PlayerUI? _dpsPanel;
        private PlayerUI? _stealthPanel;
        private PlayerUI? _shockwavePanel;
        private PlayerUI? _summaryPanel;

        private ConcurrentQueue<string> _loadedFiles = new();
        
        private string _defaultPath = "";
        private Font tableFont;

        private SettingsFile FontSettings;
        private SettingsFile PathSettings;
        private SettingsFile CustomPhaseSettings;

        public Form1()
        {
            InitializeComponent();
            _ = new DarkModeCS(this);
            tableFont = new Font("Verdana", (float)nudFontSize.Value);
            Setup();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);
        }

        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string file in files){
                LoadNewFile(file);
            }
        }

        private void Setup()
        {
            _playerPanel = new(panelPlayers, _logParser.BulkLog);
            _playerPanel.PlayerSelectionChangedEvent += OnPlayerSelectionChanged;
            tabsControl.SelectedIndexChanged += (sender, e) => UpdatePanels();

            LoadSettings();
            SetupPanels();
            UpdateFonts();
            StartTimer();
        }

        private void LoadSettings()
        {
            LoadFontSettings();
            LoadCustomPhases();
            LoadPathSettings();
        }

        private void LoadCustomPhases()
        {
            CustomPhaseSettings = new SettingsFile("CustomPhase.txt", [],
                            ["# Adding custom phases can be done using the following syntax:",
                                "# 1={PhaseName}:{Description}|StartTimeInPhase|Duration",
                                "# Example:",
                                "# 1=Primordus:before first chomp|0|14",
                                "# Lines starting with a # will be ignored"]);
            foreach (var setting in CustomPhaseSettings.GetSettings())
            {
                var settingValue = setting.Item2;
                var splitPhase = settingValue.Split('|');
                if (splitPhase.Length != 3)
                {
                    continue;
                }
                _logParser.AddCustomPhase(splitPhase[0], long.Parse(splitPhase[1]), long.Parse(splitPhase[2]));
            }
        }

        private void LoadFontSettings()
        {
            FontSettings = new SettingsFile("FontSettings.txt", [("font", "8")]);
            nudFontSize.Value = int.Parse(FontSettings.GetSetting("font"));
        }

        private void LoadPathSettings()
        {
            PathSettings = new SettingsFile("DefaultPath.txt", [("path", "")]);
            _defaultPath = PathSettings.GetSetting("path");
        }

        private void SetupPanels()
        {
            _shockwavePanel = new ShockwaveUI(tableShockwave, tabShockwaves, _logParser, ActivePlayers);
            _stealthPanel = new StealthAnalysisUI(tableStealth, lblSelectedPhaseStealth, comboStealthPhase, tabStealth, _logParser, ActivePlayers, cbShowLate, cbAlgoritmn, btnShowAlgoritmns);
            _dpsPanel = new DpsUI(tableDps, lblSelectedPhaseDps, comboDpsPhase, tabDps, _logParser, ActivePlayers, cbCumulative, cbDefiance, cbAllTargets);
            _mechanicPanel = new MechanicsUI(tableMechanics, lblSelectedPhaseMechanics, lblSelectedMechanic, comboMechanicPhase, comboMechanicMechanics, tabMechanics, _logParser, cbCount, ActivePlayers);
            _boonPanel = new BoonUI(tableBoons, lblSelectedBoonBoons, lblSelectedPhaseBoons, comboBoonPhase, comboBoonBoons, tabBoons, cbBoonTime, cbGraph, nudBoonTime, _logParser, ActivePlayers);
            _summaryPanel = new LogSummaryUI(tabSummary, tableStealthSummary, tableShockwaveSummary, tableMechanicsSummary, tableDeaths, _logParser, comboSummaryLog, ActivePlayers);
        }

        private void StartTimer()
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(CheckQueue);
            timer.Start();
        }

        private void CheckQueue(object? sender, EventArgs e)
        {
            bool loadedFile = false;

            lbLoadedFiles.BeginUpdate();
            while (_loadedFiles.TryDequeue(out string? file))
            {
                lbLoadedFiles.Items.Add(file);
                comboSummaryLog.Items.Add(file);
                comboSummaryLog.SelectedIndex = comboSummaryLog.Items.Count - 1;
                loadedFile = true;
            }
            lbLoadedFiles.EndUpdate();
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
            openFileDialog.InitialDirectory = _defaultPath;
            openFileDialog.Filter = "zevtc files (*.zevtc)|*.zevtc";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<Task> _runningTasks = new();

                PathSettings.AddSetting("path", new FileInfo(openFileDialog.FileNames.FirstOrDefault() ?? "").Directory?.FullName ?? _defaultPath);
                foreach (string file in openFileDialog.FileNames)
                {
                    _runningTasks.Add(Task.Run(() => LoadNewFile(file)));
                }
                Task.WhenAll(_runningTasks);
            }

        }

        private void btnDeleteSelected_Click(object? sender, EventArgs e)
        {
            lbLoadedFiles.SelectedItems.Cast<string>().ToList().ForEach(file => { lbLoadedFiles.Items.Remove(file); _logParser.RemoveLog(file); comboSummaryLog.Items.Remove(file); });
            _playerPanel?.Refresh();
        }

        private string _directory = "";
        private bool _running = false;

        private List<string> _oldLogs = new();

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            var openFolderDialog = new FolderBrowserDialog();
            if(_defaultPath != "")
            {
                openFolderDialog.InitialDirectory = _defaultPath;
            }
            else
            {
               openFolderDialog.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Guild Wars 2\\addons\\arcdps\\arcdps.cbtlogs\\");
            }
            openFolderDialog.InitialDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Guild Wars 2\\addons\\arcdps\\arcdps.cbtlogs\\");

            if (openFolderDialog.ShowDialog() == DialogResult.OK)
            {
                _directory = openFolderDialog.SelectedPath;
                PathSettings.AddSetting("path", _directory);
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
            FontSettings.AddSetting("font", nudFontSize.Value.ToString());
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

        private void SelectAllLogs()
        {
            //lbLoadedFiles.BeginUpdate();
            for (int i = 0; i < lbLoadedFiles.Items.Count; i++)
            {
                lbLoadedFiles.SetSelected(i, true);
            }
            //lbLoadedFiles.EndUpdate();
        }

        private Keyboard keyboard = new();
        private void lbLoadedFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (keyboard.CtrlKeyDown && e.KeyCode == Keys.A)
            {
                SelectAllLogs();
            }
        }
    }
}
