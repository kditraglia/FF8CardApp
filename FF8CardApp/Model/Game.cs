using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FF8CardApp.Model
{
    [DebuggerDisplay("{Card} - {IsAlpha}")]
    public class GameSquare
    {
        public Card Card { get; set; }
        public bool IsAlpha { get; set; }

        public GameSquare()
        {

        }

        public GameSquare(Card card, bool isAlpha)
        {
            Card = card;
            IsAlpha = isAlpha;
        }
    }
    public class Game
    {
        public Dictionary<String, GameSquare> GameBoard { get; set; }
        public Move LastMove { get; set; }

        public Game()
        {
            GameBoard = new Dictionary<string, GameSquare>();
        }

        public void InvertBoard()
        {
            foreach(GameSquare gameSquare in GameBoard.Values)
            {
                gameSquare.IsAlpha = !gameSquare.IsAlpha;
            }
        }

        public bool isLastMove()
        {
            return GameBoard.Count >= 8;
        }

        public Move makeLastMove(Card card, bool isAlpha)
        {
            int lastX = -1;
            int lastY = -1;
            for (int xPos = 0; xPos < 3; xPos++)
            {
                for (int yPos = 0; yPos < 3; yPos++)
                {
                    if (!GameBoard.ContainsKey(keyString(xPos, yPos)))
                    {
                        lastX = xPos;
                        lastY = yPos;
                        break;
                    }
                }
            }

            return addCard(card, lastX, lastY, isAlpha);
        }

        public bool canPlay(int x, int y)
        {
            return !GameBoard.ContainsKey(keyString(x, y));
        }

        public Game clone()
        {
            Game clone = new Game();
            foreach (KeyValuePair<string, GameSquare> square in GameBoard)
            {
                clone.GameBoard.Add(square.Key, new GameSquare(square.Value.Card, square.Value.IsAlpha));
            }
            return clone;
        }

        public Move addCard(Card card, int x, int y, bool isAlpha)
        {
            GameBoard.Add(keyString(x, y), new GameSquare(card, isAlpha));

            Card westCard = x > 0 ? GameBoard.GetValueOrDefault(keyString(x - 1, y), null)?.Card : null;
            Card eastCard = x < 2 ? GameBoard.GetValueOrDefault(keyString(x + 1, y), null)?.Card : null;

            Card southCard = y < 2 ? GameBoard.GetValueOrDefault(keyString(x, y + 1), null)?.Card : null;
            Card northCard = y > 0 ? GameBoard.GetValueOrDefault(keyString(x, y - 1), null)?.Card : null;


            if (eastCard != null && getVal(eastCard.W) < getVal(card.E))
            {
                GameBoard.GetValueOrDefault(keyString(x + 1, y)).IsAlpha = isAlpha;
            }

            if (westCard != null && getVal(westCard.E) < getVal(card.W))
            {
                GameBoard.GetValueOrDefault(keyString(x - 1, y)).IsAlpha = isAlpha;
            }

            if (southCard != null && getVal(southCard.N) < getVal(card.S))
            {
                GameBoard.GetValueOrDefault(keyString(x, y + 1)).IsAlpha = isAlpha;
            }

            if (northCard != null && getVal(northCard.S) < getVal(card.N))
            {
                GameBoard.GetValueOrDefault(keyString(x, y - 1)).IsAlpha = isAlpha;
            }

            int score = GameBoard.Values.Where(sq => sq.IsAlpha).Count();
            LastMove = new Move() { X = x, Y = y, Card = card, Score = score };
            return LastMove;
        }

        private String keyString(int x, int y)
        {
            return string.Format("{0}:{1}", x, y);
        }

        private int getVal(char num)
        {
            if (num == 'A')
            {
                return 10;
            }
            return int.Parse(num.ToString());
        }
    }
}
