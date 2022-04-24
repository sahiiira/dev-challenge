using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Desafio.Umbler.Models;
using Whois.NET;
using Microsoft.EntityFrameworkCore;
using DnsClient;
using Desafio.Umbler.Service.Entities;

namespace Desafio.Umbler.Controllers
{
    [Route("api/[controller]")]
    public class DomainController : Controller
    {
        private readonly DatabaseContext _db;

        public DomainController(DatabaseContext db)
        {
            _db = db;
        }

        [HttpGet, Route("{domainName}")]
        public async Task<IActionResult> Get(string domainName)
        {
            var domain = await _db.Domains.FirstOrDefaultAsync(d => d.Name == domainName);

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

                _db.Domains.Add(domain);
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
            }

            await _db.SaveChangesAsync();

            return Ok(domain);
        }
    }
}
