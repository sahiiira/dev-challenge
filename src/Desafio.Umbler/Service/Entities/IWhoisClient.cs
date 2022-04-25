using System.Threading;
using System.Threading.Tasks;
using Whois.NET;

namespace Desafio.Umbler.Service.Entities
{
    public interface IWhoisClient
    {
        Task<WhoisResponse> QueryAsync(string query, CancellationToken cancellationToken = default);
    }
}
