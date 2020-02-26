using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomListingsMenu : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform content;
    [SerializeField]
    private RoomListing _roomListing;

    private List<RoomListing> _listings = new List<RoomListing>();
    public RoomCanvasGroup _roomsCanvases;
    private RoomListing listing;
   


    public void FirstInitialize(RoomCanvasGroup canvases){
        _roomsCanvases = canvases;
        content.DestroyChildren();
    }

    public override void OnJoinedRoom(){
        
        _roomsCanvases.CurrentRoomCanvas.Show();
        content.DestroyChildren();
        _listings.Clear();
        
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList){
        foreach(RoomInfo info in roomList){
            if (info.RemovedFromList){
                int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                if (index != -1){
                    Destroy(_listings[index].gameObject);
                    _listings.RemoveAt(index);
                    Debug.Log("hello");
                }
            }
            else{
                int index = _listings.FindIndex(x => x.RoomInfo.Name == info.Name);
                if(index == -1){
                    listing = Instantiate(_roomListing, content);
                    if (listing != null){
                        listing.SetRoomInfo(info);
                        _listings.Add(listing);
                        Debug.Log("hello2");
                        SearchingRoom.instance.listWord.Add(listing);
                        
                    }   
                }
                else{
                    //_listings[index].
                }              
            }
        }
    }
}
