using UnityEngine;

/// <summary>
/// Represents an object that can respond to clicks.
/// </summary>
public interface IClickable { // Makes object clickable. Put into OnClick whatever you want to run on click (UI opening, selecting the object, etc)
    void OnClick();
}
