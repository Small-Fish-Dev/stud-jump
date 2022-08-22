namespace Stud;

public partial class PlayerAnimator : PawnAnimator
{
	public override void Simulate()
	{
		if ( Pawn is not Player pawn ) return;
		if ( pawn.Controller is not PlayerController controller ) return;

		var rot = Rotation.LookAt( controller.LastMoveDir );
		Rotation = Rotation.FromYaw( rot.Yaw() );

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
