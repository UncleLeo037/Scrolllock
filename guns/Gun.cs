using Godot;
using System;

namespace Srolllock.guns
{
	public partial class Gun : Node3D
	{
		public string equipedSpell = string.Empty;

		public virtual void Shoot()
		{
			//do nothing
		}
	}
}