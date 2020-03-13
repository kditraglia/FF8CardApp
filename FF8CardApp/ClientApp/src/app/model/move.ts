import { Card } from './card';

export interface Move {
  x: number;
  y: number;
  card: Card;
  score: number;
}
