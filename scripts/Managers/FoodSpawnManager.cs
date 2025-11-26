using Godot;
using System.Collections.Generic;

public partial class FoodSpawnManager : Node2D
{
	[Export] public PackedScene foodScene;
	[Export] public int maxFood = 2;

	[Export] public Vector2 spawnMin = new Vector2(50, 50);
	[Export] public Vector2 spawnMax = new Vector2(1100, 650);

	private List<Food> foods = new();

	public override void _Ready()
	{
		SpawnInitialFood();
	}
	
	public List<Food> GetFoods()
{
	return foods;
}


	private void SpawnInitialFood()
	{
		for (int i = 0; i < maxFood; i++)
			SpawnFood();
	}

	public void SpawnFood()
	{
		var food = foodScene.Instantiate<Food>();

		float x = (float)GD.RandRange(spawnMin.X, spawnMax.X);
		float y = (float)GD.RandRange(spawnMin.Y, spawnMax.Y);
		food.GlobalPosition = new Vector2(x, y);

		food.FoodEaten += OnFoodEaten;

		AddChild(food);
		foods.Add(food);
	}


	private void OnFoodEaten(Food food)
	{
		if (foods.Contains(food))
			foods.Remove(food);
		CallDeferred(nameof(SpawnFood));
	}

}
