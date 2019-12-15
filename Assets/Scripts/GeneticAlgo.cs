using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//this interface must be implemented to convert from genome strings to phenotypes
//would like to flesh this out more, just an exercise really for now..
interface IEncoder<T>
{
    T Encode(T obj);
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

    //converts string to letter from a to ... (97 is a in ASCII)
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

    //reverses Encode
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

//defines the population of genomes
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

    //creates a population either based on a sample or a target string
    //target string is a legacy from evolving words, constructor could do with splitting out probably at some point
    //size must be even currently which could be resolved/caught
    public Population(int size, string symbols, string target = "", string[][] samplePopulation = null, bool variableGenomeLength = false)
    {
        _size = size;
        _genomes = new Genome[_size];
        _symbols = symbols;

        if (samplePopulation != null)
        {
            SeedFromSamples(samplePopulation);
        }
        else if (target == "")
        {
            throw new ArgumentOutOfRangeException("Must have non empty target string or sample population");
        } 
        else
        {
            SeedRandom(target);
        }

        _variableGenomeLength = variableGenomeLength;
    }

    //creates a random first generation
    public void SeedRandom(string target)
    {
        System.Random randomInt = new System.Random(); //for random seed for the genome

        for (int i = 0; i < _size; i++)
        {
            _genomes[i] = new Genome(target.Length, _symbols, randomInt.Next());
        }
        _generation++;
    }

    //populates the population using a sample of already encoded genomes
    public void SeedFromSamples(string[][] samplePopulation)
    {
        for (int i = 0; i < _size; i++)
        {
            _genomes[i] = new Genome(samplePopulation[i % samplePopulation.Length], _symbols); //adds samples to the population loops around the samples
        }
        _generation++;
    }
}

//class that determines fitness for a population
//this class isn't used in the L system evolution, it was used in word/image evolution
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

    //calculates an individual genomes fitness based on whether the base matches exactly
    private int FitnessFunction(Genome g)
    {
        int fitness = 0;
        for (int j = 0; j < g._genome.Length; j++)
        {
            //calculates the difference in characters between the candidate genome and target
            for (int i = 0; i < g._genome[j].Length; i++)
            {
                if (g._genome[j][i] == _target[i])
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

        p._bestGenome = p._genomes[0]._genome;
        p._bestFitness = p._fitnesses[0];

        //string top3fit = String.Format("{0} Population : Generation {1} : Top 3 Fitnesses {2}, {3}, {4}", p._name, p._generation, p._fitnesses[0].ToString(), p._fitnesses[1].ToString(), p._fitnesses[2].ToString());
        //string top5pheno = String.Format("{0} Population : Generation  {1} : Top 5 genomes ", p._name, p._generation);

        //for (int i = 0; i < 5; i++)
        //{
        //    top5pheno += " " + i + " - " + p._genomes[i].ToString();
        //}

        ////print out top candidates
        //Debug.Log(top3fit);
        //Debug.Log(top5pheno);

        return p;
    }
}

//class that controls selection of parents from population
public class Selection
{
    public string _selectionType;

    public Selection(string selectionType)
    {
        _selectionType = selectionType;
    }

    //creates cumulative density function for use in some of the selection types
    //again not used in the L system evolution as it's user choice based
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

    //selects from the population based on selection type specified in constructor
    public List<Genome> Select(Population p, string userSelection = "")
    {
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
                if (userSelection == "")
                {
                    selectionPool = p._genomes.Select(x => x).ToList();
                    Debug.Log("Warning : No user choice specified, whole generation used as parents");
                }
                else
                {
                    for (int i = 0; i < size; i++)
                    {
                        string mod = indices[i % selectionCount];
                        selectionPool.Add(p._genomes[int.Parse(mod)]);
                    }
                }
                break;

            //tournament selection of size 2, preserves rank so preferable to fitness proportional apparently (see metaheuristics book)
            //I suspect this is only the case at large scale and if you tend towards high fitnesses in all candidates
            //should make 2 a variable
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
            //can be more representative than fitnessProportional
            case "stochasticUniversalSampling":

                cdf = Cdf(genomes, fitnesses);

                for (int i = 0; i < size; i++)
                {
                    int r = UnityEngine.Random.Range((cdf.Count / size) * i, (cdf.Count / size) * (i + 1));
                    selectionPool.Add(cdf[r]);
                }
                break;

            //takes the top 5 by fitness and then fills the selection pool with size /5 of those
            //by far the best for the tests I've run so far
            //should make 5 a variable
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

//class that exchanges gene segments between parents selected in Selection class
public class CrossOver
{
    public string _crossoverType;
    private int _maxGeneGrowth = 10; //constrains the amount a gene can get longer

    public CrossOver(string crossoverType)
    {
        _crossoverType = crossoverType;
    }

    public void Apply(Population p, List<Genome> selectionPool)
    {
        //population has to be even number currently, because the population size is driving the selectionPool size
        p._genomes = new Genome[p._size];
        Stack<Genome> selectionStack = new Stack<Genome>(selectionPool);

        //loop over pairs, as need two parents per two children
        for (int i = 0; i < p._size; i += 2)
        {
            //add the two new children to the new genomes for this generation (have been shuffled previously in selection)
            p._genomes[i] = new Genome(selectionStack.Pop());
            p._genomes[i + 1] = new Genome(selectionStack.Pop());

            //cross them
            Cross(p._genomes[i], p._genomes[i + 1], p._variableGenomeLength, i);
        }
    }

    //takes two genomes as input and crosses them
    public void Cross(Genome p1, Genome p2, bool variableGenomeLength, int seed)
    {
        //randomly pick a gene from each genome to cross
        int geneChoice1 = UnityEngine.Random.Range(0, p1._genome.Length);
        int geneChoice2 = UnityEngine.Random.Range(0, p2._genome.Length);

        //get randomly selected genes from each genome
        var p1Gene = p1._genome[geneChoice1];
        var p2Gene = p2._genome[geneChoice2];

        System.Random random = new System.Random(seed);
        int xPt1 = random.Next(0, p1Gene.Length); //1st crossover pt on first genome
        int yPt1 = random.Next(0, p2Gene.Length); //1st crossover pt on second genome

        //essentially a two sided clamp, restricting the max dist between xPt1 and yPt1, restricts the gene growth so the fractals don't blow out!
        //also restricts the values to  be within the length of the genome
        int growthCap = Math.Max(_maxGeneGrowth, Math.Abs(yPt1 - xPt1));

        if (xPt1 < yPt1)
        {
            yPt1 = random.Next(0, Math.Min(xPt1 + growthCap, p2Gene.Length));
        }
        else
        {
            yPt1 = random.Next(Math.Max(Math.Min(xPt1 - growthCap, p2Gene.Length), 0), p2Gene.Length);
        }

        //if genomes all have the same length then cross at same point to maintain lengths
        if (!variableGenomeLength)
        {
            yPt1 = xPt1;
        }
  
        string crossedString1, crossedString2;

        //determines which crossover type to apply
        switch (_crossoverType)
        {
            //cuts the genotype at one point and swaps genes
            case "OnePt":

                crossedString1 = p1Gene.Substring(0, xPt1) + p2Gene.Substring(yPt1);

                p1.SetGene(geneChoice1, crossedString1);

                crossedString2 = p2Gene.Substring(0, yPt1) + p1Gene.Substring(xPt1);

                p2.SetGene(geneChoice2, crossedString2);

                break;

            //cuts the genotype at two points and swaps genes
            //not tested for L system evolution, worked in text/image
            case "TwoPt":

                int xPt2 = UnityEngine.Random.Range(0, p1Gene.Length); //2nd crossover pt on first genome
                int yPt2 = UnityEngine.Random.Range(0, p2Gene.Length); //2nd crossover pt on second genome

                if (!variableGenomeLength) { yPt2 = xPt2; } //if all genomes the same length, use the same 2nd crossover pt
                if (variableGenomeLength) { yPt1 = Math.Min(xPt1, yPt1); xPt1 = yPt1; }

                int xmin = Math.Min(xPt1, xPt2);
                int xmax = Math.Max(xPt1, xPt2);

                int ymin = Math.Min(yPt1, yPt2);
                int ymax = Math.Max(yPt1, yPt2);

                crossedString1 = p1Gene.Substring(0, xmin) + p2Gene.Substring(ymin, ymax - ymin) + p1Gene.Substring(xmax);
                crossedString2 = p2Gene.Substring(0, ymin) + p1Gene.Substring(xmin, xmax - xmin) + p2Gene.Substring(ymax);

                p1.SetGene(geneChoice1, crossedString1);
                p2.SetGene(geneChoice2, crossedString2);

                break;

            //swaps genes by index depending on the swap rate
            case "Uniform":

                float swapRate = 0.1f;
                char[] chars1 = p1Gene.ToCharArray();
                char[] chars2 = p2Gene.ToCharArray();

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

                p1.SetGene(geneChoice1, new string(chars1));
                p2.SetGene(geneChoice2, new string(chars2));

                break;
        }
    }
}

//class that mutates genes probabilistically
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
            //MutateGrow(p._genomes[i], seed);
        }
    }

    //takes genome as input and mutates it
    //should implement global random
    public void Mutate(Genome p1, System.Random seed)
    {
        int geneChoice1 = UnityEngine.Random.Range(0, p1._genome.Length); //choose which gene to mutate
        char[] bases = new char[p1._genome[geneChoice1].Length];
        switch (_mutationType)
        {
            //if chosen, gene is mutated to random choice in range of valid chars
            case "randomChoice":

                for (int i = 0; i < p1._genome[geneChoice1].Length; i++)
                {
                    System.Random r = new System.Random(seed.Next());
                    bases[i] = p1._genome[geneChoice1][i];

                    //mutates based on the mutationRate
                    double test = r.NextDouble();
                    if (test < _mutationRate)
                    {
                        int randomChoice = r.Next(97, 97 + p1._baseTypeCount);
                        bases[i] = (char)(randomChoice); //if mutate overwrite it
                    }
                }
                p1.SetGene(geneChoice1, new string(bases));
                break;

            //if chosen, gene is tweaked up one index in the range
            case "stepUp":

                for (int i = 0; i < p1._genome[geneChoice1].Length; i++)
                {
                    bases[i] = p1._genome[geneChoice1][i];

                    //mutates based on the mutationRate
                    if (UnityEngine.Random.Range(0f, 1f) < _mutationRate)
                    {
                        int intLetter = Convert.ToInt32(p1._genome[geneChoice1][i]);
                        intLetter = ((intLetter + 1) - 97) % (p1._baseTypeCount - 97);
                        bases[i] = (char)(intLetter + 97);
                    }
                }
                p1.SetGene(geneChoice1, new string(bases));
                break;

            //if chosen, gene is tweaked by an amount determined by gaussian mean = 0 variance = sigma
            case "gaussianConvolution":
                throw new NotImplementedException("Not yet implemented.");
        }
    }

    //takes genome as input and mutates it
    //public void MutateGrow(Genome p1, System.Random seed)
    //{
    //    System.Random r = new System.Random(seed.Next());
    //    double test = r.NextDouble();
    //    if (test < _mutationRate)
    //    {
    //        p1.AddGene();
    //        Debug.Log("New gene spawned on genome!");
    //        Debug.Log(p1._genome[p1._genome.Length - 1]);
    //    }
    //}
}

//class than combines all the modules into the final GA
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
        //only apply the fitness class if selection type is fitness based
        if (!(_selection._selectionType == "god mode"))
        {
            _fitness.Apply(Population);
        }

        List<Genome> parentSelection = _selection.Select(Population, userSelection); //applies the selection algorithm chosen to the population
        _crossover.Apply(Population, parentSelection);
        _mutation.Apply(Population);
        Population._generation++;

        Debug.Log(this);
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


/* Notes for further improvements
 * 
 * Make the Population constructor clearer
 * Currently the target is defining the length of the random seeded genomes. That's not ideal.
 * Save generation history in the population
 * Make the fitness class more modular so easy to drop in new fitness functions
 * Crossover currently requires an even number in population due to pairing
 * Could replace all random seeds with a global static
 * Implement Gaussian Convolution in the Mutation Class so then can do CMA Evolutionary Systems etc
 * Implementation of the god mode as a selection type could be more elegant
 * 
 */

