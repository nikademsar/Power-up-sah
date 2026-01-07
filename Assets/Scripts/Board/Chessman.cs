using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessman : MonoBehaviour
{
    //References to objects in our Unity Scene
    public GameObject controller;
    public GameObject movePlate;

    //Position for this Chesspiece on the Board
    //The correct position will be set later
    private int xBoard = -1;
    private int yBoard = -1;

    //Variable for keeping track of the player it belongs to "black" or "white"
    private string player;

    //References to all the possible Sprites that this Chesspiece could be
    public Sprite black_queen, black_knight, black_bishop, black_king, black_rook, black_pawn;
    public Sprite white_queen, white_knight, white_bishop, white_king, white_rook, white_pawn;

    public void Activate()
    {
        //Get the game controller
        controller = GameObject.FindGameObjectWithTag("GameController");

        //Take the instantiated location and adjust transform
        SetCoords();

        //Choose correct sprite based on piece's name
        switch (this.name)
        {
            case "black_queen": this.GetComponent<SpriteRenderer>().sprite = black_queen; player = "black"; break;
            case "black_knight": this.GetComponent<SpriteRenderer>().sprite = black_knight; player = "black"; break;
            case "black_bishop": this.GetComponent<SpriteRenderer>().sprite = black_bishop; player = "black"; break;
            case "black_king": this.GetComponent<SpriteRenderer>().sprite = black_king; player = "black"; break;
            case "black_rook": this.GetComponent<SpriteRenderer>().sprite = black_rook; player = "black"; break;
            case "black_pawn": this.GetComponent<SpriteRenderer>().sprite = black_pawn; player = "black"; break;
            case "white_queen": this.GetComponent<SpriteRenderer>().sprite = white_queen; player = "white"; break;
            case "white_knight": this.GetComponent<SpriteRenderer>().sprite = white_knight; player = "white"; break;
            case "white_bishop": this.GetComponent<SpriteRenderer>().sprite = white_bishop; player = "white"; break;
            case "white_king": this.GetComponent<SpriteRenderer>().sprite = white_king; player = "white"; break;
            case "white_rook": this.GetComponent<SpriteRenderer>().sprite = white_rook; player = "white"; break;
            case "white_pawn": this.GetComponent<SpriteRenderer>().sprite = white_pawn; player = "white"; break;
        }
    }

    public void SetCoords()
    {
        //Get the board value in order to convert to xy coords
        float x = xBoard;
        float y = yBoard;

        //Adjust by variable offset
        x *= 0.66f;
        y *= 0.66f;

        //Add constants (pos 0,0)
        x += -2.3f;
        y += -2.3f;

        //Set actual unity values
        this.transform.position = new Vector3(x, y, -1.0f);
    }

    public int GetXBoard() { return xBoard; }
    public int GetYBoard() { return yBoard; }
    public void SetXBoard(int x) { xBoard = x; }
    public void SetYBoard(int y) { yBoard = y; }

    private void OnMouseUp()
    {
        Game g = controller.GetComponent<Game>();
        if (g.currentPowerUp == 4 && g.GetCurrentPlayer() == g.powerUpPlayer)
        {
            if (g.selectedPiece1 == null)
            {
                g.selectedPiece1 = this.gameObject;
                Debug.Log("First piece selected: " + name);
            }
            else
            {
                g.selectedPiece2 = this.gameObject;
                Debug.Log("Second piece selected: " + name);

                g.SwapPieces(g.selectedPiece1, g.selectedPiece2);

                g.currentPowerUp = -1;
                g.selectedPiece1 = null;
                g.selectedPiece2 = null;

                g.NextTurn();
            }

            return;
        }

        if (!controller.GetComponent<Game>().IsGameOver() && controller.GetComponent<Game>().GetCurrentPlayer() == player)
        {
            //Remove all moveplates relating to previously selected piece
            DestroyMovePlates();

            //Create new MovePlates
            InitiateMovePlates();
        }
    }

    public void DestroyMovePlates()
    {
        //Destroy old MovePlates
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for (int i = 0; i < movePlates.Length; i++)
        {
            Destroy(movePlates[i]); // Destroy is asynchronous
        }
    }

    public void InitiateMovePlates()
    {
        Game g = controller.GetComponent<Game>();

        // powerup 2: opponent restricted to pawn movement
        bool restrictedToPawnMovement = (g.currentPowerUp == 2) && (g.powerUpPlayer != player);

        switch (this.name)
        {
            case "black_queen":
            case "white_queen":
                if (restrictedToPawnMovement) { PawnLikeMovement(); break; }
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(1, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                LineMovePlate(-1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(1, -1);
                break;

            case "black_knight":
            case "white_knight":
                if (restrictedToPawnMovement) { PawnLikeMovement(); break; }
                LMovePlate();
                break;

            case "black_bishop":
            case "white_bishop":
                if (restrictedToPawnMovement) { PawnLikeMovement(); break; }
                LineMovePlate(1, 1);
                LineMovePlate(1, -1);
                LineMovePlate(-1, 1);
                LineMovePlate(-1, -1);
                break;

            case "black_king":
            case "white_king":
                if (restrictedToPawnMovement) { PawnLikeMovement(); break; }
                SurroundMovePlate();
                break;

            case "black_rook":
            case "white_rook":
                if (restrictedToPawnMovement) { PawnLikeMovement(); break; }
                LineMovePlate(1, 0);
                LineMovePlate(0, 1);
                LineMovePlate(-1, 0);
                LineMovePlate(0, -1);
                break;

            case "black_pawn":
            case "white_pawn":
                // powerup 1: pawn moves like a queen (as you already had)
                if (g.currentPowerUp == 1 && g.powerUpPlayer == player)
                {
                    LineMovePlate(1, 0);
                    LineMovePlate(-1, 0);
                    LineMovePlate(0, 1);
                    LineMovePlate(0, -1);
                    LineMovePlate(1, 1);
                    LineMovePlate(1, -1);
                    LineMovePlate(-1, 1);
                    LineMovePlate(-1, -1);
                }
                else
                {
                    // normal pawn behavior (WITH initial 2-step + captures)
                    PawnMovePlates();
                }
                break;
        }
    }

    // Pawn-like movement now uses the same pawn logic (forward + diagonal captures + initial 2-step)
    void PawnLikeMovement()
    {
        PawnMovePlates();
    }

    public void LineMovePlate(int xIncrement, int yIncrement)
    {
        Game sc = controller.GetComponent<Game>();

        int x = xBoard + xIncrement;
        int y = yBoard + yIncrement;

        while (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y) == null)
        {
            MovePlateSpawn(x, y);
            x += xIncrement;
            y += yIncrement;
        }

        if (sc.PositionOnBoard(x, y) && sc.GetPosition(x, y).GetComponent<Chessman>().player != player)
        {
            MovePlateAttackSpawn(x, y);
        }
    }

    public void LMovePlate()
    {
        PointMovePlate(xBoard + 1, yBoard + 2);
        PointMovePlate(xBoard - 1, yBoard + 2);
        PointMovePlate(xBoard + 2, yBoard + 1);
        PointMovePlate(xBoard + 2, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard - 2);
        PointMovePlate(xBoard - 1, yBoard - 2);
        PointMovePlate(xBoard - 2, yBoard + 1);
        PointMovePlate(xBoard - 2, yBoard - 1);
    }

    public void SurroundMovePlate()
    {
        PointMovePlate(xBoard, yBoard + 1);
        PointMovePlate(xBoard, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 0);
        PointMovePlate(xBoard - 1, yBoard - 1);
        PointMovePlate(xBoard - 1, yBoard + 1);
        PointMovePlate(xBoard + 1, yBoard + 0);
        PointMovePlate(xBoard + 1, yBoard - 1);
        PointMovePlate(xBoard + 1, yBoard + 1);
    }

    public void PointMovePlate(int x, int y)
    {
        Game sc = controller.GetComponent<Game>();
        if (sc.PositionOnBoard(x, y))
        {
            GameObject cp = sc.GetPosition(x, y);

            if (cp == null)
            {
                MovePlateSpawn(x, y);
            }
            else if (cp.GetComponent<Chessman>().player != player)
            {
                MovePlateAttackSpawn(x, y);
            }
        }
    }

    // NEW: full pawn logic: 1-step, 2-step from start, diagonal captures
    private void PawnMovePlates()
    {
        Game sc = controller.GetComponent<Game>();

        int dir = (player == "white") ? 1 : -1;
        int startRank = (player == "white") ? 1 : 6;

        int x = xBoard;
        int y1 = yBoard + dir;

        // forward 1 (must be empty)
        if (sc.PositionOnBoard(x, y1) && sc.GetPosition(x, y1) == null)
        {
            MovePlateSpawn(x, y1);

            // forward 2 from starting rank (both squares must be empty)
            int y2 = yBoard + 2 * dir;
            if (yBoard == startRank && sc.PositionOnBoard(x, y2) && sc.GetPosition(x, y2) == null)
            {
                MovePlateSpawn(x, y2);
            }
        }

        // diagonal captures
        int capY = yBoard + dir;

        int capX1 = xBoard + 1;
        if (sc.PositionOnBoard(capX1, capY))
        {
            var t = sc.GetPosition(capX1, capY);
            if (t != null && t.GetComponent<Chessman>().player != player)
                MovePlateAttackSpawn(capX1, capY);
        }

        int capX2 = xBoard - 1;
        if (sc.PositionOnBoard(capX2, capY))
        {
            var t = sc.GetPosition(capX2, capY);
            if (t != null && t.GetComponent<Chessman>().player != player)
                MovePlateAttackSpawn(capX2, capY);
        }
    }

    public void MovePlateSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;

        x *= 0.66f;
        y *= 0.66f;

        x += -2.3f;
        y += -2.3f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }

    public void MovePlateAttackSpawn(int matrixX, int matrixY)
    {
        float x = matrixX;
        float y = matrixY;

        x *= 0.66f;
        y *= 0.66f;

        x += -2.3f;
        y += -2.3f;

        GameObject mp = Instantiate(movePlate, new Vector3(x, y, -3.0f), Quaternion.identity);

        MovePlate mpScript = mp.GetComponent<MovePlate>();
        mpScript.attack = true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoords(matrixX, matrixY);
    }
}
