using Godot;

public partial class SaveButton : Button
{
	RandomizeButton randomizeButton;
	string pokemonUUID;

	public override void _Ready()
	{
		randomizeButton = GetNode<RandomizeButton>("/root/Genetics test scene/Main/Buttons/RandomizeButton");
	}

	public override void _Pressed()
	{
		FileManager.MovePokemon(FileManager.TempPath, FileManager.PokemonPath, pokemonUUID);
		randomizeButton.RandomizePokemon();
	}

	public void _on_randomize_button_pokemon_uuid_changed(string pokemonUUID)
	{
		this.pokemonUUID = pokemonUUID;
	}
}
