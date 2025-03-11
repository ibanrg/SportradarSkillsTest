using SportradarSkillsTest.Domain;

namespace SportradarSkillsTest.Services
{
    public interface IBetProcessingService
    {
        public void AddBet(Bet bet);
        public Summary GetSummary();
        public List<Bet> GetBetsToReview();
        public void Shutdown();
    }
}
