using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

public static class FileManager
{
	static readonly string path = "user://";
	public static readonly string PokemonPath = path + "Pokemon/";
	public static readonly string TempPath = PokemonPath + "Temp/";
	public static readonly string SpeciesPath = path + "Species/";


	public static string ReadCSV(string filePath)
	{
		if (FileAccess.FileExists(filePath))
		{
			FileAccess dataFile = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
			string parsedResult = dataFile.GetAsText();
			dataFile.Close();

			if (parsedResult != null)
			{
				return parsedResult;
			}
			else
			{
				GD.PrintErr("Cannot read" + filePath);
				return null;
			}
		}
		else
		{
			GD.PrintErr("File Doesn't Exist");
			return null;
		}
	}

	public static void WritePokemon(string filePath, Pokemon pokemon)
	{
		string savePath = filePath + pokemon.uuid + ".json";
		FileAccess dataFile = FileAccess.Open(savePath, FileAccess.ModeFlags.Write);
		dataFile.StoreLine(JsonConvert.SerializeObject(pokemon));
		dataFile.Close();
	}

	public static void MovePokemon(string oldPath, string newPath, string pokemonUUID)
	{
		DirAccess newDir = DirAccess.Open(newPath);
		string oldFile = oldPath + pokemonUUID + ".json";
		string newFile = newPath + pokemonUUID + ".json";
		if (!newDir.FileExists(newFile))
		{
			newDir.Copy(oldFile, newFile);
		}
		else
		{
			GD.PrintErr("File already exists");
			return;
		}
	}

	public static Pokemon ReadPokemon(string uuid)
	{
		Pokemon parsedResult;

		if (FileAccess.FileExists(TempPath + uuid + ".json"))
		{
			FileAccess dataFile = FileAccess.Open(TempPath + uuid + ".json", FileAccess.ModeFlags.Read);
			parsedResult = JsonConvert.DeserializeObject<Pokemon>(dataFile.GetAsText());
			dataFile.Close();
		}
		else if (FileAccess.FileExists(PokemonPath + uuid + ".json"))
		{
			FileAccess dataFile = FileAccess.Open(PokemonPath + uuid + ".json", FileAccess.ModeFlags.Read);
			parsedResult = JsonConvert.DeserializeObject<Pokemon>(dataFile.GetAsText());
			dataFile.Close();
		}
		else
		{
			GD.PrintErr("File Doesn't Exist");
			return null;
		}

		if (parsedResult != null)
		{
			return parsedResult;
		}
		else
		{
			GD.PrintErr("Cannot read path");
			return null;
		}
	}

	public static Species[] ReadSpeciesJson(string SpeciesPath = "user://Species/")
	{
		List<Species> species = new();
		string[] FilePath = DirAccess.GetFilesAt(SpeciesPath);
		foreach (string Name in FilePath)
		{
			if (FileAccess.FileExists(SpeciesPath + Name))
			{
				FileAccess dataFile = FileAccess.Open(SpeciesPath + Name, FileAccess.ModeFlags.Read);
				Species parsedResult = JsonConvert.DeserializeObject<Species>(dataFile.GetLine());
				dataFile.Close();

				if (parsedResult != null)
				{
					species.Add(parsedResult);
				}
				else
				{
					GD.PrintErr("Cannot read " + SpeciesPath + Name);
				}
			}
			else
			{
				GD.PrintErr("File Doesn't Exist");
			}
		}

		return species.ToArray();

	}

	public static void DeleteTempFiles(string filePath = "user://Pokemon/Temp/")
	{
		string[] files = DirAccess.GetFilesAt(filePath);
		foreach (string name in files)
		{
			string fullPath = filePath + name;
			if (FileAccess.FileExists(fullPath))
			{
				DirAccess dataDir = DirAccess.Open(filePath);
				dataDir.Remove(fullPath);
			}
		}
	}

	public static void WriteSpeciesJson(Species[] data, string filePath = "user://Species/")
	{
		foreach (Species species in data)
		{
			FileAccess dataFile = FileAccess.Open(filePath + species.Name + ".json", FileAccess.ModeFlags.Write);
			dataFile.StoreLine(JsonConvert.SerializeObject(species));
			dataFile.Close();
		}

	}

	public class Wrapper<T>
	{
		public T[] Items;
	}

	public static string UUID()
	{
		string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
		int outputLength = 16;
		string output = "";

		for (int i = 0; i < outputLength; i++)
		{
			output += chars[(int)new RandomNumberGenerator().RandfRange(0, chars.Length)];
		}

		return output;
	}
}
