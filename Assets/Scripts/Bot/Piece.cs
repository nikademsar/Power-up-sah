namespace PowerUpChess.Engine
{
    public enum Piece : sbyte
    {
        Empty = 0, // na tem polju ni figure
        WP = 1,    // white pawn- kmet
        WN = 2,    // white knight - konj
        WB = 3,    // white bishop - tekac
        WR = 4,    // white rook - trdnjava
        WQ = 5,    // white queen - kraljica
        WK = 6,    // white king - kralj
        BP = -1,   // crni kmet
        BN = -2,   // crni konj
        BB = -3,   // crni tekac
        BR = -4,   // crna trdnjava
        BQ = -5,   // crna kraljica
        BK = -6    // crn kralj
    }

    public struct Move
    {
        public int fromX, fromY, toX, toY;
        public Piece captured;
        public Piece promotion;
    }
}
