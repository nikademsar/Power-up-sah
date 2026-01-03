using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MoveLoggerSimple : MonoBehaviour
{
    [SerializeField] private TMP_Text logText;
    [SerializeField] private ScrollRect scrollRect;


    private readonly StringBuilder sb = new StringBuilder(2048);
    private int ply = 0;

    public void Clear()
    {
        sb.Clear();
        ply = 0;
        if (logText) logText.text = "";
    }

    public void Log(string moveText)
    {
        int moveNumber = (ply / 2) + 1;
        bool whiteSlot = (ply % 2 == 0);

        if (whiteSlot) sb.Append(moveNumber).Append(". ").Append(moveText);
        else sb.Append("   ").Append(moveText).Append('\n');

        ply++;
        if (logText)
        {
            logText.text = sb.ToString();

            Canvas.ForceUpdateCanvases();
            if (scrollRect)
                scrollRect.verticalNormalizedPosition = 0f; // dno
        }

    }

    // če rabiš koordinate -> šahovski zapis
    public static string ToSquare(int x, int y)
    {
        char file = (char)('a' + x);
        char rank = (char)('1' + y);
        return $"{file}{rank}";
    }
    
    

    public void LogCoords(int fromX, int fromY, int toX, int toY)
    {
        Log($"{ToSquare(fromX, fromY)}-{ToSquare(toX, toY)}");
    }
}