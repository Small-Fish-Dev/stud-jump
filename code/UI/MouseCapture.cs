namespace Stud;

public partial class MouseCapture : Panel
{
	public MouseCapture()
	{
		Style.PointerEvents = PointerEvents.All;
		
	}

	[Event.BuildInput]
	private void buildInput()
	{

		foreach( var panel in FindRootPanel().Children )
		{

			panel.Style.PointerEvents = Input.Down( InputButton.SecondaryAttack )
				? PointerEvents.None
				: PointerEvents.All;

		}

	}
}

