namespace Stud;

public class Jumpscare : Panel
{

	TimeSince spawnTime;

	public Jumpscare()
	{

		spawnTime = 0f;

		var sound = Sound.FromScreen( "sounds/jumpscare.sound" );
		sound.SetVolume( 5f );

	}
	public override void Tick()
	{

		if ( spawnTime >= 1f )
		{

			Delete();

		}

	}

}
