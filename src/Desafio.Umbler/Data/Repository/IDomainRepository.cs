using Desafio.Umbler.Service.Entities;
using System.Threading.Tasks;

namespace Desafio.Umbler.Data.Repository
{
    public interface IDomainRepository
    {
        void Add(Domain domain);
        void Update(Domain domain);
        Task<Domain> GetByNameAsync(string domainName);
        Task SaveAsync();
    }
}
