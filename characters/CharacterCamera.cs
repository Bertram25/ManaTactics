using Godot;
using System;

public class CharacterCamera : Camera
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}
	
	public void SetAsActiveCamera() {
		var formerCamera = GetViewport().GetCamera();
		this.GlobalTranslation = new Vector3(formerCamera.GlobalTranslation);
		this.GlobalRotation = new Vector3(formerCamera.GlobalRotation);
		this.Current = true;
		
		// Capture the mouse to permit handling rotation
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
}
