using Godot;
using System.Collections.Generic;

public partial class AntSpawnManager : Node2D
{
	[Export] public PackedScene antScene;
	[Export] public PackedScene soldierAntScene;
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
			GD.PrintErr("Ants node not found!");

		if (antScene == null)
			GD.PrintErr("antScene not assigned!");

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

		var antNode = antScene.Instantiate<Ant>();

		if (position.HasValue)
			antNode.GlobalPosition = position.Value;
		else if (nest != null)
			antNode.GlobalPosition = nest.GlobalPosition;
		else
			antNode.GlobalPosition = Vector2.Zero;

		antsParent.AddChild(antNode);

		antNode.Set("pheromoneLayer", pheromoneLayer);
		antNode.Set("pheromoneAtlasCoords", pheromoneAtlasCoords);

		antNode.AddToGroup("Ants");
		antNode.ExitNest();

		ants.Add(antNode);
		EmitSignal(SignalName.AntSpawned, antNode);
		return antNode;
	}

	public void SpawnSoldiers(Vector2 spawnPos, Enemy enemy, int count = 10)
	{
		if (soldierAntScene == null || antsParent == null) return;

		for (int i = 0; i < count; i++)
		{
			var soldier = soldierAntScene.Instantiate<SoldierAnt>();
			soldier.GlobalPosition = spawnPos;

			soldier.Set("pheromoneLayer", pheromoneLayer);
			soldier.Set("pheromoneAtlasCoords", pheromoneAtlasCoords);
			antsParent.AddChild(soldier);

			soldier.ExitNest();
			soldier.SetTargetEnemy(enemy);
			soldier.AddToGroup("Ants");
			ants.Add(soldier);
			EmitSignal(SignalName.AntSpawned, soldier);
		}
	}
}
