using UnityEngine;
using UnityEngine.UI;

public class PowerUpButton : MonoBehaviour
{
    [Header("Setup")]
    public int powerUpIndex;              // 0–4
    public Game game;                     // reference na Game

    [Header("Visuals")]
    public Sprite greySprite;
    public Sprite activeSprite;

    private Image img;
    private Button btn;

    void Awake()
    {
        img = GetComponent<Image>();
        btn = GetComponent<Button>();

        SetActive(false);
    }

    void Update()
    {
        // power-up je aktiven SAMO če:
        // - powerUpsEnabled
        // - pravi igralec je na potezi
        bool canUse =
            game != null &&
            game.powerUpsEnabled &&
            game.GetCurrentPlayer() == game.powerUpPlayer;

        SetActive(canUse);
    }

    private void SetActive(bool active)
    {
        if (img)
            img.sprite = active ? activeSprite : greySprite;

        if (btn)
            btn.interactable = active;
    }

    public void OnClick()
    {
        if (game == null) return;

        string player = game.GetCurrentPlayer();

        switch (powerUpIndex)
        {
            case 0: game.ActivateDoubleMovePowerUp(player); break;
            case 1: game.ActivatePawnQueenPowerUp(player); break;
            case 2: game.ActivateRestrictToPawnPowerUp(player); break;
            case 3: game.ActivatePawnUpgradePowerUp(player); break;
            case 4: game.ActivateSwapPowerUp(player); break;
        }

        // po uporabi -> disable
        SetActive(false);
    }
}
