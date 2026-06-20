using Godot;
using Srolllock.guns;
using System;
using System.Collections.Generic;

public partial class Radial : Control
{
	public int Select = 0;

	private int _radius = 200;
	private Vector2 _offset = Vector2.One * 64 / -2;
	private Vector2 _flip = new Vector2(-1, 1);
	private TextureRect _select;
	private object[] _options;
	public override void _Ready()
	{
		SetProcess(false);
		_select = GetNode<TextureRect>("TextureRect");
	}

    public override void _Process(double delta)
    {
		Vector2 mos_pos = GetLocalMousePosition();
		if (mos_pos == Vector2.Zero) mos_pos = new Vector2(0, -0.1f);
        float angle = (mos_pos.Angle() - float.Tau / 2 / _options.Length + float.Tau / 4) * -1;
		if (angle < 0.0f)
		{
			angle += float.Tau;
		}
		float rad = angle % float.Tau;
		Select = (int)Math.Ceiling(rad / float.Tau * _options.Length);
		if (_options[Select - 1] is Vector2 vector)
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
			Equipment item = (Equipment)_options[i];
			Vector2 angle = Vector2.FromAngle((float.Tau * i / _options.Length) - float.Tau / 4) * _flip;
			Vector2 pos = _radius * angle + _offset;
			DrawTexture(
				item.Icon,
				pos
			);
			_options[i] = pos;
		}
	}
}
