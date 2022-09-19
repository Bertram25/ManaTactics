using Godot;
using System;

public class CameraPivot : SpringArm
{
	private readonly float _mouseSensitivity = 0.05f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Avoid being transformed by parent nodes
		SetAsToplevel(true);
		
		// Capture the mouse to permit rotation
		//Input.SetMouseMode(Input.MouseModeEnum.Captured);
	}
/*
	public override void _UnhandledInput(InputEvent inputEvent) {
		if (inputEvent is InputEventMouseMotion mouseEvent) {

			// Handle mouse X axis to circle left and right
			float mouseMovement = RotationDegrees.y - (mouseEvent.Relative.x * _mouseSensitivity);
			mouseMovement = Mathf.Clamp(mouseMovement, 0f, 360f);
			RotationDegrees.y = mouveMovement;

			// Handle mouse Y axis to circle up and down
			mouseMovement = RotationDegrees.x - (mouseEvent.Relative.y * _mouseSensitivity);
			mouseMovement = Mathf.Wrapf(mouseMovement, -90f, 30f);
			RotationDegrees.x = mouseMovement;
		}
	}*/
}
