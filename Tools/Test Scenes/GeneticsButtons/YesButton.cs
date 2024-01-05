using System.Text.RegularExpressions;
using Godot;

public partial class YesButton : Button
{
	[Signal]
	public delegate void SavedPokemonEventHandler();
	public string pokemonUUID;
	public string pokemonName = "";

	public Regex pokemonNameRegex = new Regex(@"^[a-zA-Z -]+$");

	public override void _Pressed()
	{
		if (pokemonNameRegex.IsMatch(pokemonName))
		{
			GD.Print(pokemonName + " " + pokemonUUID);
			Pokemon pokemon = FileManager.ReadPokemon(pokemonUUID);
			pokemon.Name = pokemonName;
			FileManager.WritePokemon(FileManager.TempPath, pokemon);
			FileManager.MovePokemon(FileManager.TempPath, FileManager.PokemonPath, pokemonUUID);
			EmitSignal(SignalName.SavedPokemon);
		} else {
			GD.PrintErr("Invalid name: " + pokemonName);
		}
	}

	public void _on_text_edit_changed_text(string text)
	{
		pokemonName = text;
	}

	public void _on_breeding_reader_breeding_complete(string uuid)
	{
		pokemonUUID = uuid;
	}
}
