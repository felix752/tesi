using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using Godot.Collections;

public partial class Inventory : Node2D
{

	public const int CONSUMABLE = 0;
	public const int KEY = 1;
	public const int COLLECTABLE = 2;
	private int type;
	private const int NUM_BTN = 16;
	private string ANIMATION_NAME = "new_animation";

	private List<Button> HeaderButtonList;
	private List<Button> gameObjectButtons;
	private List<List<GameObject>> gameObjectList;

	private int indHeaderSelected;
	private int indObjectSelected;
	private List<Callable> callables;

	[Export] private Label objectSelectedDescription;
	[Export] private Label objectSelectedName;
	[Export] private GameObjectCamera gameObjectCamera;
	[Export] private Button utilizeBtn;
	[Export] private AnimationPlayer animationPlayer;
	[Export] private RichTextLabel collectableDescriptionLabel;
	[Export] private Texture2D consumableButtonTexture;
	[Export] private Texture2D collectableButtonTexture;


	public override void _Ready()
	{
		HeaderButtonList = new List<Button>
		{
			GetNode<Button>("ConsumableButton"),
			GetNode<Button>("KeyButton"),
			GetNode<Button>("CollectableButton")
		};

		gameObjectButtons = new List<Button>();

		for (int i = 0; i < NUM_BTN; i++)
		{
			gameObjectButtons.Add(GetNode<Button>("Control/Button" + i));
		}

		gameObjectList = new List<List<GameObject>>
		{
			new(),
			new(),
			new()
		};

		type = CONSUMABLE;
		indHeaderSelected = 0;
		indObjectSelected = 0;

		callables = new()
		{
			new Callable(this, nameof(OnConsumablesSelected)),
			new Callable(this, nameof(OnKeysSelected)),
			new Callable(this, nameof(OnCollectablesSelected))
		};

	}

	public override void _Input(InputEvent @event)
	{

		if (@event.IsActionReleased(Main.ACTION_NAME_SELECT_UP_RIGHT))
		{

			if (indHeaderSelected < HeaderButtonList.Count - 1)
			{

				callables[indHeaderSelected + 1].Call();
			}

		}
		else if (@event.IsActionReleased(Main.ACTION_NAME_SELECT_UP_LEFT))
		{

			if (indHeaderSelected > 0)
			{
				callables[indHeaderSelected - 1].Call();
			}

		}
		else if (@event.IsActionPressed(Main.ACTION_NAME_SELECT_LEFT))
		{

			if (indObjectSelected > 0)
			{

				gameObjectButtons[indObjectSelected].Disabled = false;
				indObjectSelected--;
				OnObjectSelected(indObjectSelected);

			}
		}
		else if (@event.IsActionPressed(Main.ACTION_NAME_SELECT_RIGHT))
		{

			if (gameObjectButtons[indObjectSelected + 1].Disabled == false)
			{

				gameObjectButtons[indObjectSelected].Disabled = false;
				indObjectSelected++;
				OnObjectSelected(indObjectSelected);
			}

		}
		else if (@event.IsActionPressed(Main.ACTION_NAME_BACK))
		{
			if (collectableDescriptionLabel.Modulate.A > 0)
			{
				this.GetViewport().SetInputAsHandled();

				if (!animationPlayer.IsPlaying())
					animationPlayer.PlayBackwards(ANIMATION_NAME);
			}

		}
		else if (@event.IsActionReleased(Main.ACTION_NAME_SELECT))
		{
			if (!collectableDescriptionLabel.Visible)
			{
				if (type == CONSUMABLE)
				{
					OnConsumableConsumed();
				}
				else if (type == COLLECTABLE)
				{
					OnOcchiettPremut();
				}	
			}
			
		} 
	}

	public void OnConsumablesSelected()
	{
		if (OnHeaderButtonPressed(HeaderButtonList[CONSUMABLE], HeaderButtonList))
		{
			indHeaderSelected = CONSUMABLE;
			type = CONSUMABLE;
			UploadGameObjects(CONSUMABLE);
		}
	}

	public void OnKeysSelected()
	{
		if (OnHeaderButtonPressed(HeaderButtonList[KEY], HeaderButtonList))
		{
			indHeaderSelected = KEY;
			type = KEY;
			UploadGameObjects(KEY);
		}
	}


	public void OnCollectablesSelected()
	{
		if (OnHeaderButtonPressed(HeaderButtonList[COLLECTABLE], HeaderButtonList))
		{

			indHeaderSelected = COLLECTABLE;
			type = COLLECTABLE;
			UploadGameObjects(COLLECTABLE);
		}
	}

	private void OnObjectSelected(int i)
	{

		if (i >= gameObjectList[type].Count) return;

		if (OnButtonPressed(gameObjectButtons[i], gameObjectButtons))
		{
			objectSelectedName.Text = gameObjectList[type][i].ObjectName;
			objectSelectedDescription.Text = gameObjectList[type][i].Description;
			gameObjectCamera.ChangeObject(gameObjectList[type][i]);
			indObjectSelected = i;
		}
	}
	private static bool OnHeaderButtonPressed(Button buttonPressed, List<Button> list)
	{

		if (!buttonPressed.Disabled)
		{
			buttonPressed.Disabled = true;
			foreach (Button button in list)
			{
				if (button != buttonPressed)
					button.Disabled = false;
			}
			return true;
		}
		return false;
	}
	private static bool OnButtonPressed(Button buttonPressed, List<Button> list)
	{

		if (!buttonPressed.Disabled)
		{
			buttonPressed.Disabled = true;
			foreach (Button button in list)
			{
				if (button != buttonPressed && button.Icon != null)
					button.Disabled = false;
			}
			return true;
		}
		return false;
	}

	public void AddGameObject(GameObject gameObject)
	{
		int gameObjectType;

		if (gameObject.Type == "Consumable")
			gameObjectType = CONSUMABLE;
		else if (gameObject.Type == "Key")
			gameObjectType = KEY;
		else if (gameObject.Type == "Collectable")
			gameObjectType = COLLECTABLE;
		else
			throw new DataException();

		gameObjectList[gameObjectType].Add(gameObject);
	}

	public void UploadConsumables()
	{

		OnConsumablesSelected();
		type = CONSUMABLE;
		UploadGameObjects(CONSUMABLE);

	}

	private void UploadGameObjects(int objectType)
	{
		int i = 0;
		indObjectSelected = 0;
		utilizeBtn.Visible = false;

		for (; i < gameObjectList[objectType].Count; i++)
		{

			int index = i;

			gameObjectButtons[i].Pressed += () => OnObjectSelected(index);

			gameObjectButtons[i].Disabled = false;
			gameObjectButtons[i].Icon = gameObjectList[objectType][i].sprite;
		}

		gameObjectButtons[0].Disabled = true;


		animationPlayer.Play("RESET"); 
		if (gameObjectList[objectType].Count != 0)
		{
			objectSelectedName.Text = gameObjectList[objectType][0].ObjectName;
			objectSelectedDescription.Text = gameObjectList[objectType][0].Description;
			gameObjectCamera.ChangeObject(gameObjectList[type][0]);

			foreach (Dictionary signal in utilizeBtn.GetSignalConnectionList("pressed"))
			{
				utilizeBtn.Disconnect("pressed", (Callable)signal["callable"]);
			}
			
			if (objectType == COLLECTABLE)
			{
				utilizeBtn.Icon = collectableButtonTexture;

				utilizeBtn.Pressed += OnOcchiettPremut;
				utilizeBtn.Visible = true;
			}
			else if (objectType == CONSUMABLE)
			{
				utilizeBtn.Icon = consumableButtonTexture;

				utilizeBtn.Pressed += OnConsumableConsumed;
				utilizeBtn.Visible = true;
			}

		}
		else
		{
			objectSelectedName.Text = "";
			objectSelectedDescription.Text = "";
			gameObjectCamera.StopAnimation();
			utilizeBtn.Visible = false;
		}

		for (; i < NUM_BTN; i++)
		{
			gameObjectButtons[i].Disabled = false;
			gameObjectButtons[i].Icon = null;
		}

	}

	public void RemoveObject(string name)
	{
		foreach (List<GameObject> list in gameObjectList)
        	list.RemoveAll(gameObject => gameObject.ObjectName == name);

	}
	public bool Contain(string gameObjectName)
	{

		foreach (List<GameObject> list in gameObjectList)
			foreach (GameObject gameObject in list)
				if (gameObject.ObjectName.Equals(gameObjectName))
					return true;

		return false;
	}

	public void SetScrollBar(int num)
    {
		VScrollBar scrollBar = this.collectableDescriptionLabel.GetVScrollBar();
		scrollBar.Value = num;
    }


	public void OnOcchiettPremut()
	{
		string description = (string)gameObjectList[COLLECTABLE][indObjectSelected].FullDescription;
		collectableDescriptionLabel.Text = description;
		animationPlayer.Play(ANIMATION_NAME);
	}

	public void OnConsumableConsumed()
	{

		if (gameObjectList[CONSUMABLE][indObjectSelected] is IConsumable consumable)
		{
			consumable.OnItemConsumed();
			gameObjectList[CONSUMABLE].RemoveAt(indObjectSelected);
			this.UploadGameObjects(CONSUMABLE);
		}
		else
		{
			throw new Exception("Questo consumabile non consuma");
		}
		
	}

    public void Remove()
    {
        GetParent().RemoveChild(this);
    }
}