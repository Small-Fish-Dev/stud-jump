namespace Stud;

public partial class AdminController : PawnController
{

	private float speed => 16f * Game.StudToInch;

	private Vector3 mins = new Vector3( -16f, -16f, 0f );
	private Vector3 maxs = new Vector3( 16f, 16f, 72f );

	public BBox CollisionBox => new( mins, maxs );
	public Vector3 LastMoveDir { get; set; } = Vector3.Forward;

	public override void Simulate()
	{

		if ( Pawn is not Player pawn ) return;

		EyeLocalPosition = Vector3.Up * (CollisionBox.Maxs.z - 8);

		// Handle wished direction and speed.
		var wishVelocity = pawn.InputDirection.WithZ( 0 );
		if ( wishVelocity != 0 )
			LastMoveDir = wishVelocity * pawn.EyeRotation;

		var inSpeed = wishVelocity.Length.Clamp( 0, 1 );
		var rot = EyeRotation;
		wishVelocity = wishVelocity.Normal
			* inSpeed
			* speed
			* rot;

		// Smoothen the movement a little.
		Velocity = Vector3.Lerp( Velocity, wishVelocity, 15f * Time.Delta ) + new Vector3( 0f, 0f, Input.Down( InputButton.Jump ) ? speed / 3 : Input.Down( InputButton.Duck ) ? -speed / 3 : 0 );

		Velocity *= Input.Down( InputButton.Run ) ? 1.4f : 1f;
		// Get and set new position according to velocity.
		var helper = new MoveHelper( Position, Velocity );
		helper.Trace = helper.Trace.Size( CollisionBox.Mins, CollisionBox.Maxs )
			.IncludeClientside()
			.Ignore( Pawn );
		helper.TryUnstuck();
		helper.TryMoveWithStep( Time.Delta, 2f * Game.StudToInch );

		Position = helper.Position;
		Velocity = helper.Velocity;

	}
}
