using Godot;
using System;

public partial class GianmarcoDefaultDialog : Dialog {

	[Export] private Node answerYes;
	[Export] private Node answerNo;
	[Export] DialogLabel dialogLabel;
	[Export] GameObject pill;
	[Signal] public delegate void FinishedDialogEventHandler();

	public override void Method0()
	{
		dialogLabel.Clean();
		DisplayServer.MouseSetMode(DisplayServer.MouseMode.Captured);
		dialogLabel.StartDialog(answerYes.GetPath(), new Callable(this,nameof(this.Signal)));
	}

	public override void Method1()
	{
		dialogLabel.Clean();
		DisplayServer.MouseSetMode(DisplayServer.MouseMode.Captured);
		dialogLabel.StartDialog(answerNo.GetPath(), new Callable(this,nameof(this.Signal)));
	}

	public override void Method2()
	{
		throw new NotImplementedException();
	}

	public override void Method3()
	{
		throw new NotImplementedException();
	}

	public override Callable[] GetMethodes() {

		Callable[] callables;

		callables = new Callable[2]
		{
			new(this, nameof(Method0)),
			new(this, nameof(Method1))
		};

		return callables;
	}

	private void Signal()
	{
		this.EmitSignal(SignalName.FinishedDialog);
		pill.ObtainObject();

	}

}
