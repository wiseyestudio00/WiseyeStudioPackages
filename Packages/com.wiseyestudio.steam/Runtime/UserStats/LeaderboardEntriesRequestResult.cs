using System.Collections.Generic;

namespace WiseyeStudio.Steam.UserStats
{
    public class LeaderboardEntriesRequestResult
    {
        public readonly bool Success;
        public readonly List<LeaderboardEntry> Entries;
        public readonly string ErrorMessage;

        private LeaderboardEntriesRequestResult(List<LeaderboardEntry> entries)
        {
            Success = true;
            Entries = entries;
            ErrorMessage = string.Empty;
        }

        private LeaderboardEntriesRequestResult(string errorMessage)
        {
            Success = false;
            Entries = new List<LeaderboardEntry>();
            ErrorMessage = errorMessage;
        }

        public static LeaderboardEntriesRequestResult CreateSuccess(List<LeaderboardEntry> entries)
        {
            return new LeaderboardEntriesRequestResult(entries);
        }

        public static LeaderboardEntriesRequestResult CreateFailure(string errorMessage)
        {
            return new LeaderboardEntriesRequestResult(errorMessage);
        }
    }
}