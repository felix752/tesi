using Godot;
using System;

public partial class Felice : Node3D
{
	private const string ROTATION_ANIMATION_NAME = "rotation";

	[Export] private Node3D model;
	[Export] private AnimationPlayer elPersonajoQueSeGiraAnimation;
	[Export] private Player player;
	[Export] private DialogLabel dialogLabel;
	[Export] private Node firstDialog;
	[Export] private Node loopDialog;
	
	private bool firstTalk;
	private int trackKey;
	private Animation animation;
	[Export] private GameObjectObtainable key;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//model.GetNode<InteractibleObject>("StaticBody3D").Connect(InteractibleObject.SignalName.OnClick, new Callable(this, "OnStartDialog"));

		animation = elPersonajoQueSeGiraAnimation.GetAnimation(ROTATION_ANIMATION_NAME);
		trackKey = animation.AddTrack(Animation.TrackType.Value);
		firstTalk = true;
	}

	public void OnStartDialog()
	{
		if (elPersonajoQueSeGiraAnimation.IsPlaying())
		{
			return;
		}


		Vector3 direction = this.player.GlobalPosition - model.GlobalPosition;
		direction.Y = 0;
		direction = direction.Normalized();
		float angleRadians = Mathf.Atan2(direction.X, direction.Z);

		if (Math.Abs(angleRadians - model.GlobalRotation.Y) > Math.PI)
		{

			if (angleRadians < 0)
				angleRadians = MathF.PI * 2 + angleRadians;

			else
				angleRadians -= MathF.PI * 2;

		}

		animation.TrackSetPath(trackKey, model.GetPath().ToString() + ":global_rotation");
		animation.TrackInsertKey(trackKey, 0, model.GlobalRotation);
		animation.TrackInsertKey(trackKey, animation.Length, new Vector3(model.GlobalRotation.X, angleRadians, model.GlobalRotation.Z));
		elPersonajoQueSeGiraAnimation.Play(ROTATION_ANIMATION_NAME);


		NodePath path = firstDialog.GetPath();

		if (!firstTalk)
		{
			path = loopDialog.GetPath();
		}
		
		dialogLabel.StartDialog(path, new Callable(this, nameof(this.OnDialogFinished)));
	}

	public void OnDialogFinished()
	{
		if (firstTalk)
		{
			key.ObtainObject();
		}

		firstTalk = false;
		elPersonajoQueSeGiraAnimation.PlayBackwards();
		dialogLabel.Visible = false;
		return;
	}
}
