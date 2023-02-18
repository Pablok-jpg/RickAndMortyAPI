using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class APIManager : MonoBehaviour
{
    [SerializeField] private RawImage YourRawImage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GetCharacterClick()
    {
       // StartCoroutine(GetCharacters());
    }

    IEnumerator GetCharacters(int ID, int place)
    {
        UnityWebRequest www = UnityWebRequest.Get("https://rickandmortyapi.com/api/character/" + ID);
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
           // Debug.Log(www.GetResponseHeader("content-type")); 
            // Show results as text
            Debug.Log(www.downloadHandler.text);

        }

        if (www.responseCode == 200)
        {
            string json = www.downloadHandler.text;
            JsonData user = JsonUtility.FromJson<JsonData>(json);

            Debug.Log("Username:" + user.name);
            Debug.Log("User ID:" + user.id);

            for (int i = 0; i < user.deck.Length; i++)
            {
                StartCoroutine(GetCharacters(user.deck[i], i));
                yield return new WaitForSeconds(0.1f);
            }

        }

        else if (www.responseCode == 404)
        {
            Debug.Log("Character not found!");
        }
        else
        {
            string mensaje = "Status: " + www.responseCode;
            mensaje += "\ncontent-type: " + www.GetResponseHeader("content-type");
            mensaje += "\nError : " + www.error;
            Debug.Log(mensaje);
        }
    }
    IEnumerator DownloadImage(string MediaUrl, int place)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError) Debug.Log(request.error);
        else YourRawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }
}

[System.Serializable]
public class CharacterAPP
{
    public string name;
    public string species;
    public string image;
}

[System.Serializable]
public class JsonData
{
    public int id;
    public string name;
    public int[] deck;
}
