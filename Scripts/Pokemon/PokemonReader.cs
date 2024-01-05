using System.Collections.Generic;
using Godot;

public partial class PokemonReader : Node
{
	[Export]
	Label PokemonName;
	[Export]
	Label Species;
	[Export]
	Label Type1;
	[Export]
	Label Type2;
	[Export]
	Label Ability;
	[Export]
	RichTextLabel Offensive;
	[Export]
	RichTextLabel Defensive;
	[Export]
	RichTextLabel Status;
	[Export]
	RichTextLabel BloodTypes;
	[Export]
	RichTextLabel Allele;

	public void ReadPokemon(Pokemon pokemon)
	{
		GD.Print(PokemonName);
		PokemonName.Text = pokemon.uuid;
		Species.Text = pokemon.Species.Name;
		Type1.Text = pokemon.Species.Type1.ToString();
		Type2.Text = pokemon.Species.Type2.ToString();
		Ability.Text = pokemon.Ability.ToString();
		Offensive.Text = ReadTypeLists("[b]Offensive:[/b]\n", pokemon.Offensive);
		Defensive.Text = ReadTypeLists("[b]Defensive:[/b]\n", pokemon.Defensive);
		Status.Text = ReadStatusLists("[b]Status Suseptability:[/b]\n", pokemon.Status);
		BloodTypes.Text = ReadBlood(pokemon.MajorBlood, pokemon.MinorBlood);
		Allele.Text = ReadAllele(pokemon.ApexAllele, pokemon.ShinyAllele);
	}

	public string ReadTypeLists(string StartingString, int[] List)
	{
		string[] Result = new string[List.Length];
		string Return = new string(StartingString);

		for (int i = 0; i < List.Length; i++)
		{
			Result[i] = ((Lists.Type)i).ToString() + ": " + List[i] + "%\n";

			Return += Result[i];
		}

		return Return;
	}

	public string ReadStatusLists(string StartingString, int[] List)
	{
		string[] Result = new string[List.Length];
		string Return = new string(StartingString);

		for (int i = 0; i < List.Length; i++)
		{
			Result[i] = ((Lists.Status)i).ToString() + ": " + List[i] + "%\n";

			Return += Result[i];
		}

		return Return;
	}

	string ReadBlood(List<Lists.Type> MajorBlood, List<Lists.Type> MinorBlood)
	{
		string Return = new string("[b]Major Blood:\n[/b]");

		for (int i = 0; i < MajorBlood.Count; i++)
		{

			Return += MajorBlood[i] + "\n";
		}

		if (MinorBlood.Count != 0)
		{
			Return += "\n[b]Minor Blood:[/b]\n";
		}

		for (int i = 0; i < MinorBlood.Count; i++)
		{

			Return += MinorBlood[i] + "\n";
		}

		return Return;
	}

	public string ReadAllele(int[] ApexAllele, int[] ShinyAllele)
	{
		string Return = "";
		//Return += FormAllele[0];
		//Return += ": ";
		//for (int i = 0; i < FormAllele.Count; i++)
		//{
		//    Return += FormAllele[i];
		//}

		Return += "\nApex";
		if (ApexAllele[0] == 5) Return += "*";
		Return += ": ";
		foreach (int Allele in ApexAllele)
		{
			Return += Allele;
		}

		Return += "\nShiny";
		if (ShinyAllele[0] == 5) Return += "*";
		Return += ": ";
		foreach (int Allele in ShinyAllele)
		{
			Return += Allele;
		}

		return Return;
	}
}
