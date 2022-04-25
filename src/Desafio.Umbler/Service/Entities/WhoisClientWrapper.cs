using System.Threading;
using System.Threading.Tasks;
using Whois.NET;

namespace Desafio.Umbler.Service.Entities
{
    public class WhoisClientWrapper : IWhoisClient
    {
        public async Task<WhoisResponse> QueryAsync(string query, CancellationToken cancellationToken = default)
        {
            return await WhoisClient.QueryAsync(query, token: cancellationToken);
        }
    }
}
