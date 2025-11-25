using Godot;
using System;

public partial class Menues : Node2D
{
	[Export] private Node2D pause;
	[Export] private SettingsScript settings;
	[Export] private Node2D commands;
	[Export] private Inventory inventory;
	public Inventory InventoryNode { get => inventory; } 
	
	private bool lockInventory;

    public bool LockInventory { set => lockInventory = value; }

	[Signal] public delegate void OnAllMenuesDeletedEventHandler();

	[Export] private AudioStreamPlayer musicStreamPlayer;
	[Export] private WorldEnvironment worldEnvironment;
	[Signal] public delegate void EffectsValueChangedEventHandler(float value);
	[Signal] public delegate void InventoryOpenedEventHandler();


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
		this.RemoveChild(pause);
		this.RemoveChild(settings);
		this.RemoveChild(inventory);
		this.RemoveChild(commands);

		settings.MusicStreamPlayer = musicStreamPlayer;
		settings.WorldEnvironment = worldEnvironment;
    }

	public void Resume()
    {
		this.RemoveChild(this.pause);
		this.EmitSignal(SignalName.OnAllMenuesDeleted);
    }
	public void AddSettings()
    {
		this.RemoveChild(pause);
		this.AddChild(settings);
    }

	public void AddCommands()
    {
		this.RemoveChild(pause);
		this.AddChild(commands);
    }


	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed(Main.ACTION_NAME_PAUSE) && this.inventory.GetParent() != this)
		{
			if (this.settings.GetParent() == this)
			{
				GoBackFromSettings();
			}
			//se il menù è disattivo attivalo
			else if (this.commands.GetParent() == this)
			{
				this.RemoveChild(this.commands);
				this.AddChild(pause);
			}
			else if (this.pause.GetParent() != this)
			{
				this.AddChild(this.pause);
				this.GetTree().Paused = true;
				DisplayServer.MouseSetMode(DisplayServer.MouseMode.Visible);
			}
			else
			{ //altrimenti disattivalo
				this.RemoveChild(this.pause);
				this.EmitSignal(SignalName.OnAllMenuesDeleted);
			}
		}
		else if (@event.IsActionPressed(Main.ACTION_NAME_INVENTORY))
		{
			if (this.inventory.GetParent() == this)
			{
				this.RemoveChild(this.inventory);
				this.EmitSignal(SignalName.OnAllMenuesDeleted);

			}
			else if (!lockInventory)
			{
				this.EmitSignal(SignalName.InventoryOpened);

				this.inventory.UploadConsumables();
				this.AddChild(this.inventory);
				this.GetTree().Paused = true;

				DisplayServer.MouseSetMode(DisplayServer.MouseMode.Visible);
			}
		}
		else if (@event.IsActionPressed(Main.ACTION_NAME_BACK) && this.inventory.GetParent() == this)
		{
			this.RemoveChild(this.inventory);
			this.EmitSignal(SignalName.OnAllMenuesDeleted);
		}
	}

	public void OnEffectsValueChanged(float value)//todo dacci un'occhiata
	{
		this.EmitSignal(SignalName.EffectsValueChanged, value);
	}

	public void GoBackFromSettings()
	{
		this.RemoveChild(settings);
		this.AddChild(pause);
	}
	
	





}
