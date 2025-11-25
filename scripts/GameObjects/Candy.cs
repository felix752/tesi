using Godot;
using System;

public partial class Candy : GameObject, IConsumable
{
	[Export] private DialogLabel label;
	[Export] private Player player;

	[Signal] public delegate void closeInventoryEventHandler();

	public override void _Ready()
	{
		base._Ready();
		this.SetProcessUnhandledInput(false);
	}


	public void OnItemConsumed()
	{
		player.StopInteractionInput();
		label.OutputText("molto buona questa caramella");
		label.VisibilityChanged += SetLabelVisible;
		inventory.Remove(); 
		EmitSignal(SignalName.closeInventory);
		SetProcessUnhandledInput(true);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionReleased(Main.ACTION_NAME_INTERACT))
		{
			label.VisibilityChanged -= SetLabelVisible;
			label.Visible = false;
			SetProcessUnhandledInput(false);
			GetViewport().SetInputAsHandled();
			player.ResumeInteractionInput();
		}
	}


	public void SetLabelVisible()
	{
		label.Visible = true;
		label.StopAudio();
	}


}
