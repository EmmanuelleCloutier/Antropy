using Godot;
using System.Threading.Tasks;

public partial class Nest : Area2D
{
	[Export] private float waitTime = 1.5f;

	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
	}

	private async void OnBodyEntered(Node body)
	{
		if (body is not Ant ant)
			return;

		// On ne s'occupe que des fourmis avec nourriture
		if (!ant.HasFood())
			return;

		// Marque la fourmi comme dans le Nest
		ant.OnReachNest();

		// Attend un moment avant de la faire sortir
		await ToSignal(GetTree().CreateTimer(waitTime), "timeout");

		ant.DropFood();
		ant.ExitNest();
	}
}
