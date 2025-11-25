using Godot;

public abstract partial class DoorBussable : Door
{
	[Export] protected DialogLabel dialogLabel;
	[Export] protected Player player;
	private InteractibleObject interactibleObject;

	protected const string NOT_OPEN_DOOR_ANIMATION = "openButNotOpen2";
	protected const string DOOR_CLOSED_TEXT = "*la porta è chiusa a chiave*";

	public bool hasPlayerInteract;

	[Signal] public delegate void KnockEventHandler();
	[Signal] public delegate void AwayEventHandler();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		hasPlayerInteract = false;
		interactibleObject = GetNode<InteractibleObject>("doorAxis/doorBody");
	}

	public override void DoorClicked()
	{
		if (!hasPlayerInteract)
		{
			FirstInteraction();
		}
		else
		{
			Talk();
		}

	}

	private void FirstInteraction()
	{
		this.EmitSignal(SignalName.Knock);
		player.Stop();
		dialogLabel.OutputText(DOOR_CLOSED_TEXT);
		this.animation.Play(NOT_OPEN_DOOR_ANIMATION);

		string[] buttonNames = { "prova a bussare", "vai via" };

		Callable[] callables = { new (this, nameof(this.Talk)), new (this, nameof(this.GoAway))};

		dialogLabel.AddButtons(buttonNames, callables);
	}

	private void GoAway()
	{
		this.EmitSignal(SignalName.Away);
		DisplayServer.MouseSetMode(DisplayServer.MouseMode.Captured);
		player.Resume();
		dialogLabel.Visible = false;
		dialogLabel.Clean();
	}


	protected virtual void Talk()
	{
		hasPlayerInteract = true;
		dialogLabel.Clean();
		interactibleObject.pointObject = GD.Load<Texture2D>("res://2D/Bussbuss.png");
		//interactibleObject.objectClickedPointer = GD.Load<Texture2D>("res://2D/DefGrab.png");
}

	


}
