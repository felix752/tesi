using Godot;
using System;

public partial class MicheleDefaultDialog : Dialog
{
  [Export] DialogLabel dialogLabel;
  [Export] private Node ANSWER_NO;
  [Export] private Node ANSWER_MAYBE_NOT;
  [Export] private Node ANSWER_YES;
  [Signal] public delegate void DialogFinishedEventHandler();
  public override void Method0()
  {
    this.dialogLabel.Clean();
    this.dialogLabel.StartDialog(ANSWER_NO.GetPath(), new Callable(this, nameof(this.Signal)));
  }

  public override void Method1()
  {

    this.dialogLabel.Clean();
    this.dialogLabel.StartDialog(ANSWER_MAYBE_NOT.GetPath(), new Callable(this, nameof(this.Signal)));
  }

  public override void Method2()
  {

    this.dialogLabel.Clean();
    this.dialogLabel.StartDialog(ANSWER_YES.GetPath(), new Callable(this, nameof(this.Signal)));

  }

  public override void Method3()
  {

    throw new NotImplementedException();
  }

  public override Callable[] GetMethodes()
  {

    Callable[] callables = {
      new (this, nameof(this.Method0)),
      new(this, nameof(this.Method1)),
      new(this, nameof(this.Method2))
    };

    return callables;

  }

public void Signal()
{
    this.EmitSignal(SignalName.DialogFinished);
}

}
