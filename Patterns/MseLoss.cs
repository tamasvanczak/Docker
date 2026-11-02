using NeuralNet_WPF_UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNet_WPF_UI.Patterns
{
    public class MseLoss : ILossStrategy
    {
        public double CalculateLoss(double target, double output)
        {
            return Math.Pow(target - output, 2);
        }

        public string GetStrategyName()
        {
            return "Átlagos Négyzetes Hiba (MSE)";
        }
    }
}
