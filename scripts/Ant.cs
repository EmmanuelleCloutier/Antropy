using Godot;
using System;

public partial class Ant : CharacterBody2D
{
	public enum AntState
	{
		Wander,
		ChercheNourriture,
		Transporte,
		Mort
	}

	[Export] private float speed = 80f;
	[Export] private TileMapLayer pheromoneLayer;
	[Export] private Vector2I pheromoneAtlasCoords;

	[Export] private NavigationAgent2D navAgent;

	private AnimatedSprite2D anim;
	private AntState currentState = AntState.Wander;

	private Vector2 wanderDirection;
	private float wanderTimer = 0f;

	// Food / Targets
	private Vector2 targetFoodPos;
	private Vector2 nestPosition = new Vector2(600, 350);
	private bool hasFood = false;

	public override void _Ready()
	{
		anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		navAgent = GetNode<NavigationAgent2D>("NavAgent");
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
			case AntState.Wander:
				Wander(delta);
				CheckFoodNearby();
				break;

			case AntState.ChercheNourriture:
				MoveWithAStar(delta);
				break;

			case AntState.Transporte:
				TransportFood(delta);
				break;

			case AntState.Mort:
				Die();
				break;
		}

		LeaveTrail();
	}

	// -------------------- Wander aléatoire --------------------
	private void Wander(double delta)
	{
		wanderTimer -= (float)delta;
		if (wanderTimer <= 0f)
			PickNewWanderDirection();

		Velocity = wanderDirection * speed * 0.5f; // vitesse plus douce
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

	// -------------------- Détection de nourriture --------------------
	private void CheckFoodNearby()
	{
		FoodSpawnManager manager = GetTree().Root.GetNode<FoodSpawnManager>("Game/FoodSpawnManager");
		if (manager == null) return;

		foreach (var foodPos in manager.GetFoodPositions())
		{
			if (GlobalPosition.DistanceTo(foodPos) < 100f) // si proche de la food
			{
				GoToFood(foodPos);
				break;
			}
		}
	}

	// -------------------- A* vers la nourriture --------------------
	private void MoveWithAStar(double delta)
	{
		if (navAgent.IsNavigationFinished())
		{
			if (!hasFood)
			{
				hasFood = true;
				currentState = AntState.Transporte;
			}
			return;
		}

		Vector2 nextPos = navAgent.GetNextPathPosition();
		Vector2 dir = (nextPos - GlobalPosition).Normalized();

		Velocity = dir * speed;
		MoveAndSlide();

		if (!anim.IsPlaying())
			anim.Play("walk");
	}

	public void GoToFood(Vector2 foodPos)
	{
		targetFoodPos = foodPos;
		navAgent.TargetPosition = targetFoodPos;
		currentState = AntState.ChercheNourriture;
	}

	private void TransportFood(double delta)
	{
		navAgent.TargetPosition = nestPosition;
		MoveWithAStar(delta);

		if (GlobalPosition.DistanceTo(nestPosition) < 10f)
		{
			hasFood = false;
			currentState = AntState.Wander;
		}
	}

	private void LeaveTrail()
	{
		if (pheromoneLayer == null)
			return;

		Vector2 localPos = pheromoneLayer.ToLocal(GlobalPosition);
		Vector2I cell = pheromoneLayer.LocalToMap(localPos);

		pheromoneLayer.SetCell(cell, 0, pheromoneAtlasCoords, 0);
		pheromoneLayer.UpdateInternals();
	}

	private void UpdateSpriteFlip()
	{
		if (Velocity.X > 1f)
			anim.FlipH = true;
		else if (Velocity.X < -1f)
			anim.FlipH = false;
	}

	private void Die()
	{
		QueueFree();
	}
}
