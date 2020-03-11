using FF8CardApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FF8CardApp
{
    public class GameSolver
    {

        public static Move BestMove(List<Card> alphaCards, List<Card> betaCards, Game g)
        {
            List<Move> possibleMoves = new List<Move>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (!g.canPlay(i, j))
                    {
                        continue;
                    }

                    for (int k = 0; k < alphaCards.Count; k++)
                    {
                        //Clone the list as to not interfere with other trees references
                        List<Card> newHand = new List<Card>(alphaCards);
                        Card playedCard = newHand[k];
                        newHand.RemoveAt(k);

                        //Clone the game to not interfere with other tree's reference to the game
                        Game cloneGame = g.clone();
                        cloneGame.addCard(playedCard, i, j, true);


                        int value = Solve(newHand, betaCards, cloneGame, false, int.MinValue, int.MaxValue);
                        Move m = new Move() { X = i, Y = j, Card = playedCard, Score = value };

                        possibleMoves.Add(m);
                    }
                }
            }

            return possibleMoves.Where(m => m.Score == possibleMoves.Max(m => m.Score)).First();
        }


        public static int Solve(List<Card> alphaCards, List<Card> betaCards, Game g, bool isAlpha, int alpha, int beta)
        {
            if (g.isLastMove())
            {
                Card lastCard = isAlpha ? alphaCards[0] : betaCards[0];

                Game cloneGame = g.clone();
                Move m = cloneGame.makeLastMove(lastCard, isAlpha);

                return m.Score;
            }

            List<Card> currentPlayerHand = isAlpha ? alphaCards : betaCards;

            if (isAlpha)
            {
                int bestVal = int.MinValue;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (!g.canPlay(i, j))
                        {
                            continue;
                        }

                        for (int k = 0; k < currentPlayerHand.Count; k++)
                        {
                            //Clone the list as to not interfere with other trees references
                            List<Card> newHand = new List<Card>(currentPlayerHand);
                            Card playedCard = newHand[k];
                            newHand.RemoveAt(k);

                            //Clone the game to not interfere with other tree's reference to the game
                            Game cloneGame = g.clone();
                            cloneGame.addCard(playedCard, i, j, isAlpha);

                            //Record the move as we go down the stack, so the terminal states know how we got here
                            int value = Solve(isAlpha ? newHand : alphaCards, isAlpha ? betaCards : newHand, cloneGame, !isAlpha, alpha, beta);
                            //Pop off the stack as we come back up

                            //Based on my understanding of the algorithm, take the max between the value and bestVal, and the max between bestVal and alpha
                            bestVal = bestVal > value ? bestVal : value;
                            alpha = bestVal > alpha ? bestVal : alpha;
                            if (beta <= alpha)
                            {
                                goto breakAlpha;
                            }

                        }
                    }
                }

                breakAlpha:
                return bestVal;
            }
            else
            {
                int bestVal = int.MaxValue;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (!g.canPlay(i, j))
                        {
                            continue;
                        }
                        for (int k = 0; k < currentPlayerHand.Count; k++)
                        {
                            //Clone the list as to not interfere with other trees references
                            List<Card> newHand = new List<Card>(currentPlayerHand);
                            Card playedCard = newHand[k];
                            newHand.RemoveAt(k);

                            //Clone the game to not interfere with other tree's reference to the game
                            Game cloneGame = g.clone();
                            cloneGame.addCard(playedCard, i, j, isAlpha);

                            int value = Solve(isAlpha ? newHand : alphaCards, isAlpha ? betaCards : newHand, cloneGame, !isAlpha, alpha, beta);

                            //Based on my understanding of the algorithm, take the min between the value and bestVal, and the min between bestVal and beta
                            bestVal = bestVal < value ? bestVal : value;
                            beta = bestVal < beta ? bestVal : beta;
                            if (beta <= alpha)
                            {
                                goto breakBeta;
                            }

                        }
                    }
                }

                breakBeta:
                return bestVal;
            }
        }
    }
}

