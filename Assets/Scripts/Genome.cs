using System;
using System.Linq;
using UnityEngine;

//represents genome as an array of integers/
//can have more than one array if there objects are more complex
public class Genome {

	//public char[][] genes;
	public string[] genome;
	private float mutateRate;
    public int baseTypeCount;

    public void setGene(int geneIndex, string value)
    {
        genome[geneIndex] = value;
    }

    //constructor for genome from parent phenotype
    public Genome(string[] parentGenome, string symbols)
    {
        int geneCount = parentGenome.Length;
        genome = parentGenome.ToArray();
        baseTypeCount = symbols.Length;
    }

    //constructor for first random genotype by length
    public Genome(int length, string symbols, int randomSeed){

        baseTypeCount = symbols.Length;
        GenerateRandomGenome(length, randomSeed);
	}

    //copy constructor
    public Genome(Genome g)
    {
        genome = g.genome.ToArray();
        mutateRate = g.mutateRate;
        baseTypeCount = g.baseTypeCount;
    }

    public void GenerateRandomGenome(int length, int randomSeed)
    {
        System.Random randomInt = new System.Random(randomSeed);
        for (int j = 0; j < genome.Length; j++)
        { //generate a new random genome with genes a-z
            char[] bases = new char[length];

            for (int i = 0; i < length; i++)
            {
                //issue here with timing and random being same
                int randInt = randomInt.Next(97, 97 + baseTypeCount); //MODULEO!!!
                bases[i] = (char)randInt;

            }
            genome[j] = new string(bases);
        }
    }

	public int Fitness(string targetString){

		int score = 0;
        for (int j = 0; j < genome.Length; j++)

        {
            for (int i = 0; i < genome[j].Length; i++)
            {

                if (genome[j][i] == targetString[i])
                {
                    score++;
                }
            }

        }
		return score;
	}

	public override string ToString()
	{
        string outputString = "";

        for (int i = 0; i < genome.Length; i++)
        {
            outputString += genome[i] + " ";
        }

        return outputString;
	}

    //not implemented properly
	public double randDist(){

		System.Random rand = new System.Random(); //reuse this if you are generating many
		double u1 = 1.0-rand.NextDouble(); //uniform(0,1] random doubles
		double u2 = 1.0-rand.NextDouble();
		double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
			Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
		double randNormal =	0 + 1 * randStdNormal; //random normal(mean,stdDev^2)
		
		return randNormal;
	}
}



