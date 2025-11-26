using Godot;
using System.Threading.Tasks;

public partial class Nest : Area2D
{
	[Export] private float waitTime = 1.5f;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node body)
	{
		if (body is not Ant ant) return;

		if (!ant.HasFood())
		{
			// Forcer la fourmi à continuer sa route
			ant.ExitNest();
			return;
		}

		ant.OnReachNest();
		StartExitSequence(ant);
	}

	private async void StartExitSequence(Ant ant)
	{
		await ToSignal(GetTree().CreateTimer(waitTime), "timeout");

		ant.DropFood();
		ant.ExitNest();
	}
}
