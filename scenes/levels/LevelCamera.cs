using Godot;
using System;

public class LevelCamera : Camera
{
	// Distance at which the camera stops
	private readonly float proximity = 7f;
	
	// Speed factor at which the camera rotates.
	private readonly float rotationSpeed = 6f;
	
	// Selected target, use for chasing and viewing processes in the level view
	private Spatial _selectedLookAtTarget;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	public override void _Process(float delta)
	{
		if (_selectedLookAtTarget != null) {
			ProcessCameraToLookAt(_selectedLookAtTarget, delta);    
		}
	}
	
	public void MoveViewToTarget(Spatial target) {
		_selectedLookAtTarget = target;
	}
	

	private void ProcessCameraToLookAt(Spatial target, float delta) {
		
		// Move camera close to character
		var targetPosition = target.Translation;
		targetPosition.y += 2.0f;
		
		var distance2 = targetPosition.DistanceSquaredTo(this.Translation);
		
		var nearDistanceFactor = Math.Max(0, distance2 - proximity) / distance2;
		var weight = delta * nearDistanceFactor;
		
		this.Translation = this.Translation.LinearInterpolate(targetPosition, weight);
		
		// Rotate camera towards character
		var directionCamera = new Spatial();
		directionCamera.Hide();
		directionCamera.LookAtFromPosition(this.GlobalTranslation, target.GlobalTranslation, new Vector3(0,1,0));
		var interpolatedDirection = this.Rotation.LinearInterpolate(directionCamera.Rotation, delta * rotationSpeed);
		this.Rotation = new Vector3(this.Rotation.x, interpolatedDirection.y, this.Rotation.z);
	}
}
