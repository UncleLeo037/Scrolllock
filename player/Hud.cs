using Godot;
using Srolllock.guns;
using Srolllock.spells;
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

	public object CloseRadial()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_radial.SetProcess(false);
		_radial.Hide();
		return Loadout[_radial.Select - 1];
	}

	public override void _Input(InputEvent @event)
	{
		Visible = Input.MouseMode != Input.MouseModeEnum.Visible;

		if (Input.IsActionJustPressed("radial"))
		{
			Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
			_radial.SetProcess(true);
			_radial.Show();
		}
		else if (Input.IsActionJustReleased("radial"))
		{
			CloseRadial();
		}
	}
}
