using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Whois.NET;
using DnsClient;
using Desafio.Umbler.Service.Entities;
using Desafio.Umbler.Service.DTOs;
using Desafio.Umbler.Data.Repository;
using DnsClient.Protocol;

namespace Desafio.Umbler.Controllers
{
    [Route("api/[controller]")]
    public class DomainController : Controller
    {
        private readonly IDomainRepository _repDomain;
        private readonly ILookupClient _lookupClient;
        private readonly IWhoisClient _whoisClient;

        public DomainController(IDomainRepository repDomain,
                                ILookupClient lookupClient,
                                IWhoisClient whoisClient)
        {
            _repDomain = repDomain;
            _lookupClient = lookupClient;
            _whoisClient = whoisClient;
        }

        [HttpGet, Route("{domainName}")]
        public async Task<IActionResult> Get(string domainName)
        {
            try
            {
                var domain = await _repDomain.GetByNameAsync(domainName);

                if (domain == null)
                {
                    domain = new Domain(domainName);

                    var (result, record) = await SearchForDomain(domainName);

                    domain.SetDomain(result, record);

                    _repDomain.Add(domain);
                }

                if (DateTime.Now.Subtract(domain.UpdatedAt).TotalMinutes > domain.Ttl)
                {
                    var (result, record) = await SearchForDomain(domainName);

                    domain.SetDomain(result, record);
                }

                await _repDomain.SaveAsync();

                return Ok(ToDomainDTO(domain));
            }
            catch (Exception e)
            {
                return BadRequest("Não foi possível obter as informações do domínio: " + e.Message);
            }
        }

        private async Task<(WhoisResponse response, ARecord record)> SearchForDomain(string domainName)
        {
            try
            {
                var response = await _whoisClient.QueryAsync(domainName);

                var result = await _lookupClient.QueryAsync(domainName, QueryType.A);
                var record = result.Answers.ARecords().FirstOrDefault();

                return (response, record);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static DomainDTO ToDomainDTO(Domain domain)
        {
            return new DomainDTO()
            {
                Name = domain.Name,
                Ip = domain.Ip,
                WhoIs = domain.WhoIs,
                HostedAt = domain.HostedAt
            };
        }
    }
}
