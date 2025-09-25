using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeometriaModels.DALs.Utilities
{
    public interface ITransactionDAL<T> : IDisposable
    {

        void Commit();
        void Rollback();


        Task CommitAsync();

        Task RollbackAsync();
        T GetInternalTransaction();
        Task BeginTransaction();


    }
}
