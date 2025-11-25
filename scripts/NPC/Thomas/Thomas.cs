using Godot;

public partial class Thomas : DoorBussable
{

	private const int INDEX_DEFAULT_DIALOG = 0;
	private const int INDEX_DEFAULT_DIALOG_LOOP = 1;
	private const int INDEX_DIALOG_WITH_7 = 2;
	private int currentDialogIndex;
	[Export] private Node[] dialogs;
	private bool triggerSeven;

	[Export] GameObject seven;
	[Export] Gianmarco gianmarco;

	public override void _Ready()
	{
		base._Ready();
		currentDialogIndex = INDEX_DEFAULT_DIALOG;
		triggerSeven = false;
	}


	protected override void Talk()
	{
		base.Talk();
		player.Stop();

		if (triggerSeven && currentDialogIndex == INDEX_DEFAULT_DIALOG_LOOP)
		{
			currentDialogIndex = INDEX_DIALOG_WITH_7;
			//triggerSeven = false;
		}

		dialogLabel.StartDialog(dialogs[currentDialogIndex].GetPath());

		if (currentDialogIndex == INDEX_DEFAULT_DIALOG || currentDialogIndex == INDEX_DIALOG_WITH_7)
		{
			currentDialogIndex = INDEX_DEFAULT_DIALOG_LOOP;
		}

		gianmarco.TriggerGesualdoUnlocked();
	}

	public void SetTrigger7Unlocked()
	{
		triggerSeven = true;
	}

	//segnale
	public void GiveSeven()
	{
		player.Resume();
		seven.ObtainObject();
		triggerSeven = false;
	}
}
