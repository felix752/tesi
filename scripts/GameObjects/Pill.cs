using Godot;

public partial class Pill : GameObject, IConsumable
{

	[Export] AnimationPlayer animationPlayer;
	[Signal] public delegate void closeInventoryEventHandler();

	public void OnItemConsumed()
	{
		EmitSignal(SignalName.closeInventory);
		animationPlayer.Play("new_animation");
		inventory.Remove(); 
	}

}
