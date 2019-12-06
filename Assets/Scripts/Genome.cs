using System;
using System.Linq;
using UnityEngine;

//represents genome as an array of integers/
//can have more than one array if there objects are more complex
public class Genome {

	public string[] genome;
	private float mutateRate;
    public int baseTypeCount;
    private int _defaultGeneLength = 10;

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
        { 
            genome[j] = GenerateRandomGene(randomInt);
        }
    }

    public string GenerateRandomGene(System.Random r)
    {

        char[] bases = new char[_defaultGeneLength];

        for (int i = 0; i < _defaultGeneLength; i++)
        {
            int randInt = r.Next(97, 97 + baseTypeCount);
            bases[i] = (char)randInt;
        }
        return new string(bases);
    }

    public void AddGene()
    {
        string[] newGenome = new string[genome.Length + 1];
        System.Random randomInt = new System.Random();
        newGenome[genome.Length] = GenerateRandomGene(randomInt);
        genome.CopyTo(newGenome, genome.Length - 1);
        genome = newGenome;
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



