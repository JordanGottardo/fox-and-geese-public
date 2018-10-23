using System.Collections;
using System.Collections.Generic;

using System;
using FoxAndGeese;

public class AlphaBeta {

	//indica la mossa migliore da fare a partire 
	//dallo stato da cui viene invocato AlfaBeta
	public Move bestMove { get; set; }
	//indica chi è il giocatore controllato dalla IA
	public PawnType aiPlayer {  get; set; }
	public int maxPly { get; set; }
	public WeightsForBoardEval weights { get; set; }
	public bool random { get; set; }

	public AlphaBeta(PawnType aiPlayer, int maxPly, WeightsForBoardEval weights, bool random) {
		this.aiPlayer = aiPlayer;
		this.maxPly = maxPly;
		this.weights = weights;
		this.random = random;
	}

	//board indica lo stato di gioco
	public Move RunAlphaBeta(Game game) {
		if (random) {
			return RandomMove(game);
		}
		Game deepCopiedGame = game.GetDeepCopy();
		MaxValue(deepCopiedGame, int.MinValue, int.MaxValue, 0);
		return bestMove;
	}

	public Move RandomMove(Game game) {
		List<Move> moves = game.GetPossibleMoves();
		int size = moves.Count;
		System.Random r = new System.Random();
		int rInt = r.Next(0, size); //for ints
		//Console.WriteLine(game.board);
		try {
			return moves[rInt];
		}
		catch (Exception e) {
			Console.WriteLine(game.board);
			Console.WriteLine("estratta mossa numero " + rInt + " mosse disponibili " + moves.Count);
			return moves[rInt];
		}
	}

	private int MaxValue(Game game, int alpha, int beta, int currentPly) {
		//Console.WriteLine("MaxValue " + currentPly);
		currentPly++;
		if (currentPly > maxPly || game.GameOver()) {
			return game.EvaluateBoard(aiPlayer, weights);
		}
		List<Move> moves = game.GetPossibleMoves();

		foreach (Move move in moves) {
			Game deepCopiedGame = game.GetDeepCopy();
			deepCopiedGame.MovePawn(move); //il turn viene cambiato da questo metodo
			int score = MinValue(deepCopiedGame, alpha, beta, currentPly);

			if (score > alpha) {
				alpha = score;
				if (currentPly == 1) {
					bestMove = move;
				}
			}

			if (alpha >= beta) {
				break;
			}
		}
		return alpha;
	}

	private int MinValue(Game game, int alpha, int beta, int currentPly) {
		//Console.WriteLine("MinValue " + currentPly);
		currentPly++;
		if (currentPly > maxPly || game.GameOver()) {
			return game.EvaluateBoard(aiPlayer, weights);
		}
		List<Move> moves = game.GetPossibleMoves();

		foreach (Move move in moves) {
			Game deepCopiedGame = game.GetDeepCopy();
			deepCopiedGame.MovePawn(move); //il turn viene cambiato da questo metodo
			int score = MaxValue(deepCopiedGame, alpha, beta, currentPly);

			if (score < beta) {
				beta = score;
				//bestMove = move;
			}

			if (alpha >= beta) {
				break;
			}
		}
		return beta;
	}

}
