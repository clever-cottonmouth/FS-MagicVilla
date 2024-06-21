using MagicVilla_VillaAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
    public interface IVillaRepository
    {
        Task<List<Villa>> GetAllAsync(Expression<Func<Villa, bool>> filter = null);
        Task<Villa> GetAsync(Expression<Func<Villa, bool>> filter = null, bool tracked=true);
        Task CreateAsync(Villa entity);
        Task UpdateAsync(Villa entity);
        Task RemoveAsync(Villa entity);
        Task SaveAsync();
    }
}
