using Godot;
using System;

public class MainGameScene : Node
{
	private Node _mainMenu = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_OpenMainMenu();
	}
	
	private void _OpenMainMenu() {
		_mainMenu = GD.Load<PackedScene>("res://scenes/menus/MainMenu.tscn").Instance();
		GD.Print(_mainMenu);
		var startButton = _mainMenu.GetNode<Button>("VSplitContainer/StartButton");
		startButton.Connect("pressed", this, "_StartGame");
		AddChild(_mainMenu);		
	}
	
	private void _CloseMainMenu() {
		if (_mainMenu != null) {
			RemoveChild(_mainMenu);
		}
	}
	
	private void _StartGame() {
		_CloseMainMenu();
		
		var generatedLevel = GD.Load<PackedScene>("res://scenes/levels/GeneratedLevel.tscn").Instance();
		AddChild(generatedLevel);
	}
}
