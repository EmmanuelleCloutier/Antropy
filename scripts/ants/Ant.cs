using Godot;

public partial class Ant : CharacterBody2D
{
	public enum AntState
	{
		Wander,
		GoToFood,
		CarryFood,
		InNest
	}

	[Export] protected float speed = 100f;
	[Export] private PackedScene foodOnBackScene;

	private AnimatedSprite2D anim;
	private NavigationAgent2D navAgent;
	public CollisionShape2D myCollider;
	private Area2D nest;
	private Vector2 lastNestEntryPosition;
	private bool navAgentActive = true;


	[Export] private TileMapLayer pheromoneLayer;
	[Export] private Vector2I pheromoneAtlasCoords;


	protected AntState state = AntState.Wander;

	private Vector2 wanderDir;
	private float wanderTimer;

	protected Food targetFood;
	protected bool hasFood = false;
	private Node2D foodOnBack;

	public override void _Ready()
	{
		anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		navAgent = GetNode<NavigationAgent2D>("NavAgent");
		myCollider = GetNode<CollisionShape2D>("CollisionShape2D");

		nest = GetTree().Root.GetNode<Area2D>("Game/Nest");

		PickNewWander();
	}

	public override void _PhysicsProcess(double delta)
	{
		switch (state)
		{
			case AntState.Wander:
				Wander(delta);
				break;

			case AntState.GoToFood:
			case AntState.CarryFood:
				MoveAStar();
				break;
		}

		MoveAndSlide();
		FlipSprite();
		LeaveTrail();

	}
	
	public void SetPheromoneData(TileMapLayer layer, Vector2I coords)
	{
		pheromoneLayer = layer;
		pheromoneAtlasCoords = coords;
	}


	// ----------- Wander -----------
	void Wander(double delta)
	{
		wanderTimer -= (float)delta;
		if (wanderTimer <= 0)
			PickNewWander();

		Velocity = wanderDir * speed * 0.5f;

		SearchFood();
	}

	void PickNewWander()
	{
		wanderTimer = (float)GD.RandRange(1, 2);
		wanderDir = new Vector2(
			(float)GD.RandRange(-1, 1),
			(float)GD.RandRange(-1, 1)
		).Normalized();
	}

	// ---------- Find food ----------
	void SearchFood()
	{
		var manager = GetTree().Root.GetNode<FoodSpawnManager>("Game/FoodSpawnManager");

		foreach (var f in manager.GetFoods())
		{
			if (f == null) continue;

			if (GlobalPosition.DistanceTo(f.GlobalPosition) < 120)
			{
				targetFood = f;
				navAgent.TargetPosition = targetFood.GlobalPosition;
				state = AntState.GoToFood;
				break;
			}
		}
	}

	// ---------- Movement ----------
void MoveAStar()
{
	if (!navAgentActive) return;

	if (navAgent.IsNavigationFinished())
	{
		if (state == AntState.GoToFood && targetFood != null)
			PickFood();
		return;
	}

	Vector2 next = navAgent.GetNextPathPosition();
	Velocity = (next - GlobalPosition).Normalized() * speed;
}


	void PickFood()
	{
		if (targetFood == null || !IsInstanceValid(targetFood))
		{
			state = AntState.Wander;
			return;
		}

		hasFood = true;
		targetFood.Eat();
		targetFood = null;

		AttachFoodVisual();

		navAgent.TargetPosition = nest.GlobalPosition;
		state = AntState.CarryFood;
	}
	
private void LeaveTrail()
{
	if (pheromoneLayer == null)
		return;

	// Transforme la position globale de la fourmi en coordonnées locales du TileMapLayer
	Vector2 localPos = pheromoneLayer.GlobalTransform.AffineInverse() * GlobalPosition;
	Vector2I cell = pheromoneLayer.LocalToMap(localPos);

	// Pose la phéromone
	pheromoneLayer.SetCell(cell, 0, pheromoneAtlasCoords, 0);
}




	// ---------- Called by Nest ----------
	public bool HasFood() => hasFood;

		public void OnReachNest()
	{
		if (!hasFood) return;

		lastNestEntryPosition = GlobalPosition;
		state = AntState.InNest;
		Velocity = Vector2.Zero;

		// Désactive le collider de manière différée
		myCollider.CallDeferred("set_disabled", true);

		Visible = false;
	}




	public void DropFood()
	{
		if (foodOnBack != null && IsInstanceValid(foodOnBack))
			foodOnBack.QueueFree();

		foodOnBack = null;
		hasFood = false;
	}

public void ExitNest()
{
	Visible = true;

	// Position de sortie avec petit offset
	Vector2 exitOffset = new Vector2((float)GD.RandRange(-2, 2), (float)GD.RandRange(-2, 2));
	GlobalPosition = new Vector2(10, 10) + exitOffset;

	Velocity = Vector2.Zero;
	state = AntState.Wander;

	// Désactiver temporairement le navAgent pour éviter les déplacements forcés
	navAgentActive = false;

	GetTree().CreateTimer(2f).Timeout += () =>
{
	myCollider.CallDeferred("set_disabled", false);
	navAgentActive = true;
	PickNewWander();
};

}






	// ---------- Visual ----------
	void AttachFoodVisual()
	{
		if (foodOnBackScene == null) return;

		foodOnBack = foodOnBackScene.Instantiate<Node2D>();
		AddChild(foodOnBack);
		foodOnBack.Position = new Vector2(0, -10);
	}

	void FlipSprite()
	{
		if (Velocity.X > 1) anim.FlipH = true;
		if (Velocity.X < -1) anim.FlipH = false;
	}
}
