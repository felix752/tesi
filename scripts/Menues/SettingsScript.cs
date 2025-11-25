using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SettingsScript : Node2D
{
    private const int FULL_SCREEN_BUTTON_IDX = 0;
    private const int LUMINOSITY_BUTTON_IDX = 1;
    private const int MUSIC_BUTTON_IDX = 2;
    private const int EFFECTS_BUTTON_IDX = 3;
    private const int ESC_BUTTON_IDX = 4;

    private const int LUMINOSITY_SLIDER_IDX = 0; 
    private const int MUSIC_SLIDER_IDX = 1;
    private const int EFFECTS_SLIDER_IDX = 2;

    private readonly Color normalButtonColor = new(255, 255, 255, 1);
    private readonly Color selectedButtonColor = new(255, 0, 0, 1);


    [Export] private Button[] buttonList;
    private int indSelected;
    [Export] private HSlider[] sliders;
    private List<Callable> hoveredMethods;


    private AudioStreamPlayer musicStreamPlayer;
    private WorldEnvironment worldEnvironment;

    public AudioStreamPlayer MusicStreamPlayer { set => musicStreamPlayer = value; }
    public WorldEnvironment WorldEnvironment { set => worldEnvironment = value; }

    [Signal] public delegate void EffectsValueChangedEventHandler(float value);
    [Signal] public delegate void GoToMenuEventHandler();


    public override void _Ready()
    {

        indSelected = FULL_SCREEN_BUTTON_IDX;
        buttonList[FULL_SCREEN_BUTTON_IDX].Modulate = selectedButtonColor;
        buttonList[LUMINOSITY_BUTTON_IDX].Modulate = normalButtonColor;
        buttonList[MUSIC_BUTTON_IDX].Modulate = normalButtonColor;
        buttonList[EFFECTS_BUTTON_IDX].Modulate = normalButtonColor;
        buttonList[ESC_BUTTON_IDX].Modulate = normalButtonColor;

        hoveredMethods = new()
        {
            new Callable(this, nameof(this.OnFullScreenHovered)),
            new Callable(this, nameof(this.OnLuminosityButtonHovered)),
            new Callable(this, nameof(this.OnMusicButtonHovered)),
            new Callable(this, nameof(this.OnEffectsButtonHovered)),
            new Callable(this, nameof(this.OnGoBackHovered))
        };

    }

    public void OnFullScreenSelected()
    {
		if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Fullscreen)
		{

			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
		}
		else
		{
			DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
		}
    }



    public void OnMusicValueChanged(float value)
    {
        this.OnMusicButtonHovered();

        musicStreamPlayer.VolumeDb = value * 64 / 100 - 32; //da -32 a 32. Base 0

        if (value==0)
        {
            musicStreamPlayer.VolumeDb = -80;
        }
    }

    public void OnEffectsValueChanged(float value)//todo dacci un'occhiata
    { 
        this.OnEffectsButtonHovered();
        value = value * 64 / 100 - 32;

        this.EmitSignal(SignalName.EffectsValueChanged, value);//da -53 a 10
    }

    public void OnLuminosityValueChanged(float value)
    {
        this.OnLuminosityButtonHovered();
        worldEnvironment.Environment.AmbientLightEnergy = 0.1f + value / 100;
    }


    public void OnGoToMenuPressed()
    {
        this.EmitSignal(SignalName.GoToMenu);
    }

    public void OnFullScreenHovered()
    {
        indSelected = FULL_SCREEN_BUTTON_IDX;

        ColorButtons(indSelected, normalButtonColor);
        buttonList[indSelected].Modulate = selectedButtonColor;

        ColorSliders(normalButtonColor);
    }
    public void OnMusicButtonHovered()
    {
        indSelected = MUSIC_BUTTON_IDX;

        ColorButtons(indSelected, normalButtonColor);
        buttonList[indSelected].Modulate = selectedButtonColor;

        ColorSliders(normalButtonColor);
        sliders[MUSIC_SLIDER_IDX].Modulate = selectedButtonColor;
    }
    public void OnLuminosityButtonHovered()
    {
        indSelected = LUMINOSITY_BUTTON_IDX;

        ColorButtons(indSelected, normalButtonColor);
        buttonList[indSelected].Modulate = selectedButtonColor;

        ColorSliders(normalButtonColor);
        sliders[LUMINOSITY_SLIDER_IDX].Modulate = selectedButtonColor;
    }

    public void OnEffectsButtonHovered()
    {
        indSelected = EFFECTS_BUTTON_IDX;

        ColorButtons(indSelected, normalButtonColor);
        buttonList[indSelected].Modulate = selectedButtonColor;

        ColorSliders(normalButtonColor);
        sliders[EFFECTS_SLIDER_IDX].Modulate = selectedButtonColor;
    }
    public void OnGoBackHovered()
    {
        indSelected = ESC_BUTTON_IDX;

        ColorButtons(indSelected, normalButtonColor);
        buttonList[indSelected].Modulate = selectedButtonColor;

        ColorSliders(normalButtonColor);
    }



    public void ColorButtons(int i, Color color)
    {

        for (int ind = 0; ind < buttonList.Length; ind++)
            buttonList[ind].Modulate = color;
    }

    public void ColorSliders( Color color)
    {
            
        for (int ind = 0; ind < sliders.Length; ind++)
            sliders[ind].Modulate = color;

    }




    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed(Main.ACTION_NAME_SELECT_UP))
        {

            if (indSelected > 0)  {
                hoveredMethods[--indSelected].Call();
            }
        }
        else if (@event.IsActionPressed(Main.ACTION_NAME_SELECT_DOWN))
        {

            if (indSelected < buttonList.Length - 1) {
                hoveredMethods[++indSelected].Call();
            }
        }
        else if (@event.IsActionReleased(Main.ACTION_NAME_SELECT))
        {
            GetViewport().SetInputAsHandled();

            if (indSelected == FULL_SCREEN_BUTTON_IDX)
                OnFullScreenSelected();

            else if (indSelected == ESC_BUTTON_IDX)
                this.OnGoToMenuPressed();

        }
        else
        {

            float leftEvent = @event.GetActionStrength(Main.ACTION_NAME_GO_LEFT);
            float rightEvent = @event.GetActionStrength(Main.ACTION_NAME_GO_RIGHT);

            float delta = (rightEvent - leftEvent) * 10;

            if (indSelected == MUSIC_BUTTON_IDX)
                sliders[MUSIC_SLIDER_IDX].Value += delta;

            else if (indSelected == EFFECTS_BUTTON_IDX)
                sliders[EFFECTS_SLIDER_IDX].Value += delta;

            else if (indSelected == LUMINOSITY_BUTTON_IDX)
                sliders[LUMINOSITY_SLIDER_IDX].Value += delta;

        }

    }

}