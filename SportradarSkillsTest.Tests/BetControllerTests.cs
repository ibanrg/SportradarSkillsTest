using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using NSubstitute.Core.Arguments;
using SportradarSkillsTest.Controllers;
using SportradarSkillsTest.Domain;
using SportradarSkillsTest.Services;

namespace SportradarSkillsTest.Tests
{
    [TestFixture]
    public class BetControllerTests
    {
        private BetsController _controller;
        private IBetProcessingService _serviceMock;
        private IHostApplicationLifetime _appLifetimeMock;

        [SetUp]
        public void SetUp()
        {
            _serviceMock = Substitute.For<IBetProcessingService>();
            _appLifetimeMock = Substitute.For<IHostApplicationLifetime>();
            _controller = new BetsController(_serviceMock, _appLifetimeMock);
        }

        [Test]
        public void AddBet_ShouldReturnOk()
        {
            var bet = new Bet
            {
                Id = 1,
                Amount = 100,
                Odds = 2.0,
                Client = "Client1",
                Status = BetStatus.OPEN
            };

            var result = _controller.AddBet(bet);

            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual("Bet received", ((OkObjectResult)result).Value);
        }

        [Test]
        public void GetSummary_ShouldReturnSummary()
        {
            var summary = new Summary()
            {
                TotalNumberOfBets = 0,
                TotalAmountBet = 0,
                TotalProfitOrLoss = 0,
                TopFiveClientsByProfit = new List<ClientResult>(),
                TopFiveClientsByLoss = new List<ClientResult>()
            };

            _serviceMock.GetSummary().Returns(summary);

            var result = _controller.GetSummary();

            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(summary, ((OkObjectResult)result).Value);
        }

        [Test]
        public void ShutdownSystem_ShouldReturnShutDownMessage()
        {
            var result = _controller.ShutdownSystem();

            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual("The system will gracefully shut down after processing all pending bets.", ((OkObjectResult)result).Value);
        }

        [Test]
        public void GetBetsToReview_ShouldReturnBetsToReview()
        {
            var toReview = new List<Bet>()
            {
                new Bet
                {
                    Id = 1,
                    Amount = 100,
                    Odds = 2.0,
                    Client = "Client1",
                    Status = BetStatus.OPEN
                }
            };

            _serviceMock.GetBetsToReview().Returns(toReview);

            var result = _controller.GetBetsToReview();

            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual(toReview, ((OkObjectResult)result).Value);
        }

        [Test]
        public void Initialize_ShouldReturnAddedBets()
        {
            var result = _controller.Initialize();

            Assert.IsInstanceOf<OkObjectResult>(result);
            var bets = (List<Bet>)((OkObjectResult)result).Value;
            Assert.IsTrue(bets.Count == 100);
        }
    }
}