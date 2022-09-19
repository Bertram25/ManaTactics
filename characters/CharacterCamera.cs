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
		
		// Reset the spring arm rotation offset based on the camera
		// so that it is facing the right angle at start
		var cameraPivot = GetParent<Spatial>();
		cameraPivot.GlobalRotation = new Vector3(
			cameraPivot.GlobalRotation.x,
			formerCamera.GlobalRotation.y,
			cameraPivot.GlobalRotation.z);
		
		// Setup the camera position and rotation to be the same than the previous camera
		this.GlobalTranslation = new Vector3(formerCamera.GlobalTranslation);
		this.GlobalRotation = new Vector3(formerCamera.GlobalRotation);
		
		// Capture the mouse to permit handling rotation
		Input.MouseMode = Input.MouseModeEnum.Captured;
		
		// Activate the camera
		this.Current = true;
	}
}
