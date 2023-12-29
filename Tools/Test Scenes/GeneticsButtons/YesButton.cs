using System.Text.RegularExpressions;
using Godot;

public partial class YesButton : Button
{
	[Signal]
	public delegate void SavedPokemonEventHandler();
	public RandomizeButton randomizeButton;
	public string pokemonUUID;
	public string pokemonName = "";

	public Regex pokemonNameRegex = new Regex(@"^[a-zA-Z]+$");

	public override void _Ready()
	{
		randomizeButton = GetNode<RandomizeButton>("/root/Genetics test scene/Main/Buttons/RandomizeButton");
	}

	public override void _Pressed()
	{
		if (pokemonNameRegex.IsMatch(pokemonName))
		{
			GD.Print(pokemonName);
			Pokemon pokemon = FileManager.ReadPokemon(pokemonUUID);
			pokemon.Name = pokemonName;
			FileManager.WritePokemon(FileManager.TempPath, pokemon);
			FileManager.MovePokemon(FileManager.TempPath, FileManager.PokemonPath, pokemonUUID);
			randomizeButton.RandomizePokemon();
			EmitSignal(SignalName.SavedPokemon);
		} else {
			GD.PrintErr("Invalid name: " + pokemonName);
		}

	}

	public void _on_randomize_button_pokemon_uuid_changed(string pokemonUUID)
	{
		this.pokemonUUID = pokemonUUID;
	}

	public void _on_text_edit_changed_text(string text)
	{
		pokemonName = text;
	}
}
