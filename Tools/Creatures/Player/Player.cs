using Godot;
using System;
using System.Collections.Generic;

public partial class Player : Node3D
{

	[Export]
	public int TILE_SIZE = 2;
	[Export]
	public float walkSpeed = 2.0f;

	private List<string> directionKeys = new();

	AnimationTree animTree;
	AnimationNodeStateMachinePlayback animState;

	enum PlayerState
	{
		IDLE,
		TURNING,
		WALKING
	}

	enum FacingDirection
	{
		LEFT,
		RIGHT,
		UP,
		DOWN
	}

	PlayerState playerState = PlayerState.IDLE;
	FacingDirection facingDirection = FacingDirection.DOWN;

	Vector3 initalPosition = new(0, 0, 0);
	Vector2 inputDirection = new(0, 0);
	bool isMoving = false;
	float percentMovedToNextTile = 0.0f;

	public override void _Ready()
	{
		initalPosition = Position;
		animTree = GetNode<AnimationTree>("AnimationTree");
		animTree.Active = true;
		animState = (AnimationNodeStateMachinePlayback)animTree.Get("parameters/playback");

		animTree.Set("parameters/Idle/blend_position", inputDirection);
		animTree.Set("parameters/Walk/blend_position", inputDirection);
		animTree.Set("parameters/Turn/blend_position", inputDirection);
	}

	public override void _Process(double delta)
    {
        DirectionStorage();
	}

	private void DirectionStorage()
    {
        string[] directions = { "RIGHT", "LEFT", "DOWN", "UP" };

        foreach (string dir in directions)
        {
            if (Input.IsActionJustPressed(dir))
            {
                directionKeys.Add(dir);
            }
            else if (Input.IsActionJustReleased(dir))
            {
                directionKeys.Remove(dir);
            }
        }

        if (directionKeys.Count == 0)
        {
            directionKeys.Clear();
        }
    }

	public override void _PhysicsProcess(double delta)
	{

		if (playerState == PlayerState.TURNING)
		{
			return;
		}
		else if (!isMoving)
		{
			ProcessPlayerInput();
		}
		else if (inputDirection != Vector2.Zero)
		{
			animState.Travel("Walk");
			move(delta);
			playerState = PlayerState.WALKING;
		}
		else
		{
			animState.Travel("Idle");
			isMoving = false;
			playerState = PlayerState.IDLE;
		}
	}

	void ProcessPlayerInput()
	{
		Dictionary<string, Vector2> directionMap = new()

        {
            { "RIGHT", new Vector2(1, 0) },
            { "LEFT", new Vector2(-1, 0) },
            { "DOWN", new Vector2(0, 1) },
            { "UP", new Vector2(0, -1) }
        };

        if (directionKeys.Count > 0)
        {
            string key = directionKeys[^1];
            inputDirection = directionMap.ContainsKey(key) ? directionMap[key] : Vector2.Zero;
        }
        else
        {
            inputDirection = Vector2.Zero;
        }

		if (inputDirection != Vector2.Zero)
		{
			animTree.Set("parameters/Idle/blend_position", inputDirection);
			animTree.Set("parameters/Walk/blend_position", inputDirection);
			animTree.Set("parameters/Turn/blend_position", inputDirection);

			if (NeedToTurn())
			{
				playerState = PlayerState.TURNING;
				animState.Travel("Turn");
			}
			else
			{
				initalPosition = Position;
				isMoving = true;
				playerState = PlayerState.WALKING;
			}

		}
		else
		{
			animState.Travel("Idle");
			playerState = PlayerState.IDLE;
		}
	}

	bool NeedToTurn()
	{
		FacingDirection newFacingDirection = facingDirection;

		if (inputDirection.X < 0)
		{
			newFacingDirection = FacingDirection.LEFT;
		}
		else if (inputDirection.X > 0)
		{
			newFacingDirection = FacingDirection.RIGHT;
		}
		else if (inputDirection.Y > 0)
		{
			newFacingDirection = FacingDirection.UP;
		}
		else if (inputDirection.Y < 0)
		{
			newFacingDirection = FacingDirection.DOWN;
		}

		if (facingDirection != newFacingDirection)
		{
			facingDirection = newFacingDirection;
			return true;
		}
		else
		{
			facingDirection = newFacingDirection;
			return false;
		}
	}

	void FinishedTurning()
	{
		playerState = PlayerState.IDLE;
	}

	void move(double delta)
	{
		percentMovedToNextTile += walkSpeed * (float)delta;

		if (percentMovedToNextTile >= 1.0f)
		{
			Vector3 newPosition = new()
            {
				X = initalPosition.X + (TILE_SIZE * inputDirection.X),
				Y = Position.Y,
				Z = initalPosition.Z + (TILE_SIZE * inputDirection.Y)
			};
			Position = newPosition;
			percentMovedToNextTile = 0.0f;
			isMoving = false;
		}
		else
		{
			Vector3 newPosition = new()
            {
				X = initalPosition.X + (TILE_SIZE * inputDirection.X * percentMovedToNextTile),
				Y = Position.Y,
				Z = initalPosition.Z + (TILE_SIZE * inputDirection.Y * percentMovedToNextTile)
			};
			Position = newPosition;
		}
	}
}
