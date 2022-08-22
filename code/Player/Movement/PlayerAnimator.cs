namespace Stud;

public partial class PlayerAnimator : PawnAnimator
{
	public override void Simulate()
	{
		if ( Pawn is not Player pawn ) return;
		if ( pawn.Controller is not PlayerController controller ) return;

		var rot = Rotation.LookAt( controller.LastMoveDir.WithZ( 0f ), Vector3.Up );
		Rotation = Rotation.Lerp( Rotation, rot, Time.Delta * 10f );

		var helper = new CitizenAnimationHelper( Pawn as AnimatedEntity );
		helper.WithVelocity( Velocity );
		helper.IsGrounded = GroundEntity != null;
	}	
	
	public override void FrameSimulate()
	{
		if ( Local.Pawn is not Player pawn ) return;

		EyeRotation = Input.Rotation;
	}
}
