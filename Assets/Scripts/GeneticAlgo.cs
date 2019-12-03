using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//this interface must be implemented to convert from genome strings to phenotypes
//T is the type that the phenotype is represented by
interface IEncoder<T>
{
    string Encode(T obj);
    T Decode(string genomeString);
}
//converts from a LSystem to string based genome
public class Encoder : IEncoder<string>
{
    public string _symbols;
    public Encoder(string symbols)
    {
       _symbols = symbols;
    }
    public string Encode(string rule)
    {
        char[] chars = new char[rule.Length];

        for (int i = 0; i < rule.Length; i++)
        {
            chars[i] = (char)(_symbols.IndexOf(rule[i]) + 97);
        }

        string output = new string(chars);

        return output;
    }

    public string Decode(string genomeString)
    {
        string rule = "";
        char[] chars = genomeString.ToCharArray();
        for (int i = 0; i < genomeString.Length; i++)
        {
            int index = (chars[i] - 97);
            rule += _symbols[index].ToString();
        }

        return rule;
    }
}


public class Population
{
    public string _name, _symbols;

    public int _generation = 0;
    public int _bestFitness;
    public int _size;

    public string[] _bestGenome;
    public Genome[] _genomes;
    public int[] _fitnesses;

    public bool _variableGenomeLength;

    public Population(int size, string symbols, string target = "", string[][] samplePopulation = null, bool variableGenomeLength = false)
    {
        _size = size;
        _genomes = new Genome[_size];
        _symbols = symbols;

        if (samplePopulation != null)
        {
            SeedFromSamples(samplePopulation);
        }
        else
        {
            SeedRandom(target);
        }

        _variableGenomeLength = variableGenomeLength;
    }

    //creates first generation //NEED TO MAKE THIS WORK FOR STRING ARRAYS and NEED TO CHECK FOR TARGETSTRING
    public void SeedRandom(string target)
    {
        System.Random randomInt = new System.Random(); //for random seed for the genome

        for (int i = 0; i < _size; i++)
        {
            _genomes[i] = new Genome(target.Length, _symbols, randomInt.Next());
        }
        _generation++;
    }

    public void SeedFromSamples(string[][] samplePopulation)
    {
        for (int i = 0; i < _size; i++)
        {
            _genomes[i] = new Genome(samplePopulation[i % samplePopulation.Length], _symbols); //adds samples to the population loops around the samples
        }
        _generation++;
    }
}


public class Fitness
{
    public string _target;

    public Fitness(string target)
    {
        _target = target;
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
        for (int j = 0; j < g.genome.Length; j++)
        {
            //calculates the difference in characters between the candidate genome and target
            for (int i = 0; i < g.genome[j].Length; i++)
            {
                if (g.genome[j][i] == _target[i])
                {
                    fitness++;
                }
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

            //this is when the user interactively chooses after each generation the best candidates
            case "god mode":

                string[] indices = userSelection.Split(' ');
                int selectionCount = indices.Length;
                //if user doesn't choose any, just use the whole population again as parents
                if (selectionCount == 0)
                {
                    selectionPool = p._genomes.Select(x => x).ToList();
                }
                else
                {
                    for (int i = 0; i < size; i++)
                    {
                        string mod = indices[i % selectionCount];
                        //Debug.Log("User chose Candidate " + mod + " Genome : " + p._genomes[int.Parse(mod)].ToString());
                        //adds the selected candidates to the pool in roughly equal proportion
                        //slight issue here with low populations/odd numbers of selections which biases the selectionPool a bit
                        selectionPool.Add(p._genomes[int.Parse(mod)]);
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

    public CrossOver(string crossoverType)
    {
        _crossoverType = crossoverType;
    }

    public void Apply(Population p, List<Genome> selectionPool)
    {
        //population has to be even number currently......
        //because the population size is driving the selectionPool size
        p._genomes = new Genome[p._size];
        Stack<Genome> selectionStack = new Stack<Genome>(selectionPool);

        //loop over pairs, as need two parents per two children
        for (int i = 0; i < p._size; i += 2)
        {
            //add the two new children to the new genomes for this generation (have been shuffled previously)

            p._genomes[i] = new Genome(selectionStack.Pop());
            p._genomes[i + 1] = new Genome(selectionStack.Pop());
            //evolve them
            Cross(p._genomes[i], p._genomes[i + 1], p._variableGenomeLength, i);
            //maybe could write test to check if variable genome lengths
        }
    }

    //takes two genomes as input and mutates them
    public void Cross(Genome p1, Genome p2, bool variableGenomeLength, int seed)
    {
        int minGenes = Math.Min(p1.genome.Length, p2.genome.Length);
        int genomeChoice = UnityEngine.Random.Range(0, minGenes);

        //these two random variables are the same.......
        System.Random random = new System.Random(seed);
        int xPt1 = random.Next(0, p1.genome[genomeChoice].Length); //1st crossover pt on first genome
        int yPt1 = random.Next(0, p2.genome[genomeChoice].Length); //1st crossover pt on second genome
        //int xPt1 = UnityEngine.Random.Range(0, p1.genome[genomeChoice].Length); //1st crossover pt on first genome
        //int yPt1 = UnityEngine.Random.Range(0, p2.genome[genomeChoice].Length); //1st crossover pt on second genome

        //if genomes all have the same length then cross at same point to maintain this.
        if (!variableGenomeLength) { yPt1 = xPt1; } //if all genomes the same length, use the same 1st crossover pt

        //if genomes can be different lengths, need to limit the crossing point to the shortest genome
        if (variableGenomeLength) { yPt1 = Math.Min(xPt1, yPt1); xPt1 = yPt1; }

        Debug.Log("first cross pt " + xPt1.ToString() + " " + yPt1.ToString());
        string crossedString1, crossedString2;

        switch (_crossoverType)
        {
            //cuts the genotype at one point and swaps genes
            case "OnePt":

                crossedString1 = p1.genome[genomeChoice].Substring(0, xPt1) + p2.genome[genomeChoice].Substring(yPt1);

                p1.setGene(genomeChoice, crossedString1);

                crossedString2 = p2.genome[genomeChoice].Substring(0, yPt1) + p1.genome[genomeChoice].Substring(xPt1);

                p2.setGene(genomeChoice, crossedString2);

                break;

            //cuts the genotype at two points and swaps genes
            case "TwoPt":

                int xPt2 = UnityEngine.Random.Range(0, p1.genome.Length); //2nd crossover pt on first genome
                int yPt2 = UnityEngine.Random.Range(0, p2.genome.Length); //2nd crossover pt on first genome

                if (!variableGenomeLength) { yPt2 = xPt2; } //if all genomes the same length, use the same 2nd crossover pt
                if (variableGenomeLength) { yPt1 = Math.Min(xPt1, yPt1); xPt1 = yPt1; }

                int xmin = Math.Min(xPt1, xPt2);
                int xmax = Math.Max(xPt1, xPt2);

                int ymin = Math.Min(yPt1, yPt2);
                int ymax = Math.Max(yPt1, yPt2);

                crossedString1 = p1.genome[genomeChoice].Substring(0, xmin) + p2.genome[genomeChoice].Substring(ymin, ymax - ymin) + p1.genome[genomeChoice].Substring(xmax);
                crossedString2 = p2.genome[genomeChoice].Substring(0, ymin) + p1.genome[genomeChoice].Substring(xmin, xmax - xmin) + p2.genome[genomeChoice].Substring(ymax);

                p1.setGene(genomeChoice, crossedString1);
                p2.setGene(genomeChoice, crossedString2);

                break;

            //swaps genes by index depending on the swap rate
            case "Uniform":

                float swapRate = 0.1f;
                char[] chars1 = p1.genome[genomeChoice].ToCharArray();
                char[] chars2 = p2.genome[genomeChoice].ToCharArray();

                int minLength = Math.Min(chars1.Length, chars2.Length); //only swaps up till the end of shortest genome

                for (int i = 0; i < minLength; i++)
                {
                    //if under swapRate swap genes at index
                    if (UnityEngine.Random.Range(0f, 1f) < swapRate)
                    {
                        char temp = chars1[i];
                        chars1[i] = chars2[i];
                        chars2[i] = temp;
                    }
                }

                p1.setGene(genomeChoice, new string(chars1));
                p2.setGene(genomeChoice, new string(chars2));

                break;
        }
    }
}

public class Mutation
{
    public string _mutationType;
    float _mutationRate;

    public Mutation(string mutationType, float mutationRate = 0.1f)
    {
        _mutationType = mutationType;
        _mutationRate = mutationRate;
    }

    public void Apply(Population p)
    {
        for (int i = 0; i < p._size; i += 1)
        {
            System.Random seed = new System.Random(i);
            Mutate(p._genomes[i], seed);
        }
    }

    //takes genome as input and mutates it
    public void Mutate(Genome p1, System.Random seed)
    {
        int genomeChoice = UnityEngine.Random.Range(0, p1.genome.Length); //choose which gene to mutate
        char[] bases = new char[p1.genome[genomeChoice].Length];
        switch (_mutationType)
        {
            //if chosen, gene is mutated to random choice in range
            case "randomChoice":

                for (int i = 0; i < p1.genome[genomeChoice].Length; i++)
                {
                    System.Random r = new System.Random(seed.Next());
                    bases[i] = p1.genome[genomeChoice][i];

                    //mutates based on the mutationRate
                    double test = r.NextDouble();
                    if (test < _mutationRate)
                    {
                        int randomChoice = r.Next(97, 97 + p1.baseTypeCount);
                        bases[i] = (char)(randomChoice); //if mutate overwrite it
                    }
                }
                p1.setGene(genomeChoice, new string(bases));
                break;

            //if chosen, gene is tweaked up or down one index in the range
            case "stepUp":

                for (int i = 0; i < p1.genome[genomeChoice].Length; i++)
                {
                    bases[i] = p1.genome[genomeChoice][i];

                    //mutates based on the mutationRate
                    if (UnityEngine.Random.Range(0f, 1f) < _mutationRate)
                    {
                        int intLetter = Convert.ToInt32(p1.genome[genomeChoice][i]);
                        intLetter = ((intLetter + 1) - 97) % (p1.baseTypeCount - 97);
                        bases[i] = (char)(intLetter + 97);
                    }
                }
                p1.setGene(genomeChoice, new string(bases));
                break;

            //if chosen, gene is tweaked by an amount determined by gaussian mean = 0 variance = sigma
            case "gaussianConvolution":
                throw new NotImplementedException("Not yet implemented.");
        }
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
    public Population Population { get { return _population; } set { _population = value; } }
    public Selection Selection { get { return _selection; } set { _selection = value; } }
    public Encoder Encoder { get { return _encoder; } set { _encoder = value; } }
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

        //target is blank (if selection type is user, don't need a target eh?
        //make genome more generic, and specifiable

        //currently the target is defining the length of the seeded genomes. That's not ideal.

        //I should specify the seeding information in the encoder....

        //how do I deal with variable length genomes???? This matters in crossover, fitness, and seeding the population
        //for the next step only need to focus on crossover and seeding.

        //need to be able to seed a population from a list of values

        if (!(_selection._selectionType == "god mode"))
        {
            _fitness.Apply(Population);
        }

        List<Genome> parentSelection = _selection.Select(Population, userSelection); //applies the selection algorithm chosen to the population
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