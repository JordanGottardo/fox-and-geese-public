using System.Collections;
using System.Collections.Generic;

using System;


public class WeightsForBoardEval {
	
    public Int32 wWinningState { get; set; } // meglio per volpe se basso
	public Dictionary<string, Int16> weightDict { get; set; }

	//public Int16 wGooseNumber { get; set; } // meglio per volpe se basso
	//public Int16 wAheadGooseNumber { get; set; } // meglio per volpe se basso
	//public Int16 wFoxEatingMoves { get; set; } // meglio per volpe se alto
	//public Int16 wFoxMoves { get; set; } // meglio per volpe se alto
	//public Int16 wGooseFreedomness { get; set; } // meglio per volpe se alta
	//public Int16 wInterness { get; set; } // meglio per volpe se alto
	//public Int16 wExterness { get; set; } // meglio per volpe se basso

	public WeightsForBoardEval(System.Random rand) {
		Initialize();
		List<string> keys = new List<string>(weightDict.Keys);

		foreach (var key in keys) {
			weightDict[key] = (Int16)(rand.Next(Int16.MinValue, Int16.MaxValue));
		}
	}


	public WeightsForBoardEval(int numberOfFeature, Int16 weight) {
		Initialize();
		string selectedFeature = "";
		
		if (numberOfFeature == 0) {
			selectedFeature = "wGooseNumber";
		} else if (numberOfFeature == 1) {
			selectedFeature = "wAheadGooseNumber";
		} else if (numberOfFeature == 2) {
			selectedFeature = "wFoxEatingMoves";
		} else if (numberOfFeature == 3) {
			selectedFeature = "wFoxMoves";
		} else if (numberOfFeature == 4) {
			selectedFeature = "wGooseFreedomness";
		} else if (numberOfFeature == 5) {
			selectedFeature = "wInterness"; 
		} else if (numberOfFeature == 6) {
			selectedFeature = "wExterness";
		}
		weightDict[selectedFeature] = weight;
	}

	public WeightsForBoardEval(Int16 wGooseNumber, Int16 wAheadGooseNumber,
		Int16 wFoxEatingMoves, Int16 wFoxMoves, Int16 wGooseFreedomness, Int16 wInterness, Int16 wExterness) {
		this.wWinningState = Int32.MaxValue / 2;
		weightDict = new Dictionary<string, short>();
		weightDict.Add("wGooseNumber", wGooseNumber);
		weightDict.Add("wAheadGooseNumber", wAheadGooseNumber);
		weightDict.Add("wFoxEatingMoves", wFoxEatingMoves);
		weightDict.Add("wFoxMoves", wFoxMoves);
		weightDict.Add("wGooseFreedomness", wGooseFreedomness);
		weightDict.Add("wInterness", wInterness);
		weightDict.Add("wExterness", wExterness);
    }

	private void Initialize() {
		this.wWinningState = Int32.MaxValue / 2;
		weightDict = new Dictionary<string, short>();
		weightDict.Add("wGooseNumber", 0);
		weightDict.Add("wAheadGooseNumber", 0);
		weightDict.Add("wFoxEatingMoves", 0);
		weightDict.Add("wFoxMoves", 0);
		weightDict.Add("wGooseFreedomness", 0);
		weightDict.Add("wInterness", 0);
		weightDict.Add("wExterness", 0);
	}



	public override string ToString() {
		return "win = " + wWinningState + " gN= " + weightDict["wGooseNumber"] + " agN= " +
			weightDict["wAheadGooseNumber"] + " fEatMov= " + weightDict["wFoxEatingMoves"] + " fMov= " +
			weightDict["wFoxMoves"] + " gFre= " + weightDict["wGooseFreedomness"] + " int= "
			+ weightDict["wInterness"] + " ext= " + weightDict["wExterness"];
	}

	public override bool Equals(object obj) {
		var eval = obj as WeightsForBoardEval;
		return eval != null &&
			   wWinningState == eval.wWinningState &&
			   EqualityComparer<Dictionary<string, short>>.Default.Equals(weightDict, eval.weightDict);
	}

	public override int GetHashCode() {
		var hashCode = -685122608;
		hashCode = hashCode * -1521134295 + wWinningState.GetHashCode();
		hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<string, short>>.Default.GetHashCode(weightDict);
		return hashCode;
	}
}
