namespace Stud;
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

		var pawn = Client.Pawn as Player;

		builder.SetButton( InputButton.Jump, false );
		builder.InputDirection = pawn.Rotation.Forward;

		if ( Client.Pawn.GroundEntity != null )
		{

			if ( Noise.Perlin( lifeTime * 10f, randSeed, randSeed ) * 2.2f - 0.5f >= (Crazyness / 20f * 1.8f) )
			{

				var startPos = Client.Pawn.Position + Vector3.Up * 3f * Game.StudToInch;
				var endPos = startPos + pawn.Rotation.Forward * 50f;
				var wallTrace = Trace.Ray( startPos, endPos )
					.Size( Game.StudToInch * 4f )
					.WithTag( "trigger" )
					.Ignore( pawn )
					.Run();

				if ( wallTrace.Hit )
				{

					builder.SetButton( InputButton.Jump, true );

				}

			}

			var closeEntities = Entity.FindInSphere( pawn.Position, Game.StudToInch * 30f );

			Checkpoint closestCheckpoint = closeEntities
				.OfType<Checkpoint>()
				.OrderBy( x => x.Position.Distance( pawn.Position ) )
				.FirstOrDefault( pawn.CheckpointReached );

			if ( closestCheckpoint != pawn.CheckpointReached )
			{

				if ( closestCheckpoint.Level > pawn.CheckpointReached.Level )
				{

					closestCheckpoint.Reached( pawn );

				}

			}

		}

	}

	public static string[] BotPhrases = new string[]
	{
		"i just wanna play normal stud jump",
		"what is going on!?!?",
		"what are all these hamsters doing here?",
		"is this hamsteria?",
		"what did small fish do now?!?!",
		"WTF!",
		"WHAT THE FRICK",
		"where are the steps",
		"is this meant to be hard?",
		"i reached pink! finally!",
		"hey i had admin where did it go",
		"guys do not join that other dedicated server, it sucks",
		"wait, WHAT?!",
		"HAMSTERS EVERYWHERE",
		"my pc is burning",
		"I AM AFRAID OF HAMSTERS",
		"HAMSTERS INCOMING!!!",
		"uhhh.... poop...",
		"lololol",
		"this is EPIC",
		"if only i had joined small fish discord to learn about this event"
	};

	TimeUntil nextMessage = Rand.Float( 30f );

	public override void Tick()
	{

		Client.Pawn.Rotation = Rotation.FromYaw( ( Noise.Perlin( lifeTime * 10f * Crazyness / 2f, randSeed, randSeed ) - 0.5f ) * 40f * Crazyness );

		if ( nextMessage <= 0f )
		{

			(Client.Pawn as Player).Say( BotPhrases[ Rand.Int( BotPhrases.Count() - 1 )] );
			nextMessage = Rand.Float( 6f, 30f );

		}

	}
}
