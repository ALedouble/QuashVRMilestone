using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SearchingRoom : MonoBehaviour
{
    public static SearchingRoom instance;
    public List<RoomListing> listWord;
    public TextMeshProUGUI textBar;
    public RoomListing roomList;

    void Awake() {
        instance = this;
    }

    void Update()
    {
        if (listWord.Count == 0){
            return;
        }
        else{
            UpdateList();
        }
       
        
    }

    void UpdateList(){
        for(int i = 0; i < listWord.Count; i++){
            if(listWord[i]._text.text.Contains(textBar.text)){
                listWord[i].transform.gameObject.SetActive(true);
            }
            else{
                listWord[i].transform.gameObject.SetActive(false);
            }
        }
    }
}
