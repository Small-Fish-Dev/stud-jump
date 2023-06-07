namespace Stud;

public partial class Game
{

	static Sound backgroundMusic;
	public static bool MusicPlaying = true;

	public void StartMusic()
	{
		if (!Sandbox.Game.IsClient)
			return;

		backgroundMusic = Sound.FromScreen("goumi-remix");
	}

	[GameEvent.Tick.Client]
	public void LoadMusic()
	{
		if (!backgroundMusic.IsPlaying)
			StartMusic();

		backgroundMusic.SetVolume(MusicPlaying ? 1f : 0f);
	}

}
