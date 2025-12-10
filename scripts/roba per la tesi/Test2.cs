using Godot;
using System.Collections.Generic;
using System.Text.Json;

public partial class Test2 : Node3D
{
	[Export] private DialogTextEdit dialogTextEdit;
	private List<Dictionary<string, string>> chatHistory; // e se usassi direttamente una stringa Json??????
	private const string SYSTEM_PROMPT = "Tu sei Juanfrancescangelo Francantonio detto Carlos. Sei un burlone";
	
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
