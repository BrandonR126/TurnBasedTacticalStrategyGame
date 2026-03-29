using UnityEngine;
/// <summary>
/// Represents an object that can be selected and deselected in the scene.
/// Must also be clickable (i.e., implement <see cref="IClickable"/>).
/// </summary>
public interface ISelectable : IClickable
{
    void Selected();
    void Deselected();
}
