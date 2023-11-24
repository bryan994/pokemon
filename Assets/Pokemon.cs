using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class Sprites
{
    public string front_default;
}

public class Species
{
    public string name;
}

public class PokemonData
{
    public string name;
    public string height;
    public Species species;
    public Sprites sprites;
    // Add other fields as per your JSON structure
}
public class Pokemon : MonoBehaviour
{
    [SerializeField] RawImage pokeImg;
    [SerializeField] TextMeshProUGUI pokeName;

    private string apiUrl = "https://pokeapi.co/api/v2/pokemon/";
    // Start is called before the first frame update
    void Start()
    {
        GetPokemon();
    }

    public void GetPokemon()
    {
        // Start the coroutine to make the API request
        StartCoroutine(GetDataFromAPI());
    }

    IEnumerator GetDataFromAPI()
    {
        // Create a UnityWebRequest object with the API URL
        int randomIndex = Random.Range(0, 100);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl + randomIndex))
        {

            // Use DownloadHandlerBuffer for large data
            DownloadHandlerBuffer downloadHandler = new DownloadHandlerBuffer();
            webRequest.downloadHandler = downloadHandler;

            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            // Check for errors
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                string json = downloadHandler.text;
                PokemonData pokeData = JsonUtility.FromJson<PokemonData>(json);

                StartCoroutine(LoadImageFromURL("https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/" + randomIndex + ".png"));
                pokeName.text = pokeData.name;

            }
        }
    }

    IEnumerator LoadImageFromURL(string imageUrl)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error loading image: " + webRequest.error);
            }
            else
            {
                // Set the texture to the RawImage component
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);
                pokeImg.texture = texture;
            }
        }
    }
}
