using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] private GameBot bot;

    [SerializeField] private Toggle vsBotToggle;
    [SerializeField] private GameObject difficultyDropdown;
    [SerializeField] private Toggle botPlaysBlackToggle;
    [SerializeField] private Text difficultyText;
    public static bool DifficultyChosen = false;




    private int[] difficultyDepth = {3, 4, 7};


public void SetDifficultyIndex(int index)
{
    DifficultyChosen = true;
    GameSettings.Depth = difficultyDepth[index];

    if (difficultyText != null)
        difficultyText.text = index == 0 ? "Easy" : index == 1 ? "Medium" : "Hard";

    ApplyToBot();
}




private void OnEnable()
{
    if (vsBotToggle != null)
        vsBotToggle.isOn = GameSettings.VsBot;

    if (difficultyDropdown != null)
    {
        var dd = difficultyDropdown.GetComponent<Dropdown>();
        if (dd != null)
            dd.value = DropdownIndexFromDepth(GameSettings.Depth);
    }

    if (difficultyText != null)
    {
        if (!DifficultyChosen)
            difficultyText.text = "Difficulty";
        else
        {
            int idx = DropdownIndexFromDepth(GameSettings.Depth);
            difficultyText.text = idx == 0 ? "Easy" : idx == 1 ? "Medium" : "Hard";
        }
    }

    if (botPlaysBlackToggle != null)
        botPlaysBlackToggle.isOn = GameSettings.BotPlaysBlack;

    ApplyToBot();
}



    public void OnAnySettingChanged()
    {
        Debug.Log("Before: toggle=" + vsBotToggle.isOn + " prefs=" + GameSettings.VsBot);

        if (vsBotToggle != null) GameSettings.VsBot = vsBotToggle.isOn;

        if (difficultyDropdown == null) return;

        Dropdown dd = difficultyDropdown.GetComponent<Dropdown>();
        if (dd == null) return;

        int index = dd.value;



        GameSettings.Depth = difficultyDepth[index];

        Debug.Log("Dropdown index: " + index);


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

        return 3;
    }
}
