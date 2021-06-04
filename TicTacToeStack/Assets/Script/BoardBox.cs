using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardBox : MonoBehaviour
{
    private GameManager manager;
    [SerializeField] private int MyId;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
    }
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        // If this part of Board is clicked
        Debug.Log("Boad-" + MyId + " Clicked");
        // If Clicked by Player, send massage to Game Manager
        manager.BoardClicked(MyId);
    }
}
