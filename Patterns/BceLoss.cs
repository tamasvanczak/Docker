using NeuralNet_WPF_UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNet_WPF_UI.Patterns
{
    internal class BceLoss : ILossStrategy
    {
        public double CalculateLoss(double target, double output)
        {
            double epsilon = 1e-15;
            output = Math.Max(epsilon, Math.Min(1.0 - epsilon, output));

            return -(target * Math.Log(output) + (1.0 - target) * Math.Log(1.0 - output));
        }

        public string GetStrategyName()
        {
            return "BCE (Binary Cross-Entropy)";
        }
    }
}
