using Godot;
using System.Collections.Generic;

public partial class WorkerAnt : Ant
{
	protected Food targetFood = null;
	protected bool hasFood = false;

	private Node2D foodOnBack;

	public override void _PhysicsProcess(double delta)
	{
		switch (state)
		{
			case AntState.Wander:
				Wander(delta);
				SearchFood();
				break;

			case AntState.GoToFood:
			case AntState.CarryFood:
				MoveAStar();
				break;

			case AntState.InNest:
				Velocity = Vector2.Zero;
				return;
		}


		MoveAndSlide();
		FlipSprite();
		LeaveTrail();
	}

	// Cherche la nourriture proche et passe en GoToFood si trouvée
	private void SearchFood()
	{
		if (state != AntState.Wander) return;

		var manager = GetTree().Root.GetNode<FoodSpawnManager>("Game/FoodSpawnManager");
		Food closest = null;
		float closestDist = 120; // Distance de détection
		foreach (var f in manager.GetFoods())
		{
			if (!IsInstanceValid(f)) continue;

			float dist = GlobalPosition.DistanceTo(f.GlobalPosition);
			if (dist < closestDist)
			{
				closestDist = dist;
				closest = f;
			}
		}

		if (closest != null)
		{
			targetFood = closest;
			navAgent.TargetPosition = targetFood.GlobalPosition;
			state = AntState.GoToFood;
		}
	}

	// Déplacement A* vers la cible ou le nid
	private void MoveAStar()
	{
		if (!IsInstanceValid(navAgent)) return;

		if (navAgent.IsNavigationFinished())
		{
			if (state == AntState.GoToFood && targetFood != null)
				PickFood();
			else if (state == AntState.CarryFood)
			{
				DropFood();
				ExitNest();
			}
			return;
		}

		Vector2 next = navAgent.GetNextPathPosition();
		Velocity = (next - GlobalPosition).Normalized() * speed;
	}


	// Ramasser la nourriture
	private void PickFood()
	{
		if (targetFood == null || !IsInstanceValid(targetFood)) return;

		targetFood.Eat();
		hasFood = true;
		AttachFoodVisual();

		// Retour au nid
		navAgent.TargetPosition = nest.GlobalPosition;
		state = AntState.CarryFood;
		targetFood = null;
	}

	// Déposer la nourriture et retourner au wander
	private void DropFood()
	{
		if (foodOnBack != null && IsInstanceValid(foodOnBack))
		{
			foodOnBack.QueueFree();
			foodOnBack = null;
		}

		hasFood = false;
	}

	// Attacher visuel de nourriture sur la fourmi
	private void AttachFoodVisual()
	{
		if (foodOnBack != null || foodOnBackScene == null) return;

		foodOnBack = foodOnBackScene.Instantiate<Node2D>();
		AddChild(foodOnBack);
		foodOnBack.Position = new Vector2(0, -10);
	}

	// Sortie du nid
	public new async void ExitNest()
	{
		ResetAnt();
		Vector2 dir = new Vector2((float)GD.RandRange(-1, 1), (float)GD.RandRange(-1, 1)).Normalized();
		GlobalPosition = nest.GlobalPosition + dir * safeExitDistance;
		await ToSignal(GetTree().CreateTimer(0.2f), "timeout");
		myCollider.CallDeferred(CollisionShape2D.MethodName.SetDisabled, false);
	}

	public new void OnReachNest()
	{
		if (!hasFood || isUsingNest) return;

		isUsingNest = true;
		state = AntState.InNest;
		Velocity = Vector2.Zero;
		myCollider.CallDeferred(CollisionShape2D.MethodName.SetDisabled, true);
		DropFood();
		Visible = false;
	}

}
