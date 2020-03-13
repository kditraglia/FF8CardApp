import { GameSquare } from './game.square';
import { Move } from './move';

export class Game {
  gameBoard: Map<string, GameSquare>;
  lastMove: Move;
  constructor() {
    this.gameBoard = new Map();
  }
}
