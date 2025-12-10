using Godot;
using System.Collections.Generic;
using System.Text.Json;

public partial class Test : Node3D
{
	[Export] private DialogTextEdit dialogTextEdit;
	private List<Dictionary<string, string>> chatHistory; // e se usassi direttamente una stringa Json??????
	private const string SYSTEM_PROMPT ="Tu sei Gianmarco, un infermiere responsabile della sorveglianza del primo piano del “manicomio Rockroll”. Sei un tipo gentile ed educato. Ti rivolgerai a Francesco, un paziente con diversi problemi di memoria, anche se eviti di chiamarlo per nome. Se Francesco inizia a ricordare qualcosa che stai nascondendo: voi infermieri che lo portate via, il dottore che esegue degli esperimenti su di lui e il Messia o il corpo di un uomo in una vasca, inizi a proecuparti. Non usare emoticon nelle risposte e non andare a capo e cerca di evitare di parlare di azioni che richiedono fisicamente di spostarsi, come fare una passaggiata o simili.";

	

	public override void _Ready()
    {
		chatHistory = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string>
            {
                {"role", "system"},
                {"content", SYSTEM_PROMPT}
            }
        };
	}
		

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnInteraction()
    {
		Callable callable = new (this, "OnInteractionClosed");
        dialogTextEdit.Active(chatHistory, callable);
    }

	public void OnInteractionClosed(string newChatHistory)
    {
		chatHistory=JsonSerializer.Deserialize<List<Dictionary<string, string>>>(newChatHistory);

        GD.Print("storia ritornata: " + newChatHistory);
    }
}
