using Godot;
using System;

public partial class NamePokemon : Panel
{
	public override void _Ready()
	{
		Hide();
	}

	public void _on_save_button_pressed()
	{
		GetNode<TextEdit>("GridContainer/TextEdit").Text = "";
		GetNode<Label>("GridContainer/Label").Text = "What would you like to name your " + FileManager.ReadPokemon(GetNode<YesButton>("GridContainer/YesButton").pokemonUUID).Species.Name + "?";
		Show();
	}

	public void _on_yes_button_saved_pokemon()
	{
		Hide();
	}
	public void _on_no_button_pressed()
	{
		Hide();
	}

	void _on_breeding_reader_breeding_complete(string uuid)
	{
		GetNode<TextEdit>("GridContainer/TextEdit").Text = "";
		GetNode<Label>("GridContainer/Label").Text = "Your " + FileManager.ReadPokemon(uuid).Species.Name + " is ready to be saved!";
		Show();
	}
}
