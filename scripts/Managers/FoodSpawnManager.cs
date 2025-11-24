using Godot;
using System.Collections.Generic;

public partial class FoodSpawnManager : Node2D
{
	[Export] public PackedScene foodScene;
	[Export] public int maxFood = 2;

	[Export] public Vector2 spawnMin = new Vector2(50, 50);
	[Export] public Vector2 spawnMax = new Vector2(1100, 650);

	private List<Food> foods = new List<Food>();

	public override void _Ready()
	{
		GD.Print("FoodSpawnManager prêt !");
		SpawnInitialFood();
	}

	private void SpawnInitialFood()
	{
		for (int i = 0; i < maxFood; i++)
		{
			SpawnFood();
		}
	}

	public void SpawnFood()
	{
		if (foodScene == null)
		{
			GD.PrintErr("Food Scene pas assignée !");
			return;
		}

		// Instancie la food
		var foodInstance = foodScene.Instantiate() as Food;
		if (foodInstance == null)
		{
			GD.PrintErr("Impossible d'instancier Food. Vérifie que Food.tscn a le script attaché !");
			return;
		}

		// Position random
		float x = (float)GD.RandRange(spawnMin.X, spawnMax.X);
		float y = (float)GD.RandRange(spawnMin.Y, spawnMax.Y);
		foodInstance.GlobalPosition = new Vector2(x, y);

		// Ajoute dans la scène et liste
		AddChild(foodInstance);
		foods.Add(foodInstance);

		GD.Print("Food spawn à : ", foodInstance.GlobalPosition);

		// Connecte le signal
		foodInstance.FoodEaten += OnFoodEaten;
	}
	
	public List<Vector2> GetFoodPositions()
	{
		List<Vector2> positions = new List<Vector2>();
		foreach (var food in foods)
		{
			if (food != null)
				positions.Add(food.GlobalPosition);
		}
		return positions;
	}


	private void OnFoodEaten(Food food)
	{
		if (foods.Contains(food))
			foods.Remove(food);

		GD.Print("Food collectée, respawn en cours...");
		SpawnFood();
	}
}
