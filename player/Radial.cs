using Godot;
using Srolllock.guns;
using System;
using System.Collections.Generic;

public partial class Radial : Control
{
	public int Select = 0;

	private int _radius = 200;
	private Vector2 _offset = Vector2.One * 64 / -2;
	private TextureRect _select;
	private (IEquipment item, Vector2 vect)[] _options;
	public override void _Ready()
	{
		SetProcess(false);
		_select = GetNode<TextureRect>("TextureRect");
	}

	public override void _Process(double delta)
	{
		Vector2 pos = GetLocalMousePosition();
		if (pos != Vector2.Zero)
		{
			float angle = float.Tau / (2 * _options.Length) - MathF.Atan2(pos.X, -pos.Y);
			if (angle < 0.0f) angle += float.Tau;
			Select = (byte)Math.Floor(angle / float.Tau * _options.Length);
		}
		else
		{
			Select = 0;
		}

		_select.Position = _options[Select].vect;
	}

	public void Start()
	{
		//prevent radial open when in menu
		if (Input.MouseMode == Input.MouseModeEnum.Visible) return;
		Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
		Show();
		SetProcess(true);
	}

	public void Setup(List<object> loadout)
	{
		_options = new (IEquipment, Vector2)[loadout.Count];
		for (int i = 0; i < loadout.Count; i++)
		{
			_options[i].item = (IEquipment)loadout[i];
			float phi = 1.5f * float.Pi - float.Tau * i / _options.Length;
			_options[i].vect = _radius * Vector2.FromAngle(phi) + _offset;
		}
		QueueRedraw();
	}

	public override void _Draw()
	{
		for (int i = 0; i < _options.Length; i++)
		{
			DrawTexture(_options[i].item.Icon, _options[i].vect);
		}
	}
}
