namespace Stud;

public partial class Player : AnimatedEntity
{
	[Net, Predicted] public PawnController Controller { get; set; }
	[Net, Predicted] public PawnAnimator Animator { get; set; }
	public Checkpoint CheckpointReached { get; set; }

	public ClothingContainer Clothing = new();

	public override void Spawn()
	{
		Controller ??= new PlayerController();
		Animator ??= new PlayerAnimator();

		var controller = Controller as PlayerController;

		SetModel( "models/citizen/citizen.vmdl" );
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, controller.CollisionBox.Mins, controller.CollisionBox.Maxs );

		Tags.Add( "player" );

		Respawn();
	}

	public void Respawn()
	{
		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Position = Entity.All
			.OfType<Checkpoint>()
			.Where( x => x.Level == 0 )
			.First().Position;

		Rotation = Transform.Zero.Rotation;

		ResetInterpolation();
	}


	public void UpdateEyePosition()
	{
		if ( Controller is not PlayerController controller ) return;

		EyePosition = Position
			+ EyeLocalPosition;
	}

	public override void Simulate( Client cl )
	{
		Controller?.Simulate( cl, this, Animator );
		
		if ( Host.IsServer )
			UpdateEyePosition();
	}

	public override void FrameSimulate( Client cl )
	{
		Controller?.FrameSimulate( cl, this, Animator );
		UpdateEyePosition();
	}

	public override void BuildInput( InputBuilder input )
	{
		base.BuildInput( input );

		input.ViewAngles += input.AnalogLook;
		input.ViewAngles.pitch = input.ViewAngles.pitch.Clamp( -89, 89 );
		input.ViewAngles.roll = 0;

		input.InputDirection = input.AnalogMove;
	}

	TimeSince timeSinceLastFootstep = 0;

	public override void OnAnimEventFootstep( Vector3 pos, int foot, float volume )
	{
		if ( !IsClient )
			return;

		if ( timeSinceLastFootstep < 0.2f )
			return;

		timeSinceLastFootstep = 0;

		var tr = Trace.Ray( pos, pos + Vector3.Down * 20 )
			.Radius( 1 )
			.Ignore( this )
			.Run();

		if ( !tr.Hit ) return;

		volume *= Velocity.WithZ( 0 ).Length.LerpInverse( 0.0f, 200.0f ) * 0.5f;
		tr.Surface.DoFootstep( this, tr, foot, 1f );
	}

	public void SetCheckpoint( Checkpoint checkpoint )
	{

		checkpoint.CheckpointAnimation( To.Single( Client ) );

	}

}
