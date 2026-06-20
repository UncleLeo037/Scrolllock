using Godot;
using Srolllock.spells;

public partial class Force : Spell
{
	private double lifetime = 0.1;
	public override Texture2D Icon {get; set;} = GD.Load<Texture2D>($"res://equipment/spells/force/force.png");
	
	public override void _Ready()
	{
		Area3D area = GetNode<Area3D>("Area3D");
		area.BodyEntered += _on_area_3d_body_entered;
	}

	public override void _PhysicsProcess(double delta)
	{
		//change to use animation waiting system to wait for spell effect to finish playing
		lifetime -= delta;
		if (lifetime <= 0.0)
		{
			this.QueueFree();
		}
	}

	public void _on_area_3d_body_entered(Node3D body)
	{
		if (body is Player player)
		{
			player.Velocity += (player.GlobalTransform.Origin - this.GlobalTransform.Origin).Normalized() * 14.0f;
		}
	}
}
