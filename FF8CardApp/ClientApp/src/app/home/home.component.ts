import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent {
  public alphaCards: Card[];
  public betaCards: Card[];
  public game: Game;
  public http: HttpClient;
  public baseUrl: string;
  public isAlpha: boolean;
  public selectedCard: Card;
  public alphaScore: number;
  public betaScore: number;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http = http;
    this.baseUrl = baseUrl;
    this.isAlpha = true;
    this.game = new Game();
    this.alphaCards = []
    this.betaCards = []

    this.onReset();
  }

  onReset() {
    this.game = new Game();
    this.isAlpha = true;

    this.http.get<Card[]>(this.baseUrl + 'Card').subscribe(result => {
      this.alphaCards = result;
      this.calcScore();
    }, error => console.error(error));

    this.http.get<Card[]>(this.baseUrl + 'Card').subscribe(result => {
      this.betaCards = result;
      this.calcScore();
    }, error => console.error(error));
  }

  onClick() {
    this.http.post<Game>(this.baseUrl + 'Move/optimal', { alphaCards: this.alphaCards, betaCards: this.betaCards, game: this.game }).subscribe(result => {
      if (this.isAlpha) {
        this.alphaCards = this.alphaCards.filter(obj => obj.name != result.lastMove.card.name);
      } else {
        this.betaCards = this.betaCards.filter(obj => obj.name != result.lastMove.card.name);
      }
      this.isAlpha = !this.isAlpha;
      this.game = result;
      this.calcScore();
    }, error => console.error(error));
  }

  calcScore() {
    var count = 0;
    for (var i = 0; i < 3; i++) {
      for (var j = 0; j < 3; j++) {
        if (this.game.gameBoard[i + ':' + j] !== undefined) {
          count += this.game.gameBoard[i + ':' + j].isAlpha ? 1 : 0;
        }
      }
    }
    this.alphaScore = count + this.alphaCards.length;
    this.betaScore = 10 - this.alphaScore;
  }

  whoWon() {
    if (this.betaScore == this.alphaScore) {
      return "It's a TIE!"
    }
    if (this.betaScore < this.alphaScore) {
      return "You WIN!";
    }
    return "You LOSE!";
  }

  isGameOver() {
    var count = 0;
    for (var i = 0; i < 3; i++) {
      for (var j = 0; j < 3; j++) {
        if (this.game.gameBoard[i + ':' + j] !== undefined) {
          count++;
        }
      }
    }
    return count === 9;
  }

  onClickCard(event) {
    let value: string = (event.target as Element).id;
    this.selectedCard = this.alphaCards[value];

  }

  onClickSquare(event) {
    let value: string = (event.target as Element).id;
    if (this.selectedCard == null || this.selectedCard == undefined) {
      return;
    }

    let x = parseInt(value.split(":")[0]);
    let y = parseInt(value.split(":")[1]);

    this.http.post<Game>(this.baseUrl + 'Move', { game: this.game, card: this.selectedCard, X: x, y: y }).subscribe(result => {
      this.game = result;
      this.isAlpha = !this.isAlpha;
      this.alphaCards = this.alphaCards.filter(obj => obj.name != result.lastMove.card.name);
      this.calcScore();
      this.onClick();
    }, error => console.error(error));
    this.selectedCard = null;
  }
}

interface Move {
  x: number;
  y: number;
  card: Card;
  score: number;
}

interface GameSquare {
  card: Card;
  isAlpha: boolean;
}

class Game {
  gameBoard: Map<string, GameSquare>;
  lastMove: Move;
  constructor() {
    this.gameBoard = new Map();
  }
}

class Card {
  id: number;
  isAlpha: boolean;
  cardLevel: number;
  name: string;
  imageURL: string;
  n: string;
  s: string;
  e: string;
  w: string;
  type: string;
  element: string;
  location: string;
  modRatio: string;
  modItem: string;
}
