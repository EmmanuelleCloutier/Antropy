using Godot;
using System.Collections.Generic;

public partial class CameraController : Camera2D
{
	private List<Node2D> ants = new List<Node2D>();
	private int currentIndex = 0;
	private bool initialized = false; // flag pour le focus initial

public override void _Ready()
{
	MakeCurrent(); // active cette caméra

	// récupère le node parent (Game) puis cherche Ants
	Node antsParent = GetParent().GetNode("Ants");

	if (antsParent == null)
	{
		GD.PrintErr("Le node 'Ants' n'a pas été trouvé !");
		return;
	}
	else
	{
		GD.Print($"Node 'Ants' trouvé : {antsParent.Name}");
	}

	// récupère tous les enfants (assume que ce sont des Node2D)
	foreach (Node child in antsParent.GetChildren())
	{
		if (child is Node2D ant)
		{
			ants.Add(ant);
			GD.Print($"Fourmi ajoutée: {ant.Name}, position: {ant.GlobalPosition}");
		}
	}

	GD.Print($"Total fourmis détectées: {ants.Count}");

	if (ants.Count > 0)
		GlobalPosition = ants[0].GlobalPosition;
}




	public override void _PhysicsProcess(double delta)
	{
		if (ants.Count == 0)
			return;

		// focus initial sur la première fourmi
		if (!initialized)
		{
			GlobalPosition = ants[0].GlobalPosition;
			GD.Print($"Caméra focus initial sur: {ants[0].Name} à {ants[0].GlobalPosition}");
			initialized = true;
		}

		// suit la fourmi actuelle en continu
		GlobalPosition = ants[currentIndex].GlobalPosition;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent
			&& mouseEvent.Pressed
			&& mouseEvent.ButtonIndex == MouseButton.Left)
		{
			SwitchAnt();
		}
	}

	private void SwitchAnt()
	{
		if (ants.Count == 0)
		{
			GD.Print("Aucune fourmi à switcher !");
			return;
		}

		currentIndex = (currentIndex + 1) % ants.Count;
		GlobalPosition = ants[currentIndex].GlobalPosition;
		GD.Print($"Changement de fourmi : {ants[currentIndex].Name} à {ants[currentIndex].GlobalPosition}");
	}
}
