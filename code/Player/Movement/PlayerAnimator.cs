namespace Stud;

public partial class PlayerAnimator : PawnAnimator
{
	public override void Simulate()
	{
		if ( Pawn is not Player pawn ) return;

		if ( pawn.Controller is PlayerController plyController )
		{

			var rot = Rotation.LookAt( plyController.LastMoveDir.WithZ( 0f ), Vector3.Up );
			Rotation = Rotation.Lerp( Rotation, rot, Time.Delta * 10f );

		}

		if ( pawn.Controller is AdminController admController )
		{

			var rot = Rotation.LookAt( admController.LastMoveDir.WithZ( 0f ), Vector3.Up );
			Rotation = Rotation.Lerp( Rotation, rot, Time.Delta * 10f );

		}


		ComputeAnimation( pawn );

	}
	
	public override void FrameSimulate()
	{
		if ( Local.Pawn is not Player pawn ) return;

		EyeRotation = Input.Rotation;

		ComputeAnimation( pawn );

	}

	float animationStart = 0f;
	string currentAnimation = "idle";

	public void ComputeAnimation( Player pawn )
	{

		bool midair = pawn.GroundEntity == null;
		bool moving = pawn.Velocity.Length >= 5f;

		pawn.CurrentSequence.Name = midair ? "jump" : moving ? "run" : "idle";

		if ( pawn.CurrentSequence.Name != currentAnimation )
		{

			currentAnimation = pawn.CurrentSequence.Name;
			animationStart = Time.Now;

		}

		var animationTime = Time.Now - animationStart;

		pawn.CurrentSequence.Time = midair ? Math.Min( animationTime, pawn.CurrentSequence.Duration ) : animationTime % pawn.CurrentSequence.Duration;

	}
}
