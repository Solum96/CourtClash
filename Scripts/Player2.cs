using Godot;
using System;

public partial class Player2 : CharacterBody2D
{
	public const float speed = 200f;
	private AnimatedSprite2D _sprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		//Get the input direction
		Vector2 direction = Input.GetVector("p2_move_left", "p2_move_right", "p2_move_up", "p2_move_down");
		Vector2 velocity = Velocity;

		//Flip the sprite
		if(direction.X > 0)
		{
			_sprite.FlipH = false;
		}
		else if(direction.X < 0)
		{
			_sprite.FlipH = true;
		}

		//Play animations
		if(direction == Vector2.Zero)
		{
			_sprite.Play("idle");
		}
		else
		{
			_sprite.Play("run");
		}

		//Apply movement
		if(direction != Vector2.Zero)
		{
			velocity = direction.Normalized() * speed;
		}
		else
		{
			velocity = Vector2.Zero;
		}
		Velocity = velocity;
		MoveAndSlide();
	}
}
