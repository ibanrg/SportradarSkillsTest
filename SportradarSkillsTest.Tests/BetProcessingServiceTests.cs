using SportradarSkillsTest.Domain;
using SportradarSkillsTest.Services;

namespace SportradarSkillsTest.Tests
{
    [TestFixture]
    public class BetProcessingServiceTests
    {
        private BetProcessingService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new BetProcessingService(1);
        }

        [Test]
        public void AddBet_ShouldAddBetToQueue()
        {
            var bet = new Bet
            {
                Id = 1,
                Amount = 100,
                Odds = 2.0,
                Client = "Client1",
                Status = BetStatus.OPEN
            };

            _service.AddBet(bet);

            Task.Delay(1000).Wait();

            var summary = _service.GetSummary();
            Assert.IsTrue(summary.TotalNumberOfBets == 1);
        }

        [Test]
        public async Task ProcessBets_ShouldProcessBetAndCalculateProfit()
        {
            var bet1 = new Bet
            {
                Id = 1,
                Amount = 100,
                Odds = 2.0,
                Client = "Client1",
                Status = BetStatus.OPEN
            };
            var bet2 = new Bet
            {
                Id = 1,
                Amount = 100,
                Odds = 2.0,
                Client = "Client1",
                Status = BetStatus.WINNER
            };

            _service.AddBet(bet1);
            _service.AddBet(bet2);

            await Task.Delay(1000);

            var summary = _service.GetSummary();
            Assert.IsTrue(summary.TotalNumberOfBets == 2);
            Assert.IsTrue(summary.TotalProfitOrLoss == 200);
        }

        [Test]
        public async Task ProcessWinnerBetWithoutPreviousOpenBet_ShouldProcessBetAndAddToReview()
        {
            var bet1 = new Bet
            {
                Id = 1,
                Amount = 100,
                Odds = 2.0,
                Client = "Client1",
                Status = BetStatus.WINNER
            };

            _service.AddBet(bet1);

            await Task.Delay(1000);

            var toReview = _service.GetBetsToReview();
            Assert.IsTrue(toReview.Count == 1);
        }

        [Test]
        public async Task ProcessOpenBetWithPreviousOpenBet_ShouldProcessBetAndAddToReview()
        {
            var bet1 = new Bet
            {
                Id = 1,
                Amount = 100,
                Odds = 2.0,
                Client = "Client1",
                Status = BetStatus.OPEN
            };

            var bet2 = new Bet
            {
                Id = 1,
                Amount = 100,
                Odds = 2.0,
                Client = "Client1",
                Status = BetStatus.OPEN
            };

            _service.AddBet(bet1);
            _service.AddBet(bet2);

            await Task.Delay(1000);

            var toReview = _service.GetBetsToReview();
            Assert.IsTrue(toReview.Count == 1);
        }

        [Test]
        public void Shutdown_ShouldCancelProcessing()
        {
            _service.Shutdown();
            var summary = _service.GetSummary();
            Assert.IsTrue(summary.TotalNumberOfBets == 0);
        }

        [Test]
        public void CalculateNetProfit_ShouldReturnCorrectProfit()
        {
            var bet1 = new Bet
            {
                Id = 1,
                Amount = 100,
                Odds = 2.0,
                Status = BetStatus.OPEN
            };
            var bet2 = new Bet
            {
                Id = 1,
                Amount = 100,
                Odds = 2.0,
                Status = BetStatus.WINNER
            };
            var bet3 = new Bet
            {
                Id = 2,
                Amount = 50,
                Status = BetStatus.OPEN
            };
            var bet4 = new Bet
            {
                Id = 2,
                Amount = 50,
                Status = BetStatus.LOSER
            };

            _service.AddBet(bet1);
            _service.AddBet(bet2);
            _service.AddBet(bet3);
            _service.AddBet(bet4);

            Task.Delay(1000).Wait();

            var summary = _service.GetSummary();
            Assert.IsTrue(summary.ToString().Contains("Profit/Loss: 150"));
        }
    }
}