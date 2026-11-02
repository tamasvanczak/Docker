using NeuralNet_WPF_UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNet_WPF_UI.Patterns
{
    public static class LayerFactory
    {
        public static Layer CreateLayer(string type, string name)
        {
            return type.ToLower() switch
            {
                "dense" => new DenseLayer { Name = name },
                "dropout" => new DropoutLayer { Name = name },
                _ => throw new ArgumentException($"Ismeretlen rétegtípus: {type}")
            };
        }
    }
}
