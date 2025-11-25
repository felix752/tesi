using Godot;
using System;

public partial class Laboratory : Node3D
{
	[Export] private Area3D exitArea;
	private Player player;
	private readonly Vector3 playerLaboratoryPosition = new (-2, -6, 0);
	private readonly Vector3 playerLaboratoryRotation = new(0, 90, 0);

	[Export] private GameObject fazzolett;
	[Export] private ObtainItemAnimation obtainItemAnimation;

	public GameObject Fazzolett { get => fazzolett; }

	[Signal] public delegate void LaboratoryExitedEventHandler(bool boolean);

    public override void _Ready()
    {
		fazzolett.obtainItemAnimation = obtainItemAnimation;
    }


	public void OnPlayerEntered(Node3D player)
	{
		player.Position = playerLaboratoryPosition;
		this.player = (Player)player;
		this.player.SetHeadRotationDegree(playerLaboratoryRotation);
	}

	public void OnPlayerExited(Node3D body)
	{
		if (body==player)
		{
			this.EmitSignal(SignalName.LaboratoryExited, true);
		}
		
	}
}
