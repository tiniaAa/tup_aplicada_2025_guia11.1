using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometriaModels.Models
{
    public  class CirculoModel:FiguraModel
    {
        public double? Radio {  get; set; }
        
        public CirculoModel():base() { }
        public CirculoModel(int id, double area, double radio):base(id,area)
        {
            this.Radio = radio;
        }
    }
}
