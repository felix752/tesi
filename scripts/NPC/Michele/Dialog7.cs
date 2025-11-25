using Godot;

public partial class Dialog7 : Dialog
{
	[Signal] public delegate void Give7EventHandler();
	[Signal] public delegate void NoGive7EventHandler();
    public override void Method0()
    {
		this.EmitSignal(SignalName.Give7);
    }

    public override void Method1()
    {
		this.EmitSignal(SignalName.NoGive7);
    }

    public override void Method2()
    {
        throw new System.NotImplementedException();
    }

    public override void Method3()
    {
        throw new System.NotImplementedException();
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
