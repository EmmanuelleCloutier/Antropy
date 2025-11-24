using Godot;

public partial class Food : Area2D
{
	[Signal]
	public delegate void FoodEatenEventHandler(Food food);

	public override void _Ready()
	{
		// Connecte le signal BodyEntered de Area2D
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node body)
	{
		if (body is Ant)
		{
			GD.Print("Food mangée par une fourmi !");
			EmitSignal(SignalName.FoodEaten, this);
			QueueFree();
		}
	}
}
