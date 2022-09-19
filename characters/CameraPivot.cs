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

			// Handle mouse X axis to circle left and right
			float mouseMovement = RotationDegrees.y - (mouseEvent.Relative.x * _mouseSensitivity);
			mouseMovement = Mathf.Wrap(mouseMovement, 0f, 360f);
			//RotationDegrees.y = mouseMovement;
			RotationDegrees = new Vector3(RotationDegrees.x, mouseMovement, RotationDegrees.z);

			// Handle mouse Y axis to circle up and down
			mouseMovement = RotationDegrees.x - (mouseEvent.Relative.y * _mouseSensitivity);
			mouseMovement = Mathf.Clamp(mouseMovement, -8f, 20f);
			//RotationDegrees.x = mouseMovement;
			RotationDegrees = new Vector3(mouseMovement, RotationDegrees.y, RotationDegrees.z);
		}
	}
}
