using Godot;

public partial class SoldierAnt : Ant
{
	private Enemy targetEnemy;

	public void SetTargetEnemy(Enemy enemy)
	{
		targetEnemy = enemy;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (targetEnemy != null && IsInstanceValid(targetEnemy))
		{
			Vector2 dir = (targetEnemy.GlobalPosition - GlobalPosition).Normalized();
			Velocity = dir * speed;

			if (GlobalPosition.DistanceTo(targetEnemy.GlobalPosition) < 20f)
			{
				//targetEnemy.TakeDamage(1);
			}
		}
		else
		{
			base._PhysicsProcess(delta); // Wander si pas de cible
		}

		MoveAndSlide();
		FlipSprite();
		LeaveTrail();
	}
}
