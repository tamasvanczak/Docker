using NeuralNet_WPF_UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNet_WPF_UI.Patterns
{
    public class ConsoleLogger : ITrainingObserver
    {
        public void OnEpochEnd(int epoch, double loss)
        {
            Console.WriteLine($">>> [Megfigyelő] {epoch}. kör: A hiba jelenleg {loss:F4}");
        }
    }
}
