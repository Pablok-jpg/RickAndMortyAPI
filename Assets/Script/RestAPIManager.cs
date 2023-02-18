using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RestAPIManager : MonoBehaviour
{
    [SerializeField] private List<RawImage> YourRawImage;
    [SerializeField] public List<TextMeshProUGUI> textNames;

    [SerializeField] private int userID = 1;
    private List<int> generatedNumbers = new List<int>();
    public int[] cards;

    [SerializeField] private string myApiPath = "https://my-json-server.typicode.com/Pablok-jpg/RickAndMortyAPI";
    [SerializeField] private string rickApiPath = "https://rickandmortyapi.com/api/character";
    void Start()
    {

    }

    public void GetPlayerInfoClick()
    {
        userID = Random.Range(1, 6);
        StartCoroutine(GetPlayerInfo());
    }

    public void IDRandomizer(int number)
    {
        StartCoroutine(GetPlayerInfo());
    }


    IEnumerator GetPlayerInfo()
    {
        UnityWebRequest www = UnityWebRequest.Get(myApiPath + "/users/" + userID);
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: " + www.error);
        }
        else
        {

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

            byte[] results = www.downloadHandler.data;
        }
    }
    IEnumerator GetCharacters(int ID, int place)
    {
        UnityWebRequest www = UnityWebRequest.Get(rickApiPath + ID);
        Debug.Log(www);
        yield return www.Send();

        if (www.isNetworkError)
        {
            Debug.Log("NETWORK ERROR: " + www.error);
        }
        else
        {
            //Debug.Log(www.GetResponseHeader("content-type"));

            // Show results as text
            Debug.Log(www.downloadHandler.text);

            if (www.responseCode == 200)
            {
                string jsonb = www.downloadHandler.text;
                Character character = JsonUtility.FromJson<Character>(jsonb);

                Debug.Log("Name : " + character.name);

                textNames[place].text = character.name;
                StartCoroutine(DownloadImage(character.image, place));
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

            byte[] results = www.downloadHandler.data;
        }
    }
    IEnumerator DownloadImage(string MediaUrl, int place)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError) Debug.Log(request.error);
        else YourRawImage[place].texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }
}
[System.Serializable]
public class Character
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


