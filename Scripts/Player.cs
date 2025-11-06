using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] float speed = 150f;
	[Export] float baseHitForce = 120f;
	[Export] sbyte playerId = 1;
	[Export] int deviceId = 0;
    [Export] float sweetSpotMultiplier = 2f;
	[Export] float sweetSpotRadius = 6.5f;

	//Movement variables
	string left = "move_left";
	string right = "move_right";
	string up = "move_up";
	string down = "move_down";
	string swing = "swing";	

	//Node references
	private AnimatedSprite2D _sprite;
	private AnimationPlayer _animationPlayer;
	private Marker2D sweetSpotMarker;
	private bool isSwinging = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		sweetSpotMarker = GetNode<Marker2D>("Weapon/SweetSpot");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		left = $"move_left_p{playerId}";
		right = $"move_right_p{playerId}";
		up = $"move_up_p{playerId}";
		down = $"move_down_p{playerId}";
		swing = $"swing_p{playerId}";

		//Get the input direction
		Vector2 direction = Input.GetVector(left, right, up, down);
		Vector2 velocity = Velocity;

		//Swing weapon
		if(Input.IsActionJustPressed(swing)){
			Swing();
		}

		if(!isSwinging)
		{
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
		}

		//Apply movement
		if(direction != Vector2.Zero && !isSwinging)
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

	public void Swing()
	{
		if (!isSwinging)
		{
			isSwinging = true;
			_sprite.FlipH = playerId != 1; //TODO: Change this for multiplayer and eventual 2v2
			_animationPlayer.Play("swing");
		}
	}

	private void OnSwingAnimationFinished(string animName)
	{
		if (animName == "swing")
			isSwinging = false;
	}

	public void OnHitBoxBodyEntered(Node2D body)
	{
		if(body is Ball ball)
		{
			//Calculate hit force
			var hitForce = baseHitForce * (IsSweetSpotHit(ball) ? sweetSpotMultiplier: 1.0f);

			//Calculate direction
			Vector2 hitDir = (ball.GlobalPosition - GlobalPosition).Normalized();
			var playDirection = playerId != 1 ? Vector2.Left : Vector2.Right;
			var angle = playDirection.AngleTo(hitDir);

			//Apply force in correct direction
			if(Mathf.Abs(angle) < Mathf.Pi / 2f)
			{
				ball.LinearVelocity = Vector2.Zero;
				ball.ApplyImpulse(hitDir * hitForce, Vector2.Zero);
				ball.PlayHitEffects(IsSweetSpotHit(ball));
			}
			else
			{
				GD.Print("miss");
			}

			GD.Print(IsSweetSpotHit(ball) ? "Sweet spot hit!" : "Normal hit.");
		}
	}

    private bool IsSweetSpotHit(Ball ball)
    {
    	float distance = sweetSpotMarker.GlobalPosition.DistanceTo(ball.GlobalPosition);
    	return distance <= sweetSpotRadius;
    }

}
