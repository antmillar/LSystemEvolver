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
        //genes[geneIndex] = value.ToCharArray();
    }

    //constructor for genome from parent phenotype
    public Genome(string[] parentGenome)
    {
        int geneCount = parentGenome.Length;
        genome = parentGenome.ToArray();
        //genes = new char[geneCount][];

        //for (int i = 0; i < geneCount; i++)
        //{
        //   genes[i] = parentGenome[i].ToCharArray();
        //}

        baseTypeCount = 11;
    }

    //constructor for first random genotype by length
    public Genome(int length, int randomSeed){

        //int geneCount = 2; //HARCODEDEODOEDOE

        //for (int i = 0; i < geneCount; i++)
        //{
        //    genes[i] = new char[length];
        //}

        baseTypeCount = 11; //hard coded HACKY!
        GenerateRandomGenome(length, randomSeed);
	}

    //copy constructor
    public Genome(Genome g)
    {
        //genes = g.genes.ToArray();

        //for (int i = 0; i < genes.Length; i++)
        //{
        //    genes[i] = genes[i].ToArray();
        //}

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



