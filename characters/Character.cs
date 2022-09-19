using Godot;
using System;

public class Character : KinematicBody
{
	private readonly float _speed = 3.0f;
	private readonly float _jumpStrength = 10.0f;
	private readonly float _gravity = 50.0f;
	
	public static class Animations {
		public static string IdleDown = "idle_down";
		public static string WalkDown = "walk_down";
		public static string WalkLeft = "walk_left";
		public static string WalkRight = "walk_up";
		public static string WalkUp = "walk_right";
	};
	
	private Vector3 _velocity = Vector3.Zero;
	private Vector3 _snapVector = Vector3.Down;

	private Spatial _facingDirection;	
	private SpringArm _springArm;
	private AnimationPlayer _animation;
		
	private bool _isPlayer = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_facingDirection = GetNode<Spatial>("FacingDirection");
		_springArm = GetNode<SpringArm>("CameraPivot");
		_animation = GetNode<AnimationPlayer>("Sprite3D/AnimationPlayer");
		
		// Animate the character
		_animation.Play(Animations.WalkDown);
	}
	
	public override void _PhysicsProcess(float delta)
	{
		if (_isPlayer) {
			_PhysicsProcessPlayerControl(delta);
		}
	}
	
	public override void _Process(float delta) {
		_springArm.Translation = Translation;
	}
	
	public void SetIsControlledByPlayer(bool isPlayer) {
		_isPlayer = isPlayer;
	}
	
	private void _PhysicsProcessPlayerControl(float delta) {
		// Get input movement direction strength
		var moveDirection = Vector3.Zero;
		moveDirection.x = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
		moveDirection.z = Input.GetActionStrength("ui_down") - Input.GetActionStrength("ui_up");
		
		// Rotate movement by current spring arm direction to make the movement fit the view
		var activeCamera = GetViewport().GetCamera();
		var rotationY = activeCamera.Rotation.y;
		// FOR NOW: Follow the active camera rotation
		//rotationY = _springArm.Rotation.y
		moveDirection = moveDirection.Rotated(Vector3.Up, rotationY).Normalized();
		
		// Actually move the character now
		_velocity.x = moveDirection.x * _speed;
		_velocity.z = moveDirection.z * _speed;
		
		// Apply gravity
		_velocity.y -= _gravity * delta;
		bool justLanded = this.IsOnFloor() && _snapVector == Vector3.Zero;
		// Let it jump
		bool isJumping = this.IsOnFloor() && Input.IsActionJustPressed("ui_select");
		if (isJumping) {
			_velocity.y = _jumpStrength;
			_snapVector = Vector3.Zero;
		}
		else if (justLanded) {
			_snapVector =  Vector3.Down;
		}
		
		_velocity = MoveAndSlideWithSnap(_velocity, _snapVector, Vector3.Up, true);
		
		// Prevent the character from facing forward when stopping movement
		if (_velocity.Length() > 0.2) {
			// Get opposite direction
			var lookDirection = new Vector2(_velocity.z, _velocity.x);
			// Set the character facing direction using a spatial node
			_facingDirection.Rotation = new Vector3(
				_facingDirection.Rotation.x,
				lookDirection.Angle(),
				_facingDirection.Rotation.z);
			// TODO: Update animation based on facing direction and active camera view
		}
		
	}
}
