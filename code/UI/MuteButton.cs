namespace Stud;

public class MuteButton : Button
{

	Texture muted;
	Texture unmuted;

	public MuteButton()
	{
		
		muted = Texture.Load( "ui/muted.png" );
		unmuted = Texture.Load( "ui/unmuted.png" );

		AddEventListener( "onclick", () =>
		{

			Game.MusicPlaying = !Game.MusicPlaying;

		} );

	}

	[Event.Tick]
	public void MuteUnmute()
	{

		Style.BackgroundImage = Game.MusicPlaying ? unmuted : muted;

	}

	[Event.BuildInput]
	private void buildInput( InputBuilder input )
	{
		Style.PointerEvents = input.Down( InputButton.SecondaryAttack )
			? PointerEvents.None
			: PointerEvents.All;

	}

}
