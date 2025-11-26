using Godot;
using System.Threading.Tasks;

public partial class Ant : CharacterBody2D
{
	public enum AntState
	{
		Wander,
		GoToFood,
		CarryFood,
		InNest
	}

	[Export] private float speed = 100f;
	[Export] private PackedScene foodOnBackScene;

	private AnimatedSprite2D anim;
	private NavigationAgent2D navAgent;
	private CollisionShape2D myCollider;
	private Area2D nest;

	[Export] private TileMapLayer pheromoneLayer;
	[Export] private Vector2I pheromoneAtlasCoords;

	private Vector2 wanderDir;
	private float wanderTimer;

	private Food targetFood;
	private bool hasFood = false;
	private Node2D foodOnBack;

	private bool navAgentActive = true;
	private bool isUsingNest = false;
	private float safeExitDistance = 40f;

	protected AntState state = AntState.Wander;

	public override void _Ready()
	{
		anim = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		navAgent = GetNode<NavigationAgent2D>("NavAgent");
		myCollider = GetNode<CollisionShape2D>("CollisionShape2D");
		nest = GetTree().Root.GetNode<Area2D>("Game/Nest");

		ResetAnt();
	}

	public override void _PhysicsProcess(double delta)
	{
		// Movement based on state
		switch (state)
		{
			case AntState.Wander:
				Wander(delta);
				break;

			case AntState.GoToFood:
			case AntState.CarryFood:
				MoveAStar();
				break;

			case AntState.InNest:
				Velocity = Vector2.Zero;
				return; // pas de MoveAndSlide
		}

		MoveAndSlide();
		FlipSprite();
		LeaveTrail();
	}

	// -------------------- Wander --------------------
	private void Wander(double delta)
	{
		wanderTimer -= (float)delta;
		if (wanderTimer <= 0)
			PickNewWander();

		Velocity = wanderDir * speed * 0.5f;
		SearchFood();
	}

	private void PickNewWander()
	{
		wanderTimer = (float)GD.RandRange(1.0f, 2.0f);
		wanderDir = new Vector2(
			(float)GD.RandRange(-1.0f, 1.0f),
			(float)GD.RandRange(-1.0f, 1.0f)
		).Normalized();
	}

	// -------------------- Find food --------------------
	private void SearchFood()
	{
		if (state != AntState.Wander) return;

		var manager = GetTree().Root.GetNode<FoodSpawnManager>("Game/FoodSpawnManager");

		foreach (var f in manager.GetFoods())
		{
			if (f == null || !IsInstanceValid(f))
				continue;

			if (GlobalPosition.DistanceTo(f.GlobalPosition) < 120)
			{
				targetFood = f;
				navAgent.TargetPosition = targetFood.GlobalPosition;
				state = AntState.GoToFood;
				break;
			}
		}
	}

	// -------------------- Movement --------------------
	private void MoveAStar()
	{
		if (!navAgentActive)
		{
			Velocity = Vector2.Zero;
			return;
		}

		if (navAgent.IsNavigationFinished())
		{
			if (state == AntState.GoToFood && targetFood != null)
				PickFood();

			if (state == AntState.CarryFood)
				Velocity = Vector2.Zero;

			return;
		}

		Vector2 next = navAgent.GetNextPathPosition();
		Velocity = (next - GlobalPosition).Normalized() * speed;
	}

	// -------------------- Pick food --------------------
	private void PickFood()
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

		// Ne cible le Nest que si elle a de la nourriture
		navAgent.TargetPosition = nest.GlobalPosition;
		state = AntState.CarryFood;
	}

	// -------------------- Pheromones --------------------
	private void LeaveTrail()
	{
		if (pheromoneLayer == null) return;

		Vector2 localPos = pheromoneLayer.GlobalTransform.AffineInverse() * GlobalPosition;
		Vector2I cell = pheromoneLayer.LocalToMap(localPos);

		pheromoneLayer.SetCell(cell, 0, pheromoneAtlasCoords, 0);
	}

	// -------------------- Nest logic --------------------
	public bool HasFood() => hasFood;

	public void OnReachNest()
	{
		if (!hasFood || isUsingNest) return;

		isUsingNest = true;
		state = AntState.InNest;
		Velocity = Vector2.Zero;
		navAgentActive = false;

		myCollider.CallDeferred(CollisionShape2D.MethodName.SetDisabled, true);
		Visible = false;
	}

	public void DropFood()
	{
		if (foodOnBack != null && IsInstanceValid(foodOnBack))
			foodOnBack.QueueFree();

		foodOnBack = null;
		hasFood = false;
	}

	public async void ExitNest()
	{
		// Réinitialisation complète
		ResetAnt();

		// Repositionne hors du Nest
		Vector2 dir = new Vector2((float)GD.RandRange(-1, 1), (float)GD.RandRange(-1, 1)).Normalized();
		GlobalPosition = nest.GlobalPosition + dir * safeExitDistance;

		navAgentActive = false;
		await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
		myCollider.CallDeferred(CollisionShape2D.MethodName.SetDisabled, false);
		navAgentActive = true;
	}

	private void ResetAnt()
	{
		hasFood = false;
		targetFood = null;
		state = AntState.Wander;
		navAgent.TargetPosition = GlobalPosition;
		isUsingNest = false;
		PickNewWander();
		Visible = true;
	}

	// -------------------- Visual --------------------
	private void AttachFoodVisual()
	{
		if (foodOnBackScene == null) return;

		foodOnBack = foodOnBackScene.Instantiate<Node2D>();
		AddChild(foodOnBack);
		foodOnBack.Position = new Vector2(0, -10);
	}

	private void FlipSprite()
	{
		if (Velocity.X > 1) anim.FlipH = true;
		else if (Velocity.X < -1) anim.FlipH = false;
	}
}
