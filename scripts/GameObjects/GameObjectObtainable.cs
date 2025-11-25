using Godot;
using System;

public partial class GameObjectObtainable : GameObject
{
	[Export] private ObtainItemAnimation obtainItemAnimation;

	public override void _Ready()
	{
		base._Ready();
		this.SetProcessInput(false);
		this.ProcessMode = ProcessModeEnum.Always;
	}


	public void ObtainObject()
	{
		AddChild(obtainItemAnimation);
		obtainItemAnimation.StartAnimation("Hai ottenuto " + "\"" + this.ObjectName + "\"", this);

		this.CallDeferred("set_process_input",true);
		this.GetTree().Paused = true;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionReleased(Main.ACTION_NAME_INTERACT) && !obtainItemAnimation.IsPlaying())
		{
			this.SetProcessInput(false);
			obtainItemAnimation.Reset();
			obtainItemAnimation.GetParent().RemoveChild(obtainItemAnimation);
			inventory.AddGameObject(this);
			this.GetTree().SetDeferred("paused", false);
		}
		

    }

}
