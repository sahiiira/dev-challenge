using DnsClient.Protocol;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Whois.NET;

namespace Desafio.Umbler.Service.Entities
{
    public class Domain
    {
        protected Domain() { }
        public Domain(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception("O nome do domínio não deve ser nulo ou vazio.");
            if (!IsNameValid(name)) throw new Exception("O nome do domínio deve ser válido.");

            Name = name;
            UpdatedAt = DateTime.Now;
        }

        [Key]
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Ip { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public string WhoIs { get; private set; }
        public int Ttl { get; private set; }
        public string HostedAt { get; private set; }

        public Domain SetDomain(WhoisResponse response, ARecord record)
        {
            var address = record?.Address;
            var ip = address?.ToString();

            SetARecord(ip, response.OrganizationName, record?.TimeToLive ?? 0);
            SetWhoIs(response.Raw);

            return this;
        }

        public Domain SetWhoIs(string whoIs)
        {
            if (string.IsNullOrEmpty(whoIs)) throw new Exception("WhoIs não pode ser nulo.");

            WhoIs = whoIs;
            UpdatedAt = DateTime.Now;

            return this;
        }

        public Domain SetARecord(string ip, string hostedAt, int ttl)
        {
            if (!IsIpValid(ip)) throw new Exception("Deve ser um IPv4 válido.");

            Ip = ip;
            Ttl = ttl;
            HostedAt = hostedAt;
            UpdatedAt = DateTime.Now;

            return this;
        }

        public bool IsNameValid(string name)
        {
            return new Regex(@"^((?!-))(xn--)?[a-z0-9][a-z0-9-_]{0,61}[a-z0-9]{0,1}\.(xn--)?([a-z0-9\-]{1,61}|[a-z0-9-]{1,30}\.[a-z]{2,})$").Match(name).Success;
        }

        public bool IsIpValid(string ip)
        {
            return new Regex(@"^(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$").Match(ip).Success;
        }
    }
}
