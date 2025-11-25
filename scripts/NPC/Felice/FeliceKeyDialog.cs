using Godot;
using System;

public partial class FeliceKeyDialog : Dialog
{
	[Export] Node answerYes;
	[Export] Node answerNo;
	[Export] Node nextDialog;
	[Export] DialogLabel dialogLabel;


	public override void Method0()
	{
		dialogLabel.Clean();
		dialogLabel.StartDialog(answerYes.GetPath(), new Callable(this, nameof(this.StartNextDialog)));
	}

	public override void Method1()
	{
		dialogLabel.Clean();
		dialogLabel.StartDialog(answerNo.GetPath());
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
