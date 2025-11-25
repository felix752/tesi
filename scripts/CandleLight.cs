using Godot;
using System;

public partial class CandleLight : OmniLight3D {

	private float energy;
	private Random random;
	private double time;
	[Export] float percentual;
	[Export] double timePerSecond;
	private float num;
	public override void _Ready() {

		energy = LightEnergy;
		this.random = new Random();
		time = 0;

		percentual /= 100;
		percentual -= percentual / 2;
		timePerSecond = 1 / timePerSecond;
		num = 1 - percentual * 2;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		time += delta;
		if (time >= timePerSecond) {

			this.LightEnergy = energy * ((float)this.random.NextDouble() * percentual + num);
			time = 0;
		}

	}
}
