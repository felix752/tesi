using System.Collections.Generic;
using Godot;

public partial class Clock : Node3D
{
	private const string HOURS_ANIMATION_NAME = "hourAnimation";
	private const string MINUTES_ANIMATION_NAME = "minuteAnimation";
	private const string SECONDS_ANIMATION_NAME = "secondAnimation";
	private const string PENDOLUM_ANIMATION_NAME = "pendolumAnimation";
	[Export] AnimationPlayer hourAnimationPlayer;
	[Export] AnimationPlayer minAnimationPlayer;
	[Export] AnimationPlayer secondAnimationPlayer;
	[Export] AnimationPlayer pendolumAnimationPlayer;
	[Export] DialogLabel dialogLabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var timeDict = Time.GetDatetimeDictFromSystem();

		int hour = (int)timeDict["hour"];
		if (hour >= 12)
		{
			hour -= 12;
		}
		int minute = (int)timeDict["minute"];
		int second = (int)timeDict["second"];

		int time = second;

		secondAnimationPlayer.Play(SECONDS_ANIMATION_NAME);
		secondAnimationPlayer.Seek(time, true);

		time += minute * 60;

		minAnimationPlayer.Play(MINUTES_ANIMATION_NAME);
		minAnimationPlayer.Seek(time, true);

		time += hour * 3600;

		hourAnimationPlayer.Play(HOURS_ANIMATION_NAME);
		hourAnimationPlayer.Seek(time, true);

		pendolumAnimationPlayer.Play(PENDOLUM_ANIMATION_NAME);
	}

	public void ReadTime()
	{
		var timeDict = Time.GetDatetimeDictFromSystem();

		int hour = (int)timeDict["hour"];
		if (hour >= 12)
		{
			hour -= 12;
		}
		int minute = (int)timeDict["minute"];

		dialogLabel.OutputText($"{hour:00}:{minute:00}");
	}

}
