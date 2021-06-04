using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer BoardGraphic;

    private void Awake()
    {   
        /// Chnage board color
        BoardGraphic.color = new Color(73/255f, 247/255f, 102/255f, 1f);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
