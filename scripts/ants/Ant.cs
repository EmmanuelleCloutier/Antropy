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
	[Export] public TileMapLayer pheromoneLayer;
	[Export] public Vector2I pheromoneAtlasCoords;
	[Export] protected PackedScene foodOnBackScene;


	protected AnimatedSprite2D anim;
	protected NavigationAgent2D navAgent;
	protected CollisionShape2D myCollider;
	protected Area2D nest;
	


	protected Vector2 wanderDir;
	protected float wanderTimer;
	protected bool isUsingNest = false;
	protected float safeExitDistance = 40f;
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
		switch (state)
		{
			case AntState.Wander:
				Wander(delta);
				break;
			case AntState.InNest:
				Velocity = Vector2.Zero;
				return;
		}

		MoveAndSlide();
		FlipSprite();
		LeaveTrail();
	}

	protected void Wander(double delta)
	{
		wanderTimer -= (float)delta;
		if (wanderTimer <= 0) PickNewWander();
		Velocity = wanderDir * speed * 0.5f;
	}

	protected void PickNewWander()
	{
		wanderTimer = (float)GD.RandRange(1.0f, 2.0f);
		wanderDir = new Vector2(
			(float)GD.RandRange(-1.0f, 1.0f),
			(float)GD.RandRange(-1.0f, 1.0f)
		).Normalized();
	}

	protected void LeaveTrail()
	{
		if (pheromoneLayer == null) return;

		Vector2 localPos = pheromoneLayer.GlobalTransform.AffineInverse() * GlobalPosition;
		Vector2I cell = pheromoneLayer.LocalToMap(localPos);
		pheromoneLayer.SetCell(cell, 0, pheromoneAtlasCoords, 0);
	}

	public async void ExitNest()
	{
		ResetAnt();
		Vector2 dir = new Vector2((float)GD.RandRange(-1, 1), (float)GD.RandRange(-1, 1)).Normalized();
		GlobalPosition = nest.GlobalPosition + dir * safeExitDistance;
		await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
		myCollider.CallDeferred(CollisionShape2D.MethodName.SetDisabled, false);
	}

	public void OnReachNest()
	{
		if (isUsingNest) return;

		isUsingNest = true;
		state = AntState.InNest;
		Velocity = Vector2.Zero;
		myCollider.CallDeferred(CollisionShape2D.MethodName.SetDisabled, true);
		Visible = false;
	}

	protected void ResetAnt()
	{
		state = AntState.Wander;
		navAgent.TargetPosition = GlobalPosition;
		isUsingNest = false;
		PickNewWander();
		Visible = true;
	}

	protected void FlipSprite()
	{
		if (Velocity.X > 1) anim.FlipH = true;
		else if (Velocity.X < -1) anim.FlipH = false;
	}
	
	public void GoTo(Vector2 targetPosition)
	{
		if (navAgent == null) return;

		// On définit simplement la cible
		navAgent.TargetPosition = targetPosition;
	}



}
