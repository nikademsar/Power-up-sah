using System.Text;
using UnityEngine;
using TMPro;

public class MoveLogger : MonoBehaviour
{
    [SerializeField] private TMP_Text logText;

    private readonly StringBuilder sb = new StringBuilder(4096);

    // 0 = white half-move, 1 = black half-move, 2 = white, ...
    private int ply = 0;

    public void Clear()
    {
        sb.Clear();
        ply = 0;
        if (logText) logText.text = "";
    }

    // whiteMove tu ni več potreben za formatiranje (lahko ga še vedno pošiljaš zaradi info)
    public void Log(bool whiteMove, int fromX, int fromY, int toX, int toY, string san = null)
    {
        string moveText = san ?? $"{ToSquare(fromX, fromY)}-{ToSquare(toX, toY)}";

        int moveNumber = (ply / 2) + 1;
        bool isWhiteSlot = (ply % 2 == 0);

        if (isWhiteSlot)
        {
            sb.Append(moveNumber).Append(". ").Append(moveText);
        }
        else
        {
            sb.Append("  ").Append(moveText).Append('\n');
        }

        ply++;

        if (logText) logText.text = sb.ToString();
    }

    public static string ToSquare(int x, int y)
    {
        char file = (char)('a' + x);
        char rank = (char)('1' + y);
        return $"{file}{rank}";
    }
}