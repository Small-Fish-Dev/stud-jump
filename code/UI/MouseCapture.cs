namespace Stud;

public partial class MouseCapture : Panel
{
	public MouseCapture()
	{
		Style.PointerEvents = PointerEvents.All;
		
	}

	[GameEvent.Tick]
	private void buildInput()
	{

		foreach( var panel in FindRootPanel().Children )
		{

			panel.Style.PointerEvents = Input.Down("LockMouse")
				? PointerEvents.None
				: PointerEvents.All;

		}

	}
}

