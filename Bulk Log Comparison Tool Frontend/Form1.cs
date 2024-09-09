using Bulk_Log_Comparison_Tool_Frontend.Bulk_Log_Comparison_Tool;
using Bulk_Log_Comparison_Tool.LibraryClasses;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using Bulk_Log_Comparison_Tool.Util;
using System.Globalization;
using System.Linq;
using DarkModeForms;
using System.Diagnostics.Metrics;
using System.Text;
using Bulk_Log_Comparison_Tool_Frontend.UI;

namespace Bulk_Log_Comparison_Tool_Frontend
{
    public partial class Form1 : Form
    {


        private List<string> _activePlayers = new();
        private List<string> ActivePlayers => _activePlayers;
        private UILogParser _logParser = new(new LibraryParser(false));
        private PlayerPanel? _playerPanel;
        private PlayerUI? _activePanel;
        private PlayerUI? _boonPanel;
        private PlayerUI? _mechanicPanel;
        private PlayerUI? _dpsPanel;
        private PlayerUI? _stealthPanel;
        private PlayerUI? _shockwavePanel;

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
        }


        private void OnPlayerSelectionChanged(List<string> activePlayers)
        {
            _activePlayers = activePlayers;
            UpdatePanels();
        }

        private void UpdatePanels()
        {
            if(_activePlayers.Count == 0)
            {
                return;
            }
            if (tabsControl.SelectedTab == tabShockwaves) _shockwavePanel?.UpdateActivePlayers(ActivePlayers);
            if (tabsControl.SelectedTab == tabStealth) _stealthPanel?.UpdateActivePlayers(ActivePlayers);
            if (tabsControl.SelectedTab == tabDps) _dpsPanel?.UpdateActivePlayers(ActivePlayers);
            if (tabsControl.SelectedTab == tabMechanics) _mechanicPanel?.UpdateActivePlayers(ActivePlayers);
            if (tabsControl.SelectedTab == tabBoons) _boonPanel?.UpdateActivePlayers(ActivePlayers);
        }

        private void btnOpenLogs_Click(object? sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "zevtc files (*.zevtc)|*.zevtc";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in openFileDialog.FileNames)
                {
                    if (_logParser.BulkLog.Logs.Any(x => x.GetFileName() == new FileInfo(file).Name))
                    {
                        continue;
                    }
                    try
                    {
                        lbLoadedFiles.Items.Add(new FileInfo(file).Name);
                        _logParser.AddLog(file);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading file {file}: {ex.Message}");
                    }
                }
            }
            if (File.Exists("SpecificPhase.txt"))
            {
                var customPhases = File.ReadAllLines("SpecificPhase.txt").ToList();
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
                    try
                    {
                        _logParser.BulkLog.Logs.ForEach(x => x.AddPhase(splitPhase[0], long.Parse(splitPhase[1]), long.Parse(splitPhase[2])));
                    }
                    catch (Exception ex)
                    {
                        var startSucceeded = long.TryParse(phase, out long start);
                        var durationSucceeded = long.TryParse(phase, out long duration);
                        MessageBox.Show($"Error in custom phase {phase}: {ex.Message}\nStart is {(startSucceeded ? "Valid" : "Not Valid")}\nDuration is {(durationSucceeded ? "Valid" : "Not Valid")}");
                    }
                }
            }
            _shockwavePanel = new ShockwaveUI(tableShockwave, tabShockwaves, _logParser, ActivePlayers);
            _stealthPanel = new StealthAnalysisUI(tableStealth, lblSelectedPhaseStealth, cbStealthPhase, tabStealth, _logParser, ActivePlayers);
            _dpsPanel = new DpsUI(tableDps, lblSelectedPhaseDps, cbDpsPhase, tabDps, _logParser, ActivePlayers);
            _mechanicPanel = new MechanicsUI(tableMechanics, lblSelectedPhaseMechanics ,lblSelectedMechanic,cbMechanicPhase, cbMechanicMechanics, tabMechanics, _logParser, ActivePlayers);
            _boonPanel = new BoonUI(tableBoons, lblSelectedBoonBoons, lblSelectedPhaseBoons, cbBoonBoons, cbBoonPhase,  tabBoons, _logParser, ActivePlayers);
            _playerPanel?.Refresh();
        }

        private void btnDeleteSelected_Click(object? sender, EventArgs e)
        {
            lbLoadedFiles.SelectedItems.Cast<string>().ToList().ForEach(file => { lbLoadedFiles.Items.Remove(file); _logParser.RemoveLog(file); });
            _playerPanel?.Refresh();
        }
    }
}
