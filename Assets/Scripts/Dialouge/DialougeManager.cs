using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using System;
//using Ink.UnityIntegration;

public class DialogueManager : MonoBehaviour
{
    [Header("Param")]
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("Load Global JSON")]
    [SerializeField] private TextAsset loadGlobalsJSON;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text dialogueText;
    
    [SerializeField] private TMP_Text displayName;

    [Header("Dialogue Choices")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    //public Animator animator;
    public KeyCode[] interactKey;
    public bool dialogueIsPlaying { get; private set; }

    private static DialogueManager instance;

    private Story currentStory;
    private Coroutine displayLineCoroutine;

    private bool canContinueToNextLine = false;
    private bool canSkip = false;
    private bool submitSkip;

    private TagHandler tagHandler;
    private GameObject currentGameObject;

    private DialougeVariables dialougeVariables;
    // private InkExternalFunction inkExternalFunction;
    private void Awake()
    {
        instance = this;
        dialougeVariables = new DialougeVariables(loadGlobalsJSON);
        // inkExternalFunction = new InkExternalFunction(); // Dùng để bind hàm với code script
    }

    private void Start()
    {
        dialogueIsPlaying = false;
        dialogueBox.SetActive(false);
        choicesText = new TextMeshProUGUI[choices.Length];

        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }

        tagHandler = new TagHandler();
        tagHandler.SetSpeakerName(displayName);

    }

    private void Update()
    {

        if (!dialogueIsPlaying)
        {
            return;
        }

        foreach (KeyCode key in interactKey)
        {
            if (Input.GetKeyDown(key))
            {
                submitSkip = true;
            }
            if (canContinueToNextLine
                && currentStory.currentChoices.Count == 0 && Input.GetKeyDown(key))
            {
                ContinueStory();
            }
        }
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    public void EnterDialogueMode(TextAsset inkJSon)
    {
        currentStory = new Story(inkJSon.text);
        dialogueIsPlaying = true;
        dialogueBox.SetActive(true);

        dialougeVariables.StartListening(currentStory);
        // inkExternalFunction.Bind(currentStory,currentGameObject);

        ContinueStory();
    }

    public void ExitDialogueMode()
    {
        dialougeVariables.StopListening(currentStory);
        // inkExternalFunction.Unbind(currentStory);

        dialogueIsPlaying = false;
        dialogueBox.SetActive(false);
        dialogueText.text = "";
    }

    public void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            if(displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }
            String nextLine = currentStory.Continue();

            // if(nextLine.Equals("") && !currentStory.canContinue)
            // {
            //     ExitDialogueMode();
            // }
            
            //Xu li tag
            tagHandler.HandleTags(currentStory.currentTags);

            displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
            
            
        }
        else
        {
            ExitDialogueMode();
        }
    }

    

    public void PopUp()
    {
        dialogueBox.SetActive(true);
        //animator.SetBool("Pop", true);
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;
        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("Co qua nhieu option trong doan hoi thoai");
        }

        int index = 0;
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }
    }

    public void MakeChoice(int choiceIndex)
    {
        if (canContinueToNextLine)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);
            ContinueStory();
        }
    }
    
    private void HideChoices()
    {
        foreach(GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);  
        }
    }

    //Typing effect
    private IEnumerator DisplayLine(string line)
    {
        dialogueText.text = "";
        HideChoices();
        submitSkip = false;
        canContinueToNextLine = false;

        StartCoroutine(CanSkip());

        foreach(char letter in line.ToCharArray())
        {
            if (canSkip && submitSkip)
            {
                submitSkip = false;
                dialogueText.text = line;
                break;
            }
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        canContinueToNextLine = true;
        canSkip = false;
        DisplayChoices();
    }

    private IEnumerator CanSkip()
    {
        canSkip = false; //Making sure the variable is false.
        yield return new WaitForSeconds(0.05f);
        canSkip = true;
    }

    public void SetCurrentGameObject(GameObject gameObject)
    {
        this.currentGameObject = gameObject;
    }
    public GameObject GetCurrentGameObject()
    {
        return this.currentGameObject;  
    }

    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object variableValue = null;
        dialougeVariables.variables.TryGetValue(variableName, out variableValue);

        if (variableValue == null) 
        { 
            Debug.LogWarning("Ink Variable was not found to be null: " + variableName);
        }
        return variableValue;
    }

    // public void SaveData(GameData gameData)
    // {
    //     dialougeVariables.SaveVariable(gameData);
    // }
    // public void LoadData(GameData gameData)
    // {
    //     dialougeVariables.LoadVariablesJson(gameData);
    // }
}

