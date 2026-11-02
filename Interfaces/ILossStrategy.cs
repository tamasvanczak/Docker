using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNet_WPF_UI.Interfaces
{
    public interface ILossStrategy
    {
        double CalculateLoss(double target, double output);
        string GetStrategyName();
    }
}
