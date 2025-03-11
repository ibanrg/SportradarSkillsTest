using SportradarSkillsTest.Domain;
using System.Collections.Concurrent;

namespace SportradarSkillsTest.Services
{
    public class BetProcessingService : IBetProcessingService
    {
        private readonly ConcurrentQueue<Bet> _incomingBets = new();
        private readonly ConcurrentBag<Bet> _processedBets = new();
        private readonly ConcurrentBag<BetResult> _betResults = new();
        private readonly List<Task> _workers = new();
        private List<Bet> _betsForReview = new();
        private int _totalBetsProcessed = 0;
        private double _totalBetAmount = 0;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public BetProcessingService(int workerCount = 5)
        {
            for (int i = 0; i < workerCount; i++)
            {
                _workers.Add(Task.Run(ProcessBets));
            }
        }

        public void AddBet(Bet bet)
        {
            _incomingBets.Enqueue(bet);
        }

        private async Task ProcessBets()
        {
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                while (_incomingBets.TryDequeue(out var bet))
                {
                    await _semaphore.WaitAsync();
                    await Task.Delay(50);
                    lock (this)
                    {
                        var isStatusSequenceOk = CheckStatus(bet);
                        _totalBetsProcessed++;
                        _processedBets.Add(bet);
                        _semaphore.Release();

                        if (isStatusSequenceOk)
                        {
                            if (bet.Status == BetStatus.OPEN)
                            {
                                _totalBetAmount += bet.Amount;
                            }

                            if (bet.Status == BetStatus.WINNER || bet.Status == BetStatus.LOSER)
                            {
                                _betResults.Add(CalculateResult(bet));
                            }
                        }
                        else
                        {
                            _betsForReview.Add(bet);
                        }
                    }
                }
            }
        }

        // Check if status sequence is correct
        private bool CheckStatus(Bet bet)
        {
            return !(bet.Status == BetStatus.OPEN && _processedBets.Any(x => x.Id == bet.Id) ||
                     (bet.Status != BetStatus.OPEN && (!_processedBets.Any(x => x.Id == bet.Id) ||
                     _processedBets.Any(x => x.Id == bet.Id && x.Status != BetStatus.OPEN))));
        }

        // Calculates bet result
        // Void status doesn´t count because I´m not deducting from the client when the bet is in OPEN status.
        private BetResult CalculateResult(Bet bet)
        {
            return new BetResult
            {
                Id = bet.Id,
                Client = bet.Client,
                Result = bet.Status == BetStatus.WINNER ? bet.Amount * bet.Odds : -bet.Amount
            };
        }

        private double GetTotalAmount()
        {
            return _totalBetAmount;
        }

        private double GetTotalProfitOrLoss()
        {
            return _betResults.Sum(b => b.Result);
        }

        private List<ClientResult> GetTopFiveClientsProfit()
        {
            return GetClientsResult()
                .OrderByDescending(g => g.NetProfit)
                .Take(5)
                .ToList();
        }

        private List<ClientResult> GetTopFiveClientsLoss()
        {
            return GetClientsResult()
                .OrderBy(g => g.NetProfit)
                .Take(5)
                .ToList();
        }

        private IEnumerable<ClientResult> GetClientsResult()
        {
            return _betResults
                .GroupBy(b => b.Client)
                .Select(g => new ClientResult
                {
                    Client = g.Key,
                    NetProfit = g.Sum(x => x.Result)
                });
        }

        public Summary GetSummary()
        {
            return new Summary
            {
                TotalNumberOfBets = _totalBetsProcessed,
                TotalAmountBet = GetTotalAmount(),
                TotalProfitOrLoss = GetTotalProfitOrLoss(),
                TopFiveClientsByProfit = GetTopFiveClientsProfit(),
                TopFiveClientsByLoss = GetTopFiveClientsLoss()
            };
        }

        public List<Bet> GetBetsToReview()
        {
            return _betsForReview;
        }

        public void Shutdown()
        {
            _cancellationTokenSource.Cancel();
            Task.WaitAll(_workers.ToArray());
            Console.WriteLine(GetSummary().ToString());
        }
    }
}
