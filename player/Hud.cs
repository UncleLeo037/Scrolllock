using Godot;
using System.Collections.Generic;

public partial class Hud : CanvasLayer
{
	public ProgressBar HealthBar;
	public List<object> Loadout;

	private Radial _radial;

	public override void _Ready()
	{
		HealthBar = GetNode<ProgressBar>("ProgressBar");
		_radial = GetNode<Radial>("Radial");
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
		_radial.Options = Loadout;
		_radial.QueueRedraw();
	}

    public override void _Input(InputEvent @event)
    {
		Visible = Input.MouseMode != Input.MouseModeEnum.Visible;

		if (Input.IsActionJustPressed("radial"))
		{
			Input.MouseMode = Input.MouseModeEnum.Confined;
			_radial.Show();
		}
		else if (Input.IsActionJustReleased("radial"))
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
			_radial.Hide();
		}

		//only run radial controls when mode is set for it
		if (Input.MouseMode != Input.MouseModeEnum.Confined) return;
    }
}
