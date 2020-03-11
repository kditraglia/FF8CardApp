using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FF8CardApp.Model
{
    public class Game
    {
        public Card[][] board { get; set; }
        public bool?[][] boardOwnership { get; set; }
        public Move LastMove { get; set; }

        public Game()
        {
            //Ignore how ridiculous these declarations are, I was fighting with a serializer to send the whole game state to an angular app
            //I wrote to visual the results
            board = new Card[3][];
            board[0] = new Card[3];
            board[1] = new Card[3];
            board[2] = new Card[3];

            boardOwnership = new bool?[3][];
            boardOwnership[0] = new bool?[3];
            boardOwnership[1] = new bool?[3];
            boardOwnership[2] = new bool?[3];
        }

        public void invertBoard()
        {
            for (int xPos = 0; xPos < 3; xPos++)
            {
                for (int yPos = 0; yPos < 3; yPos++)
                {
                    if (board[xPos][yPos] != null)
                    {
                        boardOwnership[xPos][yPos] = !boardOwnership[xPos][yPos];
                    }
                }
            }
        }

        //Used as a short-cut in the recursion to realize we're at a terminal state
        public bool isLastMove()
        {
            int retVal = 0;
            for (int xPos = 0; xPos < 3; xPos++)
            {
                for (int yPos = 0; yPos < 3; yPos++)
                {
                    if (board[xPos][yPos] != null)
                    {
                        retVal += 1;
                    }
                }
            }

            return retVal >= 8;
        }

        public Move makeLastMove(Card card, bool isAlpha)
        {
            int lastX = -1;
            int lastY = -1;
            for (int xPos = 0; xPos < 3; xPos++)
            {
                for (int yPos = 0; yPos < 3; yPos++)
                {
                    if (board[xPos][yPos] == null)
                    {
                        lastX = xPos;
                        lastY = yPos;
                        break;
                    }
                }
            }

            return addCard(card, lastX, lastY, isAlpha);
        }

        //Simply checks if it's possible to play a card in a square
        public bool canPlay(int x, int y)
        {
            return board[x][y] == null;
        }

        //While traversing the recursive tree it becomes necessary to make moves on a cloned board to ensure each branch has it's own reference
        public Game clone()
        {
            Game clone = new Game();
            clone.board = board.Select(s => s.ToArray()).ToArray();
            clone.boardOwnership = boardOwnership.Select(s => s.ToArray()).ToArray();
            return clone;
        }

        //A simple function to convert the A to '10', cards in Triple Triad go from 1-A, A being worth 10
        public int getVal(char num)
        {
            if (num == 'A')
            {
                return 10;
            }
            return int.Parse(num.ToString());
        }

        //Adds the card and captures any cards possible.  Returns a score equal to the number of alpha cards on the board (and the move that produced it).
        public Move addCard(Card card, int x, int y, bool isAlpha)
        {
            board[x][y] = card;
            boardOwnership[x][y] = isAlpha;

            Card westCard = x > 0 ? board[x - 1][y] : null;
            Card eastCard = x < 2 ? board[x + 1][y] : null;

            Card southCard = y < 2 ? board[x][y + 1] : null;
            Card northCard = y > 0 ? board[x][y - 1] : null;


            if (eastCard != null && getVal(eastCard.W) < getVal(card.E))
            {
                boardOwnership[x + 1][y] = isAlpha;
            }

            if (westCard != null && getVal(westCard.E) < getVal(card.W))
            {
                boardOwnership[x - 1][y] = isAlpha;
            }

            if (southCard != null && getVal(southCard.N) < getVal(card.S))
            {
                boardOwnership[x][y + 1] = isAlpha;
            }

            if (northCard != null && getVal(northCard.S) < getVal(card.N))
            {
                boardOwnership[x][y - 1] = isAlpha;
            }

            int score = boardOwnership.SelectMany(o => o).Where(o => o.HasValue && o.Value).Count();
            LastMove = new Move() { X = x, Y = y, Card = card, Score = score };
            return LastMove;
        }
    }
}
