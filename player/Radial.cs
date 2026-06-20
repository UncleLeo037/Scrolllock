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
	private object[] _options;
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

		if (_options[Select] is Vector2 vector)
		{
			_select.Position = vector;
		}
	}

	public void Start(List<object> loadout)
	{
		//prevent radial open when in menu
		if (Input.MouseMode == Input.MouseModeEnum.Visible) return;

		Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
		_options = loadout.ToArray();
		//QueueRedraw();
		Show();
		SetProcess(true);
	}

	public override void _Draw()
	{
		for (int i = 0; i < _options.Length; i++)
		{
			float phi = 1.5f * float.Pi - float.Tau * i / _options.Length;
			Vector2 pos = _radius * Vector2.FromAngle(phi) + _offset;
			DrawTexture(((IEquipment)_options[i]).Icon, pos);
			_options[i] = pos;
		}
	}
}
