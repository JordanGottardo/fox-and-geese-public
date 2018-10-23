using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FoxAndGeese {

	public enum PawnType {
		None, //0
		Fox, //1
		Goose, //2	
	}
	
	public class Game {
		private int geeseNumber;

		public const string beginGameNotification = "Game.BeginGameNotification"; //iniziata partita
		public const string movePawnNotification = "Game.MovePawnNotification"; //mossa pedina
		public const string changeTurnNotification = "Game.ChangeTurnNotification"; //cambiato turno
		public const string endGameNotification = "Game.EndGameNotification"; //finita partita
		//aggiunte da me
		public const string placePawnNotification = "Game.PlacePawnNotification";
		public const string pawnEatenNotification = "Game.pawnEatenNotification";
		public const string canEatAnotherTimeNotification = "Game.canEatAnotherTimeNotification";

		public PawnType turn { get; private set; } // inizia sempre l'oca
		public PawnType winner { get; private set; }
		public PawnType[,] board { get; private set; }
        private Dictionary<string, PawnType[,]> winningBoards;
        private Dictionary<string, List<Move>> correctMoves;
        private bool hasJustEaten = false;

        //costruttore
		public Game(int geeseNumber) {
			this.geeseNumber = geeseNumber;
			board = new PawnType[7, 7]; //inizializzata a 0, quindi con tutti PawnType a None
            winningBoards = MyUtility.CreateWinningBoards();
            correctMoves = MyUtility.CreateCorrectMoves();
			//InitializeBoard();
			Reset();
			this.PostNotification(beginGameNotification);
		}

        // inizializza la board
        private void InitializeBoard() {
            PlacePawn(new Placement(PawnType.Fox, 2, 3)); //piazza la volpe

            for (int i = 0; i < 7; i++) {
                PlacePawn(new Placement(PawnType.Goose, 4, i));
            }

            for (int i = 2; i < 5; i++) {
                PlacePawn(new Placement(PawnType.Goose, 5, i));
            }

            for (int i = 2; i < 5; i++) {
                PlacePawn(new Placement(PawnType.Goose, 6, i));
            }

            if (geeseNumber >= 15) {
                PlacePawn(new Placement(PawnType.Goose, 3, 0));
                PlacePawn(new Placement(PawnType.Goose, 3, 6));
            }

            if (geeseNumber == 17) {
                PlacePawn(new Placement(PawnType.Goose, 3, 1));
                PlacePawn(new Placement(PawnType.Goose, 3, 5));
            }
        }

        // setta a null il turno
        public void SetToNullTheTurn () {
            this.turn = PawnType.None;
        }

        // ritorna se una mossa è valida
        public bool IsMoveValid(Move move) {
            // questo copre sia i casi di destinazione occupata sia i casi di movimento nella stessa posizione
			if (board[move.finalX, move.finalZ] != PawnType.None) {
				return false;
			}
            // il passo è al massimo 1 per le oche e 2 per la volpe
            int maxStep = move.pawnType == PawnType.Goose ? 1 : 2;
            if (Math.Abs(move.finalX - move.startingX) > maxStep || Math.Abs(move.finalZ - move.startingZ) > maxStep) {
                return false;
            }
            // controlli le mosse normali solo se al passo precedente non hai mangiato
            if (!hasJustEaten) {
                //se è una mossa normale e la trova, torna true
                String coordinatesString = move.startingX.ToString() + move.startingZ.ToString();
                List<Move> correctMovesForThisPosition = correctMoves[coordinatesString];
                bool found = false;
                for (int i = 0; i < correctMovesForThisPosition.Count && !found; i++) {
                    if (correctMovesForThisPosition[i] == move) return true;
                }
            }
            // se non è una mossa normale allora guarda se è una mangiata
            bool eatenCorrectly = false;
            Vector2 interpolPawn = EatingPawnPosition(move);
            eatenCorrectly = (interpolPawn.x != -1 && interpolPawn.y != -1);
            // la mossa è corretta sse la pedina è stata mangiata correttamente
            return eatenCorrectly;
		}

        // torna le coordinate della pedina mangiata
        // se la mossa ricevuta non è una mangiata corretta torna (-1, -1)
        private Vector2 EatingPawnPosition(Move move) {
            bool eatenCorrectly = false;
            Vector2 interpolPawn = Vector2.zero;
            if (move.pawnType == PawnType.Fox) {
                interpolPawn = move.InterpolateEatenPawn();
                // se è una mossa interpolabile
                if (interpolPawn.x != -1 && interpolPawn.y != -1) {
                    int x = (int)interpolPawn.x;
                    int y = (int)interpolPawn.y;
                    //se sotto l'interpolazione sta un'oca 
                    if (board[x, y] == PawnType.Goose) {
                        eatenCorrectly = true;
                    }
                }
            }
            return ((eatenCorrectly == true) ? interpolPawn : (new Vector2 (-1, -1)));
        }

        //muove la pedina, e se la mossa è di mangiamento allora mangia
		public void MovePawn(Move move) {
			board[move.startingX, move.startingZ] = PawnType.None;
			board[move.finalX, move.finalZ] = move.pawnType;
            Vector2 interpolPawn = EatingPawnPosition(move);
            //se la mossa è di mangiamento e corretta
            bool existsEatingMove = false;
            if (interpolPawn.x != -1 && interpolPawn.y != -1) {
                //mangia l'oca sotto l'interpolazione
                int x = (int)interpolPawn.x;
                int y = (int)interpolPawn.y;
                board[x, y] = PawnType.None;
                this.PostNotification(pawnEatenNotification, interpolPawn);
                hasJustEaten = true;
                existsEatingMove = ExistsEatingMove(move.finalX, move.finalZ);
                if (existsEatingMove) {
                    this.PostNotification(canEatAnotherTimeNotification);
                }
            }
			bool isWinningState = CheckForWin();
            if (!isWinningState && turn != PawnType.None && !existsEatingMove) {
                ChangeTurn();
            }
		}

        // cambia il turno
		public void ChangeTurn() {
			turn = (turn == PawnType.Fox) ? PawnType.Goose : PawnType.Fox;
			this.PostNotification(changeTurnNotification);
            hasJustEaten = false;
		}

        // controlla se qualcuno ha vinto
		private bool CheckForWin() {
            bool foxWon = IsFoxWinner();
			if (foxWon || IsGooseWinner()) {
                //o le oche o la volpe hanno vinto, controllo quale dei due e reimposto il gioco
                winner = (foxWon == true) ? PawnType.Fox : PawnType.Goose;
				turn = PawnType.None;
				this.PostNotification(endGameNotification);
                return true;
			}
            //nessuno dei due ha vinto
            return false;
		}

        // resetta la board e altri campi ausiliari
		public void Reset() {
			for (int i = 0; i < board.GetLength(0); i++) {
				for (int j = 0; j < board.GetLength(1); j++) {
					board[i, j] = PawnType.None;
				}
			}
			winner = PawnType.None;
			turn = PawnType.Goose; //inizia sempre l'oca
            hasJustEaten = false;
			InitializeBoard();
			//this.PostNotification(beginGameNotification);
		}

		// piazza la pedina di tipo pawnType in board[x][y]
		private void PlacePawn(Placement placement) {
			this.board[placement.x, placement.z] = placement.pawnType;
			this.PostNotification(placePawnNotification, placement);
		}

        // dice se la volpe ha vinto
        //la volpe vince quando ci sono 3 oche o meno in tutta la board
        private bool IsFoxWinner() {
            bool isWinner = false;
            int counterGoose = 0; 
            for(int r = 0; r < board.GetLength(0); r++) {
                for (int c = 0; c < board.GetLength(1); c++) {
                    if (board[r, c] == PawnType.Goose) {
                        counterGoose++;
                    }
                }
            }
            return isWinner = counterGoose <= 4 ? true : false;
        }
        
        // dice se le oche hanno vinto
        // le oche hanno vinto quando hanno circondato la volpe
        private bool IsGooseWinner () {
            Vector2 foxCoordinates = FindFoxCoordinates();
            if (foxCoordinates.x == 0 && foxCoordinates.y == 0) {
                // caso eccezionale
                return false;
            }
            int xFoxCoordinate = (int)foxCoordinates.x;
            int yFoxCoordinate = (int)foxCoordinates.y;
            String foxCoordinatesString = xFoxCoordinate.ToString() + yFoxCoordinate.ToString();
            if (AreBoardsEquals (board, winningBoards[foxCoordinatesString])) {
                return true;
            } else {
                return false;
            }
        }

        // confronta due board parzialmente, usato per capire se si vince
        private static bool AreBoardsEquals(PawnType[,] board, PawnType[,] winningBoard) {
            bool areEquals = true;
            for (int r = 0; r < board.GetLength(0) && areEquals; r++) {
                for (int c = 0; c < board.GetLength(1) && areEquals; c++) {
                    if (winningBoard[r, c] != PawnType.None && winningBoard[r,c] != board[r, c]) {
                        areEquals = false;
                    }
                }
            }
            return areEquals;
        }

        // trova le coordinate della volpe (se non c'è, impossibile, torna 0 0)
        private Vector2 FindFoxCoordinates() {
            for (int r = 0; r < board.GetLength(0); r++) {
                for (int c = 0; c < board.GetLength(1); c++) {
                    if (board[r, c] == PawnType.Fox) {
                        return new Vector2(r, c);
                    }
                }
            }
            // non ho trovato la volpe, impossibile,
            return new Vector2(0, 0);
        }

        // Sezione ExistsEatingMove=================================================
        //ritorna se esiste una eatingMove da un certo centro
        private bool ExistsEatingMove(int r, int c) {
            //le mosse pari hanno la diagonale
            if ((r + c) % 2 == 0) {
                int roundR2 = r - 2;
                while (roundR2 <= r + 2) {
                    int roundC = c - 2;
                    while (roundC <= c + 2) {
                        if (!MyUtility.IsPositionOutOfCross(roundR2, roundC)) {
                            Move hypoteticMove = new Move(PawnType.Fox, r, c, roundR2, roundC);
                            Vector2 interpolPawn = hypoteticMove.InterpolateEatenPawn();
                            // se è una mossa interpolabile
                            if (interpolPawn.x != -1 && interpolPawn.y != -1) {
                                int x = (int)interpolPawn.x;
                                int y = (int)interpolPawn.y;
                                //se sotto la mossa che tento di fare c'è un'oca non posso sovrappormi
                                if (board[hypoteticMove.finalX, hypoteticMove.finalZ] == PawnType.None) {
                                    //se sotto l'interpolazione sta un'oca 
                                    if (board[x, y] == PawnType.Goose) {
                                        return true;
                                    }
                                }
                            }
                        }
                        roundC = roundC + 2;
                    }
                    roundR2 = roundR2 + 2;
                }
            } else {
                // le mosse dispari non hanno la diagonale
                for (int roundR = r - 2; roundR <= r + 2; roundR++) {
                    if (!MyUtility.IsPositionOutOfCross(roundR, c)) {
                        Move hypoteticMove = new Move(PawnType.Fox, r, c, roundR, c);
                        Vector2 interpolPawn = hypoteticMove.InterpolateEatenPawn();
                        // se è una mossa interpolabile
                        if (interpolPawn.x != -1 && interpolPawn.y != -1) {
                            int x = (int)interpolPawn.x;
                            int y = (int)interpolPawn.y;
                            //se sotto la mossa che tento di fare c'è un'oca non posso sovrappormi
                            if (board[hypoteticMove.finalX, hypoteticMove.finalZ] == PawnType.None) {
                                //se sotto l'interpolazione sta un'oca 
                                if (board[x, y] == PawnType.Goose) {
                                    return true;
                                }
                            }
                        }
                    }
                }
                for (int roundC = c - 2; roundC <= c + 2; roundC++) {
                    if (!MyUtility.IsPositionOutOfCross(r, roundC)) {
                        Move hypoteticMove = new Move(PawnType.Fox, r, c, r, roundC);
                        Vector2 interpolPawn = hypoteticMove.InterpolateEatenPawn();
                        // se è una mossa interpolabile
                        if (interpolPawn.x != -1 && interpolPawn.y != -1) {
                            int x = (int)interpolPawn.x;
                            int y = (int)interpolPawn.y;
                            //se sotto la mossa che tento di fare c'è un'oca non posso sovrappormi
                            if (board[hypoteticMove.finalX, hypoteticMove.finalZ] == PawnType.None) {
                                //se sotto l'interpolazione sta un'oca 
                                if (board[x, y] == PawnType.Goose) {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            // se tutte le interpolazioni falliscono non esiste una eatingMove
            return false;
        }
    }
}