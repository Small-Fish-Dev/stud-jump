namespace Stud;

public partial class Game
{

	static Sound backgroundMusic;
	public static bool MusicPlaying = true;

	[Event.Tick.Client]
	public void LoadMusic()
	{

		if ( MusicPlaying )
		{

			if ( backgroundMusic.Finished )
			{

				backgroundMusic = Sound.FromScreen( "goumi-remix" ); // You cannot stop it.

			}

		}
		else
		{

			backgroundMusic.Stop();

		}

	}

}
