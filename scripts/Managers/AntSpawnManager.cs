using Godot;
using System.Collections.Generic;

public partial class AntSpawnManager : Node2D
{
	[Export] public PackedScene antScene;
	[Export] public int initialAntCount = 5;

	[Export] public TileMapLayer pheromoneLayer;
	[Export] public Vector2I pheromoneAtlasCoords = new Vector2I(5, 7);

	[Export] public Node2D nest;

	private Node antsParent;
	private List<Ant> ants = new();

	[Signal] public delegate void AntSpawnedEventHandler(Ant ant);

	public override void _Ready()
	{
		antsParent = GetNode<Node>("../Ants");

		if (antsParent == null)
		{
			GD.PrintErr("Ants node not found!");
			return;
		}

		if (antScene == null)
		{
			GD.PrintErr("antScene not assigned!");
			return;
		}

		SpawnInitialAnts();
	}

	private void SpawnInitialAnts()
	{
		for (int i = 0; i < initialAntCount; i++)
			SpawnAnt();
	}

	public Ant SpawnAnt(Vector2? position = null)
	{
		if (antScene == null || antsParent == null)
			return null;

		var antNode = antScene.Instantiate<Ant>();

		// Position de départ
		if (position.HasValue)
			antNode.GlobalPosition = position.Value;
		else if (nest != null)
			antNode.GlobalPosition = nest.GlobalPosition;
		else
			antNode.GlobalPosition = Vector2.Zero;

		antsParent.AddChild(antNode);

		// ✅ Injection directe des phéromones
		antNode.Set("pheromoneLayer", pheromoneLayer);
		antNode.Set("pheromoneAtlasCoords", pheromoneAtlasCoords);

		antNode.AddToGroup("Ants");

		// Spawn safe : commence hors du nid
		antNode.ExitNest();

		ants.Add(antNode);

		EmitSignal(SignalName.AntSpawned, antNode);
		return antNode;
	}
}
