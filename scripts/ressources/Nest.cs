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
		// Ne gérer que les WorkerAnt
		if (body is not WorkerAnt worker) return;

		// ExitNest direct, plus besoin de HasFood ni DropFood
		worker.OnReachNest();
		StartExitSequence(worker);
	}

	private async void StartExitSequence(WorkerAnt worker)
	{
		await ToSignal(GetTree().CreateTimer(waitTime), "timeout");

		// Juste faire sortir la fourmi, plus de DropFood
		worker.ExitNest();
	}
}
