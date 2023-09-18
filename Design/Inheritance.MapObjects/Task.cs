namespace Inheritance.MapObjects
{
    public abstract class MapObject
    {

    }

    public interface IHaveOwner
    {
        int Owner { get; set; }
    }

    public interface IHaveArmy
    {
        Army Army { get; set; }
    }

    public interface IHaveTreasure
    {
        Treasure Treasure { get; set; }
    }

    public class Dwelling : MapObject, IHaveOwner
    {
        public int Owner { get; set; }
    }

    public class Mine : MapObject, IHaveOwner, IHaveArmy, IHaveTreasure
    {
        public int Owner { get; set; }
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
    }

    public class Creeps : MapObject, IHaveArmy, IHaveTreasure
    {
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
    }

    public class Wolves : MapObject, IHaveArmy
    {
        public Army Army { get; set; }
    }

    public class ResourcePile : MapObject, IHaveTreasure
    {
        public Treasure Treasure { get; set; }
    }

    public static class Interaction
    {
        public static void Make(Player player, MapObject mapObject)
        {
            if (mapObject is IHaveArmy haveArmy)
                if (!player.CanBeat(haveArmy.Army))
                    player.Die();

            if (player.Dead)
                return;

            if (mapObject is IHaveOwner haveOwner)
                haveOwner.Owner = player.Id;

            if (mapObject is IHaveTreasure haveTreasure)
                player.Consume(haveTreasure.Treasure);
        }
    }
}