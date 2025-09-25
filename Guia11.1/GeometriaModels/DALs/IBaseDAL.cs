using GeometriaModels.DALs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometriaModels.DALs
{
    public interface IBaseDAL <E,K,T>
    {
        Task<List<E>> GetAll(ITransactionDAL<T>? transaction = null);
        Task<E?> GetByKey(K id, ITransactionDAL<T>? transaction = null);
        Task<bool> Save(E actualizar, ITransactionDAL<T>? transaction = null);
        Task<bool> Remove(K id, ITransactionDAL<T>? transaction = null);
    }
}
