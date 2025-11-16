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

			case AntState.Mort:
				QueueFree();
				break;
		}
	}

	private void Wander(double delta)
	{
		// Pour l'instant, la fourmi bouge aléatoirement
		Velocity = (Vector2.Right.Rotated((float)GD.RandRange(0, 2 * Mathf.Pi))) * 50f;
		MoveAndSlide();
	}
}
