using Godot;
using System;

public partial class Furni2 : Furni1
{
	protected const string OPEN_ANIMATION_NAME = "open";
	protected bool isOpen;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		isOpen = false;
	}

    public override void Open()
    {
        if (!animationPlayer.IsPlaying())
		{
			if (isOpen)
			{
				animationPlayer.PlayBackwards(OPEN_ANIMATION_NAME);
			}
			else
			{
				animationPlayer.Play(OPEN_ANIMATION_NAME);
			}
			isOpen = !isOpen;
		}
    }


	
}
