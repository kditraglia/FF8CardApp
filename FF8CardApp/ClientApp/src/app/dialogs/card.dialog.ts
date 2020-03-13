import { Component, Inject } from '@angular/core';
import { VERSION, MatDialogRef, MatDialog, MatSnackBar, MAT_DIALOG_DATA } from '@angular/material';
import { Card } from '../model/card';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'card-dialog',
  templateUrl: './card.dialog.html',
})


export class CardChooserComponent {
  public alphaCards: Card[];
  public availableCards: Card[];
  public selectedCard: Card;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string,
    @Inject(MAT_DIALOG_DATA) private data: any,
    private dialogRef: MatDialogRef<CardChooserComponent>) {
    if (data) {
      this.alphaCards = data.alphaCards;
    }
  }

  onClickCard(event) {
    let value: string = (event.target as Element).id;
    let index = value.slice(2);

    if (value.startsWith('a')) {
      if (this.selectedCard) {
        this.alphaCards[index] = this.selectedCard;
        this.selectedCard = null;
      } else {
        this.selectedCard = this.alphaCards[index];
      }
    }
    if (value.startsWith('b')) {
      if (this.selectedCard) {
        this.availableCards[index] = this.selectedCard;
        this.selectedCard = null;
      } else {
        this.selectedCard = this.availableCards[index];
      }
    }
  }

  onChange(event) {
    let value: string = event.target.value;
    this.http.get<Card[]>(this.baseUrl + 'card?s=' + value).subscribe(result => {
      this.availableCards = result;
    }, error => console.error(error));
  }
}
