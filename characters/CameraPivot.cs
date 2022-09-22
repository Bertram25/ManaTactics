using Godot;
using System;

public class CameraPivot : Spatial
{
	private readonly float _mouseSensitivity = 0.05f;
	
	private CharacterCamera _camera;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Avoid being transformed by parent nodes
		SetAsToplevel(true);
		
		_camera = GetNode<CharacterCamera>("Camera");
	}
	
		public override void _UnhandledInput(InputEvent inputEvent) {
		// Process this only when active
		if (_camera.Current == false) {
			return;
		}
		
		// Note: The camera pivot must be aligned with the camera once for this to work
		// This is done in the character camera initialization
		if (inputEvent is InputEventMouseMotion mouseEvent) {
			
			// Handle mouse Y axis to circle up and down
			float mouseMovementX = RotationDegrees.x - (mouseEvent.Relative.y * _mouseSensitivity);
			mouseMovementX = Mathf.Clamp(mouseMovementX, -8f, 20f);

			// Handle mouse X axis to circle left and right
			float mouseMovementY = RotationDegrees.y - (mouseEvent.Relative.x * _mouseSensitivity);
			mouseMovementY = Mathf.Wrap(mouseMovementY, 0f, 360f);

			RotationDegrees = new Vector3(mouseMovementX, mouseMovementY, RotationDegrees.z);
		}
	}
}
