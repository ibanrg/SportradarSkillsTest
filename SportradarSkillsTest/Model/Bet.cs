using System.Text.Json.Serialization;

namespace SportradarSkillsTest.Domain
{
    public class Bet
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public double Odds { get; set; }
        public string Client { get; set; }
        public string Event { get; set; }
        public string Market { get; set; }
        public string Selection { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BetStatus Status { get; set; }
    }
}
