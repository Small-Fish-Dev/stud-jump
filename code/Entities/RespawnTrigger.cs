namespace Stud;

[HammerEntity]
[Display( Name = "Respawn Trigger", GroupName = "Entities", Description = "Trigger to respawn" )]
public partial class RespawnTrigger : BaseTrigger
{

	public RespawnTrigger() { }

	public override void Spawn()
	{

		base.Spawn();

	}

	public override void Touch( Entity other )
	{

		base.Touch( other );

		if ( other is not Player player ) return;

		player.Velocity = Vector3.Zero;
		player.Position = player.CheckpointReached.Position;

	}
}
