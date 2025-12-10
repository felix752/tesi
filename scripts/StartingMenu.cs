using Godot;
using System;

public partial class StartingMenu : Node
{

    private const string WALK_ANIMATION_NAME = "Walk";
    private const string STARTING_ANIMATION_NAME = "logoAnimation";
    private const string DOCTOR_ANIMATION_NAME = "doctorAnimation";
    private const string MOVE_ANIMATION_NAME = "elDoctorQueCamina";
    [Export] private Label btn;
    [Export] private AnimationPlayer introAnimationPlayer;
    [Export] private AnimationPlayer walkAnimationPlayer;
    [Export] private AnimationPlayer moveAnimationPlayer;
    [Export] private Node3D doctor;
    [Export] private Sprite2D sprite;
    [Export] private DialogLabel textLabel;

    [Signal] public delegate void TriggeredEventHandler(AudioStreamPlayer music);
    [Signal] public delegate void ShortcutEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        introAnimationPlayer.Play(STARTING_ANIMATION_NAME);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Input(InputEvent @event)
    {

        if (@event.IsActionPressed(Main.ACTION_NAME_PAUSE)) {
            
            textLabel.Clean();
            textLabel.Visible = false;
            textLabel.SetProcessInput(false);
            GetViewport().SetInputAsHandled();
            this.SetProcessInput(false);
            this.EmitSignal(SignalName.Shortcut);
        }

        if (@event is InputEventMouseMotion)
        {
            return;
        }

        if (!@event.IsReleased()) return;


        else if (btn.Visible == true)
        {
            introAnimationPlayer.Play(DOCTOR_ANIMATION_NAME);
            doctor.Visible = true;
            btn.Visible = false;
            walkAnimationPlayer.Play(WALK_ANIMATION_NAME);
            moveAnimationPlayer.Play(MOVE_ANIMATION_NAME);
        }


    }

    public Node2D Get2DScene() 
    {
        return GetNode<Node2D>("2D");        
    }

    public void TurnOffCandle()
    {
        this.introAnimationPlayer.Play("candleAnimation");  
    }

    public void OnStartGame()
    {
        
        this.SetProcessInput(false);
        this.EmitSignal(SignalName.Triggered, this.GetNode<AudioStreamPlayer>("MusicAudioStreamPlayer"));
    }

}
