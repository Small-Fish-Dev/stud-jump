namespace Stud;

public partial class PlayerAnimator : PawnAnimator
{
	public override void Simulate()
	{
		if ( Pawn is not Player pawn ) return;
		if ( pawn.Controller is not PlayerController controller ) return;

		var rot = Rotation.LookAt( controller.LastMoveDir.WithZ( 0f ), Vector3.Up );
		Rotation = Rotation.Lerp( Rotation, rot, Time.Delta * 10f );

		ComputeAnimation( pawn );

	}	
	
	public override void FrameSimulate()
	{
		if ( Local.Pawn is not Player pawn ) return;

		EyeRotation = Input.Rotation;

		ComputeAnimation( pawn );

	}

	TimeSince animationStart = 0f;
	string currentAnimation = "idle";

	public void ComputeAnimation( Player pawn )
	{

		bool midair = pawn.GroundEntity == null;
		bool moving = pawn.Velocity.Length >= 5f;

		pawn.CurrentSequence.Name = midair ? "jump" : moving ? "run" : "idle";

		if ( pawn.CurrentSequence.Name != currentAnimation )
		{

			currentAnimation = pawn.CurrentSequence.Name;
			animationStart = 0f;

		}

		pawn.CurrentSequence.Time = midair ? Math.Min( animationStart, pawn.CurrentSequence.Duration ) : animationStart % pawn.CurrentSequence.Duration;

	}
}
