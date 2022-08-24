namespace Stud;

public abstract class BaseItem : BaseCarriable
{
	public abstract float Cost { get; }

	public virtual float JumpBoost => 0;

	public virtual bool CanCarry()
	{
		if ( !IsOwned() )
		{
			(Owner as Player).BuyShit(this);
			return false;
		}

		return true;
	}

	public bool IsOwned()
	{
		if ( Cost == 0 )
			return true;

		return (Owner as Player).OwnedItems.Contains( TypeLibrary.GetDescription( GetType() ).Title );
	}
}
