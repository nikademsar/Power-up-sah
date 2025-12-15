using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Engine;

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

    private bool aiThinking = false;


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
    }

    public void Update()
    {
        // restart logika
        if (gameOver == true && Input.GetMouseButtonDown(0))
        {
            gameOver = false;
            SceneManager.LoadScene("Game");
            return;
        }
    
        // AI poteza (primer: AI igra črne)
        if (!gameOver && !aiThinking && currentPlayer == "black")
        {
            aiThinking = true;
    
            BoardState state = ExportBoard();
            var result = Engine.SearchBestMove(state, depth: 3);
            ApplyMoveToUnity(result.best);
    
            aiThinking = false;
        }
    }

    
    public void Winner(string playerWinner)
    {
        gameOver = true;

        //Using UnityEngine.UI is needed here
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("WinnerText").GetComponent<Text>().text = playerWinner + " is the winner";

        GameObject.FindGameObjectWithTag("RestartText").GetComponent<Text>().enabled = true;
    }


    BoardState ExportBoard()
    {
        var s = new BoardState();
        s.whiteToMove = (currentPlayer == "white");
    
        for (int x = 0; x < 8; x++)
        for (int y = 0; y < 8; y++)
        {
            var go = positions[x, y];
            if (go == null)
            {
                s.b[x, y] = Piece.Empty;
                continue;
            }
    
            // ti shranjuješ ime figure v Chessman.name
            string n = go.GetComponent<Chessman>().name;
            s.b[x, y] = NameToPiece(n);
        }
    
        return s;
    }

    Piece NameToPiece(string n)
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
    
            default: return Piece.Empty;
        }
    }
    
    void ApplyMoveToUnity(Move m)
    {
        GameObject fromGO = GetPosition(m.fromX, m.fromY);
        if (fromGO == null) return; // safety
    
        GameObject toGO = GetPosition(m.toX, m.toY);
    
        // če je zajem: uniči figuro na cilju
        if (toGO != null)
        {
            if (toGO.name == "white_king") Winner("black");
            if (toGO.name == "black_king") Winner("white");
            Destroy(toGO);
        }
    
        // počisti staro pozicijo
        SetPositionEmpty(m.fromX, m.fromY);
    
        // premakni figuro
        Chessman cm = fromGO.GetComponent<Chessman>();
        cm.SetXBoard(m.toX);
        cm.SetYBoard(m.toY);
        cm.SetCoords();
    
        // zapiši novo pozicijo
        SetPosition(fromGO);
    
        // menjava poteze
        NextTurn();
    
        // pobriši moveplate-e, če so ostali iz user klika
        cm.DestroyMovePlates();
    }

}
