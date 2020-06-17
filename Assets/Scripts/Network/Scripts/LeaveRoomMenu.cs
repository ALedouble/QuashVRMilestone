using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class LeaveRoomMenu : MonoBehaviour
{
   public RoomCanvasGroup roomCanvases;

   public void FirstInitialize(RoomCanvasGroup canvases){
       roomCanvases = canvases;
   }

   public void OnClick_LeaveRoom(){
       PhotonNetwork.LeaveRoom(false);
       roomCanvases.CurrentRoomCanvas.Hide();
   }
}
