namespace Stud;

public partial class PlayerController : PawnController
{
	private float jumpStrength { get; set; } = 375f;
	private float speed { get; set; } = 215;

	private Vector3 mins = new Vector3( -16f, -16f, 0f );
	private Vector3 maxs = new Vector3( 16f, 16f, 72f );

	public BBox CollisionBox => new( mins, maxs );
	public Vector3 LastMoveDir { get; set; }
	
	float gravity => 700f;
	
	public override void Simulate()
	{
		EyeRotation = Input.Rotation;
		EyeLocalPosition = Vector3.Up * (CollisionBox.Maxs.z - 8);

		// Handle wished direction and speed.
		var wishVelocity = new Vector3( Input.Forward, Input.Left, 0 );
		if ( wishVelocity != 0 )
			LastMoveDir = wishVelocity * Rotation.FromYaw( Input.Rotation.Yaw() );

		var inSpeed = wishVelocity.Length.Clamp( 0, 1 );
		var rot = Rotation.FromYaw( Input.Rotation.Yaw() );
		wishVelocity = wishVelocity.Normal 
			* inSpeed
			* speed
			* rot;

		// Smoothen the movement a little.
		Velocity = Vector3.Lerp( Velocity, wishVelocity, 15f * Time.Delta )
			.WithZ( Velocity.z );

		// Apply gravity.
		if ( GroundEntity == null )
			Velocity += Vector3.Down * gravity * Time.Delta;
		else
		{
			// Jumping.
			if ( Input.Pressed( InputButton.Jump ) )
				Velocity += Vector3.Up * jumpStrength;
		}

		// Get and set new position according to velocity.
		var helper = new MoveHelper( Position, Velocity )
		{
			MaxStandableAngle = 60f
		};
		helper.Trace = helper.Trace.Size( CollisionBox.Mins, CollisionBox.Maxs )
			.Ignore( Pawn );
		helper.TryUnstuck();
		helper.TryMove( Time.Delta );

		Position = helper.Position;
		Velocity = helper.Velocity;

		// GroundEntity check.
		if ( Velocity.z <= 2f )
		{
			var tr = helper.TraceDirection( Vector3.Down * 2.0f );

			GroundEntity = tr.Entity;

			if ( GroundEntity != null )
			{
				Position += tr.Distance * Vector3.Down;

				if ( Velocity.z < 0.0f )
					Velocity = Velocity.WithZ( 0 );
			}
		}
		else
			GroundEntity = null;
	}
}
