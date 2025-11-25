using Godot;
using System;

public partial class DoorDoctor : DoorClosedWithKey
{
	[Export] private Michele michele;

    public Inventory InventoryNode { set=> inventory=value; }

    public override void DoorClicked()
	{
		if (!michele.IsDistracted) {

			michele.TryToOpenDoor();

		} else if (this.inventory.Contain(keyName)) {

				base.DoorClicked();

			} else {

				this.animation.Play(NOT_OPEN_DOOR_ANIMATION);
				label.OutputText(DOOR_CLOSED_TEXT);
			}

	}

}
