using Godot;
using System.Collections.Generic;

public partial class CameraController : Camera2D
{
	private List<Ant> ants = new List<Ant>();
	private int currentIndex = 0;

	public override void _Ready()
	{
		MakeCurrent(); // active cette caméra

		Node antsParent = GetParent().GetNode("Ants");
		if (antsParent == null)
		{
			GD.PrintErr("Le node 'Ants' n'a pas été trouvé !");
			return;
		}

		foreach (Node child in antsParent.GetChildren())
		{
			if (child is Ant ant)
				ants.Add(ant);
		}

		if (ants.Count > 0)
			GlobalPosition = ants[0].GlobalPosition;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (ants.Count == 0)
			return;

		// Vérifie si la fourmi actuelle est toujours visible
		if (!ants[currentIndex].Visible || !IsInstanceValid(ants[currentIndex]))
		{
			SwitchToNextActiveAnt();
		}

		GlobalPosition = ants[currentIndex].GlobalPosition;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent
			&& mouseEvent.Pressed
			&& mouseEvent.ButtonIndex == MouseButton.Left)
		{
			SwitchToNextActiveAnt();
		}
	}

	private void SwitchToNextActiveAnt()
	{
		if (ants.Count == 0)
			return;

		int startIndex = currentIndex;
		do
		{
			currentIndex = (currentIndex + 1) % ants.Count;
		}
		while ((!ants[currentIndex].Visible || !IsInstanceValid(ants[currentIndex])) 
			   && currentIndex != startIndex);

		// Si aucune fourmi visible, reste sur celle actuelle
		if (ants[currentIndex].Visible && IsInstanceValid(ants[currentIndex]))
			GlobalPosition = ants[currentIndex].GlobalPosition;
	}
}
