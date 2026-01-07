using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private GameBot bot;

    [SerializeField] private Toggle vsBotToggle;
    [SerializeField] private TMP_Dropdown difficultyDropdown;
    [SerializeField] private Toggle botPlaysBlackToggle;


    private int[] difficultyDepth = {3, 5, 7};



    public void OnEnable()
    {

        if (vsBotToggle != null) vsBotToggle.isOn = GameSettings.VsBot;

        if (difficultyDropdown != null)
            difficultyDropdown.value = DropdownIndexFromDepth(GameSettings.Depth);
            

        if (botPlaysBlackToggle != null) botPlaysBlackToggle.isOn = GameSettings.BotPlaysBlack;

        ApplyToBot();
    }

    public void OnAnySettingChanged()
    {
        Debug.Log("Before: toggle=" + vsBotToggle.isOn + " prefs=" + GameSettings.VsBot);

        if (vsBotToggle != null) GameSettings.VsBot = vsBotToggle.isOn;

         if (difficultyDropdown != null)
            GameSettings.Depth = difficultyDepth[difficultyDropdown.value];

        Debug.Log("After:  toggle=" + vsBotToggle.isOn + " prefs=" + GameSettings.VsBot);

        ApplyToBot();
    }


    private void ApplyToBot()
    {
        if (bot == null) return;

        bot.mode = GameSettings.VsBot ? GameBot.GameMode.VsBot : GameBot.GameMode.TwoPlayer;
        bot.searchDepth = GameSettings.Depth;

        bot.botPlaysBlack = GameSettings.BotPlaysBlack;
        bot.botPlaysWhite = !GameSettings.BotPlaysBlack;
    }


        private int DropdownIndexFromDepth(int depth)
    {
        for (int i = 0; i < difficultyDepth.Length; i++)
            if (difficultyDepth[i] == depth) return i;

        return 0;
    }
}
