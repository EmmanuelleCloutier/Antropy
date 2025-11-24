using Godot;

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

		if (!ant.HasFood())
			return;

		ant.OnReachNest();

		await ToSignal(GetTree().CreateTimer(waitTime), "timeout");

		ant.DropFood();
		ant.ExitNest();
	}
}
