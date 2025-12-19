using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PowerUpChess.Engine;


public class Game : MonoBehaviour
{
    //Reference from Unity IDE
    public GameObject chesspiece;

    //Matrices needed, positions of each of the GameObjects
    //Also separate arrays for the players in order to easily keep track of them all
    //Keep in mind that the same objects are going to be in "positions" and "playerBlack"/"playerWhite"
    private GameObject[,] positions = new GameObject[8, 8];
    private GameObject[] playerBlack = new GameObject[16];
    private GameObject[] playerWhite = new GameObject[16];

    //current turn
    private string currentPlayer = "white";

    //Game Ending
    private bool gameOver = false;

    private Text turnText;
    [SerializeField] private GameObject restartGameButton;
    [Header("UI Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject endPanel;

    [Header("UI Text")]
    [SerializeField] private Text winnerText;   // poveži WinnerText objekt (komponento Text)

    //TODO
    //[SerializeField] private Dropdown difficultyDropdown;
    //[SerializeField] private Dropdown themeDropdown;




    //Unity calls this right when the game starts, there are a few built in functions
    //that Unity can call for you
    public void Start()
    {
        playerWhite = new GameObject[] { Create("white_rook", 0, 0), Create("white_knight", 1, 0),
            Create("white_bishop", 2, 0), Create("white_queen", 3, 0), Create("white_king", 4, 0),
            Create("white_bishop", 5, 0), Create("white_knight", 6, 0), Create("white_rook", 7, 0),
            Create("white_pawn", 0, 1), Create("white_pawn", 1, 1), Create("white_pawn", 2, 1),
            Create("white_pawn", 3, 1), Create("white_pawn", 4, 1), Create("white_pawn", 5, 1),
            Create("white_pawn", 6, 1), Create("white_pawn", 7, 1) };
        playerBlack = new GameObject[] { Create("black_rook", 0, 7), Create("black_knight",1,7),
            Create("black_bishop",2,7), Create("black_queen",3,7), Create("black_king",4,7),
            Create("black_bishop",5,7), Create("black_knight",6,7), Create("black_rook",7,7),
            Create("black_pawn", 0, 6), Create("black_pawn", 1, 6), Create("black_pawn", 2, 6),
            Create("black_pawn", 3, 6), Create("black_pawn", 4, 6), Create("black_pawn", 5, 6),
            Create("black_pawn", 6, 6), Create("black_pawn", 7, 6) };

        //Set all piece positions on the positions board
        for (int i = 0; i < playerBlack.Length; i++)
        {
            SetPosition(playerBlack[i]);
            SetPosition(playerWhite[i]);
        }

        turnText = GameObject.FindGameObjectWithTag("TurnText").GetComponent<Text>();
        UpdateTurnText();

        if (restartButton != null)
            restartButton.SetActive(true);


    }

    public GameObject Create(string name, int x, int y)
    {
        GameObject obj = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        Chessman cm = obj.GetComponent<Chessman>(); //We have access to the GameObject, we need the script
        cm.name = name; //This is a built in variable that Unity has, so we did not have to declare it before
        cm.SetXBoard(x);
        cm.SetYBoard(y);
        cm.Activate(); //It has everything set up so it can now Activate()
        return obj;
    }

    public void SetPosition(GameObject obj)
    {
        Chessman cm = obj.GetComponent<Chessman>();

        //Overwrites either empty space or whatever was there
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
        if (x < 0 || y < 0 || x >= positions.GetLength(0) || y >= positions.GetLength(1)) return false;
        return true;
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
        if (currentPlayer == "white")
        {
            currentPlayer = "black";
        }
        else
        {
            currentPlayer = "white";
        }
        UpdateTurnText();
    }

    public void Update()
    {
        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;

            //Using UnityEngine.SceneManagement is needed here
            SceneManager.LoadScene("Game"); //Restarts the game by loading the scene over again
        }
    }
    
    public void Winner(string playerWinner)
    {
        gameOver = true;

        //Using UnityEngine.UI is needed here
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = playerWinner + " is the winner";

        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;
        
        UpdateTurnText(); // to bo TurnText skril, ker gameOver = true
        if (restartButton != null)
            restartButton.SetActive(false);

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


    // ===== BOT SUPPORT (minimal additions) =====

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
        // pričakovani formati iz tvoje Create(): "white_pawn", "black_queen", ...
        // :contentReference[oaicite:6]{index=6}
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
                Winner((currentPlayer == "white") ? "White" : "Black");
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