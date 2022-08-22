namespace Stud;

partial class Player
{
	public float Distance { get; set; }
	private float lastDist;

	public override void PostCameraSetup( ref CameraSetup setup )
	{
		if ( Local.Pawn is not Player pawn ) return;
		if ( pawn.Controller is not PlayerController controller ) return;

		lastDist = MathX.LerpTo( lastDist, Distance, 10f * Time.Delta );
		setup.Position = pawn.EyePosition + pawn.EyeRotation.Backward * lastDist;
		setup.Rotation = pawn.EyeRotation;
		setup.Viewer = null;
		setup.ZNear = 4f;
		setup.FieldOfView = 70;
	}

	[Event.BuildInput]
	private void cameraInput( InputBuilder input )
	{
		Distance = MathX.Clamp( Distance - input.MouseWheel * 5f, 100, 300 );
	}
}
