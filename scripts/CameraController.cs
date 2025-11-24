using Godot;
using System.Collections.Generic;

public partial class CameraController : Camera2D
{
	private List<Ant> ants = new List<Ant>();
	private int currentIndex = 0;

	public override void _Ready()
	{
		MakeCurrent();

		// Récupère toutes les fourmis déjà existantes dans le groupe "Ants"
		var nodesInGroup = GetTree().GetNodesInGroup("Ants");
		foreach (var node in nodesInGroup)
		{
			if (node is Ant ant)
				ants.Add(ant);
		}

		// Connecte le signal pour détecter les nouvelles fourmis spawnées après
		var manager = GetTree().Root.GetNode<AntSpawnManager>("Game/AntSpawnManager");
		if (manager != null)
			manager.Connect("AntSpawned", new Callable(this, "OnAntSpawned"));

		if (ants.Count > 0)
			GlobalPosition = ants[0].GlobalPosition;
	}

	private void OnAntSpawned(Ant ant)
	{
		if (ant != null)
			ants.Add(ant);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (ants.Count == 0) return;
		if (!ants[currentIndex].Visible || !IsInstanceValid(ants[currentIndex]))
			SwitchToNextActiveAnt();
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
		if (ants.Count == 0) return;

		int startIndex = currentIndex;
		do
		{
			currentIndex = (currentIndex + 1) % ants.Count;
		}
		while ((!ants[currentIndex].Visible || !IsInstanceValid(ants[currentIndex]))
			   && currentIndex != startIndex);

		if (ants[currentIndex].Visible && IsInstanceValid(ants[currentIndex]))
			GlobalPosition = ants[currentIndex].GlobalPosition;
	}
}
