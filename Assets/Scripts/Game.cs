using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PowerUpChess.Engine;
using System.Linq;


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
    [SerializeField] private CapturedPiecesUI capturedUI;
    [SerializeField] public MoveLoggerSimple moveLogger;

    
    





    private GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];

    private string currentPlayer = "white";
    private bool gameOver = true;
    private bool boardInitialized = false;

    public string powerUpPlayer = "white";
    private int powerUpMoves = 2;

    public GameObject selectedPiece1 = null;
    public GameObject selectedPiece2 = null;
    
    public MoveLoggerSimple MoveLogger => moveLogger;


    public int currentPowerUp = -1;
    // 0  two moves
    // 1  pawn moves like a queen 
    // 2  opponent figure moves like a pawn
    // 3  replace a random pawn with better figure
    // 4  swap two figures

    public bool powerUpsEnabled = false;

    private int whiteLostPieces = 0;
    private int blackLostPieces = 0;

    private const int LOST_PIECES_REQUIRED_FOR_POWERUP = 2;

    private void powerUpUsed()
    {
        if (powerUpPlayer == "white")
        {
            whiteLostPieces = 0;
        }
        else
        {
            blackLostPieces = 0;
        }
        
        powerUpsEnabled = false;
    }

    public void ActivateDoubleMovePowerUp(string player)
    {
        if (!powerUpsEnabled)
        {

            Debug.Log("Power ups not available!");
            return;
        }
        Debug.Log("Double move power up");
        powerUpPlayer = player;
        currentPowerUp = 0;
        resetMovesPowerUp();

        powerUpUsed();
    }

    public void ActivatePawnQueenPowerUp(string player)
    {
        if (!powerUpsEnabled)
        {
            Debug.Log("Power ups not available!");
            return;
        }
        Debug.Log("Pawn Queen power up");
        powerUpPlayer = player;
        currentPowerUp = 1;
        powerUpUsed();

    }

    public void ActivateRestrictToPawnPowerUp(string player)
    {
        if (!powerUpsEnabled)
        {
            Debug.Log("Power ups not available!");
            return;
        }
        Debug.Log("Opponent moves like pawn power up");
        powerUpPlayer = player;
        currentPowerUp = 2;
        powerUpUsed();

    }

    public GameObject GetRandomPawn(string player)
    {
        GameObject[] pawns;

        if (player == "white")
            pawns = playerWhite;
        else
            pawns = playerBlack;

        // filter only pawns still on board
        var alivePawns = pawns.Where(p => p != null && p.GetComponent<Chessman>().name.Contains("pawn")).ToArray();

        if (alivePawns.Length == 0) return null;

        int index = Random.Range(0, alivePawns.Length);
        return alivePawns[index];
    }

    public void ActivatePawnUpgradePowerUp(string player)
    {
        if (!powerUpsEnabled)
        {
            Debug.Log("Power ups not available!");
            return;
        }
        Debug.Log("Replace pawn with a better figure power up");
        powerUpPlayer = player;
        currentPowerUp = 3;

        GameObject pawn = GetRandomPawn(player);
        if (pawn == null) return;

        Chessman cm = pawn.GetComponent<Chessman>();

        string[] availableFigures = { "bishop", "rook", "knight" };

        string figure = availableFigures[UnityEngine.Random.Range(0, availableFigures.Length)];

        string newName = (player == "white") ? ("white_" + figure) : ("black_" + figure);
        cm.name = newName;
        cm.Activate();

        powerUpUsed();

    }

    public void ActivateSwapPowerUp(string player)
    {
        if (!powerUpsEnabled)
        {
            Debug.Log("Power ups not available!");
            return;
        }
        Debug.Log("Swap figure power up");
        powerUpPlayer = player;
        currentPowerUp = 4;
        selectedPiece1 = null;
        selectedPiece2 = null;

        powerUpUsed();

    }

    public void SwapPieces(GameObject a, GameObject b)
    {
        if (a == null || b == null) return;

        Chessman cmA = a.GetComponent<Chessman>();
        Chessman cmB = b.GetComponent<Chessman>();

        // store original positions
        int xA = cmA.GetXBoard();
        int yA = cmA.GetYBoard();
        int xB = cmB.GetXBoard();
        int yB = cmB.GetYBoard();

        // swap positions in board array
        positions[xA, yA] = b;
        positions[xB, yB] = a;

        // swap coordinates
        cmA.SetXBoard(xB);
        cmA.SetYBoard(yB);
        cmB.SetXBoard(xA);
        cmB.SetYBoard(yA);

        // update Unity positions
        cmA.SetCoords();
        cmB.SetCoords();

        Debug.Log($"Swapped {cmA.name} with {cmB.name}");
    }

    public void RegisterPieceLoss(GameObject piece)
    {
        if (piece == null) return;

        string n = piece.GetComponent<Chessman>().name;

        if (n.StartsWith("white_"))
        {
            whiteLostPieces++;

            if (whiteLostPieces >= LOST_PIECES_REQUIRED_FOR_POWERUP && powerUpPlayer == "white")
            {
                powerUpsEnabled = true;
                Debug.Log("white player powerups enabled!");
            }
        }
        else if (n.StartsWith("black_"))
        {
            blackLostPieces++;


            if (blackLostPieces >= LOST_PIECES_REQUIRED_FOR_POWERUP && powerUpPlayer == "black")
            {
                powerUpsEnabled = true;
                Debug.Log("black player powerups enabled!");
            }
        }
    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ActivateDoubleMovePowerUp("white");
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActivatePawnQueenPowerUp("white");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActivateRestrictToPawnPowerUp("white");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ActivatePawnUpgradePowerUp("white");
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ActivateSwapPowerUp("white");
        }
    }


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
        capturedUI?.Clear();
        moveLogger?.Clear();

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
    
    public void OnPieceCaptured(GameObject piece)
    {
        if (!piece) return;

        var cm = piece.GetComponent<Chessman>();
        if (!cm) return;

        bool capturedWasWhite = cm.name.StartsWith("white_");   // <-- MANJKA TI TO

        var sr = piece.GetComponent<SpriteRenderer>();
        if (sr && sr.sprite)
            capturedUI?.AddCaptured(capturedWasWhite, sr.sprite);
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
        if (currentPowerUp == 0 && currentPlayer == powerUpPlayer)
        {
            powerUpMoves--;
            if (powerUpMoves > 0)
            {
                return;
            }
            else
            {
                currentPowerUp = -1;
            }
        }

        if (currentPowerUp == 2 && currentPlayer == powerUpPlayer)
        {
            currentPowerUp = -1;
        }
        currentPlayer = (currentPlayer == "white") ? "black" : "white";
        UpdateTurnText();
    }
    



    public void resetMovesPowerUp()
    {
        powerUpMoves = 2;
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

        // --- logging data: zajemi takoj ---
        string fromName = fromObj.GetComponent<Chessman>().name;
        bool isCapture = (toObj != null);

        // (opcijsko: varovalo pred "eat own piece")
        if (isCapture)
        {
            string toName = toObj.GetComponent<Chessman>().name;
            bool fromWhite = fromName.StartsWith("white_");
            bool toWhite = toName.StartsWith("white_");
            if (fromWhite == toWhite) return false;
        }

        // --- potem šele delaj capture/move ---
        if (isCapture)
        {
            string capturedName = toObj.GetComponent<Chessman>().name;
            bool capturedKing = (capturedName == "white_king" || capturedName == "black_king");

            OnPieceCaptured(toObj);
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
        cm.hasMoved = true;


        bool isWhiteMove = fromName.StartsWith("white_");
        Debug.Log("ApplyEngineMove reached, logger = " + (moveLogger ? moveLogger.name : "NULL"));

        moveLogger?.LogMove(isWhiteMove, fromName, m.fromX, m.fromY, m.toX, m.toY, isCapture);

        NextTurn();   // <- tole
        return true;
    }

}
