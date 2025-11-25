using Godot;
using System;
using System.Collections.Generic;

public partial class Gianmarco : Node3D
{

	private const string WALK_ANIMATION_NAME = "Armature|mixamo_com|Layer0";
	private const string ROTATION_ANIMATION_NAME = "rotation";
	private const string WALK_ANIMATION_PATH = "AnimationPlayer";
	private const string AUDIO_STREAM_PLAYER_PATH = "WalkAudioStreamPlayer3D";

	private const int INDEX_DEFAULT_DIALOG = 0;
	private const int INDEX_DEFAULT_DIALOG_LOOP = 1;
	private const int INDEX_DIALOG_GESUALDO = 2;
	private const int INDEX_DIALOG_MICHELE = 3;

	[Export] private DialogLabel dialogLabel;
	[Export] private Player player;
	[Export] private Node[] dialogNodes;
	private AnimationPlayer walkAnimationPlayer;
	[Export] private AnimationPlayer elPersonajoQueSeGiraAnimation;
	[Export] private Node3D model;
	[Export] private PathFollow3D pathFollow;
	private int trackKey;
	private Animation animation;
	private int current_string_index;
	private AudioStreamPlayer3D audio;
	private bool isFirstDialogConsumed;
	private bool isDialogActive;
	private bool isPlayerInFront;

	private List<string> questions;
	private List<Callable> anwers;

	private Vector3 modelRotation;

	 

	public override void _Ready()
	{
		pathFollow.ProgressRatio = 1;
		isFirstDialogConsumed = false;
		isDialogActive = false;
		isPlayerInFront = false;
		questions = new();
		anwers = new();

		animation = elPersonajoQueSeGiraAnimation.GetAnimation(ROTATION_ANIMATION_NAME);
		animation.Length = 0.64f;
		trackKey = -1;

		walkAnimationPlayer = model.GetNode<AnimationPlayer>(WALK_ANIMATION_PATH);
		walkAnimationPlayer.Play(WALK_ANIMATION_NAME);

		model.GetNode<InteractibleObject>("StaticBody3D").Connect(InteractibleObject.SignalName.OnClick, new Callable(this, "OnStartDialog"));

		audio = model.GetNode<AudioStreamPlayer3D>(AUDIO_STREAM_PLAYER_PATH);
	}

	public override void _Process(double delta)
	{
		pathFollow.ProgressRatio -= (float) (delta*0.06);
	}


	public void OnStartDialog()
	{
		if (!isDialogActive )
		{
			isDialogActive = true;
			this.player.Stop();

			walkAnimationPlayer.Pause();
			//elPersonajoQueCaminaAnimation.CallDeferred("pause");
			audio.CallDeferred("stop");


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

			if (trackKey!=-1)
				animation.RemoveTrack(trackKey);

			trackKey = animation.AddTrack(Animation.TrackType.Value);
			animation.TrackSetPath(trackKey, model.GetPath().ToString() + ":global_rotation");
			animation.TrackInsertKey(trackKey, 0, model.GlobalRotation);
			animation.TrackInsertKey(trackKey, animation.Length, new Vector3(0, angleRadians, 0));
			elPersonajoQueSeGiraAnimation.Play(ROTATION_ANIMATION_NAME);

			modelRotation = model.GlobalRotation;

			/*

				Tween tween;
				tween = GetTree().CreateTween();


				tween.CallDeferred("tween_property", model, "global_rotation", new Vector3(0, angleRadians, 0), 4f);*/
			SetProcess(false);

			if (!isFirstDialogConsumed)
			{
				dialogLabel.StartDialog(dialogNodes[INDEX_DEFAULT_DIALOG].GetPath());
			}
			else
			{
				if (questions.Count == 0)
				{
					dialogLabel.StartDialog(dialogNodes[INDEX_DEFAULT_DIALOG_LOOP].GetPath(), new Callable(this, nameof(OnDialogFinished)));
				}
				else
				{
					dialogLabel.Clean();

					dialogLabel.AddButtons(questions.ToArray(), anwers.ToArray());
					dialogLabel.EmitSignal(DialogLabel.SignalName.StartedDialog);
					dialogLabel.Visible = true;
				}
			}

		}
	}

	public void OnDialogFinished()
	{
		modelRotation.X = 0;
		modelRotation.Z = 0;

		animation.RemoveTrack(trackKey);

		trackKey = animation.AddTrack(Animation.TrackType.Value);
		animation.TrackSetPath(trackKey, model.GetPath().ToString() + ":global_rotation");
		animation.TrackInsertKey(trackKey, 0, model.GlobalRotation);
		animation.TrackInsertKey(trackKey, animation.Length, modelRotation);
		elPersonajoQueSeGiraAnimation.Play(ROTATION_ANIMATION_NAME);

/*
		Tween tween;

		tween = GetTree().CreateTween();
		tween.TweenProperty(model, "global_rotation", new Vector3(0, modelRotation.Y, 0), 4f);*/

		walkAnimationPlayer.Play(); 

		elPersonajoQueSeGiraAnimation.Connect(AnimationPlayer.SignalName.AnimationFinished, new Callable(this, nameof(this.OnAnimationFinished)));


		this.isFirstDialogConsumed = true;
		return;
	}


	public void TriggerMicheleUnlocked()
	{
		const string MICHELE_QUESTION = "Michele?";
		if (!questions.Contains(MICHELE_QUESTION))
		{
			questions.Add(MICHELE_QUESTION);
			anwers.Add(new Callable(this, nameof(this.AnswerMichele)));
		}
	}

	public void TriggerGesualdoUnlocked()
	{
		const string GESUALDO_QUESTION = "Quella porta?";
		if (!questions.Contains(GESUALDO_QUESTION))
		{
			questions.Add(GESUALDO_QUESTION);
			anwers.Add(new Callable(this, nameof(this.AnswerGesualdo)));
		}
	}

	private void AnswerMichele()
	{
		dialogLabel.Clean();
		dialogLabel.StartDialog(dialogNodes[INDEX_DIALOG_MICHELE].GetPath(), new Callable(this, "OnDialogFinished"));
	}

	private void AnswerGesualdo()
	{
		dialogLabel.Clean();
		dialogLabel.StartDialog(dialogNodes[INDEX_DIALOG_GESUALDO].GetPath(), new Callable(this, "OnDialogFinished"));
	}

	public void PlayWalkingSound()
	{
		this.audio.Play();
	}

	public void OnEffectsValueChanged(float value)
	{
		this.audio.VolumeDb = value;
	}

	public void OnBodyEntered(Node3D body)
	{
		isPlayerInFront = true;
		SetProcess(false);

		
	}

	public void OnBodyExited(Node3D body)
	{
		if (body==player)
		{
			isPlayerInFront = false; 
			SetProcess(true);
		}
	}

	public void OnAnimationFinished(StringName animationName)
	{
		elPersonajoQueSeGiraAnimation.Disconnect("animation_finished", new Callable(this, nameof(this.OnAnimationFinished)));

		dialogLabel.Visible = false;
		isFirstDialogConsumed = true;
		isDialogActive = false;
		walkAnimationPlayer.Play(WALK_ANIMATION_NAME);
		audio.Play();

		if (!isPlayerInFront)
		{
			SetProcess(true);
		}

	}

}