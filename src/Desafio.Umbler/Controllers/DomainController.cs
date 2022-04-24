using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Whois.NET;
using Microsoft.EntityFrameworkCore;
using DnsClient;
using Desafio.Umbler.Service.Entities;
using Desafio.Umbler.Service.DTOs;
using Desafio.Umbler.Data;
using Desafio.Umbler.Data.Repository;

namespace Desafio.Umbler.Controllers
{
    [Route("api/[controller]")]
    public class DomainController : Controller
    {
        private readonly IDomainRepository _repDomain;
        private readonly DatabaseContext _db;

        public DomainController(DatabaseContext db, 
                                IDomainRepository repDomain)
        {
            _db = db;
            _repDomain = repDomain;
        }

        [HttpGet, Route("{domainName}")]
        public async Task<IActionResult> Get(string domainName)
        {
            try
            {
                var domain = await _repDomain.GetByNameAsync(domainName);

                if (domain == null)
                {
                    var response = await WhoisClient.QueryAsync(domainName);

                    var lookup = new LookupClient();
                    var result = await lookup.QueryAsync(domainName, QueryType.ANY);
                    var record = result.Answers.ARecords().FirstOrDefault();
                    var address = record?.Address;
                    var ip = address?.ToString();

                    var hostResponse = await WhoisClient.QueryAsync(ip);

                    domain = new Domain(domainName, ip, response.Raw, hostResponse.OrganizationName, record?.TimeToLive ?? 0);

                    _repDomain.Add(domain);
                }

                if (DateTime.Now.Subtract(domain.UpdatedAt).TotalMinutes > domain.Ttl)
                {
                    var response = await WhoisClient.QueryAsync(domainName);

                    var lookup = new LookupClient();
                    var result = await lookup.QueryAsync(domainName, QueryType.ANY);
                    var record = result.Answers.ARecords().FirstOrDefault();
                    var address = record?.Address;
                    var ip = address?.ToString();

                    var hostResponse = await WhoisClient.QueryAsync(ip);

                    domain.Update(domainName, ip, response.Raw, hostResponse.OrganizationName, record?.TimeToLive ?? 0);

                    _repDomain.Update(domain);
                }

                await _repDomain.SaveAsync();

                return Ok(ToDomainDTO(domain));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
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
