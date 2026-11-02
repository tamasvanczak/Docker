using NeuralNet_WPF_UI.Interfaces;
using NeuralNet_WPF_UI.Models;
using NeuralNet_WPF_UI.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NeuralNet_WPF_UI.Core
{
    public class NeuralNetwork
    {
        private List<ITrainingObserver> _observers = new List<ITrainingObserver>();
        private ILossStrategy _strategy; // Ide mentjük el
        private List<Layer> _layers = new List<Layer>();
        public NeuralNetwork(ILossStrategy strategy)
        {
            _strategy = strategy;
        }
        public void AddLayer(string type, string name)
        {
            _layers.Add(LayerFactory.CreateLayer(type, name));
        }
        public void Subscribe(ITrainingObserver observer)
        {
            _observers.Add(observer);
        }

        public void RunTraining(int epochs)
        {
            for (int i = 1; i <= epochs; i++)
            {
                System.Threading.Thread.Sleep(500);

                double simulatedOutput = 0.6 + (i * 0.033);
                if (simulatedOutput > 1.0) simulatedOutput = 1.0;

                double currentLoss = _strategy.CalculateLoss(1.0, simulatedOutput);

                foreach (var observer in _observers)
                {
                    observer.OnEpochEnd(i, currentLoss);
                }
            }
        }

        private void NotifyObservers(int epoch, double loss)
        {
            foreach (var observer in _observers)
            {
                observer.OnEpochEnd(epoch, loss);
            }
        }


    }
}
