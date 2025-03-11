using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace SportradarSkillsTest.Domain
{
    public class Summary
    {
        public int TotalNumberOfBets { get; set; }
        public double TotalAmountBet { get; set; }
        public double TotalProfitOrLoss { get; set; }
        public List<ClientResult> TopFiveClientsByProfit { get; set; }
        public List<ClientResult> TopFiveClientsByLoss { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new();

            sb.AppendLine($"Total Bets Processed: {TotalNumberOfBets}");
            sb.AppendLine($"TotalAmountBet: {TotalAmountBet}");
            sb.AppendLine($"Profit/Loss: {TotalProfitOrLoss}");

            sb.AppendLine($"TopFiveClientsByProfit:");
            foreach(var client in TopFiveClientsByProfit)
            {
                sb.AppendLine(client.ToString());
            }

            sb.AppendLine($"TopFiveClientsByLoss:");
            foreach (var client in TopFiveClientsByLoss)
            {
                sb.AppendLine(client.ToString());
            }

            return sb.ToString();
        }
    }
}
