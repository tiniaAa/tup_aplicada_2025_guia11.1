using GeometriaModels.DALs.Utilities;
using GeometriaModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometriaModels.DALs
{
    public interface FigurasDal<T>:IBaseDAL<FiguraModel,int,T>
    {
        Task ProcesarFiguras(ITransactionDAL<T>? transaccion = null);
    }
}
