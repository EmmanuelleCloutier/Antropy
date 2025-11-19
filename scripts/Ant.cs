using Godot;

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

	private AntState currentState = AntState.Explore;
	private Vector2 targetFoodPosition;
	private Vector2 nestPosition;
	private bool hasFood = false;
	private float health = 10f;

	public override void _PhysicsProcess(double delta)
	{
		ExecuteState(delta);
	}

	private void ExecuteState(double delta)
	{
		switch (currentState)
		{
			case AntState.Explore:
				Wander(delta);
				break;

			case AntState.ChercheNourriture:
				SeekFood(delta);
				break;

			case AntState.Transporte:
				TransportFood(delta);
				break;

			case AntState.RetourAuNid:
				ReturnToNest(delta);
				break;

			case AntState.Defense:
				Defend(delta);
				break;

			case AntState.Mort:
				Die();
				break;
		}
	}

	private void Wander(double delta)
	{
		// Pour l'instant, la fourmi bouge aléatoirement
		Velocity = (Vector2.Right.Rotated((float)GD.RandRange(0, 2 * Mathf.Pi))) * 50f;
		MoveAndSlide();
	}
	
	private void SetState(AntState newState)
	{
		currentState = newState;
	}

	
	private void SeekFood(double delta) { /* TODO */ }
	private void PickupFood() { /* TODO */ }
	private void ReturnToNest(double delta) { /* TODO */ }
	private void TransportFood(double delta) { /* TODO */ }
	private void Defend(double delta) { /* TODO */ }
	private void Die() { QueueFree(); }

}
