using Godot;
using System.Collections.Generic;

public partial class AntSpawnManager : Node2D
{
	[Export] public PackedScene workerAntScene; // Pour les 5 de base
	[Export] public int initialWorkerCount = 5;
	[Export] public TileMapLayer pheromoneLayer;
	[Export] public Vector2I pheromoneAtlasCoords = new Vector2I(5, 7);
	[Export] public Node2D nest;

	private Node antsParent;
	private List<WorkerAnt> workers = new();

	[Signal] public delegate void AntSpawnedEventHandler(Ant ant);

	public override void _Ready()
	{
		antsParent = GetNode<Node>("../Ants");
		if (antsParent == null)
			GD.PrintErr("Ants node not found!");
		if (workerAntScene == null)
			GD.PrintErr("workerAntScene not assigned!");

		SpawnInitialWorkers();
	}

	private void SpawnInitialWorkers()
	{
		for (int i = 0; i < initialWorkerCount; i++)
		{
			var worker = SpawnWorker();
			if (worker != null) workers.Add(worker);
		}
	}

	public WorkerAnt SpawnWorker(Vector2? position = null)
	{
		if (workerAntScene == null || antsParent == null) return null;

		var worker = workerAntScene.Instantiate<WorkerAnt>();
		worker.GlobalPosition = position ?? nest?.GlobalPosition ?? Vector2.Zero;

		antsParent.AddChild(worker);
		worker.Set("pheromoneLayer", pheromoneLayer);
		worker.Set("pheromoneAtlasCoords", pheromoneAtlasCoords);
		worker.AddToGroup("Workers");
		worker.ExitNest();

		EmitSignal(SignalName.AntSpawned, worker);
		return worker;
	}
}
