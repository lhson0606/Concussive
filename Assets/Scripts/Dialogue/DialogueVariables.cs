using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class DialogueVariables
{
    public Dictionary<string, Ink.Runtime.Object> variables { get; private set; }

    private Story globalVariablesStory;
    private string loadedVariablesJson;
    public DialogueVariables(TextAsset loadGlobalsJSON)
    {
        //Load tat ca cac bien trong file Json thanh file Story
        globalVariablesStory = new Story(loadGlobalsJSON.text);

        //Load bien Json vao file story
        if(loadedVariablesJson != null)
        {
            string jsonState = loadedVariablesJson.ToString();
            globalVariablesStory.state.LoadJson(jsonState);
        }
        //Tao danh sach cac bien bang kieu dictionanry
        variables = new Dictionary<string, Ink.Runtime.Object>();
        foreach(string name in globalVariablesStory.variablesState)
        {
            Ink.Runtime.Object value = globalVariablesStory.variablesState.GetVariableWithName(name);
            variables.Add(name, value);
        }
    }
    public void StartListening(Story story)
    {
        VariablesToStory(story);
        story.variablesState.variableChangedEvent += VariableChanged;
    }
    
    public void StopListening(Story story)
    {
        story.variablesState.variableChangedEvent -= VariableChanged;
    }

    private void VariableChanged(string name, Ink.Runtime.Object value)
    {
        if (variables.ContainsKey(name))
        {
            variables.Remove(name);
            variables.Add(name, value);
        }
    }

    //Ham truyen bien vao file story
    private void VariablesToStory(Story story)
    {
        foreach(KeyValuePair<string, Ink.Runtime.Object> variable in variables)
        {
            story.variablesState.SetGlobal(variable.Key, variable.Value);
        }
    }

    // public void SaveVariable(GameData gameData)
    // {
    //     if(globalVariablesStory != null)
    //     {
    //         VariablesToStory(globalVariablesStory);
    //         gameData.dialougeVariables = globalVariablesStory.state.ToJson();
    //     }
    // }

    // public void LoadVariablesJson(GameData gameData)
    // {
    //     loadedVariablesJson = gameData.dialougeVariables;
    // }

}
