namespace Stud;

[HammerEntity]
[Model( Model = "models/checkpoint.vmdl" )]
[Display( Name = "Checkpoint", GroupName = "Entities", Description = "Spawn here" )]
public partial class Checkpoint : ModelEntity
{

	[Property]
	public int Level { get; set; } = 0;

	public Checkpoint() { }

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

	public override void Touch( Entity other )
	{

		base.Touch( other );

		if ( other is not Player player ) return;

		if ( IsServer && (player.CheckpointReached == null || player.CheckpointReached.Level < Level) )
		{

			player.CheckpointReached = this;
			player.Experience += Level;

			if ( player.Client.IsBot ) return;

			CheckpointAnimation( To.Single( player.Client ) );

			GameServices.UpdateLeaderboard( player.Client.PlayerId, Level, "Studs" );

		}

	}

	private ModelEntity animationModel;
	private float animationStart;
	private float animationSpeed;

	[ClientRpc]
	public void CheckpointAnimation()
	{

		animationModel = new ModelEntity();
		animationModel.SetModel( Model.ResourcePath );
		animationModel.SetMaterialOverride( Material.Load( "materials/dev/primary_white_emissive.vmat" ) );
		animationModel.Position = Position;
		animationModel.Rotation = Rotation;

		animationStart = Time.Now;
		animationSpeed = Time.Delta;

	}

	[Event.Tick.Client]
	private void animateModel()
	{

		if ( animationModel == null ) return;

		animationSpeed *= 1f + Time.Delta * 8f;

		animationModel.Scale = 1f + (Time.Now - animationStart) * 2.5f - animationSpeed;
		animationModel.RenderColor = new Color( 1f, 1f, 1f, 1f - (Time.Now - animationStart) );

		if ( animationModel.Scale < 0.9f )
		{

			animationModel.Delete();
			animationModel = null;

		}

	}

}
