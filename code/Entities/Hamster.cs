using System.Text;

namespace Stud;

public partial class Hamster : AnimatedEntity
{

	float speed => 700f;
	float randomSeed = Rand.Float( 9999999 );
	static float spawnEvery => 0.1f; // seconds

	public override void Spawn()
	{

		SetModel( "models/citizenstud/citizenstud.vmdl" );
		Scale = 0.3f;
		Rotation = Rotation.FromYaw( 180 );
		SetupPhysicsFromSphere( PhysicsMotionType.Keyframed, 0, 50f );

		Tags.Clear();
		Tags.Add( "trigger" );

		base.Spawn();
	}

	[Event.Tick.Server]
	void computeMovement()
	{

		Position = Position + Rotation.Forward * Time.Delta * speed;
		Rotation = Rotation.FromYaw( 180 + MathF.Sin( Time.Now * 10 + randomSeed ) * 15 );

		if ( Position.x <= -260f )
		{

			Delete();

		}

	}

	public override void StartTouch( Entity other )
	{

		if ( Host.IsClient ) return;

		if ( other is Player player )
		{

			player.Velocity += Rotation.Forward * 10000f + Vector3.Up * 1000f;

		}

	}

	[Event.Tick.Server]
	static void spawnHamsters()
	{

		if ( Time.Tick % ( 60 * spawnEvery ) == 0 )
		{

			new Hamster()
			{
				Position = new Vector3( Rand.Float( Game.StudToInch * 15 * 3 ) + Game.StudToInch * 15 * 116, Rand.Float( Game.StudToInch * -22, Game.StudToInch * 22 ), Game.StudToInch * 0.5f )
			};

		}

	}

}
