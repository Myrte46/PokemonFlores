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
}
