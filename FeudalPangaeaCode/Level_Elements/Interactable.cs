using Godot;

//this one is just a classification for things that the player can touch
//not really designed for many interactables stacked on each other
//might need to change how this works later
public interface Interactable
{
    public void Interact();
    public bool InRangeOfPlayer();
    public Vector3 GetPosition();
    public void Highlighted(bool highlighted);
}