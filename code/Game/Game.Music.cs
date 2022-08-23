namespace Stud;

public partial class Game
{

	Sound? backgroundMusic;

	[Event.Tick.Server]
	public void LoadMusic()
	{

		if ( backgroundMusic == null )
		{

			backgroundMusic = Sound.FromScreen( "goumi-remix" );

		}
		else
		{

			if ( backgroundMusic.Value.Finished )
			{

				backgroundMusic = Sound.FromScreen( "goumi-remix" ); // You cannot stop it.

			}

		}

	}

}
