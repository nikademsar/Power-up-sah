using UnityEngine;

public static class GameSettings
{
    private const string K_VS_BOT = "settings_vsBot";
    private const string K_DEPTH  = "settings_depth";
    private const string K_BOT_BLACK = "settings_botPlaysBlack";

    public static bool VsBot
    {
        get => PlayerPrefs.GetInt(K_VS_BOT, 0) == 1;
        set { PlayerPrefs.SetInt(K_VS_BOT, value ? 1 : 0); PlayerPrefs.Save(); }
    }

    public static int Depth
    {
        get => Mathf.Clamp(PlayerPrefs.GetInt(K_DEPTH, 2), 1, 5);
        set { PlayerPrefs.SetInt(K_DEPTH, Mathf.Clamp(value, 1, 5)); PlayerPrefs.Save(); }
    }

    public static bool BotPlaysBlack
    {
        get => PlayerPrefs.GetInt(K_BOT_BLACK, 1) == 1;
        set { PlayerPrefs.SetInt(K_BOT_BLACK, value ? 1 : 0); PlayerPrefs.Save(); }
    }
}
