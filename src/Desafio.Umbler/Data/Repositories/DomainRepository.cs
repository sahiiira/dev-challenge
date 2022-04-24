using Desafio.Umbler.Models;
using Desafio.Umbler.Service.Entities;
using System.Threading.Tasks;

namespace Desafio.Umbler.Data.Repositories
{
    public class DomainRepository : IDomainRepository
    {
        private readonly DatabaseContext _context;
        public DomainRepository(DatabaseContext context)
        {
            _context = context;
        }

        void IDomainRepository.Add(Domain domain)
        {
            _context.Domains.Add(domain);
        }

        void IDomainRepository.Update(Domain domain)
        {
            _context.Domains.Update(domain);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
