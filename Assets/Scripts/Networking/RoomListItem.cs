using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class RoomListItem : MonoBehaviour
{

    //вид делегата
    public delegate void JoinRoomDelegate(MatchInfoSnapshot _matchInfoSnapshot);
    //сам делегат
    public JoinRoomDelegate joinRoomCallback;


    [SerializeField]
    private Text roomNameText;//ссылка на текст названия комнаты

    private MatchInfoSnapshot matchInfoSnapshot;//информация о созданной руме(о хосте)

    public void Setup(MatchInfoSnapshot _matchInfoSnapshot, JoinRoomDelegate _joinRoomCallback)
    {
        matchInfoSnapshot = _matchInfoSnapshot;
        joinRoomCallback = _joinRoomCallback;

        roomNameText.text = matchInfoSnapshot.name + " (" + matchInfoSnapshot.currentSize + "/" +
                            matchInfoSnapshot.maxSize + ")";
        Debug.Log(roomNameText);

    }

    public void JoinRoom()
    {
        joinRoomCallback.Invoke(matchInfoSnapshot);
    }



}
