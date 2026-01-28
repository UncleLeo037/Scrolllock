using Godot;
using System;

public partial class Orb : Node3D
{
	public void _on_area_3d_body_entered(Node3D body)
	{
		AreaOfEffect(body);
	}

    public void _on_area_3d_body_exited(Node3D body)
	{
        EndOfEffect(body);
	}

    //the below methods will be overridden in child classes
    private void AreaOfEffect(Node3D body)
    {
        //need to add code for modifying other objects later
        if (body is Player player)
        {
            player.Modify(-3.0f, false);
        }
    }

	private void EndOfEffect(Node3D body)
	{
        if (body is Player player)
        {
            player.Modify();
        }
    }
}
