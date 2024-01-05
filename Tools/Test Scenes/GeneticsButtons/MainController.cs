using Godot;
using System;
using System.Collections.Generic;

public partial class MainController : Node
{
	[Export]
	BoxContainer InfoContainer;
	[Export]
	GridContainer ListContainer;
	[Export]
	BoxContainer AncestryContainer;
	[Export]
	BoxContainer BreedContainer;
	[Export]
	Panel NamePokemonContainer;
	[Export]
	YesButton yesButton;

	[Export]
	PokemonReader pokemonReader;

	[Export]
	BreedingReader breedingReader;

	string pokemonUUID;

	public override void _Ready()
	{
		FileManager.WriteSpeciesJson(SpeciesCreation.CreateAllSpecies());
		RandomizedWildPokemon.FillDictionary();
		ShowInfoContainer();
		DelayedReady();
	}

	async void DelayedReady()
	{
		await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
		_on_randomize_button_pressed();
	}

	void ShowInfoContainer()
	{
		InfoContainer.Show();
		SetListInactive();
		AncestryContainer.Hide();
		NamePokemonContainer.Hide();
		BreedContainer.Hide();
	}

	void _on_list_button_pressed()
	{
		if (ListContainer.Visible == true)
		{
			ShowInfoContainer();
		}
		else
		{
			InfoContainer.Hide();
			SetListActive();
			AncestryContainer.Hide();
			BreedContainer.Hide();
		}
	}

	void _on_ancestry_button_pressed()
	{
		if (AncestryContainer.Visible == true)
		{
			ShowInfoContainer();
		}
		else
		{
			InfoContainer.Hide();
			SetListInactive();
			SetAncestryActive(pokemonUUID);
			BreedContainer.Hide();
		}
	}

	void _on_yes_button_saved_pokemon()
	{
		_on_randomize_button_pressed();
		ShowInfoContainer();
	}

	void _on_save_button_pressed()
	{
		NamePokemonContainer.Show();
		yesButton.pokemonUUID = pokemonUUID;
	}

	void _on_breed_button_pressed()
	{
		if (BreedContainer.Visible == true)
		{
			ShowInfoContainer();
		}
		else
		{
			InfoContainer.Hide();
			SetListInactive();
			AncestryContainer.Hide();
			SetBreedingActive();
		}
	}

	public void SetListActive()
	{
		ListContainer.Show();
		if (ListContainer.GetChildCount() > 0)
		{
			foreach (Node child in ListContainer.GetChildren())
			{
				child.QueueFree();
			}
		}
		if (FileManager.PokemonPath == null)
		{
			GD.PrintErr("PokemonPath is null");
			return;
		}

		if (FileManager.ReadAllFilesJson<Pokemon>(FileManager.PokemonPath) == null)
		{
			GD.PrintErr("You have no pokemon");
			return;
		}

		FileManager.ReadAllFilesJson<Pokemon>(FileManager.PokemonPath).ForEach(pokemon =>
		{
			PokemonListButton pokemonButton = GD.Load<PackedScene>("res://Tools/Test Scenes/GeneticsButtons/PokemonListButton.tscn").Instantiate() as PokemonListButton;
			pokemonButton.Pokemon = pokemon;
			pokemonButton.Text = pokemon.Name;
			pokemonButton.PokemonSelected += PokemonSelected;
			ListContainer.AddChild(pokemonButton);
		});
	}

	public void SetListInactive()
	{
		if (GetChildCount() > 0)
		{
			foreach (Node child in GetChildren())
			{
				child.QueueFree();
			}
		}
		ListContainer.Hide();
	}

	public void SetAncestryActive(string pokemonUUID)
	{
		AncestryContainer.Show();
		Pokemon pokemon = FileManager.ReadPokemon(pokemonUUID);
		Pokemon[] parents = new Pokemon[2];
		Pokemon[] grandparents = new Pokemon[4];
		for (int i = 0; i < pokemon.Parents.Length; i++)
		{
			parents[0] = FileManager.ReadPokemon(pokemon.Parents[0]);
			parents[1] = FileManager.ReadPokemon(pokemon.Parents[1]);
		}
		for (int i = 0; i < pokemon.GrandParents.Length; i++)
		{
			grandparents[0] = FileManager.ReadPokemon(pokemon.GrandParents[0]);
			grandparents[1] = FileManager.ReadPokemon(pokemon.GrandParents[1]);
			grandparents[2] = FileManager.ReadPokemon(pokemon.GrandParents[2]);
			grandparents[3] = FileManager.ReadPokemon(pokemon.GrandParents[3]);
		}

		// get all children
		var containers = AncestryContainer.GetChildren();
		foreach (var container in containers)
		{
			var buttons = container.GetChildren();
			foreach (Button button in buttons)
			{
				if (button.Name == "Grandparent1" && grandparents[0] != null)
				{
					button.Text = grandparents[0].Name;
				} else if (button.Name == "Grandparent1" && grandparents[0] == null)
				{
					button.Text = "Not In Database";
				}

				if (button.Name == "Grandparent2" && grandparents[1] != null)
				{
					button.Text = grandparents[1].Name;
				} else if (button.Name == "Grandparent2" && grandparents[1] == null)
				{
					button.Text = "Not In Database";
				}

				if (button.Name == "Grandparent3" && grandparents[2] != null)
				{
					button.Text = grandparents[2].Name;
				} else if (button.Name == "Grandparent3" && grandparents[2] == null)
				{
					button.Text = "Not In Database";
				}

				if (button.Name == "Grandparent4" && grandparents[3] != null)
				{
					button.Text = grandparents[3].Name;
				} else if (button.Name == "Grandparent4" && grandparents[3] == null)
				{
					button.Text = "Not In Database";
				}

				if (button.Name == "Parent1" && parents[0] != null)
				{
					button.Text = parents[0].Name;
				} else if (button.Name == "Parent1" && parents[0] == null)
				{
					button.Text = "Not In Database";
				}

				if (button.Name == "Parent2" && parents[1] != null)
				{
					button.Text = parents[1].Name;
				} else if (button.Name == "Parent2" && parents[1] == null)
				{
					button.Text = "Not In Database";
				}
			}
			if (container.GetType() == typeof(Button))
			{
				((Button)container).Text = pokemon.Name;
			}
		}
	}

	void SetBreedingActive()
	{
		BreedContainer.Show();
		breedingReader.EnableBreeding();
	}

	void PokemonSelected(string pokemonUUID)
	{
		Pokemon pokemon = FileManager.ReadPokemon(pokemonUUID);
		pokemonReader.ReadPokemon(pokemon);
		this.pokemonUUID = pokemonUUID;
		ShowInfoContainer();
	}

	public void _on_randomize_button_pressed()
	{
		FileManager.DeleteTempFiles();
		Pokemon pokemon = RandomizedWildPokemon.CreateWildPokemon();
		FileManager.WritePokemon(FileManager.TempPath, pokemon);
		pokemonReader.ReadPokemon(pokemon);
		pokemonUUID = pokemon.uuid;
	}
}
