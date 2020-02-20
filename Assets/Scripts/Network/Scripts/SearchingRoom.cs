using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SearchingRoom : MonoBehaviour
{
    public List<TextMeshProUGUI> listWord;
    public InputField input;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < listWord.Count; i++){
            if(listWord[i].text.Contains(input.text)){
                listWord[i].transform.parent.gameObject.SetActive(true);
            }
            else{
                listWord[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
