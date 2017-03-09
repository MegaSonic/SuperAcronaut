// Interface that handles any objects that can be absorbed by the player, or by other game objects
// Stick this on Game Objects to add "Absorbable" as an adjective
public interface IAbsorbable  {
    int absorbAmount
    {
        get;
    }

}
