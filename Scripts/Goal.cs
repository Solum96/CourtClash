using Godot;
using System;

public partial class Goal : Area2D
{
    [Export] public sbyte scoringPlayerNumber = 1;
    private GameManager gameManager;
    public override void _Ready()
    {
        gameManager = GetNode<GameManager>("%GameManager");
    }

    public void OnBodyEntered(Node2D body)
    {
        if (body is Ball ball)
        {
            //Add a point
            gameManager.AddPoint(scoringPlayerNumber);

            //Deactivate the ball
            ball.ExplodeAndDespawn();

            gameManager.StartReset(scoringPlayerNumber);
        }
    }
}
