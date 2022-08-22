namespace Stud;

[HammerEntity]
[Model( Model = "models/checkpoint.vmdl" )]
[Display( Name = "Checkpoint", GroupName = "Entities", Description = "Spawn here" )]
public partial class Checkpoint : ModelEntity
{

	[Property]
	public int Level { get; set; } = 0;

	public Checkpoint() {}

	public Checkpoint( int level )
	{

		Level = level;

	}
	public override void Spawn()
	{

		base.Spawn();

		SetModel( "models/checkpoint.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
		Tags.Add( "trigger" );

	}

	[ClientRpc]
	public void CheckpointAnimation()
	{

		

	}

}
