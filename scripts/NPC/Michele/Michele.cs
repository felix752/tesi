using Godot;
using System;
using System.Collections.Generic;

public partial class Michele : Node3D
{
	private const string ANIMATION_NAME = "Armature|mixamo_com|Layer0";
	private const int DEFAULT_DIALOG_IDX = 0;
	private const int DEFAULT_LINE_LOOP_IDX = 1;
	private const int GIVE_7_DIALOG_IDX = 2;
	private const int GIVE_7_LOOP_LINE_IDX = 3;
	private const int DOOR_DIALOG_IDX = 4;
	private const int NO_GIVE_7_DIALOG_IDX = 5;
	private const int NORMAL_7_DIALOG_IDX = 6;

	[Export] private Inventory inventory;

	public Inventory InventoryNode{ set => inventory = value; }

	[Export] private DialogLabel dialogLabel;
	[Export] private Player player;
	[Export] private Node[] dialogs;
	[Export] private Gianmarco gianmarco;
	[Export] private Candy candy;
	[Export] private Thomas gesuald;
	[Export] private Cards cards;
	private AnimationPlayer animationPlayer;

	private int currentDialogIndex;
	public bool IsDistracted { get; set; }
	private List<string> actions;
	private List<Callable> consequences;
	private Callable closeDialogMethod;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{ 
		animationPlayer = this.GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.GetAnimation(ANIMATION_NAME).LoopMode = Animation.LoopModeEnum.Linear;
		animationPlayer.Play(ANIMATION_NAME);


		currentDialogIndex = DEFAULT_DIALOG_IDX;
		this.GetNode<InteractibleObject>("StaticBody3D").Connect(InteractibleObject.SignalName.OnClick, new Callable(this, "OnStartDialog"));
		closeDialogMethod = new Callable(this, nameof(this.CloseDialog));
		IsDistracted = false;

		actions = new List<string>()
		{
			"dai il 7 di denari", "non dare il 7 di denari"
		};
		consequences = new List<Callable>()
		{
			new(this, nameof(this.Give7Action))
		};
	}


	public void OnStartDialog()
	{
		this.player.Stop();
		animationPlayer.Stop();

		if (inventory.Contain("7 di denari"))
		{
			if (currentDialogIndex == DEFAULT_LINE_LOOP_IDX)
			{
				if (consequences.Count == 1)
				{
					consequences.Add(new Callable(this, nameof(this.NoGive7Action)));
				}
				else
				{
					consequences[1] = new Callable(this, nameof(this.NoGive7Action));
				}

				dialogLabel.Clean();

				dialogLabel.AddButtons(actions.ToArray(), consequences.ToArray());
				dialogLabel.EmitSignal(DialogLabel.SignalName.StartedDialog);
				dialogLabel.Visible = true;
			}
			else
			{
				dialogLabel.StartDialog(dialogs[NORMAL_7_DIALOG_IDX].GetPath(), closeDialogMethod);
			}


			IsDistracted = true;
			return;


		}

		dialogLabel.StartDialog(dialogs[currentDialogIndex].GetPath(), closeDialogMethod);
		if (currentDialogIndex == DEFAULT_DIALOG_IDX)
		{
			currentDialogIndex = DEFAULT_LINE_LOOP_IDX;
			gesuald.SetTrigger7Unlocked();
		}

	}

	private void CloseDialog()
	{
		this.animationPlayer.Play(ANIMATION_NAME);
		gianmarco.TriggerMicheleUnlocked();
	}

	public void TryToOpenDoor()
	{

		Vector3 direction = this.player.GlobalPosition - this.GlobalPosition;
		direction.Y = 0;
		direction = direction.Normalized();
		float angleRadians = -Math.Abs(MathF.PI * 2 - Mathf.Atan2(direction.X, direction.Z));
		/*
		if (Math.Abs(angleRadians - this.GlobalRotation.Y) > Math.PI)
		{

					if (angleRadians < 0)
						angleRadians = MathF.PI * 2 + angleRadians;

					else
				angleRadians -= MathF.PI * 2;

		}*/

		this.player.Stop();

		animationPlayer.Stop();
		Skeleton3D node = this.GetNode<Skeleton3D>("Armature/Skeleton3D");
		node.SetBonePoseRotation(node.FindBone("mixamorig_Head"), new Quaternion(0, -((float)Math.PI / 2 + angleRadians) - 0.4f, 0, 1));

		player.CameraAnimation(1, new Vector3(0, angleRadians, 0), player.Position);

		dialogLabel.StartDialog(dialogs[DOOR_DIALOG_IDX].GetPath(), closeDialogMethod);
	}


	public void Give7Action()
	{
		DisplayServer.MouseSetMode(DisplayServer.MouseMode.Captured);

		dialogLabel.Clean();
		dialogLabel.StartDialog(dialogs[GIVE_7_DIALOG_IDX].GetPath(), new Callable(this, nameof(GiveCandy)));
		currentDialogIndex = GIVE_7_LOOP_LINE_IDX;
		cards.InsertSeven();
		inventory.RemoveObject("7 di denari");
	}

	public void NoGive7Action()
	{
		dialogLabel.Clean();
		dialogLabel.StartDialog(dialogs[currentDialogIndex].GetPath(), closeDialogMethod);
		currentDialogIndex = DEFAULT_LINE_LOOP_IDX;
	}

	public void NoGive7Action2()
	{
		DisplayServer.MouseSetMode(DisplayServer.MouseMode.Captured);

		dialogLabel.Clean();
		dialogLabel.StartDialog(dialogs[NO_GIVE_7_DIALOG_IDX].GetPath(), closeDialogMethod);
	}

	private void GiveCandy()
	{
		candy.ObtainObject();
		this.CloseDialog();
	}


}
