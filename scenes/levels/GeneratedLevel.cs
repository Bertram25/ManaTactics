using Godot;
using System;

public class GeneratedLevel : Spatial
{
	[Export]
	private PackedScene terrainBase;
	[Export]
	private PackedScene pineTree;
	
	private LevelCamera _camera;
	private Godot.Collections.Array<Character> _characters;
	private int _selectionIndex = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var terrainInstance = terrainBase.Instance();
		var pineTreeInstance = pineTree.Instance();
		_camera = GetNode<LevelCamera>("LevelCamera");
		_camera.Connect(nameof(LevelCamera.ReachedCharacter), this, "OnCharacterReached");

		_characters = new Godot.Collections.Array<Character>();
		
		CreateBaseTerrain(terrainInstance, pineTreeInstance);
		
		CreateCharacters();
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("ui_focus_next")) {
			_characters[_selectionIndex].SetIsControlledByPlayer(false);
			// Set camera to be the level camera again
			_camera.SetAsActiveCamera();
			
			_selectionIndex++;
			if (_selectionIndex >= _characters.Count) {
				_selectionIndex = 0;
			}
			_camera.MoveViewToTarget(_characters[_selectionIndex]);
		}
	}
	
	// Called when a character has been reached
	// and is ready to be either observed or controlled
	public void OnCharacterReached(Character character) {
		character.SetIsControlledByPlayer(true);
	}
	
	private void CreateBaseTerrain(Node terrainBlock, Node treePine) {
		for (var x = 0; x < 30; x++) {
			for (var z = 0; z < 30; z++) {
				var duplicate = terrainBlock.Duplicate() as RigidBody;
				duplicate.Name = $"newBlock{x}-{z}";
				duplicate.Translation = new Vector3(x, 0, z);
				duplicate.Mode = Godot.RigidBody.ModeEnum.Static;
				
				AddChild(duplicate);
				
				// Sometimes place a tree
				var chance = new Random().Next(1, 10);
				if (chance > 8) {
					var duplicateTree = treePine.Duplicate() as RigidBody;
					duplicateTree.Translation = new Vector3(x, 0, z);
					duplicateTree.Mode = Godot.RigidBody.ModeEnum.Static;
					AddChild(duplicateTree);
				}
			}
		}
	}
	
	private void CreateCharacters() {
		var character = GD.Load<PackedScene>("res://characters/Character.tscn").Instance();

		for (var number = 0; number < 30; ++number) {
			var duplicate = character.Duplicate() as Character;
			
			var characterLocation = new Vector3((float)GD.RandRange(0f, 30f), 0f, (float)GD.RandRange(0f, 30f));
			
			duplicate.Translation = characterLocation;
			duplicate.Name = $"Hero{number}";
			
			AddChild(duplicate);
			
			_characters.Add(duplicate);
		}
	}
}
