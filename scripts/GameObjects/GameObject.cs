using Godot;

public partial class GameObject : Node3D
{
	private const string COLLISION_PATH = "StaticBody3D";
	
	public string Type {get; set;}

	[Export] public string ObjectName { get; set; } 
	[Export] public string Description {get; set;} 
	[Export] public string FullDescription {get; set;} = "per i consumabili si può ignorare";
	[Export] public float CameraScale {get; set;}

	[Export] public Node3D Model {get; set;}
	[Export] protected Inventory inventory;
	public Inventory InventoryNode { set => inventory = value; }
	[Export] public Texture2D sprite;

	[Export] public ObtainItemAnimation obtainItemAnimation;


	public override void _Ready()
	{
		this.Type = this.GetGroups()[0];

		FullDescription = FullDescription.Replace("\\n", "\n");
		Description = Description.Replace("\\n", "\n");

		this.SetProcessInput(false);
		this.ProcessMode = ProcessModeEnum.Always;
	}
	

	public void ObtainObject()
	{
		AddChild(obtainItemAnimation);
		obtainItemAnimation.StartAnimation("Hai ottenuto " + "\"" + this.ObjectName + "\"", this);

		this.CallDeferred("set_process_input",true);
		this.GetTree().Paused = true;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionReleased(Main.ACTION_NAME_INTERACT) && !obtainItemAnimation.IsPlaying())
		{
			this.SetProcessInput(false);
			obtainItemAnimation.Reset();
			obtainItemAnimation.GetParent().RemoveChild(obtainItemAnimation);
			inventory.AddGameObject(this);
			this.GetTree().SetDeferred("paused", false);
			Model.GetParent().RemoveChild(Model);
			this.GetNodeOrNull(COLLISION_PATH)?.QueueFree();
			//this.QueueFree();
		}
		

    }

}
