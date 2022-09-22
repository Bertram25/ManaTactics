using Godot;
using System;

public class LevelCamera : Camera
{
	[Signal]
	public delegate void ReachedCharacter(Character character);
	
	// Distance at which the camera stops
	private readonly float _proximity = 5f;
	private readonly float _cameraElevation = 3f;
	private readonly float _cameraTargetAngle = -45f;
	private readonly float _tolerance = 0.75f;
	private readonly float _maxTimeFlyingWithoutGuard = 3f;
	
	// Speed factor at which the camera rotates.
	private readonly float rotationSpeed = 6f;
	
	// Selected target, use for chasing and viewing processes in the level view
	private Spatial _selectedLookAtTarget;
	// Node used to ease the computation to look at target
	private Spatial _directionCamera;
	// Fools proof time to unstuck flying when go from a close object to another one.
	private float _timeFlying;

	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_directionCamera = new Spatial();
		_directionCamera.Hide();
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
		
		// Fix the end rotation so that the camera looks 45° down is looking horizontally
		_directionCamera.LookAtFromPosition(this.GlobalTranslation, target.GlobalTranslation, Vector3.Up);
		_directionCamera.RotationDegrees = new Vector3(
			_cameraTargetAngle,
			_directionCamera.RotationDegrees.y,
			_directionCamera.RotationDegrees.z
			);

		_timeFlying = 0.0f;
	}
	
	private void ProcessCameraToLookAt(Spatial target, float delta) {
		
		// Move camera close to character
		var targetPosition = target.GlobalTranslation;
		targetPosition.y += _cameraElevation;
		
		var distance2 = targetPosition.DistanceSquaredTo(this.GlobalTranslation);
		
		var nearDistanceFactor = Math.Max(0, distance2 - _proximity) / distance2;
		var weight = delta * nearDistanceFactor;
		
		this.GlobalTranslation = this.GlobalTranslation.LinearInterpolate(targetPosition, weight);
		
		// Rotate camera towards character
		var interpolatedRotation = this.Rotation.LinearInterpolate(_directionCamera.Rotation, delta * rotationSpeed);
		var interpolationY = this.RotationDegrees.y > _cameraTargetAngle ? interpolatedRotation.x : this.Rotation.x;
		this.Rotation = new Vector3(interpolationY, interpolatedRotation.y, this.Rotation.z);

		// Guard the time spent flying to unstuck camera in corner cases
		_timeFlying += delta;

		_EmitReachCharacterSignalIfPossible(target, _directionCamera.RotationDegrees);
	}
	
	
	// Emit reach signal when the camera is place with the right angle and sufficient elevation
	private void _EmitReachCharacterSignalIfPossible(Spatial target, Vector3 desiredRotationDegrees) {
		if (this != GetViewport().GetCamera()) {
			return;
		}

		if (this.RotationDegrees.x > _cameraTargetAngle * _tolerance) {
			if (_timeFlying > _maxTimeFlyingWithoutGuard) {
				this.RotationDegrees = new Vector3(
					_cameraTargetAngle * _tolerance,
					this.RotationDegrees.y,
					this.RotationDegrees.z
				);
			}
			return;
		}
		
		if (this.GlobalTranslation.y - target.GlobalTranslation.y < _cameraElevation * _tolerance) {
			if (_timeFlying > _maxTimeFlyingWithoutGuard) {
				this.GlobalTranslation = new Vector3(
					this.GlobalTranslation.x,
					target.GlobalTranslation.y + (_cameraElevation * _tolerance),
					this.GlobalTranslation.z
				);
			}
			return;
		}
		
		if ((this.GlobalTranslation - target.GlobalTranslation).Length() > _proximity) {
			return;
		}

		if (target is Character character) {
			// Tell the level we reached the character
			EmitSignal(nameof(ReachedCharacter), character);
		}
	}
}
