using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OthelloIAG8
{
    public class BoardG8 : IPlayable.IPlayable
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public BoardG8()
        {
            NbTiles = 8;
            Reset();

            initNeighbour();
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nbTiles"></param>
        public BoardG8(int nbTiles = 8)
        {
            NbTiles = nbTiles;
            Reset();
            initNeighbour();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public BoardG8(BoardG8 other)
        {
            NbTiles = other.NbTiles;
            isWhite = other.isWhite;
            GameBoard = other.GameBoard.Clone() as int[,];
            neighbour = other.neighbour;
        }

        /// <summary>
        /// initNeighbour
        /// List that contains VectorInt
        /// </summary>
        private void initNeighbour()
        {
            neighbour = new List<IntVector>();
            neighbour.Add(new IntVector(0, 1));
            neighbour.Add(new IntVector(0, -1));
            neighbour.Add(new IntVector(1, 0));
            neighbour.Add(new IntVector(1, 1));
            neighbour.Add(new IntVector(1, -1));
            neighbour.Add(new IntVector(-1, 0));
            neighbour.Add(new IntVector(-1, 1));
            neighbour.Add(new IntVector(-1, -1));
        }

        /// <summary>
        /// Reset the game
        /// </summary>
        public void Reset()
        {
            isWhite = false;
            GameBoard = new int[NbTiles, NbTiles];
            int half = NbTiles / 2;
            GameBoard[half - 1, half - 1] = (int)TileState.Nsa;
            GameBoard[half, half] = (int)TileState.Nsa;
            GameBoard[half - 1, half] = (int)TileState.Anonymous;
            GameBoard[half, half - 1] = (int)TileState.Anonymous;
            BlackScore = WhiteScore = 2;
        }

        /// <summary>
        /// GetName
        /// </summary>
        /// <returns>return our name</returns>
        public string GetName()
        {
            return "Gander Laurent & Andre Neto Da Silva";
        }

        /// <summary>
        /// getBoard
        /// </summary>
        /// <returns>return an int[,] with the state of any tile</returns>
        public int[,] GetBoard()
        {
            int[,] board = new int[8, 8];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (GameBoard[i, j] == (int)TileState.Empty)
                        board[i, j] = -1;
                    else if (GameBoard[i, j] == (int)TileState.Nsa)
                        board[i, j] = 0;
                    else
                        board[i, j] = 1;
                }
            }

            return board;
        }
        /// <summary>
        /// IsPlayable
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        /// <param name="isWhite"></param>
        /// <returns>return true is the move is valid</returns>
        public bool IsPlayable(int column, int line, bool isWhite)
        {
            if ((GameBoard[column, line] != 0)) return false;

            int color = (int)TileState.Anonymous;
            if (isWhite) color = (int)TileState.Nsa;
            IntVector pos = new IntVector(column, line);

            foreach (var n in neighbour)
            {
                IntVector sideTile = pos + n;
                if (sideTile.IsValid(NbTiles) &&
                    GameBoard[sideTile.x, sideTile.y] != color &&
                    GameBoard[sideTile.x, sideTile.y] != 0)
                {
                    IntVector tile = sideTile + n;
                    while (tile.IsValid(NbTiles) &&
                           GameBoard[tile.x, tile.y] != color &&
                           GameBoard[tile.x, tile.y] != 0)
                    {
                        tile = tile + n;
                    }
                    if (tile.IsValid(NbTiles) && GameBoard[tile.x, tile.y] == color)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// PlayMove
        /// </summary>
        /// <param name="column"></param>
        /// <param name="line"></param>
        /// <param name="isWhite"></param>
        /// <returns>return the board status if the move is valid</returns>
        public bool PlayMove(int column, int line, bool isWhite)
        {
            if (IsPlayable(column, line, isWhite))
            {
                int color = (int)TileState.Anonymous;
                if (isWhite) color = (int)TileState.Nsa;

                IntVector pos = new IntVector(column, line);
                List<Tuple<int, int>> pawnsToReplace = new List<Tuple<int, int>>();


                foreach (var n in neighbour)
                {
                    List<Tuple<int, int>> tmp = new List<Tuple<int, int>>();
                    IntVector tile = pos + n;

                    while (tile.IsValid(NbTiles) &&
                           GameBoard[tile.x, tile.y] != color &&
                           GameBoard[tile.x, tile.y] != 0)
                    {
                        tmp.Add(new Tuple<int, int>(tile.x, tile.y));
                        tile = tile + n;
                    }

                    if (tile.IsValid(NbTiles) && GameBoard[tile.x, tile.y] == color)
                    {
                        pawnsToReplace.Add(new Tuple<int, int>(column, line));
                        pawnsToReplace.AddRange(tmp);
                    }
                }


                foreach (var pair in pawnsToReplace)
                {
                    GameBoard[pair.Item1, pair.Item2] = color;
                }

                BlackScore = GetBlackScore();
                WhiteScore = GetWhiteScore();

                if (BlackScore + WhiteScore > 63)
                {
                    for (int i = 0; i < scoreMatrix.GetLength(0); i++)
                    {
                        for (int j = 0; j < scoreMatrix.GetLength(1); j++)
                        {
                            scoreMatrix[i, j] = 666;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// getNextMove
        /// </summary>
        /// <param name="game"></param>
        /// <param name="level"></param>
        /// <param name="whiteTurn"></param>
        /// <returns>return a tuple with the next move</returns>
        public Tuple<int, int> GetNextMove(int[,] game, int level, bool whiteTurn)
        {
            isWhite = whiteTurn;
            List<Tuple<int, int>> moves = this.MovesPossible(whiteTurn);
            if (moves.Count > 0)
                return MinMax(this, 5, 1, Int32.MaxValue).Item2;
            else
                return new Tuple<int, int>(-1, -1);
        }

        /// <summary>
        /// MinMax
        /// </summary>
        /// <param name="state"></param>
        /// <param name="depth"></param>
        /// <param name="minOrMax"></param>
        /// <param name="parentValue"></param>
        /// <returns>return the "best" possible move</returns>
        private Tuple<int, Tuple<int, int>> MinMax(BoardG8 state, int depth, int minOrMax, int parentValue)
        {
            List<Tuple<int, int>> moves = this.MovesPossible(state.isWhite);
            if (depth == 0 || moves.Count == 0)
            {
                return new Tuple<int, Tuple<int, int>>(state.Evaluation(), null);
            }
            else
            {
                int optVal = minOrMax * -6666666;
                Tuple<int, int> optOp = null;
                foreach (var move in moves)
                {
                    BoardG8 nextState = state.Apply(move);
                    int next = MinMax(nextState, depth - 1, -minOrMax, optVal).Item1;
                    if (next * minOrMax > optVal * minOrMax)
                    {
                        optVal = next;
                        optOp = move;
                        if (optVal * minOrMax > parentValue * minOrMax)
                            break;
                    }
                }
                return new Tuple<int, Tuple<int, int>>(optVal, optOp);
            }
        }
        /// <summary>
        /// Evaluation
        /// </summary>
        /// <returns>return the evaluate score for a tile</returns>
        private int Evaluation()
        {
            const int weightMobility = 4;
            const int weightScore = 6;
            int numPawns = anonymousScore + nsaScore;

            int mobi = this.MovesPossible(this.isWhite).Count;
            int score = 0;



            for (int i = 0; i < NbTiles; i++)
            {
                for (int j = 0; j < NbTiles; j++)
                {
                    int pawn = GameBoard[i, j];
                    score += pawn * scoreMatrix[i, j];
                }
            }

            if (!isWhite) score = -score;

            return (64 - numPawns) * weightMobility * mobi + numPawns * weightScore * score;
        }
        /// <summary>
        /// Apply
        /// </summary>
        /// <param name="move"></param>
        /// <returns>return a BoardG8 with the new state</returns>
        private BoardG8 Apply(Tuple<int, int> move)
        {
            BoardG8 newState = new BoardG8(this);
            newState.PlayMove(move.Item1, move.Item2, newState.isWhite);
            return newState;
        }

        /// <summary>
        /// MovesPossible
        /// </summary>
        /// <param name="whiteTurn"></param>
        /// <returns>return a list of tuple than contains the possibles moves</returns>
        private List<Tuple<int, int>> MovesPossible(bool whiteTurn)
        {
            List<Tuple<int, int>> moves = new List<Tuple<int, int>>();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (IsPlayable(i, j, whiteTurn))
                        moves.Add(new Tuple<int, int>(i, j));
                }
            }
            return moves;
        }
        /// <summary>
        /// GetWhiteScore
        /// </summary>
        /// <returns>return the score of NSA</returns>
        public int GetWhiteScore()
        {
            int score = 0;
            foreach (var i in GameBoard)
            {
                if (i == 1)
                {
                    score++;
                }
            }
            return score;
        }
        /// <summary>
        /// GetBlackScore
        /// </summary>
        /// <returns>return the score of Anonymous</returns>
        public int GetBlackScore()
        {
            int score = 0;
            foreach (var i in GameBoard)
            {
                if (i == -1)
                {
                    score++;
                }
            }
            return score;
        }
        /// <summary>
        /// Function IsWhiteTurn
        /// </summary>
        /// <returns>isWhite</returns>
        public bool IsWhiteTurn()
        {
            return isWhite;
        }


        /********************************************************************/
        /*                          Private Attibuts                        */
        /********************************************************************/
        private int[,] scoreMatrix =
        {
            {60, 5, 30, 27, 27, 30, 5, 60},
            {5, 0, 20, 20, 20, 20, 0, 5},
            {30, 20, 40, 30, 30, 40, 20, 30},
            {27, 20, 30, 40, 40, 30, 20, 27},
            {27, 20, 30, 40, 40, 30, 20, 27},
            {30, 20, 40, 30, 30, 40, 20, 30},
            {5, 0, 20, 20, 20, 20, 0, 5},
            {60, 5, 30, 27, 27, 30, 5, 60}
        };

        private List<IntVector> neighbour;
        private bool isWhite;
        private int nsaScore;
        private int anonymousScore;

        /********************************************************************/
        /*                             Property                             */
        /********************************************************************/

        public int[,] GameBoard { get; set; }

        public int WhiteScore
        {
            get { return nsaScore; }

            set
            { nsaScore = value; }
        }
        public int BlackScore
        {
            get { return anonymousScore; }

            set { anonymousScore = value; }
        }
        public int NbTiles { get; private set; }


        /********************************************************************/
        /*                              Enum                                */
        /********************************************************************/
        public enum TileState
        {
            Empty = 0,
            Anonymous = -1,
            Nsa = 1
        };


        /********************************************************************/
        /*                              Structure                           */
        /********************************************************************/
        public struct IntVector
        {
            public IntVector(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

            public int x;
            public int y;

            public static IntVector operator +(IntVector v1, IntVector v2)
            {
                return new IntVector(v1.x + v2.x, v1.y + v2.y);
            }

            public bool IsValid(int size)
            {
                bool response = true;

                response &= (this.x >= 0 && this.x < size);
                response &= (this.y >= 0 && this.y < size);

                return response;
            }
        }
    }
}
