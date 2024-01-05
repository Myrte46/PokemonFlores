using Godot;

public partial class RandomizeButton : Button
{
	PokemonReader pokemonReader;
	public string pokemonUUID;

	[Signal]
	public delegate void PokemonUUIDChangedEventHandler(string pokemonUUID);

	public override void _Ready()
	{
		pokemonReader = GetNode<PokemonReader>("/root/Genetics test scene");
		FileManager.WriteSpeciesJson(SpeciesCreation.CreateAllSpecies());
		RandomizedWildPokemon.FillDictionary();

		DelayedReady();
	}

	async void DelayedReady()
	{
		await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
		RandomizePokemon();
	}

	public override void _Pressed()
	{
		RandomizePokemon();
	}

	public void RandomizePokemon(){
		FileManager.DeleteTempFiles();
		Pokemon pokemon = RandomizedWildPokemon.CreateWildPokemon();
		FileManager.WritePokemon(FileManager.TempPath, pokemon);
		pokemonReader.ReadPokemon(pokemon);
		EmitSignal(SignalName.PokemonUUIDChanged, pokemon.uuid);
	}
}