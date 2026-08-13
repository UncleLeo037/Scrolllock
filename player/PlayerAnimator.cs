using Godot;
using System;

public partial class PlayerAnimator : AnimationPlayer
{
	[Rpc(CallLocal = true)]
	public void PlayAnim(string animation)
	{
		Play(animation);
	}
}
