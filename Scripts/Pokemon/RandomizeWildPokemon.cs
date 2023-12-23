using System;
using System.Collections.Generic;
using Godot;

public static class RandomizedWildPokemon
{
	static readonly Species[] AllSpecies = FileManager.ReadSpeciesJson();

	static Dictionary<Lists.EggGroup, List<Species>> EggGroups = new();

	static readonly int ApexShinyLength = 5;


	public static Pokemon CreateWildPokemon()
	{
		FillDictionary();
		Pokemon pokemon = new()
		{
			Species = AllSpecies[(int)new RandomNumberGenerator().RandfRange(0.0f, AllSpecies.Length)]
		};

		if (pokemon.Species.EggGroup1 != Lists.EggGroup.Infertile)
		{
			return RandomizeAncestors(pokemon, pokemon.Species);
		}
		else
		{
			return CreateBasePokemon(pokemon.Species);
		}
	}

	static Pokemon RandomizeAncestors(Pokemon pokemon, Species species)
	{
		pokemon.GrandParents = new string[4];
		pokemon.Parents = new string[2];

		Pokemon GrandParent1;
		Pokemon GrandParent2;
		Pokemon GrandParent3;
		Pokemon GrandParent4;
		Pokemon Parent1;
		Pokemon Parent2;

		GrandParent1 = CreateBasePokemon(species);
		GrandParent2 = CreateBasePokemon(species);

		if (species.EggGroup2 == Lists.EggGroup.None)
		{
			GrandParent3 = CreateBasePokemon(RandomSpecies(species.EggGroup1));
			GrandParent4 = CreateBasePokemon(RandomSpecies(species.EggGroup1));
		}
		else if (new RandomNumberGenerator().Randf() <= 0.5)
		{
			GrandParent3 = CreateBasePokemon(RandomSpecies(species.EggGroup1));
			GrandParent4 = CreateBasePokemon(RandomSpecies(species.EggGroup1));
		}
		else
		{
			GrandParent3 = CreateBasePokemon(RandomSpecies(species.EggGroup2));
			GrandParent4 = CreateBasePokemon(RandomSpecies(species.EggGroup2));
		}

		FileManager.WritePokemon(FileManager.TempPath, GrandParent1);
		FileManager.WritePokemon(FileManager.TempPath, GrandParent2);
		FileManager.WritePokemon(FileManager.TempPath, GrandParent3);
		FileManager.WritePokemon(FileManager.TempPath, GrandParent4);

		Parent1 = BreedPokemon(GrandParent1, GrandParent2);
		Parent2 = BreedPokemon(GrandParent3, GrandParent4);

		FileManager.WritePokemon(FileManager.TempPath, Parent1);
		FileManager.WritePokemon(FileManager.TempPath, Parent2);

		return BreedPokemon(Parent1, Parent2, species);
	}

	static Pokemon CreateBasePokemon(Species species)
	{
		Pokemon pokemon = new()
		{
			uuid = FileManager.UUID(),
			MajorBlood = new List<Lists.Type>(),
			MinorBlood = new List<Lists.Type>(),

			Name = species.Name,
			Species = species,
			Ability = RandomAbility(),
			Offensive = species.BaseOffensive,
			Defensive = species.BaseDefensive,
			Status = species.BaseStatus
		};
		pokemon.MajorBlood.Add(species.Type1);
		if (species.Type2 != Lists.Type.None) pokemon.MajorBlood.Add(species.Type2);
		pokemon.ApexAllele = CreateAllele(ApexShinyLength);
		pokemon.ShinyAllele = CreateAllele(ApexShinyLength);

		return pokemon;
	}

	static Species RandomSpecies(Lists.EggGroup eggGroup)
	{
		return EggGroups[eggGroup][(int)new RandomNumberGenerator().RandfRange(0, EggGroups[eggGroup].Count)];
	}

	static Lists.Ability RandomAbility()
	{
		Lists.Ability ability = Lists.Ability.None;
		if (new RandomNumberGenerator().Randf() < 0.25)
		{
			ability = GetRandomEnum<Lists.Ability>();
		}
		return ability;
	}

	public static int[] CreateAllele(int length)
	{
		int[] Alleles = new int[length];

		for (int i = 0; i < length; i++)
		{
			Alleles[i] = (int)new RandomNumberGenerator().RandfRange(0, length);
		}

		if (Alleles != null) Array.Sort(Alleles);

		return Alleles;
	}

	static Pokemon BreedPokemon(Pokemon Parent1, Pokemon Parent2)
	{
		Pokemon ChildPokemon = new()
		{
			uuid = FileManager.UUID(),
			Parents = new string[2],
			Offensive = new int[Parent1.Offensive.Length],
			Defensive = new int[Parent1.Defensive.Length],
			Status = new int[Parent1.Status.Length],
			MajorBlood = new List<Lists.Type>(),
			MinorBlood = new List<Lists.Type>(),
			ApexAllele = new int[ApexShinyLength],
			ShinyAllele = new int[ApexShinyLength],

			//Species
			Species = PickOne(Parent1.Species, Parent2.Species),
			//Ability
			Ability = PickOne(Parent1.Ability, Parent2.Ability)
		};
		ChildPokemon.Name = ChildPokemon.Species.Name;
		//Parents
		ChildPokemon.Parents[0] = Parent1.uuid;
		ChildPokemon.Parents[1] = Parent2.uuid;

		//Grandparents
		if (Parent1.Parents != null && Parent2.Parents != null)
		{
			ChildPokemon.GrandParents = new string[4];
			ChildPokemon.GrandParents[0] = Parent1.Parents[0];
			ChildPokemon.GrandParents[1] = Parent1.Parents[1];
			ChildPokemon.GrandParents[2] = Parent2.Parents[0];
			ChildPokemon.GrandParents[3] = Parent2.Parents[1];
		}

		//Apex & Shiny
		for (int i = 0; i < ApexShinyLength; i++)
		{
			ChildPokemon.ApexAllele[i] = PickOne(Parent1.ApexAllele[i], Parent2.ApexAllele[i]);
			ChildPokemon.ShinyAllele[i] = PickOne(Parent1.ShinyAllele[i], Parent2.ShinyAllele[i]);
		}

		//Offensive
		for (int i = 0; i < Parent1.Offensive.Length; i++)
		{
			int AproxNumber = (Parent1.Offensive[i] + Parent2.Offensive[i]) / 2;
			ChildPokemon.Offensive[i] = TypeShiftedNumber(AproxNumber, ChildPokemon.Species.BaseOffensive[i]);
		}

		//Defesive
		for (int i = 0; i < Parent1.Defensive.Length; i++)
		{
			int AproxNumber = (Parent1.Defensive[i] + Parent2.Defensive[i]) / 2;
			ChildPokemon.Defensive[i] = TypeShiftedNumber(AproxNumber, ChildPokemon.Species.BaseDefensive[i]);
		}

		//Status
		for (int i = 0; i < Parent1.Status.Length; i++)
		{
			int AproxNumber = (Parent1.Status[i] + Parent2.Status[i]) / 2;
			ChildPokemon.Status[i] = TypeShiftedNumber(AproxNumber, ChildPokemon.Species.BaseStatus[i]);
		}

		//Blood
		ChildPokemon.MajorBlood.Add(ChildPokemon.Species.Type1);
		if (ChildPokemon.Species.Type2 != Lists.Type.None) ChildPokemon.MajorBlood.Add(ChildPokemon.Species.Type2);

		Pokemon[] Parents = new Pokemon[2];

		Parents[0] = FileManager.ReadPokemon(Parent1.uuid);
		Parents[1] = FileManager.ReadPokemon(Parent2.uuid);

		foreach (Pokemon pokemon in Parents)
		{
			if (!ChildPokemon.MajorBlood.Contains(pokemon.Species.Type1)) ChildPokemon.MajorBlood.Add(pokemon.Species.Type1);
			if (!ChildPokemon.MajorBlood.Contains(pokemon.Species.Type2) && pokemon.Species.Type2 != Lists.Type.None) ChildPokemon.MajorBlood.Add(pokemon.Species.Type2);

			foreach (Lists.Type type in pokemon.MajorBlood)
			{
				if (!ChildPokemon.MajorBlood.Contains(type) && !ChildPokemon.MinorBlood.Contains(type)) ChildPokemon.MinorBlood.Add(type);
				else if (ChildPokemon.MinorBlood.Contains(type))
				{
					ChildPokemon.MinorBlood.Remove(type);
					ChildPokemon.MajorBlood.Add(type);
				}
			}
		}

		return ChildPokemon;
	}

	static Pokemon BreedPokemon(Pokemon Parent1, Pokemon Parent2, Species species)
	{
		Pokemon ChildPokemon = new()
		{
			Parents = new string[2],
			Offensive = new int[Parent1.Offensive.Length],
			Defensive = new int[Parent1.Defensive.Length],
			Status = new int[Parent1.Status.Length],
			MajorBlood = new List<Lists.Type>(),
			MinorBlood = new List<Lists.Type>(),
			ApexAllele = new int[ApexShinyLength],
			ShinyAllele = new int[ApexShinyLength],

			//Ability
			Ability = PickOne(Parent1.Ability, Parent2.Ability),
			//Species
			Species = species
		};
		ChildPokemon.Name = ChildPokemon.Species.Name;
		//Parents
		ChildPokemon.Parents[0] = Parent1.uuid;
		ChildPokemon.Parents[1] = Parent2.uuid;

		//Grandparents
		if (Parent1.Parents != null && Parent2.Parents != null)
		{
			ChildPokemon.GrandParents = new string[4];
			ChildPokemon.GrandParents[0] = Parent1.Parents[0];
			ChildPokemon.GrandParents[1] = Parent1.Parents[1];
			ChildPokemon.GrandParents[2] = Parent2.Parents[0];
			ChildPokemon.GrandParents[3] = Parent2.Parents[1];
		}

		//Apex & Shiny
		for (int i = 0; i < ApexShinyLength; i++)
		{
			ChildPokemon.ApexAllele[i] = PickOne(Parent1.ApexAllele[i], Parent2.ApexAllele[i]);
			ChildPokemon.ShinyAllele[i] = PickOne(Parent1.ShinyAllele[i], Parent2.ShinyAllele[i]);
		}
		Array.Sort(ChildPokemon.ApexAllele);
		Array.Sort(ChildPokemon.ShinyAllele);

		//Offensive
		for (int i = 0; i < Parent1.Offensive.Length; i++)
		{
			int AproxNumber = (Parent1.Offensive[i] + Parent2.Offensive[i]) / 2;
			ChildPokemon.Offensive[i] = TypeShiftedNumber(AproxNumber, ChildPokemon.Species.BaseOffensive[i]);
		}

		//Defesive
		for (int i = 0; i < Parent1.Defensive.Length; i++)
		{
			int AproxNumber = (Parent1.Defensive[i] + Parent2.Defensive[i]) / 2;
			ChildPokemon.Defensive[i] = TypeShiftedNumber(AproxNumber, ChildPokemon.Species.BaseDefensive[i]);
		}

		//Status
		for (int i = 0; i < Parent1.Status.Length; i++)
		{
			int AproxNumber = (Parent1.Status[i] + Parent2.Status[i]) / 2;
			ChildPokemon.Status[i] = TypeShiftedNumber(AproxNumber, ChildPokemon.Species.BaseStatus[i]);
		}

		//Blood
		ChildPokemon.MajorBlood.Add(ChildPokemon.Species.Type1);
		if (ChildPokemon.Species.Type2 != Lists.Type.None) ChildPokemon.MajorBlood.Add(ChildPokemon.Species.Type2);

		Pokemon[] Parents = new Pokemon[2];

		Parents[0] = FileManager.ReadPokemon(Parent1.uuid);
		Parents[1] = FileManager.ReadPokemon(Parent2.uuid);

		foreach (Pokemon pokemon in Parents)
		{
			if (!ChildPokemon.MajorBlood.Contains(pokemon.Species.Type1)) ChildPokemon.MajorBlood.Add(pokemon.Species.Type1);
			if (!ChildPokemon.MajorBlood.Contains(pokemon.Species.Type2) && pokemon.Species.Type2 != Lists.Type.None) ChildPokemon.MajorBlood.Add(pokemon.Species.Type2);

			foreach (Lists.Type type in pokemon.MajorBlood)
			{
				if (!ChildPokemon.MajorBlood.Contains(type) && !ChildPokemon.MinorBlood.Contains(type)) ChildPokemon.MinorBlood.Add(type);
				else if (ChildPokemon.MinorBlood.Contains(type))
				{
					ChildPokemon.MinorBlood.Remove(type);
					ChildPokemon.MajorBlood.Add(type);
				}
			}
		}

		return ChildPokemon;
	}

	static int TypeShiftedNumber(int AproxNumber, int BaseSpecies)
	{
		int modulo = AproxNumber % 5;
		if (modulo == 0)
		{
		}
		else if (modulo < 2.5)
		{
			AproxNumber += modulo + 1;
		}
		else if (modulo >= 2.5)
		{
			AproxNumber -= modulo;
		}

		if (AproxNumber < BaseSpecies)
		{
			return AproxNumber + 10;
		}
		else if (AproxNumber > BaseSpecies)
		{
			return AproxNumber - 10;
		}
		return AproxNumber;

	}

	static void FillDictionary()
	{
		for (int i = 0; i < Enum.GetValues(typeof(Lists.EggGroup)).Length - 1; i++)
		{
			EggGroups.Add((Lists.EggGroup)i, new List<Species>());
		}

		foreach (Species species in AllSpecies)
		{
			EggGroups[species.EggGroup1].Add(species);
			if (species.EggGroup2 != Lists.EggGroup.None)
			{
				EggGroups[species.EggGroup2].Add(species);
			}
		}
	}

	static T GetRandomEnum<T>()
	{
		Array A = Enum.GetValues(typeof(T));
		T V = (T)A.GetValue((int)new RandomNumberGenerator().RandfRange(0, A.Length - 1)); //Loop without the none option
		return V;
	}

	static T PickOne<T>(T Choice1, T Choice2)
	{
		if (new RandomNumberGenerator().Randf() < 0.5)
		{
			return Choice1;
		}
		return Choice2;
	}
}
