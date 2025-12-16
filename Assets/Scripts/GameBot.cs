using System.Collections;
using UnityEngine;
using PowerUpChess.Engine;

public class GameBot : MonoBehaviour
{
    public enum GameMode { TwoPlayer, VsBot }

    [Header("Mode")]
    public GameMode mode = GameMode.TwoPlayer;

    [Header("Bot side (only used in VsBot)")]
    public bool botPlaysBlack = true;  // tipièno: igralec white, bot black
    public bool botPlaysWhite = false;

    [Header("Bot settings")]
    [Range(1, 5)]
    public int searchDepth = 2;

    public float thinkDelaySeconds = 0.15f;

    private Game game;
    private bool botThinking = false;

    void Awake()
    {
        game = FindFirstObjectByType<Game>();
    }

    void Update()
    {
        if (game == null) return;
        if (mode != GameMode.VsBot) return;
        if (game.IsGameOver()) return;
        if (botThinking) return;

        bool isWhiteTurn = (game.GetCurrentPlayer() == "white");
        bool botTurn = (isWhiteTurn && botPlaysWhite) || (!isWhiteTurn && botPlaysBlack);

        if (!botTurn) return;

        StartCoroutine(DoBotMove());
    }

    private IEnumerator DoBotMove()
    {
        botThinking = true;
        if (thinkDelaySeconds > 0) yield return new WaitForSeconds(thinkDelaySeconds);

        // 1) Export Unity -> BoardState
        BoardState s = game.ExportBoardState();

        // 2) Search best move
        var (best, score) = Engine.SearchBestMove(s, searchDepth);

        // 3) Apply to Unity
        // èe engine vrne default/invalid potezo (npr. ni potez), poskusi varno zakljuèiti
        bool ok = game.ApplyEngineMove(best);

        botThinking = false;

        if (!ok)
        {
            // èe engine vrne potezo, ki je Unity ne sprejme, je engine/board-map še neskladen
            // v tem primeru ne delaj niè (da ne zmrzneš igre s loopom)
        }
    }

    public void SetModeFromToggle(bool aiOn)
    {
        mode = aiOn ? GameMode.VsBot : GameMode.TwoPlayer;
    }


    // Optional: èe želiš UI gumb za preklop
    public void SetTwoPlayer() => mode = GameMode.TwoPlayer;
    public void SetVsBot() => mode = GameMode.VsBot;
}
