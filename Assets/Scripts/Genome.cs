using System.Linq;

//genome represented as array of strings, each string represents a gene, each letter represents a base
public class Genome
{

    public string[] _genome;
    public int _baseTypeCount;
    float _mutateRate;
    int _defaultGeneLength = 10;

    //constructor for genome from parent genome
    public Genome(string[] parentGenome, string symbols)
    {
        _genome = parentGenome.ToArray();
        _baseTypeCount = symbols.Length;
    }

    //constructor for random genotype by length
    public Genome(int length, string symbols, int randomSeed)
    {

        _baseTypeCount = symbols.Length;
        GenerateRandomGenome(length, randomSeed);
    }

    //copy constructor
    public Genome(Genome g)
    {
        _genome = g._genome.ToArray();
        _mutateRate = g._mutateRate;
        _baseTypeCount = g._baseTypeCount;
    }

    //assigns new value to a gene
    public void SetGene(int geneIndex, string value)
    {
        _genome[geneIndex] = value;
    }

    //should make random global seed when have time
    //creates a random genome
    public void GenerateRandomGenome(int length, int randomSeed)
    {
        System.Random randomInt = new System.Random(randomSeed);
        for (int j = 0; j < _genome.Length; j++)
        {
            _genome[j] = GenerateRandomGene(randomInt);
        }
    }

    //creates a random gene
    public string GenerateRandomGene(System.Random r)
    {
        char[] bases = new char[_defaultGeneLength];

        for (int i = 0; i < _defaultGeneLength; i++)
        {
            int randInt = r.Next(97, 97 + _baseTypeCount);
            bases[i] = (char)randInt;
        }
        return new string(bases);
    }

    public override string ToString()
    {
        string outputString = "";

        for (int i = 0; i < _genome.Length; i++)
        {
            for (int j = 0; j < _genome[i].Length; j++)
            {
                outputString += _genome[i][j];
            }
            outputString += " ";
        }

        return outputString;
    }

    //extends a genome by one randomly generated gene
    //public void AddGene()
    //{
    //    string[] newGenome = new string[_genome.Length + 1];
    //    System.Random randomInt = new System.Random();
    //    newGenome[_genome.Length] = GenerateRandomGene(randomInt);
    //    _genome.CopyTo(newGenome, _genome.Length - 1);
    //    _genome = newGenome;
    //}

    //   //to implement later, normal dist approximation
    //public double randDist(){

    //	System.Random rand = new System.Random(); //reuse this if you are generating many
    //	double u1 = 1.0-rand.NextDouble(); //uniform(0,1] random doubles
    //	double u2 = 1.0-rand.NextDouble();
    //	double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
    //		Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
    //	double randNormal =	0 + 1 * randStdNormal; //random normal(mean,stdDev^2)

    //	return randNormal;
    //}
}



