using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NeuralNet_WPF_UI.Models
{
    public class DenseLayer : Layer
    {
        public override void Forward()
        {
            Console.WriteLine($" -> {Name}: Súlyok és bemenetek szorzása...");
        }
    }
}
