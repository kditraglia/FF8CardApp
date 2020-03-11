using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FF8CardApp.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FF8CardApp.Controllers
{
    public class OptimalMoveRequest
    {
        public Game Game { get; set; }
        public List<Card> AlphaCards { get; set; }
        public List<Card> BetaCards { get; set; }
    }

    public class MoveRequest
    {
        public Game Game { get; set; }

        public Card Card { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
    }

    [Route("[controller]")]
    [ApiController]
    public class MoveController : ControllerBase
    {
        [HttpPost]
        [Route("optimal")]
        public Game Optimal(OptimalMoveRequest optimalMoveRequest)
        {
            if (optimalMoveRequest.Game.isLastMove())
            {
                return optimalMoveRequest.Game;
            }
            optimalMoveRequest.Game.InvertBoard();
            Move m = GameSolver.BestMove(
                optimalMoveRequest.BetaCards, 
                optimalMoveRequest.AlphaCards,
                optimalMoveRequest.Game);
            optimalMoveRequest.Game.InvertBoard();
            optimalMoveRequest.Game.addCard(m.Card, m.X, m.Y, false);

            return optimalMoveRequest.Game;
        }

        [HttpPost]
        public Game Move(MoveRequest moveRequest)
        {
            moveRequest.Game.addCard(moveRequest.Card, moveRequest.X, moveRequest.Y, true);

            return moveRequest.Game;
        }
    }
}