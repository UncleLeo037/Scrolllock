using Godot;
using Srolllock.guns;
using System;
using System.Collections.Generic;

public partial class Radial : Control
{
	public List<object> Options;
	public int Select = 0;

	private int _radius = 200;
	private Vector2 _offset = Vector2.One * 64 / -2;
	private Vector2 _flip = new Vector2(-1, 1);
	private TextureRect _select;
	private List<Vector2> _coords;
	public override void _Ready()
	{
		SetProcess(false);
		_select = GetNode<TextureRect>("TextureRect");
	}

    public override void _Process(double delta)
    {
		Vector2 mos_pos = GetLocalMousePosition();// * _flip;
		if (mos_pos == Vector2.Zero) return;
        float angle = (mos_pos.Angle() - float.Tau / 2 / Options.Count + float.Tau / 4) * -1;
		if (angle < 0.0f)
		{
			angle += float.Tau;
		}
		float rad = angle % float.Tau;
		Select = (int)Math.Ceiling(rad / float.Tau * Options.Count);
		if (_coords[Select - 1] is Vector2 vector)
		{
			_select.Position = vector;
		}
    }

	public override void _Draw()
	{
		_coords = new List<Vector2>();
		DrawArc(Vector2.Zero, _radius - 50, 0, float.Tau, 128, Colors.Black, 4, true);
		int i = 0;
		foreach (var item in Options)
		{
			i++;
			var angle = Vector2.FromAngle((float.Tau * (i - 1) / Options.Count) - float.Tau / 4) * _flip;
			var pos = _radius * angle + _offset;
			string name = item.GetType().Name;
			string type = item.GetType().BaseType.Name;
			DrawTexture(
				GD.Load<Texture2D>($"res://{type}s/{name}/{name}.tres"),
				pos
			);
			_coords.Add(pos);
		}
	}
}
