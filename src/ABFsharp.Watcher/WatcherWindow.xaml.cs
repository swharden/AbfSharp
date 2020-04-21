using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinForms = System.Windows.Forms;

namespace ABFsharp.Watcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string testFolder = @"D:\Data\abfs-2019\project2\abfs";
            FolderPathTextbox.Text = (System.IO.Directory.Exists(testFolder)) ? testFolder : "./";
            ScanFolder(null, null);
        }

        private void AnalyzeNow(object sender, RoutedEventArgs e)
        {
            ScanFolder(null, null);

            LogTextBox.Clear();
            FolderControlPanel.IsEnabled = false;

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted; ;
            worker.RunWorkerAsync();
        }

        readonly Stopwatch stopwatch = new Stopwatch();
        private string[] abfPaths;
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // do NOT access GUI components from this thread
            stopwatch.Restart();
            for (int i = 0; i < abfPaths.Length; i++)
            {
                (sender as BackgroundWorker).ReportProgress(0, i);
                Stopwatch stopwatchSingle = Stopwatch.StartNew();

                string abfFilePath = abfPaths[i];
                ABFsharp.Analyzer.AutoAnalyze.AbfFile(abfFilePath);

                double msec = 1000 * (double)stopwatchSingle.ElapsedTicks / Stopwatch.Frequency;
                (sender as BackgroundWorker).ReportProgress(100, msec);
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // you may access GUI components from this thread
            if (e.ProgressPercentage == 0)
            {
                int abfIndex = (int)e.UserState;
                string abfPath = abfPaths[abfIndex];
                string abfFileName = System.IO.Path.GetFileName(abfPath);

                //AbfListBox.Items[abfIndex] = $"{abfFileName} ✔️";
                AbfListBox.ScrollIntoView(AbfListBox.Items[abfIndex]);
                AbfListBox.SelectedItem = AbfListBox.Items[abfIndex];

                LogTextBox.AppendText($"analyzing {abfFileName}... ");
            }
            else
            {
                double msec = (double)e.UserState;
                LogTextBox.AppendText($"{msec:N2} ms" + Environment.NewLine);
            }

            LogTextBox.ScrollToEnd();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            double sec = (double)stopwatch.ElapsedTicks / Stopwatch.Frequency;
            LogTextBox.AppendText($"Analyzed {abfPaths.Length} ABFs in {sec:N2} seconds");
            LogTextBox.ScrollToEnd();
            FolderControlPanel.IsEnabled = true;
        }

        private void ScanFolder(object sender, RoutedEventArgs e)
        {
            var abfFolder = System.IO.Path.GetFullPath(FolderPathTextbox.Text);
            if (System.IO.Directory.Exists(abfFolder))
            {
                abfPaths = System.IO.Directory.GetFiles(abfFolder, "*.abf");
                LogTextBox.Text = $"folder contains {abfPaths.Length} ABF files";
                AnalyzeButton.Content = $"🔬 {abfPaths.Length}";

                AbfListBox.Items.Clear();
                foreach (var abfFilePath in abfPaths)
                    AbfListBox.Items.Add(System.IO.Path.GetFileName(abfFilePath));
            }
            else
            {
                LogTextBox.Text = $"folder does not exist: {abfFolder}";
            }

        }

        private void SelectFolder(object sender, RoutedEventArgs e)
        {
            var diag = new WinForms.FolderBrowserDialog();
            if (diag.ShowDialog() == WinForms.DialogResult.OK)
                FolderPathTextbox.Text = diag.SelectedPath;
        }
    }
}
