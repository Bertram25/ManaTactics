using Godot;
using System;

public class LevelCamera : Camera
{
	[Signal]
	public delegate void ReachedCharacter(Character character);
	
	// Distance at which the camera stops
	private readonly float _proximity = 5f;
	private readonly float _cameraElevation = 2f;
	
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
	
	public void SetAsActiveCamera() {
		var formerCamera = GetViewport().GetCamera();
		this.GlobalTranslation = new Vector3(formerCamera.GlobalTranslation);
		this.GlobalRotation = new Vector3(formerCamera.GlobalRotation);
		this.Current = true;
	}
	
	public void MoveViewToTarget(Spatial target) {
		_selectedLookAtTarget = target;
	}
	

	private void ProcessCameraToLookAt(Spatial target, float delta) {
		
		// Move camera close to character
		var targetPosition = target.Translation;
		targetPosition.y += _cameraElevation;
		
		var distance2 = targetPosition.DistanceSquaredTo(this.Translation);
		
		var nearDistanceFactor = Math.Max(0, distance2 - _proximity) / distance2;
		var weight = delta * nearDistanceFactor;
		
		this.Translation = this.Translation.LinearInterpolate(targetPosition, weight);
		
		// Rotate camera towards character
		var directionCamera = new Spatial();
		directionCamera.Hide();
		directionCamera.LookAtFromPosition(this.GlobalTranslation, target.GlobalTranslation, Vector3.Up);
		// Fix the end rotation so that the camera looks 35° down is looking horizontally
		directionCamera.RotationDegrees = new Vector3(
			-35f,
			directionCamera.RotationDegrees.y,
			directionCamera.RotationDegrees.z);

		var interpolatedDirection = this.Rotation.LinearInterpolate(directionCamera.Rotation, delta * rotationSpeed);
		this.Rotation = new Vector3(this.Rotation.x, interpolatedDirection.y, this.Rotation.z);
		
		// TODO: The signal should be emitted when the camera has the right angle and right elevation
		if (GetViewport().GetCamera() == this && target is Character character) {
			if ((this.GlobalTranslation - target.GlobalTranslation).Length() < _proximity) {
				// Tell the level we reached the character
				EmitSignal(nameof(ReachedCharacter), character);
			}
		}
	}
}
