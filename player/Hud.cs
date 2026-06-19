using Godot;
using System.Collections.Generic;

public partial class Hud : CanvasLayer
{
	public ProgressBar HealthBar;
	public List<object> Loadout;

	public override void _Ready()
	{
		HealthBar = GetNode<ProgressBar>("ProgressBar");
		
		Loadout = new List<object>()
		{
			new Pistols(),
			new Force(),
			new Wall(),
			new Tornado(),
			new Slick(),
			new Blunderbuss(),
			new Rifle()
		};
	}

    public override void _Input(InputEvent @event)
    {
		Visible = Input.MouseMode == Input.MouseModeEnum.Captured;
    }
}
