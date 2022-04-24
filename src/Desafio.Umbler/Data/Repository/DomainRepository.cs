using Desafio.Umbler.Service.Entities;

namespace Desafio.Umbler.Data.Repository
{
    public class DomainRepository : IDomainRepository
    {
        private readonly DatabaseContext _db;
        public DomainRepository(DatabaseContext db)
        {
            _db = db;
        }

        void IDomainRepository.Add(Domain domain)
        {
            _db.Domains.Add(domain);
        }
    }
}
