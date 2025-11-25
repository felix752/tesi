using Godot;
using System;

public partial class TextureRectScript : TextureRect
{
	ShaderMaterial shaderMat;
	public override void _Ready()
	{
		shaderMat = this.Material as ShaderMaterial;
	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void SetNoise(float noise)
	{
		shaderMat.SetShaderParameter("white_noise_rate", noise);
		
	}
}
