using Godot;
using System;
using System.Collections.Generic;

public partial class Furni3 : Furni2
{
	private readonly NodePath[] shelfPaths = { "Shelf1", "Shelf2", "Shelf3", "Shelf4" };

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
    {
		base._Ready();
		this.Open();
		Random random = new();

		foreach(NodePath path in shelfPaths)
		{
			Node shelf = GetNode(path);

            foreach(Node bottle in shelf.GetChildren(true))
            {
                if (random.Next(0,2)==0)
                {
					bottle.QueueFree();
                }
            }
        }

    }

	
}
