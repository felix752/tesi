using Godot;
using System;

public partial class FeliceAnswerNoDialog : Dialog
{

	[Export] private Node DialogAnswerIDK;
	[Export] private Node DialogAnswerName;
	[Export] private Node nextDialog;
	[Export] private DialogLabel dialogLabel;

	public override void Method0()
	{
		dialogLabel.Clean();
		dialogLabel.StartDialog(DialogAnswerName.GetPath(), new Callable(this, nameof(this.StartNextDialog)));
	}

	public override void Method1()
	{
		dialogLabel.Clean();
		dialogLabel.StartDialog(DialogAnswerIDK.GetPath(), new Callable(this, nameof(this.StartNextDialog)));
	}


	public override void Method2()
	{
		throw new NotImplementedException();
	}

	public override void Method3()
	{
		throw new NotImplementedException();
	}

	public override Callable[] GetMethodes()
	{

		Callable[] callables;

		callables = new Callable[2]
		{
			new(this, nameof(Method0)),
			new(this, nameof(Method1))
		};

		return callables;
	}
	
	private void StartNextDialog()
	{
		dialogLabel.StartDialog(nextDialog.GetPath());
	}
	

}
