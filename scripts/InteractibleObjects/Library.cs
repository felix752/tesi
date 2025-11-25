using Godot;
using System;

public partial class Library : Node3D {

	[Export] public StaticBody3D collision;
	[Export] private Sprite2D pointer;
	[Export] private Texture2D pointLibrary;
	[Export] private Texture2D pointDoor;
	[Export] private Texture2D normalPointer;
	[Export] private Player player;
	[Export] private Shelve shelve;
	public bool isPuzzleActived; 
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {

		this.SetProcessInput(false);
		isPuzzleActived=false;
	}

	public void Interact()
	{
		SetProcessInput(true);
		if (!isPuzzleActived)
		{
			isPuzzleActived = true;

			this.collision.CollisionLayer=4;

			pointer.Texture = pointDoor;
			shelve.StartPuzzle();
		}
		
	}
	public override void _Input(InputEvent @event)
	{

	if (@event.IsActionPressed(Main.ACTION_NAME_BACK) && isPuzzleActived
					  && !shelve.IsAnimationPlaying() && !player.IsAnimationPlaying())
		{

			GetViewport().SetInputAsHandled();

			isPuzzleActived = false;

			shelve.EscFromPuzzle();

			this.collision.CollisionLayer=1;

			this.SetProcessInput(false);
			pointer.Texture = pointLibrary;
		}

	}

}
