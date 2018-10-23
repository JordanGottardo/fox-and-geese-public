using System.Collections;
using System.Collections.Generic;

using System;

public class WeightScore : IComparable {

	public WeightsForBoardEval weights { get; set; } // pesi
	public int score { get; set; } //punteggio di vittorie

	public WeightScore(WeightsForBoardEval weights, int score) {
		this.weights = weights;
		this.score = score;
	}	

	public int CompareTo(object obj) {
		WeightScore w = (WeightScore)obj;
		if (score >= w.score) {
			return -1;
		}
		return 1;
	}

	public override bool Equals(object obj) {
		var score = obj as WeightScore;
		return score != null &&
			   EqualityComparer<WeightsForBoardEval>.Default.Equals(weights, score.weights);
	}

	public override int GetHashCode() {
		return -1248457298 + EqualityComparer<WeightsForBoardEval>.Default.GetHashCode(weights);
	}

	public override string ToString() {
		return "weightScore weights = " + weights.ToString() + " score = " + score;
	}
}
