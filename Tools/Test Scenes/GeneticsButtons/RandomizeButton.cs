using Godot;

public partial class RandomizeButton : Button
{
	PokemonReader pokemonReader;

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
		FileManager.DeleteTempFiles();
		Pokemon pokemon = RandomizedWildPokemon.CreateWildPokemon();
		pokemonReader.ReadPokemon(pokemon);
	}

	public override void _Pressed()
	{
		FileManager.DeleteTempFiles();
		Pokemon pokemon = RandomizedWildPokemon.CreateWildPokemon();
		pokemonReader.ReadPokemon(pokemon);
	}
}
