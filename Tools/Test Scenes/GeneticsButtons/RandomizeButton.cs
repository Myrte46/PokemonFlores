using Godot;

public partial class RandomizeButton : Button
{
	PokemonReader pokemonReader;

	public override void _Ready()
	{
		pokemonReader = GetNode<PokemonReader>("/root/Genetics test scene");
		FileManager.WriteSpeciesJson(SpeciesCreation.CreateAllSpecies());
	}

	public override void _Pressed()
	{
		Pokemon pokemon = RandomizedWildPokemon.CreateWildPokemon();
		GD.Print(pokemon);
		pokemonReader.ReadPokemon(pokemon);
	}
}
