using Godot;

public partial class Archive : Node3D
{
	[Export] Label label;
	[Export] Node3D object1;
	[Export] ArchiveDrawer normalDrawer1;
	[Export] ArchiveDrawer normalDrawer2;
	[Export] ArchiveDrawer drawerWithObject;


	public override void _Ready()
	{
		if (drawerWithObject is DrawerWithObject drawer && object1 != null)
		{
			drawer.AddObject(object1, (float)GetMeta("objectFinalScale"));
		}
		else
		{
			drawerWithObject.label = label;
		}
		normalDrawer1.label = label;
		normalDrawer2.label = label;
	}
}
