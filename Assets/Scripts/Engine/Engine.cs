namespace PowerUpChess.Engine;

public class Engine
{

    public static List<Move> GenerateMoves(BoardState s)
        /*
         * pogleda celo sahounico
         * sestavi seznam vseh možnih potez za igralca ki je na vrsti
         * -----preskoci prazna polja, preskoci fig nasprotnika, za vcsako figuro poklive addPieceMove()
         * vrne list vseh moves
         */
    {
        var moves = new List<Move>(64);
        bool white = s.whiteToMove;

        for (int x = 0; x < 8; x++)
        for (int y = 0; y < 8; y++)
        {
            var p = s.b[x, y];
            if (p == Piece.Empty) continue;
            if (white && (sbyte)p < 0) continue;
            if (!white && (sbyte)p < 0) continue;

            AddPieceMoves(s, x, y, p, moves);
        }
        
        return moves;
    }

    static void AddPieceMoves(BoardState s, int x, int y, Piece p, List<Move> moves)
        /*
         * to je samo usmerjevalnik - glede na figuro poklice pravo  funkcijo za generiranje potez
         */

    {
        switch (p)
        {
            case Piece.WR:
            case Piece.BR:
                AddLine(s, x, y, 1, 0, moves);
                AddLine(s, x, y, -1, 0, moves);
                AddLine(s, x, y, 0, 1, moves);
                AddLine(s, x, y, 0, -1, moves);
                break;
            
            case Piece.WB:
            case Piece.BB:
                AddLine(s, x, y, 1, 1, moves);
                AddLine(s, x, y, -1, -1, moves);
                AddLine(s, x, y, -1, 1, moves);
                AddLine(s, x, y, 1, -1, moves);
                break;
            
            case Piece.WQ:
            case Piece.BQ:
                AddLine(s, x, y, 1, 1, moves);
                AddLine(s, x, y, -1, -1, moves);
                AddLine(s, x, y, -1, 1, moves);
                AddLine(s, x, y, 1, -1, moves);
                AddLine(s, x, y, 1, 0, moves);
                AddLine(s, x, y, -1, 0, moves);
                AddLine(s, x, y, 0, 1, moves);
                AddLine(s, x, y, 0, -1, moves);
                break;
            
            case Piece.WN:
            case Piece.BN:
                AddKnight(s, x, y, moves);
                break;
            
            case Piece.WP:
                AddPawn(s, x, y, +1, moves);
                break;
            
            case Piece.BP:
                AddPawn(s, x, y, -1, moves);
                break;
            
            case Piece.WK:
            case Piece.BK:
                AddKing(s, x, y, moves);
                break;
        }
    }

    static void AddLine(BoardState s, int x, int y, int dx, int dy, List<Move> moves)
        /*
         * generira poteze za drseće figure (tekac, kraljica, trdnjava)
         * ko zadane prvo zasedeno polje, ce je tam nasprotnik ga poje in konca, ce je tam svoja figura ne doda nic in koonca
         */
    {
        var me = s.b[x, y];
        int nx = x + dx;
        int ny = y + dy;

        while (OnBoard(nx, ny) && s.b[nx, ny] == Piece.Empty)
        {
            moves.Add(new Move
            {
                fromX = x, fromY = y, toX = nx, toY = ny, captured = Piece.Empty, promotion = Piece.Empty
            });
        }
    }

    static void AddKnight(BoardState s, int x, int y, List<Move> moves)
        /*
         * poteze konja - 8 možnih L skokov (ignorira cilje, če so izven plošče, če je nasprotnik ga poje, ...)
         */
    {
        int[] xs = { +1, +2, +2, +1, -1, -2, -2, -1 };
        int[] ys = { +1, +2, +2, +1, -1, -2, -2, -1 };
        var me = s.b[x, y];

        for (int i = 0; i < 8; i++)
        {
            int nx = x + xs[i];
            int ny = y + ys[i];
            
            if (!OnBoard(nx, ny)) continue;
            
            var t = s.b[nx, ny];
            if ((t == Piece.Empty) || IsEnemy(me, t)) moves.Add(new Move { fromX = x, fromY = y, toX = nx, toY = ny, captured = t, promotion = Piece.Empty });
            {
                
            }
        }
    }

    static void AddKing(BoardState s, int x, int y, List<Move> moves)
        /*
         * tgenerira poteze za kralja za vseh 8 polj (sosednih) razen za tisto, kjer je on
         */
    {
        var me = s.b[x, y];
        for (int dx = -1; dx <= 1; dx++)
        for (int dy = -1; dy <= 1; dy++)
        {
            if (dx == 0 && dy == 0) continue;
            int nx = x + dx;
            int ny = y + dy;
            
            var t = s.b[nx, ny];
            if (t == Piece.Empty || IsEnemy(me, t)) 
                moves.Add(new Move { fromX = x, fromY = y, toX = nx, toY = ny, captured = t, promotion = Piece.Empty });
        }

    }

    static void AddPawn(BoardState s, int x, int y, int dir, List<Move> moves)
        /*
         * poteze kmeta (en korak naprej če je polje prazno, diagonalni zajem - ni se dodan zacetni korak za 2, en passant in sprememba kmeta v nekaj ko pride do konca)
         */
    {
        var me = s.b[x, y];

        int ny = y + dir;
        //naprej
        if (OnBoard(x, y) && s.b[x, y] == Piece.Empty)
            moves.Add(new Move { fromX = x, fromY = y, toX = x, toY =  ny, captured = Piece.Empty, promotion = Piece.Empty });
        
        // kmet poje
        if (OnBoard(x+1, ny) && s.b[x+1, ny] != Piece.Empty && IsEnemy(me, s.b[x+1, ny]))
            moves.Add(new Move { fromX=x, fromY=y, toX=x+1, toY=ny, captured=s.b[x+1, ny], promotion=Piece.Empty });

        if (OnBoard(x-1, ny) && s.b[x-1, ny] != Piece.Empty && IsEnemy(me, s.b[x-1, ny]))
            moves.Add(new Move { fromX=x, fromY=y, toX=x-1, toY=ny, captured=s.b[x-1, ny], promotion=Piece.Empty });
        
    }
    
        public static void MakeMove(BoardState s, ref Move m)
            /*
             * ta izvede potezo na unitiju
             *      v m.captured shrani figuro za undo
             *      premakne figuro
             *      izvorno polje nastavi na empty
             *      zamenja igralca na potezi
             */
    {
        m.captured = s.b[m.toX, m.toY];
        var piece = s.b[m.fromX, m.fromY];
        s.b[m.toX, m.toY] = piece;
        s.b[m.fromX, m.fromY] = Piece.Empty;
        s.whiteToMove = !s.whiteToMove;
    }

    public static void UndoMove(BoardState s, Move m)
        /*
         * razveljavi potezo, ki jo je nardil MakeMove - obrne white to move nazaj, premakne nazaj na izvorno polje
         *      će ke koga pojedel ga vrne na polje
         */
    {
        s.whiteToMove = !s.whiteToMove;
        var piece = s.b[m.toX, m.toY];
        s.b[m.fromX, m.fromY] = piece;
        s.b[m.toX, m.toY] = m.captured;
    }

    public static int Eval(BoardState s)
        /*
         * oceni pozicijo - vrne score (celo stevilo)
         *      - gre cez vsa polja + sešteje PieceValue za vsako figuro
         *      REZULTAT:
         *          poz: prednost ma beli
         *          neg: prednost ma črni
         */
    {
        int score = 0;
        for (int x=0;x<8;x++)
        for (int y=0;y<8;y++)
            score += PieceValue(s.b[x,y]);
        return score;
    }

    static int PieceValue(Piece p) => p switch
        /*
         * kmet: 100, konj: 320, lovec/tekac: 330, trdnjava: 500, kraljica: 900, kralj 200000, prazno: 0
         * črne majo iste vrednosti samo negativne
         */
    {
        Piece.WP => 100,  Piece.WN => 320,  Piece.WB => 330,  Piece.WR => 500,  Piece.WQ => 900,  Piece.WK => 20000,
        Piece.BP => -100, Piece.BN => -320, Piece.BB => -330, Piece.BR => -500, Piece.BQ => -900, Piece.BK => -20000,
        _ => 0
    };

    public static (Move best, int score) SearchBestMove(BoardState s, int depth)
        /*
         * to je funkcija ki jo klicemo da AI izbere potezo
         *      - nastavi zacetne meje alpha in beta
         *      - doloci color: beli +1, crni -1
         *      - klice megamax() -> da dobi najbolso potezo
         *
         * vrne best (najboljsa poteza) in score
         */
    {
        Move best = default;
        int alpha = int.MinValue + 1;
        int beta = int.MaxValue;
        int color = s.whiteToMove ? 1 : -1;

        int score = Negamax(s, depth, alpha, beta, color, ref best, root:true);
        return (best, score);
    }

    static int Negamax(BoardState s, int depth, int alpha, int beta, int color, ref Move bestMove, bool root=false)
        /*
         * MINIMAX + ALPHA BETA OBREZOVANJE
         *
         * če je globina 0 vrne color + Eval(s) - color nardi predznak da gre ocena v prid unega ki je na potezi
         * generira poteze
         * poteze uredi tako, da preizkusa zajeme (hitreje najde dobre poteze -> boljše obrezovanje)
         * za vsako potezo:
         *      - MakeMove
         *      - rekurzivni izracun rezultata
         *      - UndoMove
         *      - posodobi best score in alpha
         *      - če je alpha >= beta prekine (pruning)
         * če je root == true, shrani  najboljso potezo v bestMove
         */
    {
        if (depth == 0) return color * Eval(s);

        var moves = GenerateMoves(s);
        if (moves.Count == 0) return color * Eval(s);

        // capture-first ordering
        moves.Sort((a,b) => Math.Abs(PieceValue(b.captured)).CompareTo(Math.Abs(PieceValue(a.captured))));

        int bestScore = int.MinValue + 1;
        Move localBest = default;

        foreach (var raw in moves)
        {
            var m = raw;
            MakeMove(s, ref m);

            Move dummy = default;
            int score = -Negamax(s, depth-1, -beta, -alpha, -color, ref dummy);

            UndoMove(s, m);

            if (score > bestScore) { bestScore = score; localBest = m; }
            if (score > alpha) alpha = score;
            if (alpha >= beta) break;
        }

        if (root) bestMove = localBest;
        return bestScore;
    }

    static bool OnBoard(int x, int y) => x >= 0 && y >= 0 && x < 8 && y < 8;
    /*
     * preveri ali je koordinata znotraj šahovnice
     */

    static bool IsEnemy(Piece me, Piece other)
        /*
         * preveri če je other nasprotnik od me (uporaba pri zajemih -> ko poje nasprotnika)
         */
    {
        if (me == Piece.Empty || other == Piece.Empty) return false;
        return ((sbyte)me > 0 && (sbyte)other < 0) || ((sbyte)me < 0 && (sbyte)other > 0);
    }
    
}