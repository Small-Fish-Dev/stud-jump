namespace Stud;

[Library("mousecapture")]
public partial class MouseCapture : Panel
{
	public static MouseCapture Instance { get; set; }

	public MouseCapture()
	{
		Instance = this;
		Style.PointerEvents = PointerEvents.All;
	}

	[ClientRpc]
	public static void CaptureMouse()
	{
		if ( Instance is null )
			return;

		Instance.Style.PointerEvents = PointerEvents.None;
	}

	[ClientRpc]
	public static void ReleaseMouse()
	{
		if ( Instance is null )
			return;

		Instance.Style.PointerEvents = PointerEvents.All;
	}
}

