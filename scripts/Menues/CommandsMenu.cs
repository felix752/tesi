using Godot;
using System;

public partial class CommandsMenu : Node2D
{
	private const int CONTROLLER = 0;
	private const int KEYBOARD = 1;

	private readonly Color normalButtonColor = new(255, 255, 255, 1);
	private readonly Color selectedButtonColor = new(255, 0, 0, 1);
	
	[Export] private Sprite2D controllerImage;
	[Export] private Sprite2D keyboardImage;
	[Export] private Button controllerButton;
	[Export] private Button KeyboardButton;

	private int indSelected;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.RemoveChild(keyboardImage);
		indSelected = CONTROLLER;
	}
	
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Input(InputEvent @event)
	{

		if (@event.IsActionPressed(Main.ACTION_NAME_SELECT_RIGHT))
		{
			if (indSelected == 0)
			{
				OnKeyboardSelected();
			}
		}
		else if (@event.IsActionPressed(Main.ACTION_NAME_SELECT_LEFT))
		{
			if (indSelected == 1)
			{
				OnControllerSelected();
			}
		}

	}




	public void OnControllerSelected()
	{
		if (controllerImage.GetParent()!=this)
		{
			this.RemoveChild(keyboardImage);
			this.AddChild(controllerImage);

			KeyboardButton.Disabled = false;
			controllerButton.Disabled = true;

			indSelected = CONTROLLER;
		}
	}
	
	public void OnKeyboardSelected()
	{
		if (keyboardImage.GetParent()!=this)
		{
			this.RemoveChild(controllerImage);
			this.AddChild(keyboardImage);

			controllerButton.Disabled = false;
			KeyboardButton.Disabled = true;

			indSelected = KEYBOARD;
		}        
    }
}
