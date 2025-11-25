using Godot;
using System;

public partial class BigDoor : Node3D {
    private const string OUTPUT_TEXT = "*La porta è chiusa*";
    private const string ANIMATION_NAME = "openButNotOpen";
    private const string KEY_NAME = "Chiave manicomio";
    [Export] private AnimationPlayer animation;
	[Export] private DialogLabel label;
	[Export] private VideoStream demoFinalCutScene;

	[Signal] public delegate void OpenMainGateEventHandler(VideoStream video, Callable action);


	private Inventory inventory;

    public Inventory InventoryNode { set => inventory = value; } 

	public void TryToOpen()
	{
		if (inventory.Contain(KEY_NAME))
		{
			Callable callable = new(this, "TitleScreen");
			EmitSignal(SignalName.OpenMainGate, demoFinalCutScene, callable);			
		}

		else
		{
			if (animation.IsPlaying()) return;
			label.OutputText(OUTPUT_TEXT);
			animation.Play(ANIMATION_NAME);
		}


	}
	
	public void TitleScreen()
    {
        GetTree().ChangeSceneToFile("res://scenes/titoliDiCoda.tscn");
    }

}
