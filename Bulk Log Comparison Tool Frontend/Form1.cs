using Bulk_Log_Comparison_Tool_Frontend.Bulk_Log_Comparison_Tool;
using Bulk_Log_Comparison_Tool.LibraryClasses;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;

namespace Bulk_Log_Comparison_Tool_Frontend
{
    public partial class Form1 : Form
    {

        private List<Panel> _panels = new();
        private UILogParser _logParser = new(new LibraryParser(false));
        private PlayerPanel _playerPanel;
        private Panel _selectedPanel;
        private List<string> _activePlayers = new();
        private string _selectedPhase = "";

        public Form1()
        {
            InitializeComponent();
            Setup();
        }

        private void Setup()
        {
            _playerPanel = new(panelPlayers, _logParser.BulkLog);
            //panels.Add(panelManageLogs);
            _panels.Add(panelStealth);
            _selectedPanel = panelStealth;
            btnStealth.Click += (sender, e) => ShowSinglePanel(panelStealth);
            ShowSinglePanel(panelStealth);
            _activePlayers = ActivePlayers();
            _playerPanel.PlayerSelectionChangedEvent += OnPlayerSelectionChanged;
        }

        public List<string> ActivePlayers() => _activePlayers;

        private void OnPlayerSelectionChanged(List<string> activePlayers)
        {
            _activePlayers = activePlayers;
            UpdatePanel(_selectedPanel);
        }

        private void ShowSinglePanel(Panel panelToShow)
        {
            foreach (Panel panel in _panels)
            {
                panel.Visible = false;
            }
            panelToShow.Visible = true;
            UpdatePanel(panelToShow);
        }

        private void UpdatePanel(Panel panel)
        {
            if (panel == panelStealth)
            {
                UpdateStealthPanel();
                return;
            }
        }

        private void UpdateStealthPanel()
        {

            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.AutoSize = false;
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.RowCount = _activePlayers.Count + 2;
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.ColumnCount = _logParser.BulkLog.Logs.Count() + 1;

            var StealthPhases = _logParser.BulkLog.GetStealthPhases();
            if(_selectedPhase == "" || !StealthPhases.Contains(_selectedPhase))
            {
                var Phase = StealthPhases.FirstOrDefault();
                if(Phase == null)
                {
                    return;
                }
                _selectedPhase = Phase;
            }
            cbStealthPhase.Items.Clear();
            cbStealthPhase.Items.AddRange(StealthPhases);

            tableLayoutPanel1.Controls.Add(new Label() { Text = _selectedPhase, AutoSize = true }, 0, 0);
            for (int x = 0; x < _logParser.BulkLog.Logs.Count(); x++)
            {
                tableLayoutPanel1.Controls.Add(new Label() { Text = _logParser.BulkLog.Logs[x].GetFileName(), AutoSize = true }, x+1, 0);
            }
            for (int y = 0; y < _activePlayers.Count; y++)
            {
                tableLayoutPanel1.Controls.Add(new Label() { Text = _activePlayers[y] }, 0, y + 1);
                for (int x = 0; x < _logParser.BulkLog.Logs.Count(); x++)
                {
                    var text = _logParser.BulkLog.Logs[x].GetStealthResult(_activePlayers[y]).Where(x => x.Item1 == _selectedPhase).Select(x => x.Item2).FirstOrDefault();
                    if (text == null)
                    {
                        text = "";
                    }
                    tableLayoutPanel1.Controls.Add(new Label() { Text = text, AutoSize = true }, x + 1, y + 1);
                }
            }
            tableLayoutPanel1.AutoSize = true;
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
                    if(_logParser.BulkLog.Logs.Any(x => x.GetFileName() == new FileInfo(file).Name))
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

        private void cbStealthPhase_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedPhase = cbStealthPhase.SelectedItem.ToString();
            UpdateStealthPanel();
        }
    }
}
