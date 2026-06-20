using Godot;
using Srolllock.guns;
using Srolllock.spells;
using System.Collections.Generic;

public partial class Hud : CanvasLayer
{
	public ProgressBar HealthBar;
	public List<object> Loadout;
	private Radial _radial;

	private TextureRect _gun;
	private TextureRect _spell;

	private Dictionary<string, TextureRect> _icons = new Dictionary<string, TextureRect>()
	{
		{"Gun", null},
		{"Spell", null}
	};

	public override void _Ready()
	{
		HealthBar = GetNode<ProgressBar>("ProgressBar");
		_radial = GetNode<Radial>("Radial");
		_icons["Gun"] = GetNode<TextureRect>("TextureGun");
		_icons["Spell"] = GetNode<TextureRect>("TextureSpell");
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

	public object CloseRadial(bool equip = true)
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_radial.SetProcess(false);
		_radial.Hide();
		if (equip)
		{
			var item = (IEquipment)Loadout[_radial.Select - 1];
			string type = item.GetType().BaseType.Name;
			_icons[type].Texture = item.Icon;
			return item;
		}
		return null;
	}

	public override void _Input(InputEvent @event)
	{
		Visible = Input.MouseMode != Input.MouseModeEnum.Visible;

		if (Input.IsActionJustPressed("radial"))
		{
			_radial.Start(Loadout);
		}
		else if (Input.IsActionJustReleased("radial") && _radial.Visible)
		{
			CloseRadial(false);
		}
	}
}
