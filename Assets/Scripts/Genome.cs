﻿using System;

//represents genome as an array of integers
public class Genome {

	public char[] genes;
	public string genome;
	private float mutateRate;
    public int geneTypeCount;

    //constructor for genome from parent phenotype
    public Genome(string parentGenome)
    {

        genome = parentGenome;
        genes = parentGenome.ToCharArray();
        geneTypeCount = 8;

    }
    //constructor for first random genotype by length
    public Genome(int length, int randomSeed){

		genes = new char[length];
        geneTypeCount = 8; //hard coded HACKY!
        GenerateRandomGenome(length, randomSeed);
	}

    public void GenerateRandomGenome(int length, int randomSeed)
    {
        System.Random randomInt = new System.Random(randomSeed);

        //generate a new random genome with genes a-z
        for (int i = 0; i < length; i++)
        {
            //issue here with timing and random being same
            int randInt = randomInt.Next(97, 97 + geneTypeCount); //MODULEO!!!
            genes[i] = (char)randInt;
            
        }
        genome = new string(genes);
    }


	public int Fitness(string targetString){

		int score = 0;
		for(int i = 0; i < genome.Length; i++) {

			if (genome [i] == targetString [i]) {
				score++;
			}
		}
		return score;
	}

	public override string ToString()
	{
		return genome;
	}

    //unused
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



