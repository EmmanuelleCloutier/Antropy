using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	public enum EnemyState
	{
		Wander,
		Attack
	}

	[Export] private float speed = 80f;
	[Export] private float detectionRange = 100f;

	private NavigationAgent2D navAgent;
	private Vector2 wanderDir;
	private float wanderTimer;

	private EnemyState state = EnemyState.Wander;
	private AnimatedSprite2D anim;

	public override void _Ready()
	{
		navAgent = GetNode<NavigationAgent2D>("NavAgent");
		anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		PickNewWander();
	}

	private void FlipSprite()
	{
		if (Velocity.X > 1) anim.FlipH = true;
		else if (Velocity.X < -1) anim.FlipH = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		switch (state)
		{
			case EnemyState.Wander:
				Wander(delta);
				break;
			case EnemyState.Attack:
				// Rien pour l'instant
				break;
		}

		MoveAndSlide();
		FlipSprite();
	}

	private void Wander(double delta)
	{
		wanderTimer -= (float)delta;
		if (wanderTimer <= 0)
			PickNewWander();

		Velocity = wanderDir * speed * 0.5f;
	}

	private void PickNewWander()
	{
		wanderTimer = (float)GD.RandRange(1.0f, 3.0f);
		wanderDir = new Vector2(
			(float)GD.RandRange(-1.0f, 1.0f),
			(float)GD.RandRange(-1.0f, 1.0f)
		).Normalized();
	}
}
