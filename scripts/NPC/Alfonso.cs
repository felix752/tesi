using Godot;
using System;

public partial class Alfonso : Node3D
{
	private const string ANIMATION_NAME = "Armature|mixamo_com|Layer0";
	private const string ANIMATION_PLAYER_PATH = "AnimationPlayer";
	private AnimationPlayer animationPlayer;
	private Animation animation;
	[Export] DialogLabel dialogLabel;
	[Export] Node dialog;
	private RandomNumberGenerator random;

	public override void _Ready()
	{
		random = new();

		animationPlayer = GetNode<AnimationPlayer>(ANIMATION_PLAYER_PATH);
		animation = animationPlayer.GetAnimation(ANIMATION_NAME);
		animation.LoopMode = Animation.LoopModeEnum.Linear;
		animationPlayer.Play(ANIMATION_NAME);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		float value = random.Randf(); // Valore tra 0 e 1

		animationPlayer.SpeedScale = Mathf.Abs(value - 0.5f) * 6.0f; // Sposta verso i margini (0 o 2)
	}

	public void Talk()
	{
		animationPlayer.Pause();
		SetProcess(false);
		dialogLabel.StartDialog(dialog.GetPath(), new Callable(this, nameof(OnDialogFinished)));
	}


	private void OnDialogFinished()
	{
		SetProcess(true);
		animationPlayer.Play(ANIMATION_NAME);
	}


}
