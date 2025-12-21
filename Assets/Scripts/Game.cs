using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PowerUpChess.Engine;

public class Game : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject chesspiece;

    [Header("UI")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Button resetButton;        // Assign the Button component (not the GameObject)
    [SerializeField] private Text turnText;             // Prefer assigning in Inspector; fallback is by tag
    [SerializeField] private Text endWinnerText;
    [SerializeField] private ScoreUI scoreUI;


    private GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];

    private string currentPlayer = "white";
    private bool gameOver = true;
    private bool boardInitialized = false;

    private void Awake()
    {
        if (turnText == null)
            turnText = GameObject.FindGameObjectWithTag("TurnText")?.GetComponent<Text>();

        if (resetButton != null)
            resetButton.onClick.AddListener(RestartGame);

        if (endWinnerText == null)
            Debug.LogError("Game: endWinnerText is not assigned in Inspector.");

    }

    public void Start()
    {
        ShowStartState();
    }

    public void OnPlayPressed()
    {
        // Start a new match from a clean state
        if (boardInitialized)
            ClearBoard();

        currentPlayer = "white";
        gameOver = false;

        if (startPanel != null) startPanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(false);

        if (resetButton != null) resetButton.gameObject.SetActive(true);

        SetupBoard();
        boardInitialized = true;

        UpdateTurnText();
    }

    private void ShowStartState()
    {
        gameOver = true;

        if (startPanel != null) startPanel.SetActive(true);
        if (endPanel != null) endPanel.SetActive(false);

        // Keep reset button visible if you want it on start screen; otherwise set to false
        if (resetButton != null) resetButton.gameObject.SetActive(true);

        UpdateTurnText();
    }

    private void SetupBoard()
    {
        positions = new GameObject[8, 8];

        playerWhite = new GameObject[]
        {
            Create("white_rook", 0, 0), Create("white_knight", 1, 0),
            Create("white_bishop", 2, 0), Create("white_queen", 3, 0), Create("white_king", 4, 0),
            Create("white_bishop", 5, 0), Create("white_knight", 6, 0), Create("white_rook", 7, 0),
            Create("white_pawn", 0, 1), Create("white_pawn", 1, 1), Create("white_pawn", 2, 1),
            Create("white_pawn", 3, 1), Create("white_pawn", 4, 1), Create("white_pawn", 5, 1),
            Create("white_pawn", 6, 1), Create("white_pawn", 7, 1)
        };

        playerBlack = new GameObject[]
        {
            Create("black_rook", 0, 7), Create("black_knight", 1, 7),
            Create("black_bishop", 2, 7), Create("black_queen", 3, 7), Create("black_king", 4, 7),
            Create("black_bishop", 5, 7), Create("black_knight", 6, 7), Create("black_rook", 7, 7),
            Create("black_pawn", 0, 6), Create("black_pawn", 1, 6), Create("black_pawn", 2, 6),
            Create("black_pawn", 3, 6), Create("black_pawn", 4, 6), Create("black_pawn", 5, 6),
            Create("black_pawn", 6, 6), Create("black_pawn", 7, 6)
        };

        for (int i = 0; i < 16; i++)
        {
            SetPosition(playerWhite[i]);
            SetPosition(playerBlack[i]);
        }
    }

    private void ClearBoard()
    {
        // Destroy all pieces referenced by the board matrix
        for (int x = 0; x < 8; x++)
        for (int y = 0; y < 8; y++)
        {
            if (positions[x, y] != null)
            {
                Destroy(positions[x, y]);
                positions[x, y] = null;
            }
        }

        playerWhite = new GameObject[16];
        playerBlack = new GameObject[16];
        boardInitialized = false;
    }

    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>();
        cm.name = name;
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate();
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();
        positions[cm.GetXBoard(), cm.GetYBoard()] = obj;
    }

    public void SetPositionEmpty(int x, int y)
    {
        positions[x, y] = null;
    }

    public GameObject GetPosition(int x, int y)
    {
        return positions[x, y];
    }

    public bool PositionOnBoard(int x, int y)
    {
        return !(x < 0 || y < 0 || x >= 8 || y >= 8);
    }

    public string GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public void NextTurn()
    {
        currentPlayer = (currentPlayer == "white") ? "black" : "white";
        UpdateTurnText();
    }

    public void Winner(string winnerColorLowercase)
    {
        // winnerColorLowercase must be "white" or "black"
        ScoreManager.AddWinFor(winnerColorLowercase);

        if (scoreUI != null)
            scoreUI.Refresh();
        else
            Debug.LogWarning("Game: scoreUI is not assigned; score text won't auto-refresh.");


        gameOver = true;

        // Return to a clean "pre-start" board state (no pieces)
        if (boardInitialized)
            ClearBoard();

        if (startPanel != null) startPanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(true);

        if (endWinnerText != null)
        {
            string label = (winnerColorLowercase == "white") ? "White" : "Black";
            endWinnerText.text = label + " is the winner";
        }

        if (resetButton != null) resetButton.gameObject.SetActive(true);

        UpdateTurnText();
    }

    private void UpdateTurnText()
    {
        if (turnText == null) return;

        if (gameOver)
        {
            turnText.enabled = false;
            return;
        }

        turnText.enabled = true;
        turnText.text = (currentPlayer == "white") ? "White's turn" : "Black's turn";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ===== BOT SUPPORT =====

    public BoardState ExportBoardState()
    {
        var s = new BoardState();
        s.whiteToMove = (currentPlayer == "white");

        for (int x = 0; x < 8; x++)
        for (int y = 0; y < 8; y++)
        {
            var obj = positions[x, y];
            if (obj == null) { s.b[x, y] = Piece.Empty; continue; }

            var cm = obj.GetComponent<Chessman>();
            s.b[x, y] = MapChessmanToEnginePiece(cm.name);
        }

        return s;
    }

    private Piece MapChessmanToEnginePiece(string n)
    {
        switch (n)
        {
            case "white_pawn": return Piece.WP;
            case "white_knight": return Piece.WN;
            case "white_bishop": return Piece.WB;
            case "white_rook": return Piece.WR;
            case "white_queen": return Piece.WQ;
            case "white_king": return Piece.WK;

            case "black_pawn": return Piece.BP;
            case "black_knight": return Piece.BN;
            case "black_bishop": return Piece.BB;
            case "black_rook": return Piece.BR;
            case "black_queen": return Piece.BQ;
            case "black_king": return Piece.BK;
        }

        return Piece.Empty;
    }

    public bool ApplyEngineMove(Move m)
    {
        if (gameOver) return false;

        var fromObj = GetPosition(m.fromX, m.fromY);
        if (fromObj == null) return false;

        var toObj = GetPosition(m.toX, m.toY);

        var fromName = fromObj.GetComponent<Chessman>().name;
        if (toObj != null)
        {
            var toName = toObj.GetComponent<Chessman>().name;
            bool fromWhite = fromName.StartsWith("white_");
            bool toWhite = toName.StartsWith("white_");
            if (fromWhite == toWhite) return false;
        }

        if (toObj != null)
        {
            var capturedName = toObj.GetComponent<Chessman>().name;
            bool capturedKing = (capturedName == "white_king" || capturedName == "black_king");

            SetPositionEmpty(m.toX, m.toY);
            Destroy(toObj);

            if (capturedKing)
            {
                Winner(currentPlayer);
                return true;
            }
        }

        SetPositionEmpty(m.fromX, m.fromY);

        var cm = fromObj.GetComponent<Chessman>();
        cm.SetXBoard(m.toX);
        cm.SetYBoard(m.toY);
        cm.Activate();

        SetPosition(fromObj);

        NextTurn();
        return true;
    }
}
