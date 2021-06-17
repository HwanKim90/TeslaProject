using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class PhotonController : MonoBehaviourPunCallbacks
{
    public GameObject leftSideCam;
    public GameObject rightSideCam;
    public GameObject centerCam;
    public GameObject minimapCam;
    public float bestTime;

    public GameObject[] players;
    public GameObject ranking;
    public Text[] buttons;

    public int passPlayerNum;
    bool tap;
    List<float> playerBestTime = new List<float>();
    //Dictionary<string, float> playerTimeDict = new Dictionary<string, float>();

    void Start()
    {
        if (photonView.IsMine)
        {
            leftSideCam.SetActive(true);
            rightSideCam.SetActive(true);
            centerCam.SetActive(true);
            minimapCam.SetActive(true);
        }
        else
        {
            leftSideCam.SetActive(false);
            rightSideCam.SetActive(false);
            centerCam.SetActive(false);
            minimapCam.SetActive(false);
        }
    }
    
    void Update()        
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        //print(players[0].GetComponent<PhotonView>().ViewID);
        //print(players[1].GetComponent<PhotonView>().ViewID);

        GetPlayersBestTime();
        //toggle();
        PlayerFinalCheck();

    }
    void toggle()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            tap = !tap;
            ranking.SetActive(tap);
        }
    }

    void PlayerFinalCheck()
    {   
        if (players[0].GetComponent<TimeRecord>().finalPass
            && players[1].GetComponent<TimeRecord>().finalPass
            && players[2].GetComponent<TimeRecord>().finalPass
            && players[3].GetComponent<TimeRecord>().finalPass)
        {
            passPlayerNum = players.Length;
            print("passPlayerNum : " + passPlayerNum);
        }
       
    }

    void GetPlayersBestTime()
    {
        
        // 게임이 끝났을때로 조건 바꿔
        if (Input.GetKeyDown(KeyCode.Tab) || passPlayerNum == players.Length)
        {
            //foreach (GameObject playerTime in players)
            //{
            //    float time = playerTime.GetComponent<TimeRecord>().bestTime;

            //    float minutes = Mathf.FloorToInt(time / 60);
            //    float second = Mathf.FloorToInt(time - minutes * 60);

            //    string timeST = string.Format("{0:0}:{1:00}", minutes, second);
            //    playerBestTime.Add(time);

            //    playerBestTime.Sort();
            //    print(playerTime.GetPhotonView().Owner + " " + timeST);
            //    //print(playerBestTime);
            //}
            
            for (int i = 0; i < players.Length; i++)
            {
                float time = players[i].GetComponent<TimeRecord>().bestTime;
                if (playerBestTime.Count < players.Length)
                {
                    playerBestTime.Add(time);
                    print("besttime : " + playerBestTime[i]);
                }
                //print(players[i].GetPhotonView().Owner + " " + timeST);
            }
            
            playerBestTime.Sort();
            print("sort1 : " + playerBestTime[0]);
            print("sort2 : " + playerBestTime[1]); // 여기 값이 들어갓다가 sort1값으로 바뀐다

            for (int i = 0; i < players.Length; i++) 
            {
                float minutes = Mathf.FloorToInt(playerBestTime[i] / 60);
                float second = Mathf.FloorToInt(playerBestTime[i] - minutes * 60);

                string timeST = string.Format("{0:0}:{1:00}", minutes, second);

                buttons[i].text = timeST;

                
                if (GetComponent<TimeRecord>().bestTime == playerBestTime[i])
                {   
                    buttons[i].color = Color.green;
                }
            }

            ranking.SetActive(true);
        }
    }
}
