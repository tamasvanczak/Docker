using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNet_WPF_UI.Models
{
    public abstract class Layer
    {
        public string Name { get; set; }
        public abstract void Forward();
        public void PrintInfo() => Console.WriteLine($"Réteg: {Name} [{this.GetType().Name}]");
    }
}
