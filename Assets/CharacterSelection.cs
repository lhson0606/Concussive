using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    public GameObject knightPrefab; // Reference to the Knight prefab

    public void OnMouseDown()
    {
        // Check if the clicked object is the Knight
        Debug.Log("Knight selected");
        SelectCharacter("Knight");
    }

    void SelectCharacter(string character)
    {
        // Save the selected character's data

        // Load the game scene (replace "PrototypeMap" with your actual game scene name)
        SceneManager.LoadScene("TutorialScene");
    }
}
