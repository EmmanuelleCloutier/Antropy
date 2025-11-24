using Godot;
using System;

public partial class Food : Area2D
{
	public event Action<Food> FoodEaten;

	public void Eat()
	{
		// Informe le manager
		FoodEaten?.Invoke(this);

		// Disparition visuelle
		QueueFree();
	}
}
