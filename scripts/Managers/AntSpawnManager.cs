using Godot;
using System;
using System.Collections.Generic;

public partial class AntSpawnManager : Node2D
{
	[Export] public PackedScene antScene;
	[Export] public int initialAntCount = 5;
	[Export] public TileMapLayer pheromoneLayer; 
	[Export] public Vector2I pheromoneAtlasCoords = new Vector2I(5, 7); 
	[Export] public Node2D nest;

	private Node antsParent;
	private List<Ant> ants = new List<Ant>();

	[Signal] public delegate void AntSpawnedEventHandler(Ant ant); // <-- correction

	public override void _Ready()
	{
		GD.Print("AntSpawnManager: _Ready() called");
		antsParent = GetNode<Node>("../Ants");
		if (antsParent == null)
		{
			GD.PrintErr("AntSpawnManager: Ants node not found as a sibling!");
			return;
		}

		if (antScene == null)
		{
			GD.PrintErr("AntSpawnManager: antScene is not assigned!");
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
	if (antScene == null || antsParent == null) return null;

	Node2D antNode = antScene.Instantiate<Node2D>();
	if (position.HasValue)
		antNode.GlobalPosition = position.Value;
	else if (nest != null)
		antNode.GlobalPosition = nest.GlobalPosition;
	else
		antNode.GlobalPosition = Vector2.Zero;

	antsParent.AddChild(antNode);

	if (antNode is Ant antInstance)
	{
		antInstance.SetPheromoneData(pheromoneLayer, pheromoneAtlasCoords);
		ants.Add(antInstance);
		antInstance.AddToGroup("Ants");

		// Fais “spawn safe” avec pause 2s
		antInstance.ExitNest();

		EmitSignal("AntSpawned", antInstance);
		return antInstance;
	}

	return null;
}
}
