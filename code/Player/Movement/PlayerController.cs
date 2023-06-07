namespace Stud;

public partial class PlayerController : PawnController
{
	[Net] private float jumpBoost { get; set; }

	private static float StudToInch => 11f;
	private float jumpStrength => 50f * StudToInch + jumpBoost;
	private float speed => 16f * StudToInch;

	private Vector3 mins = new Vector3( -16f, -16f, 0f );
	private Vector3 maxs = new Vector3( 16f, 16f, 72f );

	public BBox CollisionBox => new( mins, maxs );
	public Vector3 LastMoveDir { get; set; } = Vector3.Forward;

	float gravity => 196.2f * StudToInch;

	public override void Simulate()
	{
		if ( Pawn is not Player pawn ) return;

		if ( Sandbox.Game.IsServer )
		{
			jumpBoost = ((pawn.Inventory as BaseInventory).Active as BaseItem)?.JumpBoost ?? 0;
		}

		EyeLocalPosition = Vector3.Up * (CollisionBox.Maxs.z - 8);

		// Handle wished direction and speed.
		var wishVelocity = pawn.InputDirection.WithZ( 0 );
		if ( wishVelocity != 0 )
			LastMoveDir = wishVelocity * Rotation.FromYaw( pawn.EyeRotation.Yaw() );
		var inSpeed = wishVelocity.Length.Clamp( 0, 1 );
		var rot = Rotation.FromYaw( pawn.EyeRotation.Yaw() );
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
			if ( Input.Down( "Jump" ) )
			{

				Velocity += Vector3.Up * ( jumpStrength + pawn.RankIndex * 30 );

				if ( Sandbox.Game.IsServer )
				{

					pawn.Experience += 1 + (int)(pawn.RankIndex / 5f);

					Sound.FromEntity( "sounds/jump_spring.sound", Pawn ).SetVolume( 3 );

				}
			}
		}

		// Get and set new position according to velocity.
		var helper = new MoveHelper( Position, Velocity )
		{
			MaxStandableAngle = 60f
		};
		helper.Trace = helper.Trace.Size( CollisionBox.Mins, CollisionBox.Maxs )
			.IncludeClientside()
			.Ignore( Pawn );
		helper.TryUnstuck();
		helper.TryMoveWithStep( Time.Delta, 2f * StudToInch );

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
