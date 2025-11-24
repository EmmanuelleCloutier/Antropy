using Godot;
using System;

public partial class WorkerAnt : Ant
{
	[Export] private float carryFoodSpeedMultiplier = 1.2f;

	public override void _PhysicsProcess(double delta)
	{
		// Comportement normal d'Ant
		base._PhysicsProcess(delta);

		// Comportement spécifique des ouvrières
		if (state == AntState.CarryFood)
		{
			Velocity *= carryFoodSpeedMultiplier;
		}
	}

	public void SpecialWorkerBehavior()
	{
		GD.Print("Je suis une ouvrière et je fais quelque chose de spécial !");
	}
}
