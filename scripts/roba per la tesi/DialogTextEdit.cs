using Godot;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Diagnostics;


public partial class DialogTextEdit : TextEdit
{
	private const string ApiUrl = "https://ruthenious-carline-centesimally.ngrok-free.dev";
	private const string PLACEHOLDER_DEFAULT_TEXT = "Inserisci testo o premi esc";
	private const string PLACEHOLDER_WAIT_TEXT = "Mo sto pensando";

	[Export] private HttpRequest client;
	[Export] private Player player;
	
	private List<char> paolo;
	private int paoloInd;
	private int max;
	private bool isOutputFinished;
	private Callable functionOnFinishedDialog;
	private List<Dictionary<string, string>> chatHistory;

	[Signal] public delegate void DialogStartedEventHandler();
	[Signal] public delegate void DialogFinishedEventHandler();

	HttpClient http ;

    public override void _Ready()
    {
        paoloInd=0;
		max=100; 
		SetProcess(false);
    }

	public override void _Process(double delta)
	{
		if ( paolo!=null && paoloInd<paolo.Count)
		{
			Text += paolo[paoloInd];
		
			if (Text.Length>=100)
			{
				if (paolo[paoloInd]=='\n' || paolo[paoloInd]=='\b' || paolo[paoloInd]==' ')
				{
					isOutputFinished = false;
					Text += "...";
					paoloInd++;
					this.SetProcess(false);
					return;
				}
			}
			paoloInd++;	

		}			
		
	}
	
	public void Active(List<Dictionary<string, string>> history, Callable updateHistory)
    {
		chatHistory=history;
        this.Visible=true;
		player?.Stop();
		this.GrabFocus();
		this.EmitSignal(SignalName.DialogStarted);
		functionOnFinishedDialog = updateHistory;
    }

    public override void _Input(InputEvent @event)
    {
		if (@event.IsActionPressed("finishDialog"))
		{
			this.GetViewport().SetInputAsHandled();
			if (this.HasFocus())
			{
				//string text = this.GetSelectedText();GD.Print("stringa" + text);
				if (Text!="")
				{
					this.PlaceholderText = PLACEHOLDER_WAIT_TEXT;
					isOutputFinished = true;
					this.StartStreaming(Text, "");
					Text="";
					this.ReleaseFocus();
				}
			}
			else
			{
				this.Text="";

				if (isOutputFinished)
				{
					GrabFocus();
					this.PlaceholderText= PLACEHOLDER_DEFAULT_TEXT;
					this.paoloInd=0;
					paolo=null;
					max=100;
				}
				else
				{
					isOutputFinished = true;
					this.SetProcess(true);
				}
			}
		}
		else if (@event.IsActionPressed(Main.ACTION_NAME_BACK))
		{
			if (HasFocus())
			{
				GD.Print("dizionario storia: " + chatHistory.ToString());
				string stringa = JsonSerializer.Serialize(chatHistory);
				functionOnFinishedDialog.Call(stringa);
				this.EmitSignal(SignalName.DialogFinished);
				player.Resume();
				this.Visible=false;		
				ReleaseFocus();	
			}

		}
    }

	public void SendMessageToAI(string text, string npcId)
	{
		
		Dictionary<string, string> dict = new()
        {
                {"role", "user"},
                {"content", text}
		};
		
		chatHistory.Add(dict);		

		GD.Print("mo sto mandando questa storia" +JsonSerializer.Deserialize<List<Dictionary<string, string>>>(chatHistory.ToString()));
		
		var payload = new {
			npc_id = npcId,
			memory = chatHistory
        };

		string json = JsonSerializer.Serialize(payload);

		// Prepara i headers
		string[] headers = { "Content-Type: application/json" };

		// Invia la richiesta POST
		var err = client.Request(
			ApiUrl,
			headers,
			HttpClient.Method.Post,
			json
		);

		if (err != Error.Ok)
		{
			GD.PrintErr("HTTP request error: " + err);
			return;
		}

		GD.Print("sto mandando la richiesta");
    }

	public void OnRequestCompleted (int result, int response_code, string[] headers, byte[] body)
    {
		if (result!=0)
		{
			GD.Print("Fermo! è tutto rotto!");
			GD.Print("result :" + result);
			return;
		}

		GD.Print("response code: " + response_code);
		GD.Print("headers: " + headers);


		GD.Print("sto ricevendo la risposta");
		string text = Encoding.UTF8.GetString(body);


		// Deserializza la risposta JSON
		var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(text);

		if (dict != null && dict.ContainsKey("response"))
        {
			Text+=((System.Text.Json.JsonElement)dict["response"]).ToString().ToArray();
            

			if (false)
			{
				Dictionary<string, string> dizionario = new()
				{
					{"role", "assistant"},
					{"content", ((System.Text.Json.JsonElement)dict["response"]).ToString()}
				};
				
				chatHistory.Add(dizionario);
			}
					
        }
        else
        {
            GD.PrintErr("JSON malformato o campo mancante.");
        }
		
    }

    /*
        public void OnRequestCompleted (int result, int response_code, string[] headers, byte[] body)
        {
            if (result!=0)
            {
                GD.Print("Fermo! è tutto rotto!");
                GD.Print("result :" + result);
                return;
            }

            GD.Print("response code: " + response_code);
            GD.Print("headers: " + headers);


            GD.Print("sto ricevendo la risposta");
            string text = Encoding.UTF8.GetString(body);


            // Deserializza la risposta JSON
            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(text);

            if (dict != null && dict.ContainsKey("response"))
            {
                paolo =((System.Text.Json.JsonElement)dict["response"]).ToString().ToArray();
                OutputText();

                Dictionary<string, string> dizionario = new()
                {
                    {"role", "assistant"},
                    {"content", ((System.Text.Json.JsonElement)dict["response"]).ToString()}
                };

                chatHistory.Add(dizionario);		
            }
            else
            {
                GD.PrintErr("JSON malformato o campo mancante.");
            }

        }*/


	private async Task StartStreaming(string text, string npcId)
    {
		HttpClient http = new (); 
		paolo = new List<char>();
 
        Error err = http.ConnectToHost(ApiUrl);
        if (err != Error.Ok)
        {
            GD.Print("Errore connessione");
            return;
        }

        while (http.GetStatus() == HttpClient.Status.Connecting || 
               http.GetStatus() == HttpClient.Status.Resolving)
        {
            http.Poll();
			GD.Print(http.GetStatus().ToString());
			await ToSignal(GetTree(), "process_frame");

        }

        if (http.GetStatus() != HttpClient.Status.Connected)
        {
            GD.Print("Non connesso");
            return;
        }
       
        GD.Print("Connesso, invio POST...");

		
		Dictionary<string, string> dict = new()
        {
                {"role", "user"},
                {"content", text}
		};
		
		chatHistory.Add(dict);		

		//GD.Print("mo sto mandando questa storia" +JsonSerializer.Deserialize<List<Dictionary<string, string>>>(chatHistory.ToString()));

		
		var payload = new {
			npc_id = npcId,
			memory = chatHistory
        };

		string json = JsonSerializer.Serialize(payload);

		// Prepara i headers
		string[] headers = { "Content-Type: application/json" };

        http.Request(
            HttpClient.Method.Post,
            "/chat",
            headers,
            json
        );

        while (http.GetStatus() == HttpClient.Status.Requesting)
        {
            http.Poll();
            await ToSignal(GetTree(), "process_frame");
        }
/*
        var result = http.GetResponseHeaders(); 
        var body = http.GetResponseBodyRaw();
        var reader = new StreamPeerBuffer();
        reader.DataArray = body;*/

        GD.Print("Inizio streaming…");
		
		SetProcess(true);

        while (true)
        {
            http.Poll();

			string chunk = Encoding.UTF8.GetString(http.ReadResponseBodyChunk());
			
			GD.Print("CHUNK: ", chunk);
	
			// 🌟 QUI la connessione si è chiusa → EOF
			if (http.GetStatus() == HttpClient.Status.Disconnected || chunk == "¶")
			{
				Dictionary<string, string> dizionario = new()
				{
					{"role", "assistant"},
					{"content", new string(paolo.ToArray())}
				};
				
				chatHistory.Add(dizionario);	
				GD.Print("ho finito di rispondere");
				GD.Print("mo stampo paolo come prima: " + paolo.ToString());
				GD.Print("mo stampo quello come mo  : " + new string(paolo.ToArray()));
				GD.Print("mo stampo pure la storia  : " + JsonSerializer.Deserialize<List<Dictionary<string, string>>>(chatHistory.ToString()));
				break;
			}

			if (chunk!=null && chunk!="")
			{
				foreach (char item in chunk.ToArray())
				{
					paolo.Add(item);	
				}
			}

            await ToSignal(GetTree(), "process_frame");
        }
    }

}