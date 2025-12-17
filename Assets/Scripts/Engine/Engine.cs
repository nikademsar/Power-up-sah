using System;
using System.Collections.Generic;

namespace PowerUpChess.Engine
{
    /// <summary>
    /// Glavni šahovski engine:
    /// - generira poteze
    /// - izvaja / razveljavlja poteze
    /// - ocenjuje pozicijo
    /// - išče najboljšo potezo (Negamax + alpha-beta)
    /// </summary>
    public class Engine
    {
        /// <summary>
        /// Generira vse možne (psevdo-legalne) poteze za igralca na potezi.
        /// Ne preverja šaha kralju.
        /// </summary>
        public static List<Move> GenerateMoves(BoardState s)
        {
            var moves = new List<Move>(64);
            bool white = s.whiteToMove;

            // Prehod čez celo šahovnico
            for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
            {
                var p = s.b[x, y];
                if (p == Piece.Empty) continue;

                // filtriranje figur glede na igralca na potezi
                if (white && (sbyte)p < 0) continue;     // beli na potezi → preskoči črne
                if (!white && (sbyte)p > 0) continue;    // črni na potezi → preskoči bele

                AddPieceMoves(s, x, y, p, moves);
            }

            return moves;
        }

        /// <summary>
        /// Glede na tip figure kliče ustrezno metodo za generiranje potez.
        /// </summary>
        static void AddPieceMoves(BoardState s, int x, int y, Piece p, List<Move> moves)
        {
            switch (p)
            {
                case Piece.WR:
                case Piece.BR:
                    // trdnjava: vodoravno in navpično
                    AddLine(s, x, y, 1, 0, moves);
                    AddLine(s, x, y, -1, 0, moves);
                    AddLine(s, x, y, 0, 1, moves);
                    AddLine(s, x, y, 0, -1, moves);
                    break;

                case Piece.WB:
                case Piece.BB:
                    // lovec: diagonale
                    AddLine(s, x, y, 1, 1, moves);
                    AddLine(s, x, y, -1, -1, moves);
                    AddLine(s, x, y, -1, 1, moves);
                    AddLine(s, x, y, 1, -1, moves);
                    break;

                case Piece.WQ:
                case Piece.BQ:
                    // dama: lovec + trdnjava
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
                    // skakač
                    AddKnight(s, x, y, moves);
                    break;

                case Piece.WP:
                    // beli kmet (premik gor)
                    AddPawn(s, x, y, +1, moves);
                    break;

                case Piece.BP:
                    // črni kmet (premik dol)
                    AddPawn(s, x, y, -1, moves);
                    break;

                case Piece.WK:
                case Piece.BK:
                    // kralj
                    AddKing(s, x, y, moves);
                    break;
            }
        }

        /// <summary>
        /// Generiranje potez za figure, ki se gibljejo po linijah (lovec, trdnjava, dama).
        /// </summary>
        static void AddLine(BoardState s, int x, int y, int dx, int dy, List<Move> moves)
        {
            var me = s.b[x, y];
            int nx = x + dx;
            int ny = y + dy;

            // prazna polja
            while (OnBoard(nx, ny) && s.b[nx, ny] == Piece.Empty)
            {
                moves.Add(new Move
                {
                    fromX = x, fromY = y,
                    toX = nx, toY = ny,
                    captured = Piece.Empty,
                    promotion = Piece.Empty
                });

                nx += dx;
                ny += dy;
            }

            // zajem nasprotnikove figure
            if (OnBoard(nx, ny))
            {
                var t = s.b[nx, ny];
                if (IsEnemy(me, t))
                {
                    moves.Add(new Move
                    {
                        fromX = x, fromY = y,
                        toX = nx, toY = ny,
                        captured = t,
                        promotion = Piece.Empty
                    });
                }
            }
        }

        /// <summary>
        /// Generira vse možne poteze skakača.
        /// </summary>
        static void AddKnight(BoardState s, int x, int y, List<Move> moves)
        {
            int[] xs = { +1, +2, +2, +1, -1, -2, -2, -1 };
            int[] ys = { +2, +1, -1, -2, -2, -1, +1, +2 };

            var me = s.b[x, y];

            for (int i = 0; i < 8; i++)
            {
                int nx = x + xs[i];
                int ny = y + ys[i];
                if (!OnBoard(nx, ny)) continue;

                var t = s.b[nx, ny];
                if (t == Piece.Empty || IsEnemy(me, t))
                {
                    moves.Add(new Move
                    {
                        fromX = x, fromY = y,
                        toX = nx, toY = ny,
                        captured = t,
                        promotion = Piece.Empty
                    });
                }
            }
        }

        /// <summary>
        /// Generira poteze kralja (brez rokade, brez preverjanja šaha).
        /// </summary>
        static void AddKing(BoardState s, int x, int y, List<Move> moves)
        {
            var me = s.b[x, y];

            for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;

                int nx = x + dx;
                int ny = y + dy;
                if (!OnBoard(nx, ny)) continue;

                var t = s.b[nx, ny];
                if (t == Piece.Empty || IsEnemy(me, t))
                {
                    moves.Add(new Move
                    {
                        fromX = x, fromY = y,
                        toX = nx, toY = ny,
                        captured = t,
                        promotion = Piece.Empty
                    });
                }
            }
        }

        /// <summary>
        /// Generira poteze kmeta (brez začetnega dvojnega koraka, en-passant in promocije).
        /// </summary>
        static void AddPawn(BoardState s, int x, int y, int dir, List<Move> moves)
        {
            var me = s.b[x, y];
            int ny = y + dir;

            // premik naprej
            if (OnBoard(x, ny) && s.b[x, ny] == Piece.Empty)
            {
                moves.Add(new Move
                {
                    fromX = x, fromY = y,
                    toX = x, toY = ny,
                    captured = Piece.Empty,
                    promotion = Piece.Empty
                });
            }

            // zajemi diagonalno
            if (OnBoard(x + 1, ny) && s.b[x + 1, ny] != Piece.Empty && IsEnemy(me, s.b[x + 1, ny]))
            {
                moves.Add(new Move { fromX = x, fromY = y, toX = x + 1, toY = ny, captured = s.b[x + 1, ny], promotion = Piece.Empty });
            }

            if (OnBoard(x - 1, ny) && s.b[x - 1, ny] != Piece.Empty && IsEnemy(me, s.b[x - 1, ny]))
            {
                moves.Add(new Move { fromX = x, fromY = y, toX = x - 1, toY = ny, captured = s.b[x - 1, ny], promotion = Piece.Empty });
            }
        }

        /// <summary>
        /// Izvede potezo na BoardState (uporablja se v iskanju).
        /// </summary>
        public static void MakeMove(BoardState s, ref Move m)
        {
            m.captured = s.b[m.toX, m.toY];
            var piece = s.b[m.fromX, m.fromY];
            s.b[m.toX, m.toY] = piece;
            s.b[m.fromX, m.fromY] = Piece.Empty;
            s.whiteToMove = !s.whiteToMove;
        }

        /// <summary>
        /// Razveljavi potezo (undo) – nujno za iskanje.
        /// </summary>
        public static void UndoMove(BoardState s, Move m)
        {
            s.whiteToMove = !s.whiteToMove;
            var piece = s.b[m.toX, m.toY];
            s.b[m.fromX, m.fromY] = piece;
            s.b[m.toX, m.toY] = m.captured;
        }

        /// <summary>
        /// Oceni pozicijo na podlagi materiala.
        /// Pozitivno = prednost belih.
        /// </summary>
        public static int Eval(BoardState s)
        {
            int score = 0;
            for (int x = 0; x < 8; x++)
            for (int y = 0; y < 8; y++)
                score += PieceValue(s.b[x, y]);
            return score;
        }

        /// <summary>
        /// Vrednosti figur.
        /// </summary>
        static int PieceValue(Piece p) => p switch
        {
            Piece.WP => 100,  Piece.WN => 320,  Piece.WB => 330,  Piece.WR => 500,  Piece.WQ => 900,  Piece.WK => 20000,
            Piece.BP => -100, Piece.BN => -320, Piece.BB => -330, Piece.BR => -500, Piece.BQ => -900, Piece.BK => -20000,
            _ => 0
        };

        /// <summary>
        /// Poišče najboljšo potezo z Negamax + alpha-beta.
        /// </summary>
        public static (Move best, int score) SearchBestMove(BoardState s, int depth)
        {
            Move best = default;
            int alpha = int.MinValue + 1;
            int beta = int.MaxValue;
            int color = s.whiteToMove ? 1 : -1;

            int score = Negamax(s, depth, alpha, beta, color, ref best, root: true);
            return (best, score);
        }

        /// <summary>
        /// Rekurzivni Negamax algoritem z alpha-beta rezanjem.
        /// </summary>
        static int Negamax(BoardState s, int depth, int alpha, int beta, int color, ref Move bestMove, bool root = false)
        {
            if (depth == 0) return color * Eval(s);

            var moves = GenerateMoves(s);
            if (moves.Count == 0) return color * Eval(s);

            // osnovno urejanje potez (zajemi prej)
            moves.Sort((a, b) => Math.Abs(PieceValue(b.captured)).CompareTo(Math.Abs(PieceValue(a.captured))));

            int bestScore = int.MinValue + 1;
            Move localBest = default;

            foreach (var raw in moves)
            {
                var m = raw;
                MakeMove(s, ref m);

                Move dummy = default;
                int score = -Negamax(s, depth - 1, -beta, -alpha, -color, ref dummy);

                UndoMove(s, m);

                if (score > bestScore) { bestScore = score; localBest = m; }
                if (score > alpha) alpha = score;
                if (alpha >= beta) break;
            }

            if (root) bestMove = localBest;
            return bestScore;
        }

        static bool OnBoard(int x, int y) => x >= 0 && y >= 0 && x < 8 && y < 8;

        /// <summary>
        /// Preveri, ali sta figuri nasprotnika.
        /// </summary>
        static bool IsEnemy(Piece me, Piece other)
        {
            if (me == Piece.Empty || other == Piece.Empty) return false;
            return ((sbyte)me > 0 && (sbyte)other < 0) || ((sbyte)me < 0 && (sbyte)other > 0);
        }
    }
}
