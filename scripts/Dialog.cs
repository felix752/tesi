

using System.Linq;
using Godot;

public abstract partial class Dialog : Node
{

    public abstract void Method0();
    public abstract void Method1();
    public abstract void Method2();
    public abstract void Method3();

    public abstract Callable[] GetMethodes();
  /*  {
        Callable[] callables = new Callable[4];

        try
        {
            callables.Append(new(this, nameof(this.Method0)));
            callables.Append(new(this, nameof(this.Method1)));
            callables.Append(new(this, nameof(this.Method2)));
            callables.Append(new(this, nameof(this.Method3)));

        }
        catch (System.NotImplementedException) { } 

         return callables;
        
    }*/
    
}