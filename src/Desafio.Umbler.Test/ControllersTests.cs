using Desafio.Umbler.Controllers;
using Desafio.Umbler.Data;
using Desafio.Umbler.Data.Repository;
using Desafio.Umbler.Models;
using Desafio.Umbler.Service.DTOs;
using Desafio.Umbler.Service.Entities;
using DnsClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading;
using Whois.NET;

namespace Desafio.Umbler.Test
{
    [TestClass]
    public class ControllersTest
    {
        private readonly string _domainName = "teste.com";

        [TestMethod]
        public void Home_Index_returns_View()
        {
            //arrange 
            var controller = new HomeController();

            //act
            var response = controller.Index();
            var result = response as ViewResult;

            //assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Home_Error_returns_View_With_Model()
        {
            //arrange 
            var controller = new HomeController();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            //act
            var response = controller.Error();
            var result = response as ViewResult;
            var model = result.Model as ErrorViewModel;

            //assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
        }

        [TestMethod]
        public void Domain_In_Database()
        {
            //arrange 
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Find_searches_url")
                .Options;

            var lookupClient = new Mock<ILookupClient>();

            var dnsResponse = new Mock<IDnsQueryResponse>();
            lookupClient.Setup(l => l.QueryAsync(_domainName, QueryType.A, QueryClass.IN, CancellationToken.None)).ReturnsAsync(dnsResponse.Object);

            var domain = new Domain(_domainName);
            domain.SetWhoIs("Ns.umbler.com");
            domain.SetARecord("192.168.0.1", "umbler.corp", 60);

            var whoisClient = new Mock<IWhoisClient>();
            var whoisresponse = new WhoisResponse();
            whoisClient.Setup(l => l.QueryAsync(_domainName, CancellationToken.None)).ReturnsAsync(whoisresponse);

            // Insert seed data into the database using one instance of the context
            using (var db = new DatabaseContext(options))
            {
                db.Domains.Add(domain);
                db.SaveChanges();
            }

            // Use a clean instance of the context to run the test
            using (var db = new DatabaseContext(options))
            {
                var repDomain = new DomainRepository(db);
                var controller = new DomainController(repDomain, lookupClient.Object, whoisClient.Object);

                //act
                var response = controller.Get(_domainName);
                var result = response.Result as OkObjectResult;
                var obj = result.Value as DomainDTO;
                Assert.AreEqual(obj.Name, domain.Name);
            }
        }

        [TestMethod]
        public void Domain_Not_In_Database()
        {
            //arrange 
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Find_searches_url")
                .Options;

            var lookupClient = new Mock<ILookupClient>();
            var _domainName = "test.com";

            var dnsResponse = new Mock<IDnsQueryResponse>();
            lookupClient.Setup(l => l.QueryAsync(_domainName, QueryType.A, QueryClass.IN, CancellationToken.None)).ReturnsAsync(dnsResponse.Object);

            var whoisClient = new Mock<IWhoisClient>();
            var whoisresponse = new WhoisResponse();
            whoisClient.Setup(l => l.QueryAsync(_domainName, CancellationToken.None)).ReturnsAsync(whoisresponse);

            // Use a clean instance of the context to run the test
            using (var db = new DatabaseContext(options))
            {
                var repDomain = new DomainRepository(db);
                var controller = new DomainController(repDomain, lookupClient.Object, whoisClient.Object);

                //act
                var response = controller.Get(_domainName);
                var result = response.Result as BadRequestObjectResult;
                var obj = result.Value as string;
                Assert.IsNotNull(obj);
            }
        }

        [TestMethod]
        public void Domain_Moking_LookupClient()
        {
            //arrange 
            var lookupClient = new Mock<ILookupClient>();
            var _domainName = "test.com";

            var dnsResponse = new Mock<IDnsQueryResponse>();
            lookupClient.Setup(l => l.QueryAsync(_domainName, QueryType.A, QueryClass.IN, CancellationToken.None)).ReturnsAsync(dnsResponse.Object);

            var whoisClient = new Mock<IWhoisClient>();
            var whoisresponse = new WhoisResponse();
            whoisClient.Setup(l => l.QueryAsync(_domainName, CancellationToken.None)).ReturnsAsync(whoisresponse);

            //arrange 
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Find_searches_url")
                .Options;

            // Use a clean instance of the context to run the test
            using (var db = new DatabaseContext(options))
            {
                //inject lookupClient in controller constructor
                var repDomain = new DomainRepository(db);
                var controller = new DomainController(repDomain, lookupClient.Object, whoisClient.Object);

                //act
                var response = controller.Get(_domainName);
                var result = response.Result as OkObjectResult;
                var obj = result.Value as DomainDTO;
                Assert.IsNotNull(obj);
            }
        }

        [TestMethod]
        public void Domain_Moking_WhoisClient()
        {
            //arrange
            //whois is a static class, we need to create a class to "wrapper" in a mockable version of WhoisClient
            var whoisClient = new Mock<IWhoisClient>();
            var lookupClient = new Mock<ILookupClient>();
            var _domainName = "test.com";

            var dnsResponse = new Mock<IDnsQueryResponse>();
            lookupClient.Setup(l => l.QueryAsync(_domainName, QueryType.A, QueryClass.IN, CancellationToken.None)).ReturnsAsync(dnsResponse.Object);

            var whoisresponse = new WhoisResponse
            {
                Raw = It.IsAny<string>(),
                OrganizationName = It.IsAny<string>()
            };
            whoisClient.Setup(l => l.QueryAsync(_domainName, CancellationToken.None)).ReturnsAsync(whoisresponse);

            //arrange 
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "Find_searches_url")
                .Options;

            // Use a clean instance of the context to run the test
            using (var db = new DatabaseContext(options))
            {
                var repDomain = new DomainRepository(db);
                //inject IWhoisClient in controller's constructor
                var controller = new DomainController(repDomain, lookupClient.Object, whoisClient.Object);

                //act
                var response = controller.Get(_domainName);
                var result = response.Result as OkObjectResult;
                var obj = result.Value as DomainDTO;
                Assert.IsNotNull(obj);
            }
        }
    }
}