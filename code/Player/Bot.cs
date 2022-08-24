﻿namespace Stud;
public class StudBot : Bot
{
	[ConCmd.Admin( "bot_custom", Help = "Spawn my custom bot." )]
	internal static void SpawnCustomBot( float crazy = 1f )
	{
		Host.AssertServer();

		// Create an instance of your custom bot.
		var bot = new StudBot( crazy );

	}

	public float Crazyness { get; set; } = 1f;
	private TimeSince lifeTime = 0f;
	private float randSeed;

	public StudBot( float crazy )
	{

		Crazyness = Math.Min( crazy, 10 );
		randSeed = Rand.Float( 500f );

	}

	public override void BuildInput( InputBuilder builder )
	{

		builder.SetButton( InputButton.Jump, false );
		builder.InputDirection = Client.Pawn.Rotation.Forward;

		if ( Client.Pawn.GroundEntity != null )
		{

			if ( Noise.Perlin( lifeTime * 10f, randSeed, randSeed ) * 2.2f - 0.5f >= (Crazyness / 20f * 1.8f) )
			{

				var startPos = Client.Pawn.Position + Vector3.Up * 2.5f * Game.StudToInch;
				var endPos = startPos + Client.Pawn.Rotation.Forward * 32f;
				var wallTrace = Trace.Ray( startPos, endPos )
					.Size( Game.StudToInch / 2f )
					.Ignore( Client.Pawn )
					.Run();

				if ( wallTrace.Hit )
				{

					builder.SetButton( InputButton.Jump, true );

				}

			}

		}

	}

	public override void Tick()
	{

		Client.Pawn.Rotation = Rotation.FromYaw( ( Noise.Perlin( lifeTime * 10f * Crazyness / 2f, randSeed, randSeed ) - 0.5f ) * 40f * Crazyness );

	}
}
