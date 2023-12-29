using Godot;

public partial class TextEdit : Godot.TextEdit
{
	[Signal]
	public delegate void ChangedTextEventHandler(string text);

	public void _on_text_changed()
	{
		EmitSignal(SignalName.ChangedText, Text);
	}
}
