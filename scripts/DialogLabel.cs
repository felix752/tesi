using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class DialogLabel : Label
{
	private const string TEXT_ANIMATION_NAME = "new_animation";
	private const int NO_INDEX_SELECTED = -1;
	private const int ROWS_BUTTON_COUNT = 2;
	private int VISIBLE_CHAR_TRACK_IDX = 1;
	[Export] private AnimationPlayer animationPlayer;
	[Export] private int CHARACTER_PER_SECOND;
	[Export] private Player player;
	[Export] private AudioStreamPlayer audio;
	private Animation animation;
	private List<Button> buttonList;
	private Array<StringName> metaList;
	private int currentDialogInd;
	private Node dialogNode;
	private Callable onFinishedDialog;

	private int currentButtonInd;
	private readonly Color normalButtonColor = new(255, 255, 255, 1);
	private readonly Color selectedButtonColor = new(255, 0, 0, 1);

	[Signal] public delegate void FinishedDialogEventHandler();
	[Signal] public delegate void StartedDialogEventHandler();

	public override void _Ready()
	{

		buttonList = new();
		for (int i = 0; i < GetChildCount(); i++)
		{

			try
			{

				Button button = GetChild<Button>(i);
				buttonList.Add(button);
				button.Visible = false;

			}
			catch (InvalidCastException) { }
		}

		this.Visible = false;
		this.SetProcessInput(false);
		this.SetProcessUnhandledInput(false);
	}

	public void AddButtons(string[] textList, Callable[] onButtonMethodes)
	{

		if (textList.Length > buttonList.Count)
			throw new Exception("too much callables for the maximum of" + buttonList.Count + "buttons");


		for (int i = 0; i < textList.Length; i++)
		{

			foreach (Dictionary signal in buttonList[i].GetSignalConnectionList("pressed"))
			{
				buttonList[i].Disconnect("pressed", (Callable)signal["callable"]);
			}

			buttonList[i].Text = textList[i];
			buttonList[i].Connect("pressed", onButtonMethodes[i]);
			buttonList[i].Visible = true;
		}


		DisplayServer.MouseSetMode(DisplayServer.MouseMode.Visible);
		currentButtonInd = NO_INDEX_SELECTED;
		ColorOtherButton(NO_INDEX_SELECTED, normalButtonColor);
		this.SetProcessUnhandledInput(true);
	}

	public void StartDialog(NodePath nodePath)
	{
		player?.Stop();/*
		Array<Dictionary> signalConnections = GetSignalConnectionList(SignalName.FinishedDialog);

		foreach (Dictionary connection in signalConnections)
		{
			this.Disconnect(SignalName.FinishedDialog, (Callable)connection["callable"]);
		}*/

		this.dialogNode = GetNodeOrNull(nodePath);

		currentDialogInd = 0;
		metaList = dialogNode.GetMetaList();

		this.SetProcessInput(true);

		if (!metaList[0].ToString().Contains("line"))
			throw new ArgumentException("the first metadata must have line in the stringName");

		this.Visible = true;
		OutputLine((string)dialogNode.GetMeta(metaList[0]));
		this.EmitSignal(SignalName.StartedDialog);

	}

	public void StartDialog(NodePath nodePath, Callable onFinishedDialog)
	{
		this.StartDialog(nodePath);
		this.onFinishedDialog = onFinishedDialog;
		//this.Connect(SignalName.FinishedDialog, onFinishedDialogSignal);
	}

	public override void _Input(InputEvent @event)
	{

		if (@event.IsActionReleased(Main.ACTION_NAME_CONTINUE_DIALOG))
		{
			if (animationPlayer.IsPlaying())
			{
				animationPlayer.Stop();
				this.VisibleRatio = 1;
			}
			else
			{
				currentDialogInd++;

				if (currentDialogInd >= metaList.Count)
				{
					if (metaList[--currentDialogInd].ToString().Contains("choices"))
					{
						return;
					}

					Close();

					return;
				}

				if (metaList[currentDialogInd].ToString().Contains("line"))
				{
					OutputLine((string)dialogNode.GetMeta(metaList[currentDialogInd]));
				}
				else if (metaList[currentDialogInd].ToString().Contains("choices"))
				{
					if (dialogNode is Dialog dialog)
					{
						string[] strings = (string[])dialogNode.GetMeta(metaList[currentDialogInd]);
						this.AddButtons(strings.ToArray(), dialog.GetMethodes());
					}
					else
						throw new Exception();
				}
				else
					throw new ArgumentException("The metalist must contains line or choices in all name");
			}
		}
	}

	public void Close()
	{
		this.SetProcessInput(false);
		animationPlayer.Stop();
		DisplayServer.MouseSetMode(DisplayServer.MouseMode.Captured);

		this.Visible = false;
		this.Clean();
		if (player != null)
		{
			this.player.Resume();
		}

		this.EmitSignal(SignalName.FinishedDialog);

		onFinishedDialog.CallDeferred();
		onFinishedDialog = new();
		/*
		Array<Dictionary> signalConnections = GetSignalConnectionList(SignalName.FinishedDialog);

		foreach (Dictionary connection in signalConnections)
		{
			this.Disconnect(SignalName.FinishedDialog, (Callable)connection["callable"]);
		}*/
	}

	public void OutputLine(string text)
	{

		animation = animationPlayer.GetAnimation(TEXT_ANIMATION_NAME);

		this.Text = text;

		animation.Length = (float)text.Length / CHARACTER_PER_SECOND;
		animation.TrackSetKeyValue(VISIBLE_CHAR_TRACK_IDX, 0, 0);
		animation.TrackSetKeyTime(VISIBLE_CHAR_TRACK_IDX, 1, animation.Length);

		animationPlayer.Stop();
		animationPlayer.Play(TEXT_ANIMATION_NAME);
	}

	public void OutputText(string text)
	{
		this.Clean();
		this.Visible = true;
		this.Text = text;
		this.VisibleRatio = 1;
	}

	public void Clean()
	{

		this.Text = "";

		foreach (Button button in buttonList)
			button.Visible = false;

	}

	public void Button0Hovered()
	{
		currentButtonInd = 0;
		ColorOtherButton(currentButtonInd, normalButtonColor);
		buttonList[currentButtonInd].Modulate = selectedButtonColor;
	}
	public void Button1Hovered()
	{
		currentButtonInd = 1;
		ColorOtherButton(currentButtonInd, normalButtonColor);
		buttonList[currentButtonInd].Modulate = selectedButtonColor;
	}

	public void Button2Hovered()
	{
		currentButtonInd = 2;
		ColorOtherButton(currentButtonInd, normalButtonColor);
		buttonList[currentButtonInd].Modulate = selectedButtonColor;
	}

	public void Button3Hovered()
	{
		currentButtonInd = 3;
		ColorOtherButton(currentButtonInd, normalButtonColor);
		buttonList[currentButtonInd].Modulate = selectedButtonColor;
	}



	public void ColorOtherButton(int i, Color color)
	{

		for (int ind = 0; ind < buttonList.Count; ind++)
			if (ind != i)
				buttonList[ind].Modulate = color;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (currentButtonInd < 0)
		{
			if (@event.IsActionPressed(Main.ACTION_NAME_SELECT_UP) || @event.IsActionPressed(Main.ACTION_NAME_SELECT_DOWN) ||
				@event.IsActionPressed(Main.ACTION_NAME_SELECT_RIGHT) || @event.IsActionPressed(Main.ACTION_NAME_SELECT_LEFT))
			{
				currentButtonInd = 0;
				buttonList[0].Modulate = selectedButtonColor;
			}
		}
		else if (@event.IsActionPressed(Main.ACTION_NAME_SELECT_UP))
		{
			if (currentButtonInd % ROWS_BUTTON_COUNT == 1 && buttonList[currentButtonInd - 1].Visible == true)
			{
				buttonList[currentButtonInd].Modulate = normalButtonColor;
				currentButtonInd--;
				buttonList[currentButtonInd].Modulate = selectedButtonColor;
			}
		}
		else if (@event.IsActionPressed(Main.ACTION_NAME_SELECT_DOWN) && buttonList[currentButtonInd + 1].Visible == true)
		{

			if (currentButtonInd % ROWS_BUTTON_COUNT == 0)
			{
				buttonList[currentButtonInd].Modulate = normalButtonColor;
				currentButtonInd++;
				buttonList[currentButtonInd].Modulate = selectedButtonColor;
			}
		}
		else if (@event.IsActionPressed(Main.ACTION_NAME_SELECT_RIGHT))
		{

			if (buttonList[currentButtonInd + 2].Visible == true)
			{
				buttonList[currentButtonInd].Modulate = normalButtonColor;
				currentButtonInd += 2;
				buttonList[currentButtonInd].Modulate = selectedButtonColor;
			}
		}
		else if (@event.IsActionPressed(Main.ACTION_NAME_SELECT_LEFT))
		{

			if (buttonList[currentButtonInd - 2].Visible == true)
			{
				buttonList[currentButtonInd].Modulate = normalButtonColor;
				currentButtonInd -= 2;
				buttonList[currentButtonInd].Modulate = selectedButtonColor;
			}
		}
		else if (@event.IsActionReleased(Main.ACTION_NAME_SELECT))
		{
			this.SetProcessUnhandledInput(false);
			this.buttonList[currentButtonInd].EmitSignal("pressed");
		}

	}

	public void OnEffectsValueChanged(float value)
	{
		this.audio.VolumeDb = value;
	}

	public void StopAudio()
	{
		this.audio.Stop();
	}

    public bool IsChoosingOptionActive()
    {
		return buttonList[0].Visible;
    }
}
