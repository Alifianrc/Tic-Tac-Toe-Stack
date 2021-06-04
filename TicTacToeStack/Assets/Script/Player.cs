using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Photon.MonoBehaviour
{
    private int MyID;

    private const int PonTotal = 5;

    [SerializeField] GameObject Pon;
    private GameObject[] PonList;

    object[] PonCustomData;

    float[] PonColor;

    private void Awake()
    {
        // List of Pon owned by this player
        PonList = new GameObject[PonTotal];
        // This player Id
        MyID = PhotonNetwork.player.ID;
        // Custom data for instantiate Pon
        PonCustomData = new object[4];
        // Pon color
        PonColor = new float[4];
    }

    void Start()
    {
        // Calculate Pon Position based on Screen Size
        Vector2 cameraMinPos = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        Vector2 cameraMaxPos = Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight));

        float xPos = (cameraMaxPos.x - cameraMinPos.x) / 11;
        float yPos = (cameraMaxPos.y - cameraMinPos.y) / 10.5f;

        float positionX = cameraMinPos.x + (xPos / 2) + (xPos * 3);
        float positionY = 0;

        if (PhotonNetwork.isMasterClient)
        {
            // Coloring
            positionY = cameraMinPos.y + yPos;
            PonColor[0] = 249 / 255f;
            PonColor[1] = 237 / 255f;
            PonColor[2] = 73 / 255f;
            PonColor[3] = 1f;
        }
        else
        {
            //Coloring
            positionY = cameraMaxPos.y - yPos;
            PonColor[0] = 224 / 255f;
            PonColor[1] = 50 / 255f;
            PonColor[2] = 80 / 255f;
            PonColor[3] = 1f;
        }

        for (int i = 0; i < PonTotal; i++)
        {
            // Fill Custon data for Instantiate Pon
            PonCustomData[0] = i + 1;
            PonCustomData[1] = MyID;
            PonCustomData[2] = new Vector3(0.4f - (i * 0.1f), 0.4f - (i * 0.1f), 0f);
            PonCustomData[3] = PonColor;

            // Determine Final Pon Position
            Vector3 Pos = new Vector3(positionX, positionY, 5 - (i));
            // Instantiate Pon
            PonList[i] = PhotonNetwork.Instantiate("Pon", Pos, Quaternion.identity, 0, PonCustomData);
            //PonList[i] = Instantiate(Pon, Pos, Quaternion.identity);
            positionX += xPos;

            //PonList[i].GetComponent<PonManager>().SetIdSize(i + 1);
            //PonList[i].GetComponent<PonManager>().SetOwnerId(MyID);
            //PonList[i].transform.localScale -= new Vector3(0.4f-(i * 0.1f), 0.4f - (i * 0.1f), 0f);
        }
    }

    
    void Update()
    {
        
    }

    int GetMyId()
    {
        return MyID;
    }
}
