using Godot;
using System;

public partial class Paint : Node3D {
    private const string META_NAME_DESCIPTION = "description";
    private const string META_NAME_POSITION = "playerPosition";
    private const string META_NAME_ROTATION = "playerRotation";
    private const string META_NAME_CAMERA_ROTATION = "cameraXRotation";
    private string description;
	private Vector3 playerPosition;
	private float playerRotation;
	private float cameraXRotation;
	[Export] private DialogLabel label;
	[Export] private Player player;

	public override void _Ready() {


		description= (string) this.GetMeta(META_NAME_DESCIPTION);
		playerPosition= (Vector3) this.GetMeta(META_NAME_POSITION);
		playerRotation= (float) this.GetMeta(META_NAME_ROTATION);
		cameraXRotation= (float) this.GetMeta(META_NAME_CAMERA_ROTATION);
	}


    public void SeeObject() {
		
		if (!label.Visible)	{

			label.OutputText(description);

			float final_Y = playerRotation;
			if (player.GetHead().GlobalRotation.Y < 0)
				final_Y = -playerRotation;

			player.CameraAnimation(2, new Vector3(cameraXRotation, final_Y, 0), playerPosition);
		}

		
	}
}