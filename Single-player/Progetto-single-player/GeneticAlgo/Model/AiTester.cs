using System.Collections;
using System.Collections.Generic;
using FoxAndGeese;
using System;


//[InitializeOnLoad]
public class AiTester {

	static void Main() {
		Console.WriteLine("inizio main ore: " + DateTime.Now.ToString("hh:mm:ss tt"));
		new AiTester().StartTest();
	}

	public void StartTest() {

		MatchAlgo matchAlgo = new MatchAlgo();
		GeneticAlgo geneticAlgo = new GeneticAlgo();
		System.Random rand = new System.Random();
		List<WeightsForBoardEval> weights = new List<WeightsForBoardEval>();

		for (int i = 0; i < 50; i++) {
			WeightsForBoardEval w = new WeightsForBoardEval(rand);
			weights.Add(w);
			Console.WriteLine("pesInizRand = " + w);
		}

		SortedList<WeightScore, int> scoreMap = new SortedList<WeightScore, int>();

		//per ogni peso = giocatore, fa giocare 3 match come oca e 3 match come volpe al giocatore contro un giocatore casuale 
		for (int i = 0; i < weights.Count; i++) {
			int score = 0;
			WeightsForBoardEval weight = weights[i];
			AlphaBeta playerAsFox = new AlphaBeta(PawnType.Fox, 3, weight, false);
			AlphaBeta playerAsGoose = new AlphaBeta(PawnType.Goose, 3, weight, false);
			score = matchAlgo.MatchTwoAi(playerAsFox, null, 3, -1, -1).first;
			score += matchAlgo.MatchTwoAi(playerAsGoose, null, 3, -1, -1).first;
			WeightScore ws = new WeightScore(weight, score);
			scoreMap.Add(ws, score);
		}

		Console.WriteLine("aiTester prima di evoluzione");
		// evolve 50 volte la popolazione
		for (int i = 0; i < 50; i++) {
			Console.WriteLine("\n \n \n \n inizio evoluzione generazione " + i);
			scoreMap = geneticAlgo.Evolve(scoreMap, 0.2, 0.05, 0.01);
		}
		Console.WriteLine("best weights " + scoreMap.Keys[0]);
		Console.WriteLine("finito completamente aiTester ore: " + DateTime.Now.ToString("h:mm:ss tt"));
		Console.ReadKey();
	}


}






