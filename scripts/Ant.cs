using Godot;
using System;

public partial class Ant : CharacterBody2D
{
	public enum AntState
	{
		Explore,
		ChercheNourriture,
		Transporte,
		RetourAuNid,
		Defense,
		Mort
	}

	[Export] private float speed = 80f;

	private AnimatedSprite2D anim;

	private AntState currentState = AntState.Explore;

	private Vector2 wanderDirection;
	private float wanderTimer = 0f;

	public override void _Ready()
	{
		anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		PickNewWanderDirection();
	}

	public override void _PhysicsProcess(double delta)
	{
		ExecuteState(delta);
		UpdateSpriteFlip();
	}

	private void ExecuteState(double delta)
	{
		switch (currentState)
		{
			case AntState.Explore:
				Wander(delta);
				break;

			case AntState.Mort:
				Die();
				break;

			// Les autres états viendront plus tard
		}
	}

	// -----------------------------
	// RANDOM WANDER
	// -----------------------------
	private void Wander(double delta)
	{
		wanderTimer -= (float)delta;

		if (wanderTimer <= 0f)
			PickNewWanderDirection();

		Velocity = wanderDirection * speed;
		MoveAndSlide();

		if (!anim.IsPlaying())
			anim.Play("walk");
	}


	private void PickNewWanderDirection()
	{
		wanderTimer = (float)GD.RandRange(1f, 2f);

		wanderDirection = new Vector2(
			(float)GD.RandRange(-1f, 1f),
			(float)GD.RandRange(-1f, 1f)
		).Normalized();
	}


	// -----------------------------
	// FLIP DU SPRITE
	// -----------------------------
	private void UpdateSpriteFlip()
	{
		if (Velocity.X > 1f)
			anim.FlipH = true;   // droite

		else if (Velocity.X < -1f)
			anim.FlipH = false;  // gauche
	}

	private void Die()
	{
		QueueFree();
	}
}
