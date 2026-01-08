using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MoveLoggerSimple : MonoBehaviour
{
    [SerializeField] private TMP_Text logText;
    [SerializeField] private ScrollRect scrollRect;


    [Header("Layout")]
    [SerializeField] private int leftColumnWidth = 10;
    [SerializeField] private int moveNoWidth = 4;


    private readonly StringBuilder sb = new StringBuilder(4096);

    private int fullMoveNumber = 1;     // 1., 2., 3. ...
    private bool waitingForBlack = false; // po white potezi čakamo black na isti vrstici

    public void Clear()
    {
        sb.Clear();
        fullMoveNumber = 1;
        waitingForBlack = false;
        if (logText) logText.text = "";
    }

    public void LogMove(bool isWhiteMove, string pieceName, int fromX, int fromY, int toX, int toY, bool isCapture)
    {
        if (!string.IsNullOrEmpty(pieceName))
            pieceName = pieceName.Replace("(Clone)", "").Trim();

        string moveText = FormatAlgebraic(pieceName, fromX, fromY, toX, toY, isCapture);
        AppendTwoColumns(moveText, isWhiteMove);

        if (logText) logText.text = sb.ToString();
        if (scrollRect)
        {
            Canvas.ForceUpdateCanvases();              // posodobi layout
            scrollRect.verticalNormalizedPosition = 0; // 0 = dno, 1 = vrh
            Canvas.ForceUpdateCanvases();
        }

    }


    private void AppendTwoColumns(string moveText, bool isWhiteMove)
    {
        if (isWhiteMove)
        {
            // če je prejšnja white ostala brez black (npr. extra turn), zaključi vrstico
            if (waitingForBlack)
            {
                sb.Append('\n');
                waitingForBlack = false;
                fullMoveNumber++;
            }

            sb.Append(fullMoveNumber).Append(". ");
            sb.Append(moveText.PadRight(leftColumnWidth));
            waitingForBlack = true;
        }
        else
        {
            // če nekako pride black brez white (redko), naredi prazno white kolono
            if (!waitingForBlack)
            {
                sb.Append(fullMoveNumber).Append(". ");
                sb.Append("".PadRight(leftColumnWidth));
            }

            sb.Append("   ").Append(moveText).Append('\n');
            waitingForBlack = false;
            fullMoveNumber++;
        }
    }

    private static string FormatAlgebraic(string pieceName, int fromX, int fromY, int toX, int toY, bool isCapture)
    {
        if (pieceName.EndsWith("_king") && Mathf.Abs(toX - fromX) == 2)
            return (toX > fromX) ? "O-O" : "O-O-O";

        string to = ToSquare(toX, toY);

        bool isPawn = pieceName.EndsWith("_pawn");
        char pieceLetter = PieceLetter(pieceName);

        if (isPawn)
        {
            if (isCapture)
            {
                char fromFile = (char)('a' + fromX);
                return $"{fromFile}x{to}";
            }
            return to;
        }

        string x = isCapture ? "x" : "";
        return $"{pieceLetter}{x}{to}";
    }

    private static char PieceLetter(string pieceName)
    {
        if (pieceName.EndsWith("_knight")) return 'N';
        if (pieceName.EndsWith("_bishop")) return 'B';
        if (pieceName.EndsWith("_rook"))   return 'R';
        if (pieceName.EndsWith("_queen"))  return 'Q';
        if (pieceName.EndsWith("_king"))   return 'K';
        return '\0';
    }

    public static string ToSquare(int x, int y)
    {
        char file = (char)('a' + x);
        char rank = (char)('1' + y);
        return $"{file}{rank}";
    }
}
