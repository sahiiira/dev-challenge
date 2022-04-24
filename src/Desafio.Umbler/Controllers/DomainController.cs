﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Whois.NET;
using DnsClient;
using Desafio.Umbler.Service.Entities;
using Desafio.Umbler.Service.DTOs;
using Desafio.Umbler.Data.Repository;

namespace Desafio.Umbler.Controllers
{
    [Route("api/[controller]")]
    public class DomainController : Controller
    {
        private readonly IDomainRepository _repDomain;
        private readonly ILookupClient _lookupClient;

        public DomainController(IDomainRepository repDomain, 
                                ILookupClient lookupClient)
        {
            _repDomain = repDomain;
            _lookupClient = lookupClient;
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

                    var result = await _lookupClient.QueryAsync(domainName, QueryType.A);
                    var record = result.Answers.ARecords().FirstOrDefault();
                    var address = record?.Address;
                    var ip = address?.ToString();

                    domain = new Domain(domainName, ip, response.Raw, response.OrganizationName, record?.TimeToLive ?? 0);

                    _repDomain.Add(domain);
                }

                if (DateTime.Now.Subtract(domain.UpdatedAt).TotalMinutes > domain.Ttl)
                {
                    var response = await WhoisClient.QueryAsync(domainName);

                    var result = await _lookupClient.QueryAsync(domainName, QueryType.A);
                    var record = result.Answers.ARecords().FirstOrDefault();
                    var address = record?.Address;
                    var ip = address?.ToString();

                    domain.Update(domainName, ip, response.Raw, response.OrganizationName, record?.TimeToLive ?? 0);
                }

                await _repDomain.SaveAsync();

                return Ok(ToDomainDTO(domain));
            }
            catch (Exception e)
            {
                return BadRequest("Não foi possível obter as informações do domínio: " + e.Message);
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
