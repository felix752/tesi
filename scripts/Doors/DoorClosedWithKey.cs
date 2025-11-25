using Godot;
using System;

public partial class DoorClosedWithKey : Door
{

	[Export] protected Inventory inventory;
	[Export] protected DialogLabel label;
	private const string KEY_META_NAME = "keyName";
	protected string keyName;
	protected const string NOT_OPEN_DOOR_ANIMATION = "openButNotOpen2";
	protected const string DOOR_CLOSED_TEXT = "*la porta è chiusa a chiave*";

	public override void _Ready()
	{
		base._Ready();

		if (GetMetaList!=null)
		{
			keyName = (string)GetMeta(KEY_META_NAME);
		}
	
		
	}

	public override void DoorClicked()
	{

		if (keyName != null && inventory != null && inventory.Contain(keyName))
		{

			base.DoorClicked();

		}
		else
		{
			label.OutputText(DOOR_CLOSED_TEXT);
			this.animation.Play(NOT_OPEN_DOOR_ANIMATION);
		}

	}




}
