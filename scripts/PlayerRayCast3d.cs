using Godot;
using System;

public partial class PlayerRayCast3d : RayCast3D
{
	private InteractibleObject collider;
	public Sprite2D pointer;
	private Texture2D normalPointer;
	public Label label;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.SetProcessInput(false);
		this.normalPointer = GD.Load<Texture2D>("res://2D/Plus.png");
	}

	public override void _Process(double delta)
	{
		
		if (IsColliding() && GetCollider() is InteractibleObject interactibleObject)
		{
			collider = interactibleObject;
			if (pointer.Texture != collider.pointObject)
			{
				pointer.Texture = collider.pointObject;
				this.SetProcessInput(true);
			}

		}
		else
		{
			collider = null;
			if (pointer.Texture != normalPointer)
			{

				pointer.Texture = normalPointer;
				this.SetProcessInput(false);

				label.Visible = false;
			}
		}
	}

	public override void _Input(InputEvent @event)
	{

		if (collider != null)
		{
			if (@event.IsActionPressed(Main.ACTION_NAME_INTERACT))
			{
				this.SetProcess(false);
				pointer.Texture = collider.objectClickedPointer;

			}
			else if (@event.IsActionReleased(Main.ACTION_NAME_INTERACT))
			{

				pointer.Texture = normalPointer;
				CallDeferred("set_process", true);
				collider.CallDeferred("ActionOnInteract");
			}

		}
	}
}
