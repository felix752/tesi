using Godot;

public partial class DrawerWithObject : ArchiveDrawer
{
	// Called when the node enters the scene tree for the first time.
	public void AddObject(Node3D gameObject, float finalScale)
	{

		gameObject.Position = new Vector3(0.9f, 0, 0);
		gameObject.GetParent().RemoveChild(gameObject);
		this.AddChild(gameObject); 


		Animation animation = this.animationPlayer.GetAnimation(OPEN_ANIMATION_NAME);
		animation.Length = 1.5f;

		int trackKey = animation.AddTrack(Animation.TrackType.Value);
		animation.TrackSetPath(trackKey, gameObject.Name + ":visible");
		animation.TrackInsertKey(trackKey, 0, false);
		animation.TrackInsertKey(trackKey, 1.3, true);

		trackKey = animation.AddTrack(Animation.TrackType.Value);
		animation.TrackSetPath(trackKey, gameObject.Name + ":position:y");
		animation.TrackInsertKey(trackKey, 1, 0.8);
		animation.TrackInsertKey(trackKey, 1.5f, 2.5);

		trackKey = animation.AddTrack(Animation.TrackType.Value);
		animation.TrackSetPath(trackKey, gameObject.Name + ":scale");
		animation.TrackInsertKey(trackKey, 1, new Vector3(0.1f,0.1f,0.1f));
		animation.TrackInsertKey(trackKey, 1.5f, new Vector3(finalScale,finalScale,finalScale));

		trackKey = animation.AddTrack(Animation.TrackType.Value);
		animation.TrackSetPath(trackKey, gameObject.Name + "/StaticBody3D/CollisionShape3D"  + ":disabled");
		animation.TrackInsertKey(trackKey, 0, true);
		animation.TrackInsertKey(trackKey, 1.5f, false);
		
	}
}
