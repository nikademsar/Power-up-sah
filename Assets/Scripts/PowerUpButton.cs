using UnityEngine;
using UnityEngine.UI;

public class PowerUpButton : MonoBehaviour
{
    [Header("Setup")]
    public Game game;                     // reference na Game
    public GameObject controller;
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

    private Game getGame()
    {
        return GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
    }


    public void doubleMoveBtnOnClick()
    {
        Game g = getGame();
        string player = g.GetCurrentPlayer();

        g.ActivateDoubleMovePowerUp(player);

        SetActive(false);
    }

    public void pawnQueenBtnOnClick()
    {
        Game g = getGame();
        string player = g.GetCurrentPlayer();

        g.ActivatePawnQueenPowerUp(player);

        SetActive(false);
    }

    public void restrictToPawnBtnOnClick()
    {
        Game g = getGame();
        string player = g.GetCurrentPlayer();

        g.ActivateRestrictToPawnPowerUp(player);

        SetActive(false);
    }

    public void upgradeBtnOnClick()
    {

        Game g = getGame();
        string player = g.GetCurrentPlayer();

        g.ActivatePawnUpgradePowerUp(player);

        SetActive(false);
    }

    public void swapBtnOnClick()
    {
        Game g = getGame();
        string player = g.GetCurrentPlayer();

        g.ActivateSwapPowerUp(player);

        SetActive(false);
    }


    public void OnClick()
    {

    }
}
