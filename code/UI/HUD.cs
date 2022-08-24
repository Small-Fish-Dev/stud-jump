namespace Stud;

class HUD : RootPanel
{
	public static HUD Instance { get; set; }

	[Event.Hotload]
	[Event("start")]
	private static void createHUD()
	{
		if ( Host.IsServer ) return;

		if ( Instance != null )
			Instance.Delete();

		Instance = new HUD();
	}

	public HUD()
	{
		SetTemplate( "/ui/Layout.html" );
	}
}
