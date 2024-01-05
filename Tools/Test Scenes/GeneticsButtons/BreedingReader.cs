using Godot;
using System;
using System.Collections.Generic;

public partial class BreedingReader : Control
{
	[Export]
	OptionButton Parent1;
	[Export]
	OptionButton Parent2;
	[Export]
	Label Parent1Species;
	[Export]
	Label Parent1EggGroup1;
	[Export]
	Label Parent1EggGroup2;
	[Export]
	Label Parent2Species;
	[Export]
	Label Parent2EggGroup1;
	[Export]
	Label Parent2EggGroup2;
	[Export]
	Button BreedButton;

	Pokemon Parent1Pokemon;
	Pokemon Parent2Pokemon;

	List<Pokemon> pokemons = new List<Pokemon>();

	[Signal]
	public delegate void BreedingCompleteEventHandler(string pokemonUUID);

	public void EnableBreeding()
	{
		Parent1.Clear();
		Parent2.Clear();
		FileManager.ReadAllFilesJson<Pokemon>(FileManager.PokemonPath).ForEach(pokemon =>
		{
			pokemons.Add(pokemon);
		});

		pokemons.ForEach(pokemon =>
		{
			Parent1.AddItem(pokemon.Name);
			Parent2.AddItem(pokemon.Name);
		});

		Parent1Pokemon = pokemons.ToArray()[0];
		Parent2Pokemon = pokemons.ToArray()[0];

		ReadParents();
	}

	void ReadParents()
	{
		Parent1Species.Text = Parent1Pokemon.Species.Name;
		Parent1EggGroup1.Text = Parent1Pokemon.Species.EggGroup1.ToString();
		Parent1EggGroup2.Text = Parent1Pokemon.Species.EggGroup2.ToString();
		Parent2Species.Text = Parent2Pokemon.Species.Name;
		Parent2EggGroup1.Text = Parent2Pokemon.Species.EggGroup1.ToString();
		Parent2EggGroup2.Text = Parent2Pokemon.Species.EggGroup2.ToString();
	}

	void _on_parent_1_item_selected(int index)
	{
		Parent1Pokemon = pokemons.ToArray()[index];
		ReadParents();
	}

	void _on_parent_2_item_selected(int index)
	{
		Parent2Pokemon = pokemons.ToArray()[index];
		ReadParents();
	}

	void _on_breed_button_pressed(){
		Pokemon child;
		if (Parent1Pokemon.Species.EggGroup1 == Lists.EggGroup.Infertile || Parent2Pokemon.Species.EggGroup1 == Lists.EggGroup.Infertile)
		{
			GD.Print("Cannot Breed");
			return;
		}

		if (Parent1Pokemon.Species.EggGroup1 == Parent2Pokemon.Species.EggGroup1 || Parent1Pokemon.Species.EggGroup1 == Parent2Pokemon.Species.EggGroup2)
		{
			child = RandomizedWildPokemon.BreedPokemon(Parent1Pokemon, Parent2Pokemon);
			FileManager.WritePokemon(FileManager.TempPath, child);
			EmitSignal(SignalName.BreedingComplete, child.uuid);
			return;
		}

		if (Parent1Pokemon.Species.EggGroup2 != Lists.EggGroup.None){
			if (Parent1Pokemon.Species.EggGroup2 == Parent2Pokemon.Species.EggGroup1 || Parent1Pokemon.Species.EggGroup2 == Parent2Pokemon.Species.EggGroup2)
			{
				child = RandomizedWildPokemon.BreedPokemon(Parent1Pokemon, Parent2Pokemon);
				FileManager.WritePokemon(FileManager.TempPath, child);
				EmitSignal(SignalName.BreedingComplete, child.uuid);
				return;
			}
		}
	}
}
