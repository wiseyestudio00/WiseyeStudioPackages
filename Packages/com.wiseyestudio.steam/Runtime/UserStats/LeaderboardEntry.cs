namespace WiseyeStudio.Steam.UserStats
{
    public class LeaderboardEntry
    {
        public readonly ulong SteamId;
        public readonly int GlobalRank;
        public readonly int Score;
        public readonly int[] Details;

        public LeaderboardEntry(ulong steamId, int globalRank, int score, int[] details)
        {
            SteamId = steamId;
            GlobalRank = globalRank;
            Score = score;
            Details = details;
        }
    }
}
