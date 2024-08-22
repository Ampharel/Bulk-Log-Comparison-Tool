using Bulk_Log_Comparison_Tool_Frontend.Bulk_Log_Comparison_Tool;
using Bulk_Log_Comparison_Tool.LibraryClasses;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using Bulk_Log_Comparison_Tool.Util;
using System.Globalization;
using System.Linq;
using DarkModeForms;

namespace Bulk_Log_Comparison_Tool_Frontend
{
    public partial class Form1 : Form
    {

        private List<string> ActivePlayers() => _activePlayers;
        private List<Panel> _panels = new();
        private UILogParser _logParser = new(new LibraryParser(false));
        private PlayerPanel _playerPanel;
        private Panel _selectedPanel;
        private List<string> _activePlayers = new();
        private string _selectedPhase = "";
        private string _selectedBoon = "";

        public Form1()
        {
            InitializeComponent();
            _ = new DarkModeCS(this);
            Setup();
        }

        private void Setup()
        {
            _playerPanel = new(panelPlayers, _logParser.BulkLog);
            _activePlayers = ActivePlayers();
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
            UpdateStealthPanel();
            UpdateDpsPanel();
            UpdateBoonPanel();
        }

        private void UpdateStealthPanel()
        {
            if (tabsControl.SelectedTab != tabStealth || _activePlayers.Count == 0)
            {
                return;
            }
            tabStealth.Controls.Remove(tableStealth);
            tableStealth.DataSource = null;
            tableStealth.RowCount = _activePlayers.Count;
            tableStealth.ColumnCount = _logParser.BulkLog.Logs.Count();

            var StealthPhases = _logParser.BulkLog.GetStealthPhases();
            if (_selectedPhase == "" || !StealthPhases.Contains(_selectedPhase))
            {
                var Phase = StealthPhases.FirstOrDefault();
                if (Phase == null)
                {
                    return;
                }
                _selectedPhase = Phase;
            }
            lblSelectedPhaseStealth.Text = _selectedPhase;
            cbStealthPhase.Items.Clear();
            cbStealthPhase.Items.AddRange(StealthPhases);

            tableStealth.TopLeftHeaderCell.Value = _selectedPhase;
            for (int x = 0; x < _logParser.BulkLog.Logs.Count(); x++)
            {
                tableStealth.Columns[x].HeaderCell.Value = _logParser.BulkLog.Logs[x].GetFileName();
                tableStealth.Columns[x].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            for (int y = 0; y < _activePlayers.Count; y++)
            {
                tableStealth.Rows[y].HeaderCell.Value = _activePlayers[y];
                for (int x = 0; x < _logParser.BulkLog.Logs.Count(); x++)
                {
                    var text = _logParser.BulkLog.Logs[x].GetStealthResult(_activePlayers[y]).Where(x => x.Item1 == _selectedPhase).Select(x => x.Item2).FirstOrDefault();
                    if (text == null && _logParser.BulkLog.GetPlayers().Contains(_activePlayers[y]))
                    {
                        text = "✓";
                    }
                    tableStealth.Rows[y].Cells[x].Value = text;
                }
            }
            tabStealth.Controls.Add(tableStealth);
        }
        private void UpdateDpsPanel()
        {
            if (tabsControl.SelectedTab != tabDps || _activePlayers.Count == 0)
            {
                return;
            }
            tabDps.Controls.Remove(tableDps);
            tableDps.DataSource = null;
            tableDps.RowCount = _activePlayers.Count + 1;
            tableDps.ColumnCount = _logParser.BulkLog.Logs.Count() + 1;

            var Phases = _logParser.BulkLog.GetPhases();
            if (_selectedPhase == "" || !Phases.Contains(_selectedPhase))
            {
                var Phase = Phases.FirstOrDefault();
                if (Phase == null)
                {
                    return;
                }
                _selectedPhase = Phase;
            }
            cbDpsPhase.Items.Clear();
            cbDpsPhase.Items.AddRange(Phases);
            lblSelectedPhaseDps.Text = _selectedPhase;

            tableDps.TopLeftHeaderCell.Value = _selectedPhase;
            for (int x = 0; x < _logParser.BulkLog.Logs.Count(); x++)
            {
                tableDps.Columns[x].HeaderCell.Value = _logParser.BulkLog.Logs[x].GetFileName();
                tableDps.Columns[x].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            tableDps.Columns[_logParser.BulkLog.Logs.Count()].HeaderCell.Value = "Trimmed Mean";
            var TotalDps = new Dictionary<string, List<int>>();
            for (int y = 0; y < _activePlayers.Count; y++)
            {
                tableDps.Rows[y].HeaderCell.Value = _activePlayers[y];
                List<int> dpsnumbers = new();
                for (int x = 0; x < _logParser.BulkLog.Logs.Count(); x++)
                {
                    if (TotalDps.ContainsKey(_logParser.BulkLog.Logs[x].GetFileName()) == false)
                    {
                        TotalDps.Add(_logParser.BulkLog.Logs[x].GetFileName(), new List<int>());
                    }
                    int dps = _logParser.BulkLog.Logs[x].GetPlayerDps(_activePlayers[y], _selectedPhase);
                    float roundedDps = (float)Math.Round(dps / 1000f, 1);
                    TotalDps[_logParser.BulkLog.Logs[x].GetFileName()].Add(dps);
                    dpsnumbers.Add(dps);
                    var text = $"{roundedDps}k";
                    tableDps.Rows[y].Cells[x].Value = text;
                }
                float RoundedAverage = (float)Math.Round(TrimmedAverage(dpsnumbers).Average() / 1000f, 1);
                tableDps.Rows[y].Cells[_logParser.BulkLog.Logs.Count()].Value = $"{RoundedAverage}k";
            }
            int row = _activePlayers.Count + 1;

            tableDps.Rows[_activePlayers.Count].HeaderCell.Value = "Total DPS";
            for (int x = 0; x < _logParser.BulkLog.Logs.Count(); x++)
            {
                tableDps.Rows[_activePlayers.Count].Cells[x].Value = $"{(float)Math.Round(TotalDps[_logParser.BulkLog.Logs[x].GetFileName()].Sum() / 1000f, 1)}k";
            }
            tableDps.AutoSize = true;
            tabDps.Controls.Add(tableDps);
        }

        private void UpdateBoonPanel()
        {
            if (tabsControl.SelectedTab != tabBoons)
            {
                return;
            }
            var Groups = _logParser.BulkLog.GetGroups();
            if(_activePlayers.Count + Groups.Count() == 0)
            {
                return;
            }
            tabBoons.Controls.Remove(tableBoons);
            tableBoons.DataSource = null;
            
            tableBoons.RowCount = _activePlayers.Count + Groups.Count();
            tableBoons.ColumnCount = _logParser.BulkLog.Logs.Count()+1;

            var Phases = _logParser.BulkLog.GetPhases();
            if (_selectedPhase == "" || !Phases.Contains(_selectedPhase))
            {
                var Phase = Phases.FirstOrDefault();
                if (Phase == null)
                {
                    return;
                }
                _selectedPhase = Phase;
            }
            cbBoonPhase.Items.Clear();
            cbBoonPhase.Items.AddRange(Phases);
            lblSelectedPhaseBoons.Text = _selectedPhase;

            var boonNames = _logParser.BulkLog.GetBoonNames();
            if (_selectedBoon == "" || !boonNames.Contains(_selectedBoon))
            {
                var Boon = boonNames.FirstOrDefault();
                if (Boon == null)
                {
                    return;
                }
                _selectedBoon = Boon;
            }

            cbBoonBoons.Items.Clear();
            cbBoonBoons.Items.AddRange(boonNames);
            lblSelectedBoonBoons.Text = _selectedBoon;

            tableBoons.TopLeftHeaderCell.Value = _selectedPhase;
            for (int x = 0; x < _logParser.BulkLog.Logs.Count(); x++)
            {
                tableBoons.Columns[x].HeaderCell.Value = _logParser.BulkLog.Logs[x].GetFileName();
            }
            tableBoons.Columns[_logParser.BulkLog.Logs.Count()].HeaderCell.Value = "Trimmed Mean";
            for (int y = 0; y < _activePlayers.Count; y++)
            {
                tableBoons.Rows[y].HeaderCell.Value = _activePlayers[y];
                List<double> boonNumbers = new();
                BuffStackTyping boonType = BuffStackTyping.Stacking;
                for (int x = 0; x < _logParser.BulkLog.Logs.Count(); x++)
                {
                    double boonUptime = _logParser.BulkLog.Logs[x].GetBoon(_activePlayers[y], _selectedBoon, _selectedPhase);
                    boonType = _logParser.BulkLog.Logs[x].GetBoonStackType(_selectedBoon);
                    boonNumbers.Add(boonUptime);
                    var text = $"{boonUptime.ToString("F1")}";
                    if (boonType == BuffStackTyping.Queue || boonType == BuffStackTyping.Regeneration)
                    {
                        text += "%";
                    }
                    tableBoons.Rows[y].Cells[x].Value = text;
                }
                float RoundedAverage = (float)Math.Round(TrimmedAverage(boonNumbers).Average(), 1);
                var averageText = $"{RoundedAverage.ToString("F1")}";
                if (boonType == BuffStackTyping.Queue || boonType == BuffStackTyping.Regeneration)
                {
                    averageText += "%";
                }
                tableBoons.Rows[y].Cells[_logParser.BulkLog.Logs.Count()].Value = averageText;
            }
            int row = _activePlayers.Count;
            foreach (var group in Groups)
            {
                tableBoons.Rows[row].HeaderCell.Value = $"Group {group}";
                for (int x = 0; x < _logParser.BulkLog.Logs.Count(); x++)
                {
                    var boonUptime = _logParser.BulkLog.Logs[x].GetBoon(group, _selectedBoon, _selectedPhase);
                    var groupText = $"{boonUptime.ToString("F1")}";
                    var bt = _logParser.BulkLog.Logs[x].GetBoonStackType(_selectedBoon);
                    if (bt == BuffStackTyping.Queue || bt == BuffStackTyping.Regeneration)
                    {
                        groupText += "%";
                    }
                    tableBoons.Rows[row].Cells[x].Value = $"{groupText}";
                }
                row++;
            }
            tableBoons.AutoSize = true;
            tabBoons.Controls.Add(tableBoons);
        }

        private void cbStealthPhase_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedPhase = cbStealthPhase.SelectedItem.ToString();
            UpdateStealthPanel();
        }
        private void cbDpsPhase_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedPhase = cbDpsPhase.SelectedItem.ToString();
            UpdateDpsPanel();
        }
        private void cbBoonBoons_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedBoon = cbBoonBoons.SelectedItem.ToString();
            UpdateBoonPanel();
        }

        private void cbBoonPhase_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedPhase = cbBoonPhase.SelectedItem.ToString();
            UpdateBoonPanel();
        }


        private void btnOpenLogs_Click(object sender, EventArgs e)
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
            _playerPanel.Refresh();
        }


        private void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            lbLoadedFiles.SelectedItems.Cast<string>().ToList().ForEach(file => { lbLoadedFiles.Items.Remove(file); _logParser.RemoveLog(file); });

            _playerPanel.Refresh();
        }


        public List<int> TrimmedAverage(List<int> ints)
        {
            if (ints.Count == 0)
            {
                return ints;
            }
            var sortedInts = ints.OrderBy(x => x).ToList();
            var Max = sortedInts.Max();
            return sortedInts.Where(x => x >= Max * 0.6).ToList();
        }
        public List<double> TrimmedAverage(List<double> doubles)
        {
            if (doubles.Count == 0)
            {
                return doubles;
            }
            var sortedDoubles = doubles.OrderBy(x => x).ToList();
            var Max = sortedDoubles.Max();
            return sortedDoubles.Where(x => x >= Max * 0.6).ToList();
        }
    }
}
