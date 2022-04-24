using Desafio.Umbler.Service.Entities;
using System.Threading.Tasks;

namespace Desafio.Umbler.Data.Repositories
{
    public interface IDomainRepository
    {
        void Add(Domain domain);
        void Update(Domain domain);
        Task SaveAsync();
    }
}
