using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    //Some functions will need reference to the controller
    public GameObject controller;

    //The Chesspiece that was tapped to create this MovePlate
    GameObject reference = null;

    //Location on the board
    int matrixX;
    int matrixY;

    //false: movement, true: attacking
    public bool attack = false;

    public void Start()
    {
        if (attack)
        {
            //Set to red
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    public void OnMouseUp()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
        Game g = controller.GetComponent<Game>();

        var cm = reference.GetComponent<Chessman>();

        string pieceName = cm.name;

        int fromX = cm.GetXBoard();
        int fromY = cm.GetYBoard();
        int toX = matrixX;
        int toY = matrixY;

        bool isWhiteMove = pieceName.StartsWith("white_");
        bool isCapture = attack;

        if (attack)
        {
            GameObject cp = g.GetPosition(matrixX, matrixY);

            string capturedName = cp.GetComponent<Chessman>().name;
            bool capturedKing = (capturedName == "white_king" || capturedName == "black_king");

            g.RegisterPieceLoss(cp);
            g.OnPieceCaptured(cp);
            Destroy(cp);

            if (capturedKing)
            {
                g.Winner(isWhiteMove ? "white" : "black");
                return;
            }
        }

        g.SetPositionEmpty(fromX, fromY);

        cm.SetXBoard(toX);
        cm.SetYBoard(toY);
        cm.SetCoords();
        
        // Mark moved
        cm.hasMoved = true;

        // CASTLING: če je kralj šel za 2, premakni tudi rook
        if (pieceName.EndsWith("_king") && Mathf.Abs(toX - fromX) == 2)
        {
            int rookFromX = (toX > fromX) ? 7 : 0;
            int rookToX   = (toX > fromX) ? 5 : 3;

            GameObject rookObj = g.GetPosition(rookFromX, toY);
            if (rookObj != null)
            {
                var rookCm = rookObj.GetComponent<Chessman>();

                // počisti staro rook pozicijo
                g.SetPositionEmpty(rookFromX, toY);

                // premakni rook
                rookCm.SetXBoard(rookToX);
                rookCm.SetYBoard(toY);
                rookCm.SetCoords();
                rookCm.hasMoved = true;

                g.SetPosition(rookObj);
            }
        }


        g.SetPosition(reference);

        // LOG (two-player)
        g.MoveLogger?.LogMove(isWhiteMove, pieceName, fromX, fromY, toX, toY, isCapture);


        if (g.currentPowerUp == 1)
            g.currentPowerUp = -1;

        g.NextTurn();
        cm.DestroyMovePlates();
    }


    public void SetCoords(int x, int y)
    {
        matrixX = x;
        matrixY = y;
    }

    public void SetReference(GameObject obj)
    {
        reference = obj;
    }

    public GameObject GetReference()
    {
        return reference;
    }
}
