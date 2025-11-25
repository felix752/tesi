using Godot;

public partial class Asylum : Node3D
{
	private const string ANIMATION_NAME_LABORATORY = "new_animation";

	private readonly Vector3 playerAsylumPosition = new (10, 1, -4);
	private readonly Vector3 playerLaboratoryRotation = new (0, 0, 0);


	[Export] private Laboratory laboratory;
	[Export] private AnimationPlayer laboratoryAnimationPlayer;
	[Export] private AudioStreamPlayer3D[] rainMusicPlayers;
	[Export] private AudioStreamPlayer stepMusicPlayers;
	[Export] private GpuParticles3D rain;

	[Export] private Player player;
	[Export] private Michele michele;
	[Export] private Gianmarco gianmarco;
	[Export] private CollisionShape3D collisionShape3D;
	[Export] private FeliceDoor felice1;
	[Export] private Door olddoor;
	[Export] private Door newDoor;
	[Export] private DoorDoctor doorDoctor;
	[Export] private Node3D felice2;
	[Export] private BigDoor MainGate;
	[Export] private Door[] doorList;

	[Export] GameObject[] gameObjects;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.RemoveChild(newDoor);
		this.RemoveChild(felice2);
	}
	


	public void OnLaboratoryEntered(Node3D body)
	{
		if (body == player)
		{
			laboratoryAnimationPlayer.Play(ANIMATION_NAME_LABORATORY);

			this.laboratory.OnPlayerEntered(player);
		}

	}

	public void OnLaboratoryExited(bool boolean)
	{
		laboratoryAnimationPlayer.Play(ANIMATION_NAME_LABORATORY);
		laboratoryAnimationPlayer.AnimationFinished += SetPlayerAfterLaboratory;

		if (boolean)
		{
			gianmarco.QueueFree();
			michele.QueueFree();
			collisionShape3D.SetDeferred("disabled", false);
			olddoor.QueueFree();

			this.AddChild(newDoor);
			this.AddChild(felice2);
		}

	}

    public void SetPlayerAfterLaboratory(StringName animName)
    {
		this.player.Position = playerAsylumPosition;
		this.player.SetHeadRotationDegree(playerLaboratoryRotation);
	
		laboratoryAnimationPlayer.AnimationFinished -= SetPlayerAfterLaboratory;
    }

	public void ChangeEffectVolume(float value)
	{
		foreach (AudioStreamPlayer3D audio in rainMusicPlayers)
		{
			audio.VolumeDb = value - 10;
		}
		foreach (Door door in doorList)
		{
			door.OnEffectsValueChanged(value/2);
		}

		gianmarco.OnEffectsValueChanged(value + 20);
		stepMusicPlayers.VolumeDb = value;
	}
	
	public void SetInventory(Inventory inventory)
    {
		foreach (GameObject gameObject in gameObjects)
		{
			gameObject.InventoryNode = inventory;
		}
		felice1.InventoryNode = inventory;
		laboratory.Fazzolett.InventoryNode = inventory;
		michele.InventoryNode = inventory;
		doorDoctor.InventoryNode = inventory;
		MainGate.InventoryNode = inventory;
    }
}
