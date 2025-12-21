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
            depthSlider.minValue = 1;
            depthSlider.maxValue = 3;
            depthSlider.wholeNumbers = true;
            depthSlider.value = GameSettings.Depth;
        }

        if (botPlaysBlackToggle != null) botPlaysBlackToggle.isOn = GameSettings.BotPlaysBlack;

        ApplyToBot();
    }

    public void OnAnySettingChanged()
    {
        if (vsBotToggle != null) GameSettings.VsBot = vsBotToggle.isOn;
        if (depthSlider != null) GameSettings.Depth = Mathf.RoundToInt(depthSlider.value);
        if (botPlaysBlackToggle != null) GameSettings.BotPlaysBlack = botPlaysBlackToggle.isOn;

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
