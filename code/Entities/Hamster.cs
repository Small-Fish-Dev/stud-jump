using System.Text;

namespace Stud;

public partial class Hamster : AnimatedEntity
{

	int randomSeed = Rand.Int( 99999 );
	float speed => 300f;

	public override void Spawn()
	{

		Tags.Clear();
		Tags.Add( "trigger" );

		base.Spawn();

		SetModel( "models/hampter/hampter.vmdl" );
		SetupPhysicsFromSphere( PhysicsMotionType.Keyframed, 0, 8f );

		Transmit = TransmitType.Pvs;
	}


	[Event.Tick.Server]
	void computeMovement()
	{

		Position = Position + Rotation.Forward * Time.Delta * speed;
		Rotation = Rotation.FromYaw( 180 + MathF.Sin( Time.Now * 10 + randomSeed ) * 15 );

		var trace = Trace.Ray( Position, Position - Vector3.Up * 1000f )
			.Size( Scale * 8f)
			.WithoutTags( "player", "trigger" )
			.Run();

		Position = trace.EndPosition;

		if ( Position.x <= -260f )
		{

			Delete();

		}

		if ( ( Time.Tick + randomSeed ) % 60 == 0 && Host.IsClient )
		{

			
			Sound.FromEntity( "sounds/hampter.sound", this ).SetVolume( 10 );

		}

	}

	public override void StartTouch( Entity other )
	{

		if ( Host.IsClient ) return;

		if ( other is Player player )
		{

			player.Velocity += ( Rotation.Forward * 10000f + Vector3.Up * 1000f ) * Scale / 3;

		}

	}
}
