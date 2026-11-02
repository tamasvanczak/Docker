using NeuralNet_WPF_UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNet_WPF_UI.Patterns
{
    public class MaeLoss : ILossStrategy
    {
        public double CalculateLoss(double target, double output)
        {
            return Math.Abs(target - output);
        }

        public string GetStrategyName()
        {
            return "Átlagos Abszulót Hiba (MAE)";
        }
    }
}
