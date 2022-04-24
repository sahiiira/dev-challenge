using System;
using System.ComponentModel.DataAnnotations;

namespace Desafio.Umbler.Service.Entities
{
    public class Domain
    {
        const string ErrorNameNullOrEmpty = "O nome do domínio não deve ser nulo ou vazio.";

        protected Domain() { }
        public Domain(string name, string ip, string whoIs, string hostedAt, int ttl)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception(ErrorNameNullOrEmpty);

            Name = name;
            Ip = ip;
            UpdatedAt = DateTime.Now;
            WhoIs = whoIs;
            Ttl = ttl;
            HostedAt = hostedAt;
        }

        [Key]
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Ip { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public string WhoIs { get; private set; }
        public int Ttl { get; private set; }
        public string HostedAt { get; private set; }

        public Domain Update(string name, string ip, string whoIs, string hostedAt, int ttl)
        {
            if (string.IsNullOrEmpty(name)) throw new Exception(ErrorNameNullOrEmpty);

            Name = name;
            Ip = ip;
            UpdatedAt = DateTime.Now;
            WhoIs = whoIs;
            Ttl = ttl;
            HostedAt = hostedAt;

            return this;
        }
    }
}
