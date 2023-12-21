using System.IO;
using Godot;
using Newtonsoft.Json;

public partial class FileManager : Node
{
	// Create a field for the save file.
	string saveFile;
	string path;
	public string PokemonPath;
	public string SpeciesPath;

	public override void _Ready()
	{
		// Update the path once the persistent path exists.
		path = "user://";
		PokemonPath = path + "pokemon";
		SpeciesPath = path + "species.json";
		SpeciesCreation speciesCreation = new();
		WriteSpeciesJson(SpeciesPath, speciesCreation.CreateAllSpecies());
	}

	public static string ReadCSV(string filePath)
	{
		if (Godot.FileAccess.FileExists(filePath))
		{
			Godot.FileAccess dataFile = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
			string parsedResult = dataFile.GetAsText();

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

	public static Species[] ReadSpeciesJson(string filePath)
	{
		if (Godot.FileAccess.FileExists(filePath))
		{
			Godot.FileAccess dataFile = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Read);
			Wrapper<Species> wrapper = JsonConvert.DeserializeObject<Wrapper<Species>>(dataFile.GetAsText());
			Species[] parsedResult = wrapper.Items;

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

	public static void WriteSpeciesJson(string filePath, Species[] data)
	{
		Wrapper<Species> wrapper = new Wrapper<Species>
		{
			Items = data
		};

		Godot.FileAccess dataFile = Godot.FileAccess.Open(filePath, Godot.FileAccess.ModeFlags.Write);

		dataFile.StoreLine(JsonConvert.SerializeObject(wrapper));
	}
	public class Wrapper<T>
	{
		public T[] Items;
	}
}
