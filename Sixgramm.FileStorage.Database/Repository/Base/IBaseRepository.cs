using System;
using System.Threading.Tasks;
using Sixgramm.FileStorage.Common.Base;

namespace Sixgramm.FileStorage.Database.Repository.Base
{
    public interface IBaseRepository<TModel>
        where TModel : BaseModel
    {
        Task<TModel> Create(TModel data);
        Task<TModel> GetById(Guid id);
        TModel GetOne(Func<TModel, bool> predicate);
        Task<TModel> Delete(Guid id);
    }
}