using Godot;

public partial class InteractibleObject : StaticBody3D {

	[Export] public Texture2D pointObject = GD.Load<Texture2D>("res://2D/DefHand.png");
	[Export] public Texture2D objectClickedPointer = GD.Load<Texture2D>("res://2D/DefGrab.png");

	[Signal] public delegate void OnClickEventHandler();

	public override void _Ready()
	{		
		if (pointObject==null && objectClickedPointer==null)
		{
			this.pointObject = GD.Load<Texture2D>("res://2D/DefHand.png");
			this.objectClickedPointer = GD.Load<Texture2D>("res://2D/DefGrab.png");
		}
		
	}

	public virtual void ActionOnInteract()
	{
		EmitSignal(SignalName.OnClick);
	}
}