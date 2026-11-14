using Godot;
using System;

public partial interface ISpell
{
    public double Cooldown { get; set; }
    public float Modifier { get; set; }
}
