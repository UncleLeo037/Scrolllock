using Godot;
using System;

public partial class Lift : Path3D
{
	private float speed = 0.0f;
	private PathFollow3D path;

	public override void _Ready()
	{
		path = GetNode<PathFollow3D>("PathFollow3D");
		Area3D area = GetNode<AnimatableBody3D>("AnimatableBody3D").GetNode<Area3D>("Area3D");
		area.BodyEntered += _on_area_3d_body_entered;
	}


	public override void _Process(double delta)
	{
		path.ProgressRatio += speed * (float)delta;
		if (path.ProgressRatio == 0 || path.ProgressRatio == 1)
		{
			SetProcess(false);
		}
	}

	public void _on_area_3d_body_entered(Node3D body)
	{
		if (body is Player player)
		{
			if (path.ProgressRatio == 0)
			{
				GD.Print("up");
				speed = 0.1f;
			}
			if (path.ProgressRatio == 1)
			{
				GD.Print("down");
				speed = -0.1f;
			}
			SetProcess(true);
		}
	}
}
