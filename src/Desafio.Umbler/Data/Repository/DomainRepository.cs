using Desafio.Umbler.Service.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Desafio.Umbler.Data.Repository
{
    public class DomainRepository : IDomainRepository
    {
        private readonly DatabaseContext _db;
        public DomainRepository(DatabaseContext db)
        {
            _db = db;
        }

        public void Add(Domain domain)
        {
            _db.Domains.Add(domain);
        }

        public void Update(Domain domain)
        {
            _db.Domains.Update(domain);
        }
        public async Task<Domain> GetByNameAsync(string name)
        {
            return await _db.Domains.FirstOrDefaultAsync(d => d.Name.ToUpper() == name.ToUpper());
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
