using NeuralNet_WPF_UI.Interfaces;
using NeuralNet_WPF_UI.Patterns;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace NeuralNet_WPF_UI
{
    public partial class Parameters : Window
    {
        public int Epochs { get; private set; }
        public double LearningRate { get; private set; }
        public int InputNeurons { get; private set; }
        public int HiddenNeurons { get; private set; }
        public int OutputNeurons { get; private set; }

        public Parameters()
        {
            InitializeComponent(); 
        }

        public ILossStrategy ChosenStrategy { get; private set; }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(EpochsBox.Text, out int epochs) || epochs <= 0) epochs = 10;
            if (!double.TryParse(LrBox.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double lr)) lr = 0.01;
            if (!int.TryParse(InputNeuronsBox.Text, out int inputNeurons) || inputNeurons <= 0) inputNeurons = 8;
            if (!int.TryParse(HiddenNeuronsBox.Text, out int hiddenNeurons) || hiddenNeurons <= 0) hiddenNeurons = 16;
            if (!int.TryParse(OutputNeuronsBox.Text, out int outputNeurons) || outputNeurons <= 0) outputNeurons = 1;

            if (StrategyCombo.SelectedIndex == 1)
                ChosenStrategy = new MseLoss();
            else if (StrategyCombo.SelectedIndex == 2)
                ChosenStrategy = new MaeLoss();
            else
                ChosenStrategy = new BceLoss(); 

            Epochs = epochs;
            LearningRate = lr;
            InputNeurons = inputNeurons;
            HiddenNeurons = hiddenNeurons;
            OutputNeurons = outputNeurons;

            this.DialogResult = true;
            this.Close();
        }
    }
}

