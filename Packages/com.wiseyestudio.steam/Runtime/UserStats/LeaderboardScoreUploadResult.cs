namespace WiseyeStudio.Steam.UserStats
{
    public class LeaderboardScoreUploadResult
    {
        public readonly bool Success;
        public readonly int Score;
        public readonly bool ScoreChanged;
        public readonly int NewGlobalRank;
        public readonly int PreviousGlobalRank;
        public readonly string ErrorMessage;

        private LeaderboardScoreUploadResult(
            bool success,
            int score,
            bool scoreChanged,
            int newGlobalRank,
            int previousGlobalRank)
        {
            Success = success;
            Score = score;
            ScoreChanged = scoreChanged;
            NewGlobalRank = newGlobalRank;
            PreviousGlobalRank = previousGlobalRank;
        }

        private LeaderboardScoreUploadResult(string errorMessage)
        {
            Success = false;
            ErrorMessage  = errorMessage;
        }

        internal static LeaderboardScoreUploadResult CreateSuccess(
            int score,
            bool scoreChanged,
            int newGlobalRank,
            int previousGlobalRank)
        {
            return new LeaderboardScoreUploadResult(
                true,
                score,
                scoreChanged,
                newGlobalRank,
                previousGlobalRank
            );
        }

        internal static LeaderboardScoreUploadResult CreateFail(string errorMessage)
        {
            return new LeaderboardScoreUploadResult(errorMessage);
        }
    }
}
