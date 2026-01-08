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
        scoreText.text = $"Score: White {ScoreManager.WhiteWins} vs Black {ScoreManager.BlackWins}";
    }

    public void OnResetScoreClicked()
    {
        ScoreManager.ResetScore();
        Refresh();
    }
}
