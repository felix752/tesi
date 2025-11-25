using Godot;
using System;

public partial class Furni1 : Node3D
{
	private const string ANIMATION_NAME = "openButNotOpen";
	private const string TEXT = "Non si apre";
    private const string StaticBodyPath = "StaticBody3D2";
    private const string AnimationPlayerPath = "AnimationPlayer";
    protected AnimationPlayer animationPlayer;
	[Export] private DialogLabel dialogLabel;

	public override void _Ready()
	{
		this.GetNode<InteractibleObject>(StaticBodyPath).Connect(InteractibleObject.SignalName.OnClick, new Callable(this, nameof(this.Open)));
		animationPlayer = this.GetNode<AnimationPlayer>(AnimationPlayerPath);
	}

	public virtual void Open()
	{
		if (!animationPlayer.IsPlaying())
		{
			animationPlayer.Play(ANIMATION_NAME);
			dialogLabel.OutputText(TEXT);
		}
		
	}
}
