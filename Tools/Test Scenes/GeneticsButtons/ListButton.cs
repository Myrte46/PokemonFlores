using Godot;
using System;

public partial class ListButton : Button
{
	bool IsListVisible = false;

	void _pressed(){
		if (IsListVisible){
			Text = "Show List";
			IsListVisible = false;
		}
		else{
			Text = "Hide List";
			IsListVisible = true;
		}
	}
}
