using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PonManager : MonoBehaviour
{
    private int idSize;
    private int ownerId;

    private GameManager manager;

    private Vector3 selectedSize = new Vector3(0.06f, 0.06f, 0f);

    private PhotonView photonView;

    private bool isLocked;

    void Start()
    {
        manager = FindObjectOfType<GameManager>();
        photonView = this.GetComponent<PhotonView>();
        isLocked = false;
    }

   
    void Update()
    {
        
    }

    public void SetIdSize(int a)
    {
        idSize = a;
    }
    public int GetIdSize()
    {
        return idSize;
    }
    public void SetOwnerId(int a)
    {
        ownerId = a;
    }
    public int GetOwnerId()
    {
        return ownerId;
    }

    private void OnMouseDown()
    {
        // Check is this Pon is Mine (Player) and it's not locked (or placed)
        if (photonView.isMine && !isLocked)
        {
            this.transform.localScale += selectedSize;

            Debug.Log("Clicked-" + idSize);

            manager.LockSelectedPon(this.gameObject);
        }
    }

    public void ResetSize()
    {
        // If this pon is De-selected or Placed in board, return size to normal
        this.transform.localScale -= selectedSize;
    }

    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        // It's like Constructor, the data send from Player
        object[] TheData = this.gameObject.GetPhotonView().instantiationData; 
        idSize = (int)TheData[0];
        ownerId = (int)TheData[1];
        this.transform.localScale -= (Vector3)TheData[2];

        float[] rawColor = (float[])TheData[3];
        Color PonColor = new Color(rawColor[0], rawColor[1], rawColor[2], rawColor[3]);
        this.GetComponent<SpriteRenderer>().color = PonColor;
    }

    public void SetLock(bool a)
    {
        // Change isLock value
        isLocked = a;
    }
}
