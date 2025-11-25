using Godot;

public partial class FeliceDoor : DoorBussable
{
	private const int INDEX_DEFAULT_DIALOG = 0;
	private const int INDEX_DEFAULT_DIALOG_LOOP = 1;
	private const int INDEX_DIALOG_WITH_KEY = 2;
	private const int INDEX_KEY_DIALOG_LOOP = 3;
	private int currentDialogIndex;
	[Export] private Node[] dialogs; 
	private bool triggerKey;

	[Export] private Camics camics;
	[Export] private Inventory inventory;
	public Inventory InventoryNode { set => inventory = value; }

	public override void _Ready()
	{
		base._Ready();
		currentDialogIndex = INDEX_DEFAULT_DIALOG;
		triggerKey = false;
	}


	protected override void Talk()
	{
		base.Talk();
		player.Stop();

		if (inventory.Contain("chiave misteriosa") && currentDialogIndex != INDEX_KEY_DIALOG_LOOP)
		{
			currentDialogIndex = INDEX_DIALOG_WITH_KEY;
		}

		dialogLabel.StartDialog(dialogs[currentDialogIndex].GetPath());

		if (currentDialogIndex == INDEX_DEFAULT_DIALOG)
		{
			camics.SetTriggerKeyUnlocked();
			currentDialogIndex = INDEX_DEFAULT_DIALOG_LOOP;
		}
		else if (currentDialogIndex == INDEX_DIALOG_WITH_KEY)
		{
			currentDialogIndex = INDEX_KEY_DIALOG_LOOP;
		}
		

	}


}
