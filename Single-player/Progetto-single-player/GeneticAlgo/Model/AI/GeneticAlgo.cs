using System.Collections;
using System.Collections.Generic;

using System;

public class GeneticAlgo {

	public SortedList<WeightScore, int> Evolve(SortedList<WeightScore, int> population, double retain, double randomSelect, double mutate) {
		Console.WriteLine("start genetic algo evolve");

		foreach (var key in population.Keys) {
			Console.WriteLine(key);
		}
		System.Random rand = new System.Random();
		int retainIndex = (int)(population.Count * retain);
		Console.WriteLine("population size = " + population.Count + " " + retain + " " + randomSelect + " " + mutate);
		Console.WriteLine("retain tengo " + retainIndex + " soggetti");
		SortedList<WeightScore, int> parents = new SortedList<WeightScore, int>();

		var iter = population.GetEnumerator();
		iter.MoveNext();
		for (int i = 0; i < retainIndex; i++) {
			parents[iter.Current.Key] = iter.Current.Value;
			iter.MoveNext();
		}

		//aggiunge altri soggetti con una percentuale di aggiunta pari a randomSelect
		while (iter.MoveNext()) {
			if (randomSelect > rand.NextDouble()) {
				var ws = iter.Current;
				parents[ws.Key] = ws.Value;
			}
		}

		foreach (var key in parents.Keys) {
			Console.WriteLine(key);
		}
		int parentsCount = parents.Count;
		int childrenToGenerate = population.Count - parentsCount; //quanti figli devo generare per ritornare ad avere una popolazione completa
		int generatedChildren = 0;
		Console.WriteLine("fine evolve" + population.Count + " " + parents.Count + " " + childrenToGenerate);
		
		//crossover
		while (parents.Count < population.Count - 2) {
			//Console.WriteLine("parent count= " + parents.Count + " pop count = " + population.Count);
			//Console.WriteLine("GeneticAlgo genero figli");
			
			int firstN = rand.Next(parentsCount);
			int secondN = rand.Next(parentsCount);
			WeightsForBoardEval male = parents.Keys[firstN].weights;
			WeightsForBoardEval female = parents.Keys[secondN].weights;
			if (!male.Equals(female)) {
				WeightsForBoardEval child = CrossoverAndMutate(male, female, mutate);
				WeightScore ws = new WeightScore(child, 0);
				parents[ws] = 0;
				generatedChildren++;
			}
		}
		WeightScore ws1 = new WeightScore(new WeightsForBoardEval(rand), 0);
		WeightScore ws2 = new WeightScore(new WeightsForBoardEval(rand), 0);
		parents[ws1] = 0;
		parents[ws2] = 0;
		List<WeightsForBoardEval> testPopulation = new List<WeightsForBoardEval>();
		testPopulation.Add(new WeightsForBoardEval(1, 0, 0, 0, 0, 0, 0));
		testPopulation.Add(new WeightsForBoardEval(2, 2, -2, -2, -1, -1, 0));
		return new MatchAlgo().LaunchTournament(parents, testPopulation); //lancia un torneo sulla popolazione e ritorna la popolazione con gli score aggiornati
	}

    // ritorna un WeightsForBoardEval nato da male e female con operazioni bitwise 
    private WeightsForBoardEval CrossoverAndMutate(WeightsForBoardEval male, WeightsForBoardEval female, double mutationProbability) {
        Console.WriteLine("genetic algo crossover");
        Dictionary<String, BitArray> bitFather = new Dictionary<String, BitArray>();
        Dictionary<String, BitArray> bitMother = new Dictionary<String, BitArray>();
        Dictionary<String, BitArray> bitChild = new Dictionary<String, BitArray>();
        foreach (String key in male.weightDict.Keys) {
            byte[] byteFather = BitConverter.GetBytes(male.weightDict[key]);
            byte[] byteMother = BitConverter.GetBytes(female.weightDict[key]);
            bitFather[key] = new BitArray(byteFather);
            bitMother[key] = new BitArray(byteMother);
            //crossover
            bitChild[key] = Mix(bitFather[key], bitMother[key]);
            //mutazione
            System.Random random = new System.Random();
            double prob = random.NextDouble();
            if (prob < mutationProbability) {
                bitChild[key] = Mutate(bitChild[key]);
            }
        }

        WeightsForBoardEval child = new WeightsForBoardEval(
            GetIntFromBitArray(bitChild["wGooseNumber"]),
            GetIntFromBitArray(bitChild["wAheadGooseNumber"]),
            GetIntFromBitArray(bitChild["wFoxEatingMoves"]),
            GetIntFromBitArray(bitChild["wFoxMoves"]),
            GetIntFromBitArray(bitChild["wGooseFreedomness"]),
            GetIntFromBitArray(bitChild["wInterness"]),
            GetIntFromBitArray(bitChild["wExterness"])
            );
        return child;
    }

    private BitArray Mutate(BitArray bitArray) {
        if (bitArray.Count != 16)
            throw new ArgumentException("Genetic Algo :: mutate :: non ci sono esattamente 16 bits");
        System.Random rand = new System.Random();
        int pos = Math.Abs((rand.Next()) % 15);
        bitArray.Set(pos + 1, !bitArray[pos + 1]);
        return bitArray;
    }

    private BitArray Mix(BitArray bitFather, BitArray bitMother) {
        if (bitFather.Count != 16 || bitMother.Count != 16)
            throw new ArgumentException("Genetic Algo :: mix :: non ci sono esattamente 16 bits");
        BitArray bitChild = new BitArray(bitMother);
        for (int i = 0; i < bitChild.Count; i++) {
            if (i % 4 != 3 && i % 4 != 2)
            bitChild[i] = bitFather[i];
        }
        return bitChild;
    }

    private Int16 GetIntFromBitArray(BitArray bitArray) {
        if (bitArray.Count != 16)
            throw new ArgumentException("Genetic Algo :: getIntFromBitArray :: non ci sono esattamente 16 bits");
        byte[] byteArray = new byte[2];
        bitArray.CopyTo(byteArray, 0);
        Int16 number = BitConverter.ToInt16(byteArray, 0);
        return number;
    }

    static void BitArrayToStr(BitArray bitArray) {
        for (int i = 0; i < bitArray.Count; i++) {
            Console.WriteLine(bitArray[i].ToString());
        }
    }
}