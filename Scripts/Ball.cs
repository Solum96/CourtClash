using Godot;
using System;

public partial class Ball : RigidBody2D
{
    [Export] public float speed = 300f;
	private AnimatedSprite2D sprite;
	private float spriteStartingRotation;
	private Timer slowdownTimer;
	private bool isExploding = false;

    public override void _Ready()
    {
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		slowdownTimer = GetNode<Timer>("Timer");
		spriteStartingRotation = sprite.Rotation;
        // Launch the ball to the left at the start
        LinearVelocity = new Vector2(GD.RandRange(-1, 1), (float)GD.RandRange(-0.5d, 0.5d)).Normalized() * speed;
    }

	public override void _PhysicsProcess(double delta)
	{
		// Rotate the sprite in direction of movement
		if(!isExploding)
		{
			if (LinearVelocity.Length() > 10f)
			{
				sprite.Play("flying");
				sprite.Rotation = LinearVelocity.Angle() + spriteStartingRotation;
			}
			else
			{
				sprite.Play("idle");
			}
		}
	}

	public void PlayHitEffects(bool IsSweetSpotHit = false)
	{
		if (IsSweetSpotHit)
		{
			GetNode<AudioStreamPlayer2D>("Sounds/SweetSpotSound").Play();
			Engine.TimeScale = .1;
		}
		GetNode<AudioStreamPlayer2D>("Sounds/FlameSound").Play();
		GetNode<AudioStreamPlayer2D>("Sounds/ThudSound").Play();
		slowdownTimer.Start();
	}

	public void OnTimerTimeout()
	{
		Engine.TimeScale = 1f;
	}

    public void ExplodeAndDespawn()
    {
		isExploding = true;
		LinearVelocity = Vector2.Zero;
        GetNode<AnimationPlayer>("AnimationPlayer").Play("explode");
    }

}
