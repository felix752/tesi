using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Cards : Node3D
{
	[Export] private Node3D[] cards;
	[Export] private Node3D[] tableCards;
	private RandomNumberGenerator random;
	private int sevenIndexPosition;
	[Export] private Node3D seven;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		random = new();
		List<Node3D> list = cards.ToList();
		sevenIndexPosition = random.RandiRange(1, tableCards.Length - 2);

		for (int i = 0; i < tableCards.Length; i++)
		{
			if (i != sevenIndexPosition)
			{
				int randomIndex = random.RandiRange(0, list.Count - 1);
				list[randomIndex].GlobalPosition = tableCards[i].GlobalPosition;

				if (i != tableCards.Length - 1 && i != 0 && random.RandiRange(0, 1) == 0)
				{
					list[randomIndex].GlobalRotation = new Vector3(list[randomIndex].GlobalRotation.X, list[randomIndex].GlobalRotation.Y, MathF.PI);
				}
				list.RemoveAt(randomIndex);
				tableCards[i].QueueFree();
			}
			else
			{
				tableCards[i].Visible = false;
			}

		}

		foreach (Node3D card in list)
		{
			card.QueueFree();
		}
	}


	public void InsertSeven()
	{
		seven.GlobalPosition = tableCards[sevenIndexPosition].GlobalPosition;
		seven.GlobalRotation = tableCards[sevenIndexPosition].GlobalRotation;
		seven.Visible = true;
		tableCards[sevenIndexPosition].QueueFree();
	}

	
}
