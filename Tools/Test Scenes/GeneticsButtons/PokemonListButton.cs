using Godot;
using System;

public partial class PokemonListButton : Button
{
    public Pokemon Pokemon { get; set; }

	[Signal]
	public delegate void PokemonSelectedEventHandler(string pokemonUUID);

    void _pressed(){
        EmitSignal(SignalName.PokemonSelected, Pokemon.uuid);
    }
}
