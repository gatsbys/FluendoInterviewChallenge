namespace PUBG.Stats.Core.Services.Data.Documents.Leaderboard
{
    public class LeaderboardStats
    {
        public int RankPoints { get; set; }

        public int Wins { get; set; }

        public int Games { get; set; }

        public double WinRatio { get; set; }

        public int AverageDamage { get; set; }

        public int Kills { get; set; }

        public double KillDeathRatio { get; set; }

        public double AverageRank { get; set; }
    }
}
