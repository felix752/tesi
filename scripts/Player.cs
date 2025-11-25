using System;
using Godot;

public partial class Player : CharacterBody3D
{
    private const string ANIMATION_NAME = "newAnimation";
    public const int NO_ROTATION_LIMIT = -1;

    [Export] private Node3D head;
    [Export] private PlayerRayCast3d rayCast3D;
    [Export] public AudioStreamPlayer walkAudio;
    [Export] private AnimationPlayer animationPlayer;
    [Export] private Sprite2D pointer;
    [Export] private Label label;

    private Animation animation;

    private float mouseSensibility = 0.4f;
    private float JoypadSensibility = 1.6f;
    private Vector2 previousMousePosition;

    private const float Speed = 5;
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    private RandomNumberGenerator random;

    private bool lockCameraHeight;
    private int rotationLimit;
    public override void _Ready()
    {
        this.lockCameraHeight = false;
        previousMousePosition = this.GetViewport().GetMousePosition();
        rotationLimit = NO_ROTATION_LIMIT;
        random = new();
        rayCast3D.pointer = pointer;
        rayCast3D.label = label;

        animation = animationPlayer.GetAnimation(ANIMATION_NAME);

        this.SetProcessUnhandledInput(false);
    }

    public override void _Process(double delta)
    {

        float lookRight = Input.GetActionStrength(Main.ACTION_NAME_LOOK_RIGHT);
        float lookLeft = Input.GetActionStrength(Main.ACTION_NAME_LOOK_LEFT);

        float lookUp = Input.GetActionStrength(Main.ACTION_NAME_LOOK_UP);
        float lookDown = Input.GetActionStrength(Main.ACTION_NAME_LOOK_DOWN);

        Vector2 lookVector = new(lookRight - lookLeft, lookUp - lookDown);

        if (lockCameraHeight)
        {
            lookVector.Y = 0;
        }

        if (lookVector.Length() > 1)
            lookVector = lookVector.Normalized();

        Vector3 rotation = head.RotationDegrees;
        rotation.Y -= lookVector.X * JoypadSensibility;

        if (this.rotationLimit != NO_ROTATION_LIMIT)
            rotation.Y = Mathf.Clamp(rotation.Y, 180 - rotationLimit, 180 + rotationLimit);

        if (rotation.Y > 360)
        {
            rotation.Y -= 360;
        }
        if (rotation.Y < -360)
        {
            rotation.Y += 360;
        }

        rotation.X += lookVector.Y * JoypadSensibility;
        rotation.X = Mathf.Clamp(rotation.X, -60, 45);

        head.RotationDegrees = rotation;
    }

    public override void _PhysicsProcess(double delta)
    {

        Vector3 velocity = this.Velocity;

        // Add the gravity.
        if (!this.IsOnFloor())
            velocity.Y -= gravity * (float)delta;

        // Get the input direction and handle the movement/deceleration.
        Vector2 inputDir = Input.GetVector(Main.ACTION_NAME_GO_LEFT, Main.ACTION_NAME_GO_RIGHT,
                                            Main.ACTION_NAME_GO_FORWARD, Main.ACTION_NAME_GO_BACKWARD);

        Vector3 direction = head.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y);

        if (direction != Vector3.Zero)
        {
            // Calcolo della velocità
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;

            if (!walkAudio.Playing)
            {

                random.Randomize();
                walkAudio.PitchScale = random.RandfRange(0.6f, 0.7f);//da capire
                walkAudio.Play();
            }
        }
        else
        {
            // Ferma il suono dei passi quando non c'è movimento
            if (walkAudio.Playing)
            {
                walkAudio.Stop();
            }

            // Riduzione graduale della velocità
            velocity.X = Mathf.MoveToward(this.Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(this.Velocity.Z, 0, Speed);
        }

        this.Velocity = velocity;

        this.MoveAndSlide();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion)
        {

            Vector3 rotation = head.RotationDegrees;

            if (!lockCameraHeight)
            {
                rotation.X -= eventMouseMotion.Relative.Y * mouseSensibility;
                rotation.X = Mathf.Clamp(rotation.X, -60, 45);
            }


            rotation.Y -= eventMouseMotion.Relative.X * mouseSensibility;
            if (rotationLimit != NO_ROTATION_LIMIT)
                rotation.Y = Mathf.Clamp(rotation.Y, 180 - rotationLimit, 180 + rotationLimit);

            if (rotation.Y > 360)
            {
                rotation.Y -= 360;
            }
            if (rotation.Y < -360)
            {
                rotation.Y += 360;
            }

            head.RotationDegrees = rotation;
        }

    }



    public void CameraAnimation(int time, Vector3 finalRotation, Vector3 finalPosition)
    {
        for (int i = animation.GetTrackCount() - 1; i >= 0; i--)
        {
            animation.RemoveTrack(i);
        }

        animation.Length = time;

        animation.AddTrack(Animation.TrackType.Position3D);       
        animation.AddTrack(Animation.TrackType.Value);//rotation       

        animation.TrackSetPath(0, this.GetPath().ToString());
        animation.TrackInsertKey(0, 0, this.Position);
        animation.TrackInsertKey(0, time, finalPosition);

        animation.TrackSetPath(1, head.GetPath().ToString() + ":rotation");
        animation.TrackInsertKey(1, 0, head.GlobalRotation);
        animation.TrackInsertKey(1, time, finalRotation);
/*
        animation.TrackSetPath(2, head.GetPath().ToString() + ":rotation:x");
        animation.TrackInsertKey(2, 0, head.GlobalRotation.X);
        animation.TrackInsertKey(2, time, cameraXRotation);*/

        animationPlayer.CallDeferred("play",ANIMATION_NAME);
    }

    public void Stop()
    {
        this.walkAudio.Stop();
        this.SetProcess(false);
        this.SetPhysicsProcess(false);
        this.SetProcessInput(false);
        this.rayCast3D.SetProcess(false);
        this.rayCast3D.SetProcessInput(false);
    }

    public void Resume()
    {
        this.SetProcess(true);
        this.SetPhysicsProcess(true);
        this.SetProcessInput(true);
        this.rayCast3D.SetProcess(true);
        this.rayCast3D.SetProcessInput(true);
    }

    public Node3D GetNodeViewed()
    {
        return (Node3D)rayCast3D.GetCollider();
    }

    public Camera3D GetCamera()
    {
        return this.GetNode<Camera3D>("Head/Camera3D");
    }

    public Node3D GetHead()
    {
        return head;
    }

    public AudioStreamPlayer GetAudio()
    {
        return this.walkAudio;
    }

    public void StopInteractionInput()
    {
        rayCast3D.SetProcess(false);
    }

    public void ResumeInteractionInput()
    {
        rayCast3D.SetProcess(true);
    }
    public void SetHeadRotationDegree(Vector3 newRotation)
    {
        head.RotationDegrees = new Vector3(Mathf.Clamp(newRotation.X, -60, 45), newRotation.Y % 360, newRotation.Z);
    }
    public void SetlockCameraHeight(bool Bool)
    {
        this.lockCameraHeight = Bool;
    }

    public bool IsCameraHeightLocked()
    {
        return this.lockCameraHeight;
    }

    
    public bool IsAnimationPlaying()
    {
        return animationPlayer.IsPlaying();
    }

    public void SetRotationLimit(int rotationLimit)
    {
        this.rotationLimit = rotationLimit / 2;

        if (rotationLimit == NO_ROTATION_LIMIT)
            this.rotationLimit = NO_ROTATION_LIMIT;

    }
    

    public void OnEffectsValueChanged(float value)
	{
        walkAudio.VolumeDb = value;
	}

   
}
