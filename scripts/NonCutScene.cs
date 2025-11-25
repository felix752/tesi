using Godot;
using System;

public partial class NonCutScene : Node3D
{
	[Export] private Node dialog;
	[Export] private DialogLabel dialogLabel;
	
	// Called when the node enters the scene tree for the first time.
	public void Start()
	{
		dialogLabel.Visible = true;
		dialogLabel.StartDialog(dialog.GetPath(), new Callable(this, "_queue_free"));
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
