using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private GameBot bot;

    [SerializeField] private Toggle vsBotToggle;
    [SerializeField] private Slider depthSlider;
    [SerializeField] private Toggle botPlaysBlackToggle;

    private void OnEnable()
    {
        if (vsBotToggle != null) vsBotToggle.isOn = GameSettings.VsBot;

        if (depthSlider != null)
        {
            depthSlider.minValue = 3;
            depthSlider.maxValue = 7;
            depthSlider.wholeNumbers = true;
            depthSlider.value = GameSettings.Depth;
        }

        if (botPlaysBlackToggle != null) botPlaysBlackToggle.isOn = GameSettings.BotPlaysBlack;

        ApplyToBot();
    }

    public void OnAnySettingChanged()
    {
        Debug.Log("Before: toggle=" + vsBotToggle.isOn + " prefs=" + GameSettings.VsBot);

        if (vsBotToggle != null) GameSettings.VsBot = vsBotToggle.isOn;

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
}
