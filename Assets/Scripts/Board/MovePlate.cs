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
        int fromX = cm.GetXBoard();
        int fromY = cm.GetYBoard();
        int toX = matrixX;
        int toY = matrixY;
        bool whiteMove = cm.name.StartsWith("white_");

        if (attack)
        {
            GameObject cp = g.GetPosition(matrixX, matrixY);
            g.RegisterPieceLoss(cp);
            g.OnPieceCaptured(cp);   // <-- DODAJ
            Destroy(cp);
        }


        g.SetPositionEmpty(fromX, fromY);

        cm.SetXBoard(toX);
        cm.SetYBoard(toY);
        cm.SetCoords();

        g.SetPosition(reference);
        
        
        
        // >>> LOG HERE <<<


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
