using Godot;
using System.Collections.Generic;

public partial class MenuScript : Node2D {
	[Export] private Button btnResume;
	[Export] private Button btnSettings;
	[Export] private Button btnCommands;
	[Export] private Button btnEsc;

	private List<Button> buttonList;
	private List<Callable> callables;
	private int indSelected;

	private readonly Color normalButtonColor = new(255, 255, 255, 1);
	private readonly Color selectedButtonColor = new(255, 0, 0, 1);

	[Signal] public delegate void OnResumeEventHandler();
	[Signal] public delegate void OnSettingsEventHandler();
	[Signal] public delegate void OnCommandsEventHandler();

    public override void _Ready()
	{
		buttonList = new()
		{
			btnResume,
			btnSettings,
			btnCommands,
			btnEsc
		};

		indSelected = 1;

		callables = new()
		{
			new Callable(this, nameof(OnPausePressed)),
			new Callable(this, nameof(OnSettingsPressed)),
			new Callable(this, nameof(OnCommandPressed)),
			new Callable(this, nameof(OnExitPressed))
		};

		OnResumeButtonHovered();
	}

	public void OnPausePressed(){
		this.EmitSignal(SignalName.OnResume);
	}

	public void OnSettingsPressed()
	{
		this.EmitSignal(SignalName.OnSettings);
	}
	
	public void OnCommandPressed(){
		this.EmitSignal(SignalName.OnCommands);
		
	}

	public void OnExitPressed(){
		this.GetTree().Quit();
	}

	public void OnResumeButtonHovered() {

		indSelected = 0;
		ColorOtherButton(indSelected, normalButtonColor);
		buttonList[indSelected].Modulate = selectedButtonColor;
	}
	public void OnSettingsButtonHovered()
	{

		indSelected = 1;
		ColorOtherButton(indSelected, normalButtonColor);
		buttonList[indSelected].Modulate = selectedButtonColor;

	}
	
	public void OnCommandButtonHovered() {

		indSelected=2;
		ColorOtherButton(indSelected, normalButtonColor);
		buttonList[indSelected].Modulate = selectedButtonColor;
	}

	public void OnEscButtonHovered() {

		indSelected = 3;		
		ColorOtherButton(indSelected, normalButtonColor);
		buttonList[indSelected].Modulate = selectedButtonColor;
	}

	public void ColorOtherButton(int i, Color color) {

		for (int ind = 0; ind < buttonList.Count; ind++) 	
			if (ind!=i)	
				buttonList[ind].Modulate = color;		
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed(Main.ACTION_NAME_SELECT_UP))
		{

			if (indSelected > 0)
			{
				buttonList[indSelected].Modulate = normalButtonColor;
				indSelected--;
				buttonList[indSelected].Modulate = selectedButtonColor;


			}
		}
		else if (@event.IsActionPressed(Main.ACTION_NAME_SELECT_DOWN))
		{

			if (indSelected < buttonList.Count - 1)
			{

				buttonList[indSelected].Modulate = normalButtonColor;
				indSelected++;
				buttonList[indSelected].Modulate = selectedButtonColor;
			}
		}
		else if (@event.IsActionReleased(Main.ACTION_NAME_SELECT))
		{

			callables[indSelected].Call();

		}
	}

}
