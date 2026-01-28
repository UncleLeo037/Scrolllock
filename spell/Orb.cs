using Godot;
using System;

public partial class Orb : Node3D
{
	public void _on_area_3d_body_entered(Node3D body)
	{
		if (body is Player player)
		{
			player.Modify(-3.0f, false);
		}
	}

	public void _on_area_3d_body_exited(Node3D body)
	{

		if (body is Player player)
		{
			player.Modify();
		}
	}
}
