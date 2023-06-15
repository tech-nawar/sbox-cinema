using Sandbox;
using Cinema;

namespace Conna.Inventory;

public partial class ItemEntity : ICinemaUse
{
    public bool IsUsable(Entity user)
    {
        if (user is not Cinema.Player) return false;

        return true;
    }

    public void OnStopUse(Entity user)
    {

    }

    public bool OnUse(Entity user)
    {
        if (user is not Cinema.Player player)
            return false;

        var pickedUp = player.PickupItem(Item);
        if (pickedUp) Take();

        return false;
    }
}
