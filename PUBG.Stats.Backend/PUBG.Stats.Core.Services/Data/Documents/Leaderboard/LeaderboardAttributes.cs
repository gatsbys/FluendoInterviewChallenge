namespace PUBG.Stats.Core.Services.Data.Documents.Leaderboard
{
    public class LeaderboardAttributes
    {
        public string Name { get; set; }

        public int Rank { get; set; }

        public LeaderboardStats Stats { get; set; }
    }
}
