using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Tubumu.Modules.Admin.Entities;
using Tubumu.Modules.Admin.Models.InputModels;
using Tubumu.Modules.Framework.Extensions.Object;
using XM = Tubumu.Modules.Admin.Models;

namespace Tubumu.Modules.Admin.Repositories
{
    public interface IBulletinRepository
    {
        Task<XM.Bulletin> GetItemAsync();
        Task<bool> SaveAsync(BulletinInput bulletin, ModelStateDictionary modelState);
    }

    public class BulletinRepository : IBulletinRepository
    {
        private readonly TubumuContext _tubumuContext;

        public BulletinRepository(TubumuContext tubumuContext)
        {
            _tubumuContext = tubumuContext;
        }

        public async Task<XM.Bulletin> GetItemAsync()
        {
            var item = await _tubumuContext.Bulletin.AsNoTracking().FirstOrDefaultAsync();
            return item.MapTo<XM.Bulletin>();
        }

        public async Task<bool> SaveAsync(BulletinInput bulletin, ModelStateDictionary modelState)
        {
            var dbBulletin = await _tubumuContext.Bulletin.FirstOrDefaultAsync();
            if (dbBulletin == null) return false;

            dbBulletin.UpdateFrom(bulletin);
            await _tubumuContext.SaveChangesAsync();

            return true;
        }

    }
}
