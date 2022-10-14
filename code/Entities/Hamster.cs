using System.Text;

namespace Stud;

public partial class Hamster : ModelEntity
{

	float speed => 700f;
	float randomSeed;
	static float spawnEvery => 0.1f; // seconds

	AnimatedEntity clientModel;

	public override void Spawn()
	{

		Tags.Clear();
		Tags.Add( "trigger" );

		base.Spawn();

		Transmit = TransmitType.Never;
	}

	PhysicsGroup body;

	[Event.Tick]
	void computeMovement()
	{

		body ??= SetupPhysicsFromSphere( PhysicsMotionType.Keyframed, 0, 8f );

		if ( Host.IsClient )
		{

			clientModel ??= new AnimatedEntity( "models/hampter/hampter.vmdl" );
			clientModel.Position = Position;
			clientModel.Rotation = Rotation;
			clientModel.Scale = Scale;
			clientModel.Parent = this;

		}


		Position = Position + Rotation.Forward * Time.Delta * speed;
		Rotation = Rotation.FromYaw( 180 + MathF.Sin( Time.Now * 10 + randomSeed ) * 15 );

		if ( Position.x <= -260f )
		{

			Delete();

		}

		if ( ( Time.Tick + Rand.Int( 4000 ) ) % 150 == 0 && Host.IsClient )
		{

			
			Sound.FromEntity( "sounds/hampter.sound", clientModel ).SetVolume( 10 );

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

	[Event.Tick]
	static void spawnHamsters()
	{

		if ( Time.Tick % ( 60 * spawnEvery ) == 0 )
		{

			new Hamster()
			{
				Position = new Vector3( ( Noise.Perlin( Time.Tick * 5, Time.Tick % 60, Time.Tick * 1000 ) * 2 - 1) * ( Game.StudToInch * 15 * 3 ) + Game.StudToInch * 15 * 116, ( Noise.Perlin( Time.Tick * 6, Time.Tick % 35, Time.Tick * 140 ) * 2 - 1 ) * Game.StudToInch * 40, Game.StudToInch * 0.5f ),
				Scale = Time.Tick % ( 60 * spawnEvery * 700 ) == 0 ? 27f : ( Time.Tick % (60 * spawnEvery * 30) == 0 ? 9f : 3f ),
				randomSeed = Time.Tick * 5
			};


		}

	}

}
