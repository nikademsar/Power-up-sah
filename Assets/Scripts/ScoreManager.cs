using UnityEngine;

public static class ScoreManager
{
    private const string K_WHITE_WINS = "score_whiteWins";
    private const string K_BLACK_WINS = "score_blackWins";

    public static int WhiteWins => PlayerPrefs.GetInt(K_WHITE_WINS, 0);
    public static int BlackWins => PlayerPrefs.GetInt(K_BLACK_WINS, 0);

    public static void AddWinFor(string winner) // "white" ali "black"
    {
        if (winner == "white")
            PlayerPrefs.SetInt(K_WHITE_WINS, WhiteWins + 1);
        else if (winner == "black")
            PlayerPrefs.SetInt(K_BLACK_WINS, BlackWins + 1);

        PlayerPrefs.Save();
    }

    public static void ResetScore()
    {
        PlayerPrefs.DeleteKey(K_WHITE_WINS);
        PlayerPrefs.DeleteKey(K_BLACK_WINS);
        PlayerPrefs.Save();
    }
}
