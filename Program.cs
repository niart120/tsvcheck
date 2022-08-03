using CommandLine;
using CommandLine.Text;
using PokemonStandardLibrary.CommonExtension;
using PokemonStandardLibrary;
using tsvcheck;

class Program 
{ 
	enum ROM
	{
		SM, USUM
	}

	class Options
	{

		[Option('t', "tid", Required = true, HelpText = "6 digits trainer id.")]
		public uint G7TID { get; set; }

		[Option('i', "iv", Required = true, HelpText = "Pokemon ivs.")]
		public IEnumerable<uint> IVs { get; set; }

		[Option('n', "nature", Required = true, HelpText = "Pokemon nature (Japanese).")]
		public string NatureStr { get; set; }

		[Option('u', "usum", Required = false, HelpText = "Optional. Use this option if you play USUM.")]
		public bool IsUSUM { get; set; }

		[Option('r', "range", Required = false, HelpText = "Optional. Search range (default: 15000 50000)")]
		public IEnumerable<int> Range { get; set; }

	}

	static void Main(string[] args)
	{

		var parser = new CommandLine.Parser(with => with.HelpWriter = null);
		var parserResult = parser.ParseArguments<Options>(args);

		var helpText = HelpText.AutoBuild(parserResult, h =>
		{
			//configure HelpText
			h.AdditionalNewLineAfterOption = false; //remove newline between options
			h.Heading = "tsvcheck"; //change header
			h.Copyright = "Copyright (c) 2022  https://github.com/niart120"; //change copyright text
			h.AddPreOptionsLine("Usage: tsvcheck -t g7tid -i h a b c d s -n nature [-u] [-r min max]");

			return h;
		}, e => e);

		 parserResult.WithParsed(RunOptions)
		  .WithNotParsed(err => Console.WriteLine(helpText));
	}
	static void RunOptions(Options opts)
	{
		//handle options

		bool validation = true;
		//validate tid
		if (!validateTID(opts.G7TID))
        {
			Console.WriteLine("Invalid Trainer ID");
			validation = false;
        }
		uint g7tid = opts.G7TID;

		//validate ivs
		if (!validateIVs(opts.IVs))
        {
			Console.WriteLine("Invalid IVs");
			validation = false;
		}

		List<uint> ivs = new List<uint>(opts.IVs);

		//validate nature
		if (!validateNature(opts.NatureStr))
        {
			Console.WriteLine("Invalid Nature");
			validation = false;
		}

		Nature nature = CommonExtension.ConvertToNature(opts.NatureStr);

		//validate range
		if (!validateRange(opts.Range))
        {
			Console.WriteLine("Invalid Range");
			validation = false;

        }
		var min = 15000;
		var max = 50000;
		if (opts.Range.Count() != 0) {
			min = opts.Range.Min();
			max = opts.Range.Max();
		}

		//validation checks
		if (!validation)
        {
			Console.WriteLine("Invalid input was detected. Please check input values.");
			return;
		}

		TSVChecker checker = new TSVChecker(g7tid, ivs, nature, opts.IsUSUM, min, max);
		checker.Check();

	}

    private static bool validateRange(IEnumerable<int> range)
    {
		if (range.Count() == 0)
        {
			return true;
        }
       if (range.Count() != 2)
        {
			return false;
        }
		foreach (int r in range)
		{
			if (r < 0)
			{
				return false;
			}
		}
		return true;
	}

    private static bool validateNature(string nature)
    {
        if (CommonExtension.ConvertToNature(nature) == Nature.other)
        {
			return false;
        };
		return true;

	}

    private static bool validateIVs(IEnumerable<uint> ivs)
    {
        if (ivs.Count() != 6)
        {
			return false;
        }
		foreach (uint iv in ivs)
        {
			if (iv < 0 || 31 < iv)
            {
				return false;
            }
        }
		return true;
    }

    private static bool validateTID(uint g7TID)
    {
        if (g7TID < 0 || 1_000_000 <= g7TID)
        {
			return false;
        }
		return true;
    }

}