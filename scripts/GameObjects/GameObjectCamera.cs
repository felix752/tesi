using Godot;
using System;

public partial class GameObjectCamera : Node3D {
    private const string ROTATION_ANIMATION_NAME = "new_animation";
    private Node3D actualNode;
	[Export] private AnimationPlayer animation;

	public void ChangeObject(GameObject newGameObject)
	{ 

		if (actualNode != null && actualNode.GetParent() == this)
			RemoveChild(actualNode);

		newGameObject.Model.Visible = true;
		newGameObject.Model.Scale *= newGameObject.CameraScale;
		newGameObject.Model.GetParentOrNull<Node>()?.RemoveChild(newGameObject.Model);
		AddChild(newGameObject.Model);

		newGameObject.CameraScale = 1;
		actualNode = newGameObject.Model;

		CallDeferred(nameof(SetAnimation));		
	}

	private void SetAnimation() {

		animation.RootNode = actualNode.GetPath();
		animation.Play(ROTATION_ANIMATION_NAME);
	}

	public void StopAnimation() {

		if(animation.IsPlaying()) {
		
			animation.Stop();
			
			RemoveChild(actualNode);
			actualNode=null;
			
		}

	}


}
