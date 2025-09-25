using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometriaModels.Models
{
    public class RectanguloModel:FiguraModel
    {
        public double? Ancho { get; set; }
        public double? Largo { get; set; }


        public RectanguloModel():base()
        {

        }
        public RectanguloModel(int id, double ancho, double largo, double area ):base(id) 
        {
            this.Ancho = ancho;
            this.Largo = largo;
        }

    }
}
