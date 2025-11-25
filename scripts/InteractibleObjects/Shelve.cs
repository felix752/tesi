using Godot;
using System;
using System.Collections.Generic;

public partial class Shelve : Node3D
{
	private const string END_ANIMATION_NAME = "End";
	private const string START_ANIMATION_NAME = "Start";
	private const string TAKE_BOOK_ANIMATION_NAME = "bookTakenAnimation";
	private const string PUZZLE_SOLVED_ANIMATION_NAME = "openLibrary";
	private const string SELECT_BOOK_ANIMATION_NAME = "bookSelectedAnimation";
	private const string INSERT_TAKEN_BOOK_ANIMATION_NAME = "bookTakenInsert";
	private const string INSERT_SELECTED_BOOK_ANIMATION_NAME = "bookSelectedInsert";
	private const int ROTATION_DEGREES_LIMIT = 60;


	[Export] private Player player;
	[Export] private DialogLabel label;
	[Export] private Sprite2D pointer;
	[Export] private Texture2D clickBook;
	[Export] private Texture2D normalPointer;
	[Export] private Node3D nodeBehindShelve;
	[Export] private Texture2D bookClickedPointer;
	[Export] private AnimationPlayer animationPlayer;
	[Export] private AudioStreamPlayer3D audio;


	private List<string> correctOrderBooks;
	private List<string> currentOrderBooks;
	private List<Node3D> booksNode;
	private Node3D firstBookSelected;
	private Node3D secondBookSelected;
	[Export] private AnimationTree animationTree;
	private AnimationNodeStateMachinePlayback stateMachine;
	private Vector3 firstBookPosition;
	private readonly Vector3 firstBooknewPosition = new(0.35f, 2.05f, 0.5f);
	private readonly Vector3 secondBooknewPosition = new(0.35f, 2.05f, -0.5f);

	[Signal] public delegate void TriggeredEventHandler();




	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		firstBookSelected = secondBookSelected = null;
		this.firstBookPosition = new Vector3(0, 0, 0);

		currentOrderBooks = new List<string> {
			"Sacrificio Celeste Vol II",  "Il Redentore velato",
			"Epifania del prescelto", "Apocalisse del falso profeta",
			"Sacrificio Celeste Vol I", "Matrice dell'origine universale"

		};

		correctOrderBooks = new List<string>{
		"Matrice dell'origine universale","Epifania del prescelto","Sacrificio Celeste Vol I",
		"Sacrificio Celeste Vol II","Il Redentore velato","Apocalisse del falso profeta"
		};

		booksNode = new List<Node3D>(6)  {
			this.GetChild<Node3D>(0),
			this.GetChild<Node3D>(1),
			this.GetChild<Node3D>(2),
			this.GetChild<Node3D>(3),
			this.GetChild<Node3D>(4),
			this.GetChild<Node3D>(5),
		};

		this.SetProcessUnhandledInput(false);
		this.SetProcess(false);
		stateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
	}

	public override void _Process(double delta)
	{

		try
		{
			Node node = player.GetNodeViewed().GetParent();

			if (node.Equals(nodeBehindShelve))
			{
				label.Text = "";
			}
			else
			{
				label.OutputText("\"" + node.Name + "\"");
			}
		}
		catch (NullReferenceException)
		{

			label.Text = "";
		}

	}


	public void OnBookSelected()
	{

		if (player.IsAnimationPlaying()) return;

		/*if (  player.IsCameraHeightLocked() && stateMachine.GetCurrentNode().ToString()== END_ANIMATION_NAME) {
			
			pointer.Texture = bookClickedPointer;

		} else*/
		if (player.IsCameraHeightLocked() && stateMachine.GetCurrentNode().ToString() == END_ANIMATION_NAME)
		{

			pointer.Texture = clickBook;

			if (firstBookSelected == null)
			{
				firstBookSelected = (Node3D)this.player.GetNodeViewed().GetParent();

				if (firstBookSelected.GetParent() != this)
				{
					firstBookSelected = null;
					return;
				}

				firstBookPosition = firstBookSelected.Position;

				SetBookPositionAnimation(TAKE_BOOK_ANIMATION_NAME, firstBookSelected.GetPath().ToString(), firstBookSelected.Position, firstBooknewPosition);

				stateMachine.Start(START_ANIMATION_NAME, false);
				stateMachine.Travel(END_ANIMATION_NAME, false);

			}
			else
			{

				secondBookSelected = (Node3D)this.player.GetNodeViewed().GetParent();
				if (secondBookSelected.GetParent() != this || secondBookSelected == firstBookSelected)
					return;

				SetBookPositionAnimation(SELECT_BOOK_ANIMATION_NAME, secondBookSelected.GetPath().ToString(), secondBookSelected.Position, secondBooknewPosition);
				SetBookPositionAnimation(INSERT_TAKEN_BOOK_ANIMATION_NAME, firstBookSelected.GetPath().ToString(), firstBooknewPosition, secondBookSelected.Position);
				SetBookPositionAnimation(INSERT_SELECTED_BOOK_ANIMATION_NAME, secondBookSelected.GetPath().ToString(), secondBooknewPosition, firstBookPosition);

				stateMachine.Start(TAKE_BOOK_ANIMATION_NAME, false);
				stateMachine.Travel(SELECT_BOOK_ANIMATION_NAME, false);


				int bookTakenInd = this.currentOrderBooks.IndexOf(firstBookSelected.Name);
				int bookSelectedInd = this.currentOrderBooks.IndexOf(secondBookSelected.Name);

				this.currentOrderBooks[bookTakenInd] = secondBookSelected.Name;
				this.currentOrderBooks[bookSelectedInd] = firstBookSelected.Name;

				firstBookSelected = null;


				for (int i = 0; i < currentOrderBooks.Count; i++)
					if (!currentOrderBooks[i].Equals(correctOrderBooks[i]))
						return;

				PuzzleSolved();
			}

		}
	}

	private void PuzzleSolved()
	{
		player.Stop();
		this.SetProcess(false);
		this.SetProcessUnhandledInput(false);
		this.GetParent().SetProcessInput(false);

		animationPlayer.Play(PUZZLE_SOLVED_ANIMATION_NAME);

		foreach (Node3D book in booksNode)
		{

			book.GetNode<StaticBody3D>("StaticBody3D").CollisionLayer = 4;
		}

		label.Visible = false;
		pointer.Texture = normalPointer;

		player.SetlockCameraHeight(false);
		player.SetPhysicsProcess(true);
		player.SetRotationLimit(Player.NO_ROTATION_LIMIT);

		audio.CallDeferred("play");
	}

	public void StartPuzzle()
	{
		float final_Y = player.GetHead().GlobalRotation.Y % MathF.PI;

		if (player.GetHead().GlobalRotation.Y < 0)
			final_Y = -MathF.PI;


		player.SetPhysicsProcess(false);

		player.SetRotationLimit(ROTATION_DEGREES_LIMIT);
		player.SetlockCameraHeight(true);

		player.CameraAnimation(2, new Vector3(0, final_Y, 0), new Vector3(9.95f, player.Position.Y, -2f));

		this.SetProcess(true);
		this.SetProcessUnhandledInput(true);
	}

	public void EscFromPuzzle()
	{
		int i = 0;
		bool areBooksOnShelve = true;

		foreach (Node3D book in booksNode)
		{

			if (book.Position.Z == firstBooknewPosition.Z || book.Position.Z == secondBooknewPosition.Z)
				areBooksOnShelve = false;
			i++;
		}

		this.SetProcess(false);
		label.Visible = false;


		if (!areBooksOnShelve)
		{
			animationPlayer.PlayBackwards(TAKE_BOOK_ANIMATION_NAME);
			firstBookSelected = null;
		}

		player.SetlockCameraHeight(false);
		player.SetPhysicsProcess(true);
		player.SetRotationLimit(Player.NO_ROTATION_LIMIT);
		this.GetParent<Library>().isPuzzleActived = false;

		this.SetProcessUnhandledInput(false);
	}


	private void SetBookPositionAnimation(string animationName, string bookPath, Vector3 initialPosition, Vector3 finalPosition)
	{
		Animation animation;
		int trackKeyPosition;

		animation = animationPlayer.GetAnimation(animationName);

		if (animation.GetTrackCount() > 0)
			trackKeyPosition = 0;
		else
			trackKeyPosition = animation.AddTrack(Animation.TrackType.Position3D);

		animation.TrackSetPath(trackKeyPosition, bookPath + ":position");
		animation.TrackInsertKey(trackKeyPosition, 0, initialPosition);
		animation.TrackInsertKey(trackKeyPosition, 0.5, finalPosition);
	}

	public bool IsAnimationPlaying()
	{

		string currentAnimationName = stateMachine.GetCurrentNode();

		if (currentAnimationName.Equals(END_ANIMATION_NAME) || currentAnimationName.Equals(START_ANIMATION_NAME)
										|| stateMachine.GetCurrentNode() == null)
		{

			return false;
		}
		return true;
	}
	
	public void OnEffectsValueChanged(float value)
	{
        audio.VolumeDb = value;
	}
}
