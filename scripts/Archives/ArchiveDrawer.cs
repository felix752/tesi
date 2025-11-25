using Godot;

public partial class ArchiveDrawer : MeshInstance3D
{
	protected const string OPEN_ANIMATION_NAME = "new_animation";
	[Export] protected AnimationPlayer animationPlayer;
	private bool isOpened;
	public Label label;


	public override void _Ready()
	{
		isOpened = false;
	}


	public void OnDrawerClicked()
	{

		if (!animationPlayer.IsPlaying())
		{

			if (isOpened)
			{
				animationPlayer.PlayBackwards(OPEN_ANIMATION_NAME);
			}

			else
				animationPlayer.Play(OPEN_ANIMATION_NAME);


			isOpened = !isOpened;
		}

	}

	public void PrintString(string text)
	{
		label.VisibleRatio = 1;
		label.Text = text;
		label.Visible = true;
	}

	public void SetLabelInvisible()
	{
		label.Visible = false;
		
	}
}
