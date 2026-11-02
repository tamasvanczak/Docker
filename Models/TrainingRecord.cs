using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNet_WPF_UI.Models
{
    public class TrainingRecord
    {
        public int id { get; set; }
        public string Timestamp { get; set; }
        public string Strategy { get; set; }
        public double Result { get; set; } 

        public double Accuracy { get; set; }     
        public double LearningRate { get; set; } 
    }
}
