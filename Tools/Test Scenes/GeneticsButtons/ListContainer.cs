using Godot;
using System;

public partial class ListContainer : GridContainer
{

	void OnActive(){
		Show();
		if (GetChildCount() > 0){
			foreach (Node child in GetChildren()){
				child.QueueFree();
			}
		}

		if (FileManager.PokemonPath == null){
			GD.PrintErr("PokemonPath is null");
			return;
		}

		if (FileManager.ReadAllFilesJson<Pokemon>(FileManager.PokemonPath) == null){
			GD.PrintErr("You have no pokemon");
			return;
		}

		FileManager.ReadAllFilesJson<Pokemon>(FileManager.PokemonPath).ForEach(pokemon => {
			PokemonListButton pokemonButton = GD.Load<PackedScene>("res://Tools/Test Scenes/GeneticsButtons/PokemonListButton.tscn").Instantiate() as PokemonListButton;
			GD.Print(pokemonButton);
			pokemonButton.Text = pokemon.Name;
			AddChild(pokemonButton);
		});
	}

	void _on_list_button_pressed(){
		OnActive();
	}
}
