using System;
using System.Drawing;
using Godot;
public partial class Main : Node3D
{

	public const string ACTION_NAME_INTERACT = "interact";
	public const string ACTION_NAME_PAUSE = "pause";
	public const string ACTION_NAME_INVENTORY = "inventory";
	public const string ACTION_NAME_BACK = "back";
	public const string ACTION_NAME_LOOK_RIGHT = "look_x";
	public const string ACTION_NAME_LOOK_LEFT = "look_x_left";
	public const string ACTION_NAME_LOOK_UP = "look_y";
	public const string ACTION_NAME_LOOK_DOWN = "look_y_down";
	public const string ACTION_NAME_SELECT_RIGHT = "selectRight";
	public const string ACTION_NAME_SELECT_LEFT = "selectLeft";
	public const string ACTION_NAME_SELECT_UP = "selectUp";
	public const string ACTION_NAME_SELECT_DOWN = "selectDown";
	public const string ACTION_NAME_SELECT_UP_RIGHT = "selectUpRight";
	public const string ACTION_NAME_SELECT_UP_LEFT = "selectUpLeft";

	public const string ACTION_NAME_SELECT = "select";
	public const string ACTION_NAME_GO_LEFT = "left";
	public const string ACTION_NAME_GO_RIGHT = "right";
	public const string ACTION_NAME_GO_FORWARD = "forward";
	public const string ACTION_NAME_GO_BACKWARD = "backward";
	public const string ACTION_NAME_CONTINUE_DIALOG = "continueDialog";
	private const string ANIMATION_NAME_END_INTRO = "new_animation";

	[Export] private Player player;
	[Export] private Gianmarco gianmarco;
	[Export] private Node2D obtainItemAnimation;
	[Export] private DialogLabel textEdit;
	[Export] private DialogTextEdit testEdit;
	[Export] private SubViewport subViewport;
	[Export] private TextureRect textureRect;
	[Export] private StartingMenu startingMenu;
	[Export] private Sprite2D pointer;
	[Export] private AnimationPlayer closeIntroAnimationPlayer;
	[Export] private AudioStreamPlayer musicStreamPlayer;
	[Export] private AudioStreamMP3 manicomMusic;
	[Export] private Menues menues;
	[Export] private Node3D nonCutScene;
	[Export] private Node dialog;
	[Export] private Asylum asylum;
	[Export] private VideoStream firstCutScene;
	[Export] private AnimationPlayer animationPlayer;
	private VideoStreamPlayer videoStreamPlayer;
	private float effectVolume;

	private bool lockInventory;


	public override void _Ready()
	{
		DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		DisplayServer.MouseSetMode(DisplayServer.MouseMode.Captured);

		lockInventory = false;
		textEdit.Visible = false;
		testEdit.Visible = false;
		asylum.Visible = true;

		asylum.GetParent().RemoveChild(asylum);
		
		obtainItemAnimation.GetParent().RemoveChild(obtainItemAnimation);

		this.startingMenu.Get2DScene().Scale = new(0.5f, 0.5f);

		textEdit.Visible = false;
		pointer.Visible = false;


		this.textureRect.Texture = subViewport.GetTexture();
		this.SetProcessUnhandledInput(false);
		this.asylum.SetInventory(menues.InventoryNode);
		menues.SetProcessUnhandledInput(false);
	}

	public void OnMenuDeleted()
	{
		DisplayServer.MouseSetMode(DisplayServer.MouseMode.Captured);

		if (obtainItemAnimation.GetParent() == null)
		{
			this.GetTree().Paused = false;
		}

		if (textEdit.IsChoosingOptionActive())
		{
			DisplayServer.MouseSetMode(DisplayServer.MouseMode.Visible);
		}

		menues.CallDeferred("set_process_unhandled_input", true);
		pointer.Visible = true;
	}
	
	public void StartFirstCutScene()
	{
		this.textEdit.Visible = false;
		subViewport.RemoveChild(startingMenu);
		nonCutScene.Visible=true;
		textEdit.Visible = true;
		textEdit.StartDialog(dialog.GetPath(), new Callable(this, "DeleteStartingMenu"));
    }

	public void DeleteStartingMenu()
	{
		this.pointer.Visible = true;

		subViewport.RemoveChild(startingMenu);
		subViewport.AddChild(asylum);
		subViewport.RemoveChild(nonCutScene);
		animationPlayer.Play("new_animation");
		this.textEdit.Visible = false;
		this.textEdit.SetProcessInput(false);

		this.SetProcessUnhandledInput(true);
		DisplayServer.MouseSetMode(DisplayServer.MouseMode.Captured);
		menues.CallDeferred("set_process_unhandled_input", true);

	}

	public void OnStartGame(AudioStreamPlayer startingMusicstreamPlayer)
	{
		this.textEdit.Visible = true;
		this.textEdit.OutputLine("Capisco, non che avessi altra scelta...");

		Animation animation = closeIntroAnimationPlayer.GetAnimation(ANIMATION_NAME_END_INTRO);
		int newTrack = animation.AddTrack(Animation.TrackType.Value);

		animation.TrackSetPath(newTrack, startingMusicstreamPlayer.GetPath() + ":volume_db");
		animation.TrackInsertKey(newTrack, 0, startingMusicstreamPlayer.VolumeDb);
		animation.TrackInsertKey(newTrack, animation.Length, -40);

		closeIntroAnimationPlayer.Play(ANIMATION_NAME_END_INTRO);
	}

	public void OnCloseIntroAnimationFinished(StringName name)
	{
		manicomMusic.Loop = true;
		musicStreamPlayer.Stream = manicomMusic;


		musicStreamPlayer.Play();
		gianmarco.PlayWalkingSound();
	}

	public void LockInventory()
	{
		lockInventory = true;
		menues.LockInventory = true;
	}

	public void UnlockInventory()
	{
		lockInventory = false;
		menues.LockInventory = false;
	}

	public void SetPointerVisibility(bool visibility)
	{
		pointer.Visible = visibility;
	}

	public void StartCutScene(VideoStream video, Callable methodAfterScene)
	{
		videoStreamPlayer = new();
		this.AddChild(videoStreamPlayer);
		videoStreamPlayer.Stream = video;
		videoStreamPlayer.VolumeDb = effectVolume;
		musicStreamPlayer.Stop();
		LockInventory();
		videoStreamPlayer.Play();

		videoStreamPlayer.Connect(VideoStreamPlayer.SignalName.Finished, methodAfterScene);
		videoStreamPlayer.Finished += CloseCutScene;

		pointer.Visible = false;
	}

	public void CloseCutScene()
	{
		videoStreamPlayer.QueueFree();
		pointer.Visible = true;
		musicStreamPlayer.Play();
	}
	
	public void OnEffectsValueChanged(float value)
	{
		if (value == 0)
			value = -135;

		effectVolume = value;

		asylum.ChangeEffectVolume(value);
		player.OnEffectsValueChanged(value/3);
		textEdit.OnEffectsValueChanged(value);
    }

	public void LockMenues()
    {
        this.menues.SetProcessInput(false);
        this.menues.SetProcessUnhandledInput(false);
    }

	public void UnlockMenues()
    {
        this.menues.SetProcessInput(true);
        this.menues.SetProcessUnhandledInput(true);
    }


}
