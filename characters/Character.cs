using Godot;
using System;

public class Character : KinematicBody
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Animate the character
		var animation = GetNode<AnimationPlayer>("Sprite3D/AnimationPlayer");
		animation.Play("walk_down");
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
