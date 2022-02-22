public readonly struct ScoreboardData
{
    public readonly string Nickname;
    public readonly string MovesCount;
    public readonly string Status;


    public ScoreboardData(string nickname, string movesCount, string status)
    {
        Nickname = nickname;
        MovesCount = movesCount;
        Status = status;
    }
}