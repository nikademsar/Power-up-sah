namespace PowerUpChess.Engine;

public class BoardState
{
    public Piece[,] b = new Piece[8, 8];
    public bool whiteToMove = true;

    public BoardState Clone()
    {
        var n = new BoardState { whiteToMove = whiteToMove };
        for (int x = 0; x <  8; x++)
        for (int y = 0; y < 8; y++)
            n.b[x, y] = b[x, y];
        return n;
    }


    public static BoardState CreateStartPosition()
    {
        var s = new BoardState { whiteToMove = true };
        
        //beli
        s.b[0, 0] = Piece.WR;
        s.b[1, 0] = Piece.WN;
        s.b[2, 0] = Piece.WB;
        s.b[3, 0] = Piece.WQ;
        s.b[4, 0] = Piece.WK;
        s.b[5, 0] = Piece.WB;
        s.b[6, 0] = Piece.WN;
        s.b[7, 0] = Piece.WR;
        for (int x = 0; x < 8; x++)
            s.b[x, 1] = Piece.WP;
        
        
        //crni
        s.b[0, 7] = Piece.BR;
        s.b[1, 7] = Piece.BN;
        s.b[2, 7] = Piece.BB;
        s.b[3, 7] = Piece.BQ;
        s.b[4, 7] = Piece.BK;
        s.b[5, 7] = Piece.BB;
        s.b[6, 7] = Piece.BN;
        s.b[7, 7] = Piece.BR;
        for (int x = 0; x < 8; x++)
            s.b[x, 6] = Piece.WP;

        return s;
    }
}
