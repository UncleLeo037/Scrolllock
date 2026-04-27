using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Srolllock.spells
{
	public partial class Spell : Node3D
	{
		[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
		public void Dispel()
		{
			this.QueueFree();
		}
	}
}
