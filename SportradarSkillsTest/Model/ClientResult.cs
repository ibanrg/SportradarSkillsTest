namespace SportradarSkillsTest.Domain
{
    public class ClientResult
    {
        public string Client { get; set; }
        public double NetProfit { get; set; }

        public override string ToString()
        {
            return $"Client: {Client} - NetProfit: {NetProfit}";
        }
    }
}
