using Godot;

public partial class Camics : Node3D
{
	[Export] private GameObject key;
	[Export] private StaticBody3D staticBody3D;

	public override void _Ready()
	{
		this.staticBody3D.CollisionLayer=4;

	}


	public void SetTriggerKeyUnlocked()
	{
		this.staticBody3D.CollisionLayer=1;
	}

	public void Interact()
	{
		this.staticBody3D.CollisionLayer=4;
		key.ObtainObject();
	}
}
