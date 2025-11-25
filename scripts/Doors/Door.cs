using Godot;
using System;

public partial class Door : Node3D
{
	protected const string OPEN_ANIMATION1 = "open1";
	protected const string CLOSE_ANIMATION1 = "close1";
	protected const string OPEN_ANIMATION2 = "open2";
	protected const string CLOSE_ANIMATION2 = "close2";
	protected const string OPEN_AUDIO_PATH = "res://sounds/door_open.mp3";
	protected const string CLOSE_AUDIO_PATH = "res://sounds/door_close.mp3";
	protected const string ANIMATION_PATH = "doorAxis/AnimationPlayer";
	protected const string AUDIO_PATH = "AudioStreamPlayer";

	[Export] protected AnimationPlayer animation;
	[Export] protected AudioStreamPlayer doorAudio;
	[Export] private Player player;
	[Export] private Node3D doorAxis;
	[Export] private Node3D doorName;

	private AudioStream openAudio;
	private AudioStream closeAudio;
	private bool isDoorOpen;

	protected string currentOpenAnimation;
	private string currentCloseAnimation;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		isDoorOpen = false;

		openAudio = GD.Load<AudioStream>(OPEN_AUDIO_PATH);
		closeAudio = GD.Load<AudioStream>(CLOSE_AUDIO_PATH);

		currentOpenAnimation = OPEN_ANIMATION1;
		if (doorName!=null)
		{
			doorName.GetParent().RemoveChild(doorName);
			doorAxis.AddChild(doorName);
			doorName.Position = new Vector3(0.75f, 0.75f, -0.05f);
			doorName.RotationDegrees = new Vector3(0, -90, 90);
		}
		
	}
	public void Inizialize(Player player)
	{
		this.player = player;
		this.animation = GetNode<AnimationPlayer>(ANIMATION_PATH);
		this.doorAudio = GetNode<AudioStreamPlayer>(AUDIO_PATH);
	}


	public virtual void DoorClicked()
	{
		if (animation.IsPlaying()) return;

		if (!isDoorOpen)
		{

			doorAudio.Stream = openAudio;
			doorAudio.Stream = openAudio;
			doorAudio.Play();
			//animation.Play(currentOpenAnimation);	

			animation.CallDeferred("play", currentOpenAnimation);

			currentCloseAnimation = CLOSE_ANIMATION1;
			if (currentOpenAnimation.Equals(OPEN_ANIMATION2))
				currentCloseAnimation = CLOSE_ANIMATION2;

		}
		else
		{

			doorAudio.Stream = closeAudio;
			doorAudio.Play();
			//animation.Play(currentCloseAnimation);

			animation.CallDeferred("play", currentCloseAnimation);
		}

		isDoorOpen = !isDoorOpen;
	}

	public void OnEnterArea1(Node3D body)
	{
		if (body == player)
		{

			this.SetProcess(true);
			currentOpenAnimation = OPEN_ANIMATION1;
		}
	}

	public void OnEnterArea2(Node3D body)
	{
		if (body == player)
		{

			this.SetProcess(true);
			currentOpenAnimation = OPEN_ANIMATION2;
		}
	}
	
	public void OnEffectsValueChanged(float value)
	{
        this.doorAudio.VolumeDb = value;
	}

}
