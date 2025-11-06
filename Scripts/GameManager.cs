using Godot;
using System;

public partial class GameManager : Node
{
    [Export] public byte scoreLimit = 5;
    private byte playerScore1 = 0;
    private byte playerScore2 = 0;
    private int servingPLayer;
    private Label scoreLabel;
    private bool gamePaused = false;
    private bool gameOver = false;
    Timer resetTimer;
    PackedScene ballScene;
    Node2D spawnLeft;
    Node2D spawnRight;


    public override void _Ready()
    {
        scoreLabel = GetNode<Label>("CanvasLayer/UI/ScoreLabel");
        UpdateScoreLabels();

        resetTimer = GetNode<Timer>("ResetTimer");
        resetTimer.Timeout += OnResetTimeout;

        ballScene = GD.Load<PackedScene>("res://Scenes/ball.tscn");
        spawnLeft = GetNode<Node2D>("SpawnLeft");
        spawnRight = GetNode<Node2D>("SpawnRight");

        Engine.TimeScale = 1;
    }

    public override void _Process(double delta)
    {
        if(Input.IsActionJustPressed("pause") && !gameOver)
        {
            if(!gamePaused)
            {
                gamePaused = true;
                Pause();
            }
            else
            {
                gamePaused = false;
                Unpause();
            }
        }
    }



    public void AddPoint(sbyte playerNumber)
    {
        if (playerNumber == 1)
        {
            playerScore1++;
        }
        else if (playerNumber == 2)
        {
            playerScore2++;
        }
        else
        {
            GD.PrintErr($"Player number: {playerNumber} was not found.");
        }
        UpdateScoreLabels();
    }

    private void UpdateScoreLabels()
    {
        scoreLabel.Text = $"{playerScore1} - {playerScore2}";
    }

    public void StartReset(sbyte scoringPlayerNumber)
    {
        //set serving player to be the one that didn't score
        servingPLayer = scoringPlayerNumber == 1 ? 2 : 1;

        Engine.TimeScale = .1;
        resetTimer.Start();
    }

    private void OnResetTimeout()
    {
        Engine.TimeScale = 1;
        if(playerScore1 < scoreLimit && playerScore2 < scoreLimit)
        {
            SpawnBall();
        }
        else
        {
            EndGame();
        }
    }



    private void SpawnBall()
    {
        Ball ball = ballScene.Instantiate<Ball>();
        AddChild(ball);

        if (servingPLayer == 1)
        {
            ball.Position = spawnLeft.Position;
        }
        else
        {
            ball.Position = spawnRight.Position;
        }

        ball.LinearVelocity = Vector2.Zero; // Stay still until served
    }

    private void EndGame()
    {
        gameOver = true;
        GD.Print("Game over!");
        Engine.TimeScale = 0;

        var winningPlayer = playerScore1 > playerScore2 ? "Player 1" : "Player 2";
        Label gameOverLabel = GetNode<Label>("CanvasLayer/UI/GameOverLabel");
        gameOverLabel.Text = $"{winningPlayer} Wins!";
        gameOverLabel.Visible = true;

        Button restartButton = GetNode<Button>("CanvasLayer/UI/RestartButton");
        restartButton.GrabFocus();
        restartButton.Visible = true;

        Button quitButton = GetNode<Button>("CanvasLayer/UI/MainMenuButton");
        quitButton.Visible = true;
    }

    public void OnRestartPressed()
    {
        Engine.TimeScale = 1;
        GetTree().ReloadCurrentScene();
    }

    public void OnMainMenuPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/main_menu.tscn");
    }

    public void Pause()
    {
        Engine.TimeScale = 0;

        Button restartButton = GetNode<Button>("CanvasLayer/UI/RestartButton");
        restartButton.GrabFocus();
        restartButton.Visible = true;

        Button quitButton = GetNode<Button>("CanvasLayer/UI/MainMenuButton");
        quitButton.Visible = true;
    }
    public void Unpause()
    {
        Engine.TimeScale = 1;

        GetNode<Button>("CanvasLayer/UI/RestartButton").Visible = false;
        GetNode<Button>("CanvasLayer/UI/MainMenuButton").Visible = false;
    }
}
