using Godot;
using System;

public partial class ThomasSecondDialog : Dialog
{
	[Export] DialogLabel dialogLabel;
	[Export] Node askFor7Node;
	[Signal] public delegate void GiveSevenEventHandler();

	public override void Method0()
	{
		dialogLabel.Clean();
		dialogLabel.StartDialog(askFor7Node.GetPath(), new Callable(this,nameof(this.OnGiveSeven)));
	}

	public override void Method1()
	{
		dialogLabel.Close();
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

	private void OnGiveSeven()
	{
		this.EmitSignal(SignalName.GiveSeven);
	}
}
