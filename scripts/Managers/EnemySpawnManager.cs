using Godot;
using System;
using System.Collections.Generic;

public partial class EnemySpawnManager : Node2D
{
	[Export] private PackedScene enemyScene;
	[Export] private int enemyCount = 5;
	[Export] private Vector2 spawnAreaMin;
	[Export] private Vector2 spawnAreaMax;

	private List<Enemy> enemies = new List<Enemy>();

	public override void _Ready()
	{
		SpawnEnemies();
	}

	private void SpawnEnemies()
	{
		for (int i = 0; i < enemyCount; i++)
		{
			Enemy e = enemyScene.Instantiate<Enemy>();
			AddChild(e);

			// Position aléatoire dans la zone
			float x = (float)GD.RandRange(spawnAreaMin.X, spawnAreaMax.X);
			float y = (float)GD.RandRange(spawnAreaMin.Y, spawnAreaMax.Y);
			e.GlobalPosition = new Vector2(x, y);

			enemies.Add(e);
		}
	}
}
