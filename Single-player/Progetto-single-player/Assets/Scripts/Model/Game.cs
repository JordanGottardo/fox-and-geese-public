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

		public const string finishedPlacingPawnsNotification = "Game.BeginGameNotification"; //iniziata partita
		public const string movePawnNotification = "Game.MovePawnNotification"; //mossa pedina
		public const string changeTurnNotification = "Game.ChangeTurnNotification"; //cambiato turno
		public const string endGameNotification = "Game.EndGameNotification"; //finita partita
																			  //aggiunte da me
		public const string placePawnNotification = "Game.PlacePawnNotification";
		public const string pawnEatenNotification = "Game.pawnEatenNotification";

		public PawnType turn { get; private set; } // inizia sempre l'oca
		public PawnType winner { get; private set; }
		public PawnType[,] board { get; private set; }
		private Dictionary<string, PawnType[,]> winningBoards;
		private Dictionary<string, List<Move>> correctMoves;
		private bool isSimulation = false;

		public Game() {
		}

		//costruttore
		public Game(int geeseNumber, bool isSimulationReceived) {
			this.geeseNumber = geeseNumber;
			board = new PawnType[7, 7]; //inizializzata a 0, quindi con tutti PawnType a None
			winningBoards = MyUtility.CreateWinningBoards();
			correctMoves = MyUtility.CreateCorrectMoves();
			isSimulation = isSimulationReceived;
			Reset();
			if (!isSimulation) {
				this.PostNotification(finishedPlacingPawnsNotification);
			}
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
		public void SetToNullTheTurn() {
			this.turn = PawnType.None;
		}

		// ritorna se una mossa è valida
		public bool IsMoveValid(Move move) {
			List<Move> possibleMoves = GetPossibleMoves();
			foreach (Move possibleMove in possibleMoves) {
				if (possibleMove == move) {
					return true;
				}
			}
			return false;
		}

		// torna le coordinate della pedina mangiata
		// se la mossa ricevuta non è una mangiata corretta torna (-1, -1)
		private Vector2 EatingPawnPosition(Move move) {
			bool eatenCorrectly = false;
			Vector2 interpolPawn = new Vector2();
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
			return ((eatenCorrectly == true) ? interpolPawn : (new Vector2(-1, -1)));
		}

		//muove la pedina, e se la mossa è di mangiamento allora mangia
		public void MovePawn(Move move) {
			if (!isSimulation) {
				Console.WriteLine("Game MovePawn");
			}
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
				if (!isSimulation) {
					this.PostNotification(pawnEatenNotification, interpolPawn);
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
			if (!isSimulation) {
				this.PostNotification(changeTurnNotification);
			}
		}

		// controlla se qualcuno ha vinto
		private bool CheckForWin() {
			bool foxWon = IsFoxWinner();
			if (foxWon || IsGooseWinner()) {
				//o le oche o la volpe hanno vinto, controllo quale dei due e reimposto il gioco
				winner = (foxWon == true) ? PawnType.Fox : PawnType.Goose;
				Debug.LogWarning("Game ChecKForWin winner is = " + winner);
				turn = PawnType.None;
				if (!isSimulation) {
					this.PostNotification(endGameNotification);
				}
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
			InitializeBoard();
		}

		public Game GetDeepCopy() {
			Game game = new Game();
			game.turn = turn;
			game.winner = winner;
			game.board = new PawnType[7, 7];
			for (int r = 0; r < 7; r++) {
				for (int c = 0; c < 7; c++) {
					game.board[r, c] = board[r, c];
				}
			}
			game.winningBoards = winningBoards;
			game.correctMoves = correctMoves;
			game.isSimulation = true;
			return game;
		}

		public bool GameOver() {
			return winner != PawnType.None;
		}

		// piazza la pedina di tipo pawnType in board[x][y]
		private void PlacePawn(Placement placement) {
			this.board[placement.x, placement.z] = placement.pawnType;
			if (!isSimulation) {
				this.PostNotification(placePawnNotification, placement);
			}
		}

		// dice se la volpe ha vinto
		//la volpe vince quando ci sono 3 oche o meno in tutta la board o avanti a lei
		private bool IsFoxWinner() {
			int counterGoose = GetGooseNumber();
			int counterAheadGoose = GetAheadGooseNumber();
			if (counterAheadGoose <= 1) {
				return true;
			}
			else if (counterGoose <= 4) {
				return true;
			}
			else {
				List<Move> possibleMovesOtherPlayer = GetPossibleMovesOtherPlayer();
				return possibleMovesOtherPlayer.Count() == 0;
			}
		}

		private int GetAheadGooseNumber() {
			int counterAheadGoose = 0;
			Vector2 foxCoordinates = FindFoxCoordinates();
			int rFoxpiuuno = (int)foxCoordinates.x + 1;
			for (int r = rFoxpiuuno; r < 7; r++) {
				for (int c = 0; c < 7; c++) {
					if (board[r, c] == PawnType.Goose) {
						counterAheadGoose++;
					}
				}
			}
			return counterAheadGoose;
		}

		// ritorna il numero di oche
		public int GetGooseNumber() {
			int counterGoose = 0;
			for (int r = 0; r < board.GetLength(0); r++) {
				for (int c = 0; c < board.GetLength(1); c++) {
					if (board[r, c] == PawnType.Goose) {
						counterGoose++;
					}
				}
			}
			return counterGoose;
		}

		// dice se le oche hanno vinto
		// le oche hanno vinto quando hanno circondato la volpe
		private bool IsGooseWinner() {
			List<Move> possibleMovesOtherPlayer = GetPossibleMovesOtherPlayer();
			if (possibleMovesOtherPlayer.Count() == 0) {
				return true;
			}
			return false;
		}

		//date le coordinate della fox torna quante oche ci sono a distanza euclidea >2
		private double GetNumberOfFarGoose(int rFox, int cFox) {
			int count = 0;
			for (int currentR = 0; currentR < 7; currentR++) {
				for (int currentC = 0; currentC < 7; currentC++) {
					if (
						!MyUtility.IsPositionOutOfCross(currentR, currentC) &&
						currentR != rFox - 1 &&
						currentC != cFox - 1 &&
						currentR != rFox &&
						currentC != cFox &&
						currentR != rFox + 1 &&
						currentC != cFox + 1 &&
						board[currentR, currentC] == PawnType.Goose
						) {
						count++;
					}
				}
			}
			return count;
		}

		//torna il grado di compattezza della board come media delle posizioni libere in ogni oca
		private double GetGooseFreedomness() {
			double degree = 0;
			for (int r = 0; r < board.GetLength(0); r++) {
				for (int c = 0; c < board.GetLength(1); c++) {
					if (board[r, c] == PawnType.Goose) {
						degree += calculateMeanOfFreePositions(r, c);
					}
				}
			}
			return degree / GetGooseNumber();
		}

		private double calculateMeanOfFreePositions(int r, int c) {
			int counter = 0;
			for (int roundR = r - 1; roundR <= r + 1; roundR++) {
				for (int roundC = c - 1; roundC <= c + 1; roundC++) {
					if (roundR > 0 && roundR < 7 && roundC > 0 && roundC < 7 &&
						board[roundR, roundC] != null &&
						board[roundR, roundC] == PawnType.None
					) {
						counter++;
					}
				}
			}
			return counter / 8;
		}


		// confronta due board parzialmente, usato per capire se si vince
		private static bool AreBoardsEquals(PawnType[,] board, PawnType[,] winningBoard) {
			bool areEquals = true;
			for (int r = 0; r < board.GetLength(0) && areEquals; r++) {
				for (int c = 0; c < board.GetLength(1) && areEquals; c++) {
					if (winningBoard[r, c] != PawnType.None && winningBoard[r, c] != board[r, c]) {
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
			}
			else {
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

		//dato un game (board e turno) ritorna tutte le mosse possibili per quel turno
		public List<Move> GetPossibleMoves() {
			List<Move> allPossibleMoves = new List<Move>();
			if (turn == PawnType.Goose) {
				for (int r = 0; r < 7; r++) {
					for (int c = 0; c < 7; c++) {
						if (board[r, c] == PawnType.Goose && !MyUtility.IsPositionOutOfCross(r, c)) {
							allPossibleMoves.AddRange(IACalculateGooseValidMoves(r, c));
						}
					}
				}
			}
			else if (turn == PawnType.Fox) {
				Vector2 foxCoordinates = FindFoxCoordinates();
				// nel vector2 la x è la rz e la y è la xc
				int r = (int)foxCoordinates.x;
				int c = (int)foxCoordinates.y;
				if (!MyUtility.IsPositionOutOfCross(r, c)) {
					allPossibleMoves.AddRange(IACalculateFoxValidMoves(r, c));
					allPossibleMoves.AddRange(IACalculateFoxValidEatingMoves(r, c));
				}
			}
			return allPossibleMoves;
		}

		public List<Move> GetPossibleMovesOtherPlayer() {
			List<Move> allPossibleMoves = new List<Move>();
			if (turn == PawnType.Fox) {
				for (int r = 0; r < 7; r++) {
					for (int c = 0; c < 7; c++) {
						if (board[r, c] == PawnType.Goose && !MyUtility.IsPositionOutOfCross(r, c)) {
							allPossibleMoves.AddRange(IACalculateGooseValidMoves(r, c));
						}
					}
				}
			}
			else if (turn == PawnType.Goose) {
				Vector2 foxCoordinates = FindFoxCoordinates();
				// nel vector2 la x è la rz e la y è la xc
				int r = (int)foxCoordinates.x;
				int c = (int)foxCoordinates.y;
				if (!MyUtility.IsPositionOutOfCross(r, c)) {
					//Console.WriteLine("Fox posC=" + c + " posR=" + r + " simulation=" + isSimulation);
					allPossibleMoves.AddRange(IACalculateFoxValidMoves(r, c));
					allPossibleMoves.AddRange(IACalculateFoxValidEatingMoves(r, c));
				}
			}
			return allPossibleMoves;
		}

		//valuta la qualità della board
		public int EvaluateBoard(PawnType aiPlayer, WeightsForBoardEval weights) {

			int score = 0;
			//winningState
			int signOfPlayer = aiPlayer == PawnType.Goose ? 1 : -1;
			if (IsGooseWinner()) { return weights.wWinningState * signOfPlayer; }
			if (IsFoxWinner()) { return weights.wWinningState * signOfPlayer * -1; }
			//AIplayer == oca e vince oca torna +200
			//AIplayer == oca e vince volpe torna -200
			//AIplayer == volpe e vince volpe torna +200
			//AIplayer == volpe e vince oca -200
			// else non ha vinto nessuno
			//gooseNumber
			score += GetGooseNumber() * weights.weightDict["wGooseNumber"];
			//aheadGooseNumber
			score += GetAheadGooseNumber() * weights.weightDict["wAheadGooseNumber"];
			//foxEatingMoves
			Vector2 foxCoordinates = FindFoxCoordinates();
			int rFox = (int)foxCoordinates.x;
			int cFox = (int)foxCoordinates.y;
			//foxEatingMoves
			score += IACalculateFoxValidEatingMoves(rFox, cFox).Count() * weights.weightDict["wFoxEatingMoves"];
			//foxMoves
			score += IACalculateFoxValidMoves(rFox, cFox).Count() * weights.weightDict["wFoxMoves"];
			//freedomness
			score += (int)Math.Round(GetGooseFreedomness() * weights.weightDict["wGooseFreedomness"], 0);
			//interness
			score += GetInterness(rFox, cFox) * weights.weightDict["wInterness"];
			//externess
			score += GetExterness(rFox, cFox) * weights.weightDict["wExterness"];
			return score * signOfPlayer;
		}

		//ritorna 1 se la volpe è nel quadrante centrale else -1
		private int GetInterness(int rFox, int cFox) {
			if (
				(rFox == 2 && cFox == 2) || (rFox == 2 && cFox == 3) || (rFox == 2 && cFox == 4) ||
				(rFox == 3 && cFox == 2) || (rFox == 3 && cFox == 3) || (rFox == 3 && cFox == 4) ||
				(rFox == 4 && cFox == 2) || (rFox == 4 && cFox == 3) || (rFox == 4 && cFox == 4)
				) {
				return 1;
			}
			else {
				return 0;
			}
		}

		//ritorna 1 se la volpe è in una delle 8 posizioni svantaggiate else -1
		private int GetExterness(int rFox, int cFox) {
			if (
				(rFox == 0 && cFox == 3) || (rFox == 3 && cFox == 6) ||
				(rFox == 6 && cFox == 3) || (rFox == 3 && cFox == 0) ||
				(rFox == 1 && cFox == 2) || (rFox == 1 && cFox == 4) ||
				(rFox == 2 && cFox == 5) || (rFox == 4 && cFox == 5) ||
				(rFox == 2 && cFox == 1) || (rFox == 4 && cFox == 1) ||
				(rFox == 5 && cFox == 2) || (rFox == 5 && cFox == 4)
				) {
				return 1;
			}
			else {
				return 0;
			}
		}

		// Sezione aggiunta per la IA=================================================
		//la goose non puo andare in diagonale
		//non puo retrocedere di riga (anzi, cioò per gli indici avanzare)
		//restano solo tre mosse: avanti, dx, sx
		private List<Move> IACalculateGooseValidMoves(int r, int c) {
			List<Move> gooseValidMoves = new List<Move>();
			if (!MyUtility.IsPositionOutOfCross(r - 1, c) && board[r - 1, c] == PawnType.None)
				gooseValidMoves.Add(new Move(PawnType.Goose, r, c, r - 1, c));
			if (!MyUtility.IsPositionOutOfCross(r, c - 1) && board[r, c - 1] == PawnType.None)
				gooseValidMoves.Add(new Move(PawnType.Goose, r, c, r, c - 1));
			if (!MyUtility.IsPositionOutOfCross(r, c + 1) && board[r, c + 1] == PawnType.None)
				gooseValidMoves.Add(new Move(PawnType.Goose, r, c, r, c + 1));
			return gooseValidMoves;
		}

		//la fox si puo muovere in tutte le 8 direzioni, a distanza 1
		private List<Move> IACalculateFoxValidMoves(int r, int c) {
			List<Move> foxValidMoves = new List<Move>();
			//setto l'intorno del centro dato
			for (int roundR = r - 1; roundR <= r + 1; roundR++) {
				for (int roundC = c - 1; roundC <= c + 1; roundC++) {
					if (!MyUtility.IsPositionOutOfCross(roundR, roundC) &&
						!(roundR == r && roundC == c) &&
						//le mosse dispari saltano le destinazioni dispari cioe saltano la diagonale
						!((r + c) % 2 != 0 && (roundR + roundC) % 2 != 0) &&
						board[roundR, roundC] == PawnType.None) {
						foxValidMoves.Add(new Move(PawnType.Fox, r, c, roundR, roundC));
					}
				}
			}
			return foxValidMoves;
		}

		//la fox per mangiare si puo muovere in tutte le 8 direzioni, a distanza 2
		private List<Move> IACalculateFoxValidEatingMoves(int r, int c) {
			List<Move> foxValidEatingMoves = new List<Move>();
			for (int roundR = r - 2; roundR <= r + 2; roundR += 2) {
				for (int roundC = c - 2; roundC <= c + 2; roundC += 2) {
					if (
						//la casella di destinazione deve essere nella board
						!MyUtility.IsPositionOutOfCross(roundR, roundC) &&
						//la casella interpolata deve essere nella board
						!MyUtility.IsPositionOutOfCross((roundR + r) / 2, (roundC + c) / 2) &&
						//non ci si può muovere verso la stessa casella di partenza
						!(roundR == r && roundC == c) &&
						//le mosse dispari saltano le destinazioni dispari cioe saltano la diagonale
						!(((r + c) % 2 != 0) && ((roundR != r) && (roundC != c))) &&
						//la casella di destinazione deve essere vuota
						board[roundR, roundC] == PawnType.None &&
						//la casella interpolata deve avere una goose sotto
						board[(roundR + r) / 2, (roundC + c) / 2] == PawnType.Goose) {
						foxValidEatingMoves.Add(new Move(PawnType.Fox, r, c, roundR, roundC));
					}
				}
			}
			return foxValidEatingMoves;
		}
	}
}