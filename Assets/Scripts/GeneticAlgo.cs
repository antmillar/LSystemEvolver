using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//this interface must be implemented to convert from genome strings to phenotypes
//T is the type that the phenotype is represented by
interface IEncoder<T>
{
    string Encode(T[] obj);
    T[] Decode(string genomeString);
}

//converts from a Color to string based genome
public class Encoder : IEncoder<Color>
{
    public string Encode(Color[] pixels)
    {
        char[] chars = new char[pixels.Length];

        for (int i = 0; i < pixels.Length; i++)
        {
            chars[i] = (char)(pixels[i].r * 10 + 97);
        }

        string output = new string(chars);

        Debug.Log("target string is " + output);

        return output;
    }

    public Color[] Decode(string genomeString)
    {
        Color[] colors = new Color[genomeString.Length];
        char[] chars = genomeString.ToCharArray();

        for (int i = 0; i < genomeString.Length; i++)
        {
            float colorW = (chars[i] - 97) / 10f;
            colors[i] = new Color(colorW, colorW, colorW);
        }

        return colors;
    }
}

public class Population
{
    public string _name;

    public int _generation = 0;
    public int _bestFitness;
    public int _size;
    
    public string _bestGenome;
    public Genome[] _genomes;
    public int[] _fitnesses;

    public Population(int size, string targetString)
    {
        _size = size;
        Seed(targetString);
    }

    //creates first generation
    public void Seed(string targetString)
    {
        _genomes = new Genome[_size];
        System.Random randomInt = new System.Random();

        for (int i = 0; i < _size; i++)
        {
            _genomes[i] = new Genome(targetString.Length, randomInt.Next());
        }
        _generation++;
    }
}


public class Fitness
{ 
    public string _targetString;

    public Fitness (string targetString)
    {
        _targetString = targetString;
    }

    public void Apply(Population p)
    {
        GenerateFitnesses(p);
        SortByFitness(p);
    }

    //calculates the fitnesses for a population
    private void GenerateFitnesses(Population p)
    {
        p._fitnesses = new int[p._size];

        for (int i = 0; i < p._size; i++)
        {
            p._fitnesses[i] = FitnessFunction(p._genomes[i]);
        }
    }

    //calculates an individual genomes fitness
    private int FitnessFunction(Genome g)
    {
        int fitness = 0;
        //calculates the difference in characters between the candidate genome and target
        for (int i = 0; i < g.genes.Length; i++)
        {

            if (g.genes[i] == _targetString[i])
            {
                fitness++;
            }
        }
        return fitness;
    }

    //sorts the genomes and fitness by fitness descending order, and records best
    public Population SortByFitness(Population p)
    {
        Array.Sort(p._fitnesses, p._genomes);
        Array.Reverse(p._fitnesses);
        Array.Reverse(p._genomes);

        p._bestGenome = p._genomes[0].genome;
        p._bestFitness = p._fitnesses[0];

        string top3fit = String.Format("{0} Population : Generation {1} : Top 3 Fitnesses {2}, {3}, {4}", p._name, p._generation, p._fitnesses[0].ToString(), p._fitnesses[1].ToString(), p._fitnesses[2].ToString());
        string top5pheno = String.Format("{0} Population : Generation  {1} : Top 5 genomes ", p._name, p._generation);

        for (int i = 0; i < 5; i++)
        {
            top5pheno += " " + i + " - " + p._genomes[i].ToString();
        }

        //print out top candidates
        Debug.Log(top3fit);
        Debug.Log(top5pheno);

        return p;
    }
}

public class Selection
{
    public string _selectionType;

    public Selection(string selectionType)
    {
        _selectionType = selectionType;
    }

    public List<Genome> Cdf(Genome[] genomes, int[] fitnesses)
    {
        List<Genome> cdf = new List<Genome>();

        for (int i = 0; i < genomes.Length; i++)
        {
            for (int j = 0; j < fitnesses[i]; j++)
            {
                cdf.Add(genomes[i]);
            }
        }
        return cdf;
    }



    //returns an array with frequency of the top 3 fitnesses weighted by occurence i.e cdf
    public List<Genome> Select(Population p, string userSelection = "")
    {
        //local variables

        var selectionPool = new List<Genome>();
        var size = p._size;
        var genomes = p._genomes;
        var fitnesses = p._fitnesses;

        switch (_selectionType)
        {

            //this is when the user chooses after each generation the best candidates
            case "god mode":

                Debug.Log("Please choose the candidates selected to be the parents");
                Debug.Log("Type 5 numbers between 0 and " + (p._size - 1).ToString());
                string[] indices = userSelection.Split(' ');

                //must have 5 selections.....
                for (int i = 0; i < 5; i++)
                {
                    Debug.Log("User chose Candidate " + indices[i] + "\nGenome : " + p._genomes[int.Parse(indices[i])].ToString());

                    for (int j = 0; j < (size / 5); j++)
                    {
                        selectionPool.Add(p._genomes[int.Parse(indices[i])]);
                    }
                }

                break;

            //choose sample of size n from the GA, and places the highest fitness one in the selection pool
            case "tournament":

                for (int i = 0; i < size; i++)
                {
                    //choose two values (n = 2)
                    int r = UnityEngine.Random.Range(0, size);
                    int r2 = UnityEngine.Random.Range(0, size);
                    Genome best = fitnesses[r] >= fitnesses[r2] ? genomes[r] : genomes[r2];
                    selectionPool.Add(best);
                }

                break;

            //random selection of breeding genomes from the cdf
            case "fitnessProportional":

                List<Genome> cdf = Cdf(genomes, fitnesses);

                for (int i = 0; i < size; i++)
                {
                    int r = UnityEngine.Random.Range(0, cdf.Count);
                    selectionPool.Add(cdf[r]);
                }
                break;

            //step through the cdf in steps of cdf.count / size, and picks random item in each step range
            case "stochasticUniversalSampling":

                cdf = Cdf(genomes, fitnesses);

                for (int i = 0; i < size; i++)
                {
                    int r = UnityEngine.Random.Range((cdf.Count / size) * i, (cdf.Count / size) * (i + 1));
                    selectionPool.Add(cdf[r]);
                }
                break;

            //takes the top 5 by fitness and then fills the selection pool with size /5 of those
            case "truncated":

                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < (size / 5); j++)
                    {
                        //genomes is already ordered so it's fairly simple
                        selectionPool.Add(genomes[i]);
                    }
                }
                break;

        }

        //shuffle the selection pool randomly
        System.Random rand = new System.Random();
        selectionPool = new List<Genome>(selectionPool.OrderBy(x => rand.Next()));

        return selectionPool;
    }
}

public class CrossOver
{
    public string _crossoverType;

    public CrossOver(string crossoverType )
    {
        _crossoverType = crossoverType;
    }

    public void Apply(Population p, List<Genome> selectionPool)
    {
        p._genomes = new Genome[p._size];
        Stack<Genome> selectionStack = new Stack<Genome>(selectionPool);

        //loop over pairs, as need two parents per two children
        for (int i = 0; i < p._size; i += 2)
        { 
            //add the two new children to the new genomes for this generation (have been shuffled previously)

            p._genomes[i] = new Genome(selectionStack.Pop().genome);
            p._genomes[i + 1] = new Genome(selectionStack.Pop().genome);
            //evolve them
            Cross(p._genomes[i], p._genomes[i + 1]);

        }
    }

    //takes two genomes as input and mutates them
    public void Cross(Genome p1, Genome p2)
    {

        int xPt1 = UnityEngine.Random.Range(0, p1.genome.Length);
        string crossedString1, crossedString2;

        switch (_crossoverType)
        {
            //cuts the genotype at one point and swaps genes
            case "OnePt":

                crossedString1 = p1.genome.Substring(0, xPt1) + p2.genome.Substring(xPt1);
                p1 = new Genome(crossedString1);

                crossedString2 = p2.genome.Substring(0, xPt1) + p1.genome.Substring(xPt1);
                p2 = new Genome(crossedString2);

                break;

            //cuts the genotype at two points and swaps genes
            case "TwoPt":

                int xPt2 = UnityEngine.Random.Range(0, p1.genome.Length);

                if (xPt1 <= xPt2)
                {
                    crossedString1 = p1.genome.Substring(0, xPt1) + p2.genome.Substring(xPt1, xPt2 - xPt1) + p1.genome.Substring(xPt2);
                    crossedString2 = p2.genome.Substring(0, xPt1) + p1.genome.Substring(xPt1, xPt2 - xPt1) + p2.genome.Substring(xPt2);
                }
                else
                {
                    crossedString1 = p2.genome.Substring(0, xPt2) + p1.genome.Substring(xPt2, xPt1 - xPt2) + p2.genome.Substring(xPt1);
                    crossedString2 = p1.genome.Substring(0, xPt2) + p2.genome.Substring(xPt2, xPt1 - xPt2) + p1.genome.Substring(xPt1);
                }
                p1 = new Genome(crossedString1);
                p2 = new Genome(crossedString2);
                break;

            //swaps genes by index depending on the swap rate
            case "Uniform":

                float swapRate = 0.1f;
                char[] chars1 = p1.genome.ToCharArray();
                char[] chars2 = p2.genome.ToCharArray();

                for (int i = 0; i < p1.genome.Length; i++)
                {

                    //if under swapRate swap genes at index
                    if (UnityEngine.Random.Range(0f, 1f) < swapRate)
                    {
                        char temp = chars1[i];
                        chars1[i] = chars2[i];
                        chars2[i] = temp;
                    }
                }

                p1 = new Genome(new string(chars1));
                p2 = new Genome(new string(chars2));

                break;
        }
    }
}

public class Mutation
{
    public string _mutationType;

    public Mutation(string mutationType)
    {
        _mutationType = mutationType;
    }

    public void Apply(Population p)
    {
        for (int i = 0; i < p._size; i += 1)
        {

            //evolve them
            Mutate(p._genomes[i], 0.1f);
            p._genomes[i] = p._genomes[i];

        }


    }

    //takes genome as input and mutates it
    public void Mutate(Genome p1, float mutationRate)
    {

        switch (_mutationType)
        {

            //if chosen, gene is mutated to random choice in range
            case "randomChoice":

                for (int i = 0; i < p1.genes.Length; i++)
                {
                    //mutates based on the mutationRate
                    if (UnityEngine.Random.Range(0f, 1f) < mutationRate)
                    {

                        int randomChoice = UnityEngine.Random.Range(97, 122);
                        p1.genes[i] = (char)(randomChoice);

                    }
                }
       
                break;

            //if chosen, gene is tweaked up or down one index in the range
            case "stepUp":

                for (int i = 0; i < p1.genes.Length; i++)
                {
                    //mutates based on the mutationRate
                    if (UnityEngine.Random.Range(0f, 1f) < mutationRate)
                    {

                        int intLetter = Convert.ToInt32(p1.genes[i]);
                        intLetter = ((intLetter + 1) - 97) % (122 - 97);
                        p1.genes[i] = (char)(intLetter + 97);
                    }
                }

                break;

            //if chosen, gene is tweaked by an amount determined by gaussian mean = 0 variance = sigma
            case "gaussianConvolution":
                break;
        }

        p1.genome = new string(p1.genes);
    }
}

public class GeneticAlgo
{
    //private fields
    private Encoder _encoder;
    private Population _population;
    private Fitness _fitness;
    private Selection _selection;
    private CrossOver _crossover;
    private Mutation _mutation;
    public string _name;

    //public properties
    public Population Population {get {return _population;} set {_population = value;}}
    public Selection Selection {get {return _selection;} set {_selection = value;}}
    public Encoder Encoder { get { return _encoder;} set {_encoder = value;}}
    public Boolean Stopped { get; set; }

    public GeneticAlgo(Encoder encoder, Fitness fitness, Population population, Selection selection, CrossOver crossover, Mutation mutation)
    {
        Encoder = encoder;
        Population = population;
        _fitness = fitness;
        _selection = selection;
        _crossover = crossover;
        _mutation = mutation;
    }

    public void NextGeneration(string userSelection = "")
    {
        //should i save each generation in the population object
        
        //targetstring is blank (if selection type is user, don't need a target eh?
        //make genome more generic, and specifiable

        //currently the targetstring is defining the length of the seeded genomes. That's not ideal.

        //I should specify the seeding information in the encoder....

        //how do I deal with variable length genomes???? This matters in crossover, fitness, and seeding the population
        //for the next step only need to focus on crossover and seeding.
      
        //need to be able to seed a population from a list of values

        //stop using all the letters a - z?

        if (!(_selection._selectionType == "god mode"))
        {
            _fitness.Apply(Population);
        }

        List<Genome>parentSelection = _selection.Select(Population, userSelection);
        _crossover.Apply(Population, parentSelection);
        _mutation.Apply(Population);
        Population._generation++;

        Debug.Log(this.ToString());
    }

    public override string ToString()
    {
        string output = String.Format("Population {0} / Selection : {1} , Crossover : {2}, Mutation : {3} \n " +
                        "   Generation  # {4} {5} has the highest fitness of {6} \n\n", Population._name,
                        _selection._selectionType, _crossover._crossoverType, _mutation._mutationType,
                        Population._generation.ToString(), Population._bestGenome, Population._bestFitness);

        return output;
    }
}