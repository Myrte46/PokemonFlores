using Godot;
using System;

public partial class AncestryButton : Button
{
	bool IsAncestryVisible = false;

	void _pressed(){
		if (IsAncestryVisible){
			Text = "Show Ancestry";
			IsAncestryVisible = false;
		}
		else{
			Text = "Hide Ancestry";
			IsAncestryVisible = true;
		}
	}
}
