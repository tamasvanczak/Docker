using MySqlX.XDevAPI.Common;
using NeuralNet_WPF_UI.Core;
using NeuralNet_WPF_UI.DB;
using NeuralNet_WPF_UI.Interfaces;
using NeuralNet_WPF_UI.Models;
using NeuralNet_WPF_UI.Patterns;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; // Ez kell a DataGrid automatikus frissítéséhez
using System.Diagnostics;
using System.Linq; // Ez kell a kereséshez/szűréshez
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NeuralNet_WPF_UI
{
    public partial class MainWindow : Window, ITrainingObserver
    {
        // Ez az "adatbázisunk" (ObservableCollection: ha változik a lista, a DataGrid magától frissül)
        private ObservableCollection<TrainingRecord> allRecords = new ObservableCollection<TrainingRecord>();
        private ILossStrategy selectedStrategy;
        private double lastLossValue;
        private int idCounter = 0;
        private List<double> currentTrainingLosses = new List<double>();
        TrainingRecord result = new TrainingRecord();

        public MainWindow()
        {
            InitializeComponent();

            allRecords = new ObservableCollection<TrainingRecord>();

            HistoryGrid.ItemsSource = allRecords;
        }
        private async void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            Parameters paramWindow = new Parameters();
            paramWindow.Owner = this;

            if (paramWindow.ShowDialog() == true)
            {

                // JAVÍTÁS: Nem kap fix értéket, hanem átvesszük a felugró ablakból!
                selectedStrategy = paramWindow.ChosenStrategy;
                int epochs = paramWindow.Epochs;
                double inputLR = paramWindow.LearningRate;
                int inputNeurons = paramWindow.InputNeurons;
                int hiddenNeurons = paramWindow.HiddenNeurons;
                int outputNeurons = paramWindow.OutputNeurons;

                try
                {
                    LogList.Items.Clear();
                    StartBtn.IsEnabled = false;
                    LogList.Items.Add("Rendszer inicializálása az új ablakból kapott értékekkel...");
                    LogList.Items.Add($"Topológia: {inputNeurons} -> {hiddenNeurons} -> {outputNeurons} | LR: {inputLR} | Epochs: {epochs}");

                    var net = new NeuralNetwork(selectedStrategy);
                    net.AddLayer("dense", $"Bemenet ({inputNeurons} neuron)");
                    net.AddLayer("dropout", $"Szűrő ({hiddenNeurons} neuron)");
                    net.AddLayer("dense", $"Kimenet ({outputNeurons} neuron)");

                    net.Subscribe(this);

                    await Task.Run(() => net.RunTraining(epochs));

                    double finalLoss = Math.Round(lastLossValue, 6);

                    double finalAccuracy = Math.Round(1.0 - (finalLoss * 1.5), 4);
                    if (finalAccuracy > 1.0) finalAccuracy = 0.994;
                    if (finalAccuracy < 0) finalAccuracy = 0.05;

                    DBMethods.AddResult(selectedStrategy.GetStrategyName(), finalLoss, finalAccuracy, inputLR);

                    var newRecord = new TrainingRecord
                    {
                        id = idCounter++,
                        Strategy = $"{selectedStrategy.GetStrategyName()} [{inputNeurons}-{hiddenNeurons}-{outputNeurons}]",
                        Result = finalLoss,
                        Accuracy = finalAccuracy,
                        LearningRate = inputLR,
                        Timestamp = DateTime.Now.ToString("HH:mm:ss")
                    };

                    allRecords.Insert(0, newRecord);

                    DrawChart();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba a tanítás során: {ex.Message}", "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    StartBtn.IsEnabled = true;
                }
            }
        }

        public void OnEpochEnd(int epoch, double loss)
        {
            lastLossValue = loss; 

            Dispatcher.Invoke(() =>
            {
                string info = $"[Epoch {epoch:D2}] | Aktuális hiba: {loss:F6}";
                LogList.Items.Add(info);
                LogList.ScrollIntoView(LogList.Items[LogList.Items.Count - 1]);
            });
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var selected = HistoryGrid.SelectedItem as TrainingRecord;

            if (selected == null && HistoryGrid.SelectedItem != null)
            {
                MessageBox.Show($"Hiba: A kijelölt elem létezik, de a típusa nem TrainingRecord, hanem: {HistoryGrid.SelectedItem.GetType().Name}");
                return;
            }

            if (selected != null)
            {
                var confirm = MessageBox.Show($"Biztosan törölni szeretnéd a(z) {selected.id}. ID-jú rekordot?",
                                              "Törlés", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    DBMethods.DeleteResultByid(selected.id);

                    allRecords.Remove(selected);

                    HistoryGrid.ItemsSource = null;
                    HistoryGrid.ItemsSource = allRecords;
                }
            }
            else
            {
                MessageBox.Show("Nincs kijelölve semmi! Kérlek kattints a sor elején található szürke kis négyzetre vagy magára a szövegre, hogy kék legyen a sor!");
            }
            DrawChart();
        }

        private void SearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                HistoryGrid.ItemsSource = allRecords;
            }
            else
            {
                var filteredList = allRecords.Where(r => r.Strategy.ToLower().Contains(searchText)).ToList();
                HistoryGrid.ItemsSource = filteredList;
            }
        }

        private async void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!(HistoryGrid.SelectedItem is TrainingRecord selected))
            {
                MessageBox.Show("Kérlek, előbb jelölj ki egy sort a lenti táblázatban a módosításhoz!", "Nincs kijelölés", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            UpdateBtn.IsEnabled = false;
            StartBtn.IsEnabled = false;
            LogList.Items.Clear();
            LogList.Items.Add($"Rekord ({selected.id}) újratanítása indítása...");

            ILossStrategy newStrategy;
            string oldStrategyName = selected.Strategy;

            if (oldStrategyName.Contains("MSE")) newStrategy = new MaeLoss();
            else if (oldStrategyName.Contains("MAE")) newStrategy = new BceLoss();
            else newStrategy = new MseLoss();

            var net = new NeuralNetwork(newStrategy);
            net.AddLayer("dense", "Bemenet");
            net.AddLayer("dropout", "Szűrő");
            net.AddLayer("dense", "Kimenet");
            net.Subscribe(this);

            await Task.Run(() => net.RunTraining(10));

            string newStrategyName = newStrategy.GetStrategyName();
            double newLossResult = Math.Round(lastLossValue, 6);

            double newAccuracy = Math.Round(1.0 - (newLossResult * 2), 4);
            if (newAccuracy > 1.0) newAccuracy = 0.992;
            if (newAccuracy < 0) newAccuracy = 0.10;

            double currentLR = 0.0100; 

            DBMethods.UpdateResultById(selected.id, newStrategyName, newLossResult, newAccuracy, currentLR);

            selected.Strategy = newStrategyName;
            selected.Result = newLossResult;
            selected.Accuracy = newAccuracy;
            selected.LearningRate = currentLR;
            selected.Timestamp = DateTime.Now.ToString("HH:mm:ss");

            HistoryGrid.Items.Refresh();

            DrawChart();

            UpdateBtn.IsEnabled = true;
            StartBtn.IsEnabled = true;

            MessageBox.Show($"A(z) {selected.id}. ID-jú rekord sikeresen frissítve a táblázatban és az adatbázisban is!", "Siker");
        }

        private void PreviousBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dbResults = DBMethods.GetResults();

                PreviousResults.ItemsSource = null;
                PreviousResults.Items.Clear();
                allRecords.Clear(); 

                if (dbResults != null && dbResults.Count > 0)
                {
                    foreach (var r in dbResults)
                    {
                        PreviousResults.Items.Add($"{r.Timestamp} | {r.Strategy} | Hiba: {r.Result:F6}");

                        allRecords.Add(r);
                    }
                }
                else
                {
                    PreviousResults.Items.Add("Nincsenek mentett adatok az adatbázisban.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a múltbeli adatok betöltésekor: {ex.Message}");
            }
            DrawChart();
        }

        private void DrawChart()
        {
            ChartCanvas.Children.Clear();

            if (allRecords == null || allRecords.Count < 2) return;

            var orderedRecords = allRecords.Reverse().ToList();

            double canvasWidth = ChartCanvas.Width;
            double canvasHeight = ChartCanvas.Height;

            if (double.IsNaN(canvasWidth) || canvasWidth == 0) canvasWidth = 360;
            if (double.IsNaN(canvasHeight) || canvasHeight == 0) canvasHeight = 168;

            double padding = 20;
            double chartHeight = canvasHeight - (2 * padding);

            double maxLoss = orderedRecords.Max(r => r.Result);
            double minLoss = orderedRecords.Min(r => r.Result);
            double lossRange = maxLoss - minLoss;

            if (lossRange == 0)
            {
                lossRange = maxLoss == 0 ? 1.0 : maxLoss * 0.2;
                minLoss = minLoss - (lossRange / 2);
            }

            double xStep = canvasWidth / (orderedRecords.Count - 1);

            Polyline line = new Polyline
            {
                StrokeThickness = 2,
                Stroke = Brushes.LightGray
            };

            for (int i = 0; i < orderedRecords.Count; i++)
            {
                double x = i * xStep;

                double normalizedLoss = (orderedRecords[i].Result - minLoss) / lossRange;
                double y = canvasHeight - padding - (normalizedLoss * chartHeight);

                if (y < padding) y = padding;
                if (y > canvasHeight - padding) y = canvasHeight - padding;

                Point point = new Point(x, y);
                line.Points.Add(point);

                Ellipse dot = new Ellipse
                {
                    Width = 8,
                    Height = 8,
                    Fill = orderedRecords[i].Strategy.Contains("MSE") ? Brushes.Blue :
                           orderedRecords[i].Strategy.Contains("MAE") ? Brushes.Red : Brushes.Green,
                    Margin = new Thickness(x - 4, y - 4, 0, 0)
                };

                dot.ToolTip = $"{orderedRecords[i].Strategy}\nHiba: {orderedRecords[i].Result:F6}";
                ChartCanvas.Children.Add(dot);
            }

            ChartCanvas.Children.Insert(0, line);
        }
    }
}

            