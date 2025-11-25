using Godot;

public partial class GameObjectPickable : GameObject {
    private const string ANIMATION_NAME = "elTopoQueGira";
    private const string COLLISION_PATH = "StaticBody3D";
	[Export] private AnimationPlayer animation;


	public override void _Ready()
	{

		base._Ready();

		animation.Play(ANIMATION_NAME);
	}


	public void TakeObject() {

	//	inventory.AddGameObject(this);
	//	RemoveChild(Model);
	//	this.GetNode(COLLISION_PATH).QueueFree();
		ObtainObject();
	}
	

}
