using Godot;
using System;

public partial class ReadableObject : Node3D
{
	[Export] private DialogLabel dialogLabel;
	[Export] private string outputText;
	
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void Read()
	{
		dialogLabel.OutputText(outputText);
	}
}
