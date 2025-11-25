using Godot;
using System;

public partial class IntroDialog : Dialog
{
	[Export] DialogLabel textEdit;
	[Signal] public delegate void TriggeredEventHandler();
	
	public override void Method0() //rispondo si
	{
		textEdit.Clean();
		textEdit.Visible = false;
		textEdit.SetProcessInput(false);


		EmitSignal(SignalName.Triggered);
	}

    public override void Method1()//rispondo no
    {
        GetTree().ChangeSceneToFile("res://scenes/titoliDiCoda.tscn");

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

		Callable [] callables;

		callables = new Callable[2]
		{
			new(this, nameof(Method0)),
			new(this, nameof(Method1))
		};

		return callables;
    }

}
