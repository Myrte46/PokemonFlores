using System;
using Godot;

public static class SpeciesCreation
{
	static string Types;
	static string Status;
	static string Species;

	static readonly int TypeAmount = 20;
	static readonly int StatusAmount = 5;
	static readonly int FullStatusAmount = 8;
	public static readonly int SpeciesAmount = 261;

	static int[,] FinalTypeArray;
	static int[,] FinalStatusArray;
	public static string[,] FinalSpeciesArray;

	public static Species[] CreateAllSpecies()
	{
		DirAccess.MakeDirAbsolute(FileManager.PokemonPath);
		DirAccess.MakeDirAbsolute(FileManager.TempPath);
		DirAccess.MakeDirAbsolute(FileManager.SpeciesPath);

		Types = FileManager.ReadCSV("res://Scripts/CSV Data/Types.csv");
		Status = FileManager.ReadCSV("res://Scripts/CSV Data/Status.csv");
		Species = FileManager.ReadCSV("res://Scripts/CSV Data/Species.csv");
		FinalTypeArray = MakeNumberArray(Types, TypeAmount, TypeAmount);
		FinalStatusArray = MakeNumberArray(Status, TypeAmount, StatusAmount);
		FinalSpeciesArray = MakeStringArray(Species, 5, SpeciesAmount);

		Species[] allSpecies = new Species[SpeciesAmount];

		for (int i = 0; i < SpeciesAmount; i++)
		{
			Species species = new()
			{
				BaseOffensive = new int[TypeAmount],
				BaseDefensive = new int[TypeAmount],
				BaseStatus = new int[FullStatusAmount],
				Name = FinalSpeciesArray[i, 0],
				Type1 = (Lists.Type)Enum.Parse(typeof(Lists.Type), FinalSpeciesArray[i, 1]),
				Type2 = (Lists.Type)Enum.Parse(typeof(Lists.Type), FinalSpeciesArray[i, 2]),
				EggGroup1 = (Lists.EggGroup)Enum.Parse(typeof(Lists.EggGroup), FinalSpeciesArray[i, 3]),
				EggGroup2 = (Lists.EggGroup)Enum.Parse(typeof(Lists.EggGroup), FinalSpeciesArray[i, 4])
			};
			species.BaseOffensive = CreateOffensive(species.Type1, species.Type2);
			species.BaseDefensive = CreateDefensive(species.Type1, species.Type2);
			species.BaseStatus = CreateStatus(species.Type1, species.Type2);
			allSpecies[i] = species;
		}

		return allSpecies;
	}

	public static int[,] MakeNumberArray(string asset, int width, int height)
	{
		int[,] Result = new int[width, height];
		//Should look something like {"Ground,200,100,etc...","Poison,100,50,etc..."}
		string[] SplitAsset = asset.Split(new string[] { "\r\n" }, StringSplitOptions.None);

		//The reason it starts at 1 is to filter out the type names
		for (int i = 1; i < SplitAsset.Length; i++)
		{
			string[] NumberSplit = SplitAsset[i].Split(new string[] { "," }, StringSplitOptions.None);
			for (int j = 1; j < NumberSplit.Length; j++)
			{
				Result[j - 1, i - 1] = Convert.ToInt32(NumberSplit[j]);
			}
		}

		return Result;
	}

	public static string[,] MakeStringArray(string asset, int width, int height)
	{
		string[,] Result = new string[height, width];
		//Should look something like {"Ground,200,100,etc...","Poison,100,50,etc..."}
		string[] SplitAsset = asset.Split(new string[] { "\r\n" }, StringSplitOptions.None);

		//The reason it starts at 1 is to filter out the header
		for (int i = 1; i < SplitAsset.Length; i++)
		{
			string[] NumberSplit = SplitAsset[i].Split(new string[] { "," }, StringSplitOptions.None);
			for (int j = 0; j < NumberSplit.Length; j++)
			{
				Result[i -1, j] = NumberSplit[j];
			}
		}

		return Result;
	}
	public static int[] CreateOffensive(Lists.Type Type1, Lists.Type Type2)
	{
		int Type1Int = (int)Type1;
		int Type2Int = (int)Type2;

		int[] offense = new int[TypeAmount];

		for (int i = 0; i < TypeAmount; i++)
		{
			int shifted;
			if (i == Type1Int || (i == Type2Int && Type2 != Lists.Type.None))
			{
				shifted = 200;
			}
			else
			{
				shifted = 100;
			}
			offense[i] = shifted;
		}

		return offense;
	}

	public static int[] CreateDefensive(Lists.Type Type1, Lists.Type Type2)
	{
		int Type1Int = (int)Type1;
		int Type2Int = (int)Type2;

		int[] defense = new int[TypeAmount];

		for (int i = 0; i < TypeAmount; i++)
		{
			int shifted;
			if (Type2 == Lists.Type.None || FinalTypeArray[Type2Int, i] == 100)
			{
				shifted = FinalTypeArray[Type1Int, i];
			}
			else if (FinalTypeArray[Type1Int, i] == 100)
			{
				shifted = FinalTypeArray[Type2Int, i];
			}
			else if (FinalTypeArray[Type2Int, i] == 100)
			{
				shifted = FinalTypeArray[Type1Int, i];
			}
			else
			{
				shifted = (FinalTypeArray[Type1Int, i] + FinalTypeArray[Type2Int, i]) / 2;
			}
			defense[i] = shifted;
		}

		return defense;
	}

	public static int[] CreateStatus(Lists.Type Type1, Lists.Type Type2)
	{
		int Type1Int = (int)Type1;
		int Type2Int = (int)Type2;

		int[] status = new int[FullStatusAmount];

		for (int i = 0; i < StatusAmount; i++)
		{
			int shifted;
			if (Type2 == Lists.Type.None)
			{
				shifted = FinalStatusArray[Type1Int, i];
			}
			else if ((FinalStatusArray[Type1Int, i] == 0) || (FinalStatusArray[Type2Int, i] == 0))
			{
				shifted = 0;
			}
			else
			{
				shifted = (FinalStatusArray[Type1Int, i] + FinalStatusArray[Type2Int, i]) / 2;
			}
			status[i] = shifted;
		}

		for (int i = 0; i < FullStatusAmount - StatusAmount; i++)
		{
			status[i + StatusAmount] = 100;
		}

		return status;
	}
}
