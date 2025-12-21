using UnityEngine;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private Text scoreText;

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (scoreText == null) return;
        scoreText.text = $"Score\nWhite: {ScoreManager.WhiteWins}\nBlack: {ScoreManager.BlackWins}";
    }

    public void OnResetScoreClicked()
    {
        ScoreManager.ResetScore();
        Refresh();
    }
}
