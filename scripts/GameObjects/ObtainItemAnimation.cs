using Godot;
using System;

public partial class ObtainItemAnimation : Node2D
{
	[Export] private AnimationPlayer obtainItemAnimationPlayer;
	[Export] private GameObjectCamera gameObjectCamera;
	[Export] private Label label;
	[Signal] public delegate void AnimationStartedEventHandler();
	[Signal] public delegate void AnimationFinishedEventHandler();

	public void StartAnimation(string text, GameObject gameObject)
	{
		EmitSignal(SignalName.AnimationStarted);
		label.Text = text;
		gameObjectCamera.ChangeObject(gameObject);
		obtainItemAnimationPlayer.Play("new_animation");
	}

	public void Reset()
	{
		label.Text = null;
		EmitSignal(SignalName.AnimationFinished);
		obtainItemAnimationPlayer.Play("RESET");
	}

	public bool IsPlaying()
	{
		return obtainItemAnimationPlayer.IsPlaying();
	}
	
}
