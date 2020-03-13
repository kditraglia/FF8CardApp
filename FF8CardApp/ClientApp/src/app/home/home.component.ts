import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatDialog, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Card } from '../model/card';
import { Move } from '../model/move';
import { Game } from '../model/game';
import { CardChooserComponent } from '../dialogs/card.dialog';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent {
  public alphaCards: Card[];
  public betaCards: Card[];
  public game: Game;
  public isAlpha: boolean;
  public selectedCard: Card;
  public alphaScore: number;
  public betaScore: number;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private dialog: MatDialog) {
    this.isAlpha = true;
    this.game = new Game();
    this.alphaCards = []
    this.betaCards = []

    this.onReset();
  }

  setAlphaCards() {
    this.dialog.open(CardChooserComponent, {
      data: {
        alphaCards: this.alphaCards
      }
    });
  }

  setBetaCards() {
    this.dialog.open(CardChooserComponent, {
      data: {
        alphaCards: this.betaCards
      }
    });
  }

  onReset() {
    this.game = new Game();
    this.isAlpha = true;

    this.http.get<Card[]>(this.baseUrl + 'card/random').subscribe(result => {
      this.alphaCards = result;
      this.calcScore();
    }, error => console.error(error));

    this.http.get<Card[]>(this.baseUrl + 'card/random').subscribe(result => {
      this.betaCards = result;
      this.calcScore();
    }, error => console.error(error));
  }

  goFirst() {
    this.isAlpha = false;
    this.onClick();
  }

  onClick() {
    this.http.post<Game>(this.baseUrl + 'move/optimal', { alphaCards: this.alphaCards, betaCards: this.betaCards, game: this.game }).subscribe(result => {
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

    this.http.post<Game>(this.baseUrl + 'move', { game: this.game, card: this.selectedCard, X: x, y: y }).subscribe(result => {
      this.game = result;
      this.isAlpha = !this.isAlpha;
      this.alphaCards = this.alphaCards.filter(obj => obj.name != result.lastMove.card.name);
      this.calcScore();
      this.onClick();
    }, error => console.error(error));
    this.selectedCard = null;
  }
}

