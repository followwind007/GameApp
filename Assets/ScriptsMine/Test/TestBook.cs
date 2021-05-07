using UnityEngine;
using GameApp.UI;

public class TestBook : MonoBehaviour
{
    public Book book;

    public BookFlipController controller;

    private void OnGUI()
    {
        if (GUILayout.Button("Init", GUILayout.Width(100)))
        {
            book.Init(10, 1);
        }
        
        if (GUILayout.Button("Init  __", GUILayout.Width(100)))
        {
            controller.Init(book);
        }
        
        if (GUILayout.Button("Prev", GUILayout.Width(100)))
        {
            controller.FlipToLeft();
        }
        
        if (GUILayout.Button("Next", GUILayout.Width(100)))
        {
            controller.FlipToRight();
        }
    }
}
