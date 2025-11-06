using Godot;
using System;

public partial class MainMenu : Node2D
{
    public override void _Ready()
    {
        GetNode<Button>("CanvasLayer/MainUI/VBoxContainer/PlayButton").Pressed += OnPlayPressed;
        GetNode<Button>("CanvasLayer/MainUI/VBoxContainer/QuitButton").Pressed += OnQuitPressed;

        // Set focus to first button
        GetNode<Button>("CanvasLayer/MainUI/VBoxContainer/PlayButton").GrabFocus();
    }

    private void OnPlayPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/game.tscn");
    }

    private void OnQuitPressed()
    {
        GetTree().Quit();
    }
}
