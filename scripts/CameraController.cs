using Godot;
using System.Collections.Generic;

public partial class CameraController : Camera2D
{
	private List<Ant> ants = new List<Ant>();
	private int currentIndex = -1;

	public override void _Ready()
	{
		// Récupère toutes les fourmis dans le groupe "Ants"
		foreach (Node n in GetTree().GetNodesInGroup("Ants"))
		{
			if (n is Ant ant)
				ants.Add(ant);
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButton && 
			mouseButton.Pressed && 
			mouseButton.ButtonIndex == MouseButton.Left)
		{
			SwitchToNextAnt();
		}
	}

	private void SwitchToNextAnt()
	{
		if (ants.Count == 0) return;

		currentIndex = (currentIndex + 1) % ants.Count;
	}

	public override void _Process(double delta)
	{
		if (currentIndex >= 0 && currentIndex < ants.Count)
		{
			GlobalPosition = ants[currentIndex].GlobalPosition;
		}
	}
}
