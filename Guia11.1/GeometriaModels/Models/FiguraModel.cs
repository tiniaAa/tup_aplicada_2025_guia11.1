using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometriaModels.Models
{
   public abstract class FiguraModel
    {
        public int? Id { get; set; }
        public double? Area { get; set; }


        public FiguraModel():base(){}
        public FiguraModel(int id, double area) 
        {
            this.Id=id;
            this.Area=area;
        }

    }
}
