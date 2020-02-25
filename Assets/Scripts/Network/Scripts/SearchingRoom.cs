using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SearchingRoom : MonoBehaviour
{
    public static SearchingRoom instance;
    public List<TextMeshProUGUI> listWord;
    public TextMeshProUGUI textBar;
    public RoomListing roomList;
    



    // Start is called before the first frame update
    void Start()
    {
      //  listWord.Add(roomList._text);
    }

    // Update is called once per frame
    void Update()
    {
        
        Debug.Log(listWord[0]);

        for(int i = 0; i < listWord.Count; i++){
            if(listWord[i].text.Contains(textBar.text)){
                listWord[i].transform.parent.gameObject.SetActive(true);
            }
            else{
                listWord[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
