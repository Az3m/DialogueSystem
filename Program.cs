using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public static class Program
{
    private static Stack<(string npcId, string dialogueTitle)> dialogueHistory = new Stack<(string, string)>();

    static void Main(string[] args)
    {
        string dialogueCSVPath = @"E:\Coding Lessons Alex\Lessons2_InterfaceVSAbstract\DialogueSystem\dialogue.csv";

        DialogueManager dialogueManager = new DialogueManager();

        List<NPC> NPCs = dialogueManager.LoadDialoguesFromJson(@"E:\Coding Lessons Alex\Lessons2_InterfaceVSAbstract\DialogueSystem\dialogue.json");

        foreach (NPC npc in NPCs)
        {
            Console.WriteLine(npc.ID);
            foreach (Dialogue dialogue in npc.Dialogues)
            {
                Console.WriteLine(dialogue);
            }
        }

        // dialogueManager.LoadDialoguesFromCsv(dialogueCSVPath);

        // // Start with Jack's Greetings dialogue
        // dialogueHistory.Push(("Jack", "Greetings!"));
        // RunDialogueSystem(dialogueManager, "Jack", "Greetings!");

        // Console.WriteLine("\nPress any key to exit...");
        // Console.ReadKey();
    }

    // private static void RunDialogueSystem(DialogueManager dialogueManager, string npcId, string dialogueTitle)
    // {
    //     while (true)
    //     {
    //         NPC currentNpc = dialogueManager.GetNPC(npcId);
            
    //         if (currentNpc == null || currentNpc.Dialogues.Count == 0)
    //         {
    //             Console.WriteLine($"{npcId} not found or has no dialogues!");
    //             return;
    //         }

    //         // Find the specific dialogue by title
    //         Dialogue currentDialogue = currentNpc.Dialogues.FirstOrDefault(d => d.title == dialogueTitle);
    //         if (currentDialogue == null)
    //         {
    //             Console.WriteLine($"Dialogue '{dialogueTitle}' not found for {npcId}!");
    //             return;
    //         }

    //         DisplayNpcDialogue(currentNpc, currentDialogue);
    //         string nextTarget = ProcessUserChoice(currentDialogue, dialogueHistory.Count > 1);
            
    //         // Handle the back option
    //         if (nextTarget == "BACK")
    //         {
    //             dialogueHistory.Pop();
    //             if (dialogueHistory.Count > 0)
    //             {
    //                 var previous = dialogueHistory.Peek();
    //                 RunDialogueSystem(dialogueManager, previous.npcId, previous.dialogueTitle);
    //             }
    //             return;
    //         }
    //         // Handle dialogue transition (like moving to shop)
    //         else if (!string.IsNullOrEmpty(nextTarget))
    //         {
    //             // Check if it's a dialogue title (contains spaces, not just an NPC ID)
    //             if (nextTarget.Contains(" "))
    //             {
    //                 // It's a dialogue title within the same NPC
    //                 dialogueHistory.Push((npcId, nextTarget));
    //                 RunDialogueSystem(dialogueManager, npcId, nextTarget);
    //             }
    //             else
    //             {
    //                 // It's an NPC ID
    //                 dialogueHistory.Push((nextTarget, GetFirstDialogueTitle(dialogueManager, nextTarget)));
    //                 RunDialogueSystem(dialogueManager, nextTarget, GetFirstDialogueTitle(dialogueManager, nextTarget));
    //             }
    //             return;
    //         }
    //     }
    // }

    private static string GetFirstDialogueTitle(DialogueManager dialogueManager, string npcId)
    {
        NPC npc = dialogueManager.GetNPC(npcId);
        return npc?.Dialogues.FirstOrDefault()?.title ?? "Greetings!";
    }

    private static void DisplayNpcDialogue(NPC npc, Dialogue dialogue)
    {
        Console.Clear();
        Console.WriteLine($"=== {npc.ID} ===");
        Console.WriteLine($"{dialogue.title}");
        Console.WriteLine("\nOptions:");
        
        // Show Back option if we have history (more than just the current dialogue)
        bool showBackOption = dialogueHistory.Count > 1;
        DisplayDialogueOptions(dialogue.GetOptions(), showBackOption);
    }

    private static void DisplayDialogueOptions(List<DialogueOption> options, bool showBackOption)
    {
        for (int i = 0; i < options.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {options[i].text}");
        }
        
        if (showBackOption)
        {
            Console.WriteLine($"{options.Count + 1}. Back");
        }
    }

    // private static string ProcessUserChoice(Dialogue dialogue, bool hasHistory)
    // {
    //     int maxOption = dialogue.GetOptions().Count;
    //     if (hasHistory) maxOption++;
        
    //     string input = GetUserInput("\nChoose an option: ");
        
    //     if (TryParseChoice(input, maxOption, out int choice))
    //     {
    //         if (hasHistory && choice == maxOption)
    //         {
    //             Console.WriteLine("\nGoing back...");
    //             return "BACK";
    //         }
            
    //         DialogueOption selectedOption = dialogue.GetOptions()[choice - 1];
    //         Console.WriteLine($"\nYou chose: {selectedOption.text}");
            
    //         // Execute the action and get the return value
    //         string actionResult = selectedOption.ExecuteAction();
            
    //         // If the action wants to transition, return the target immediately
    //         if (!string.IsNullOrEmpty(actionResult))
    //         {
    //             return actionResult;
    //         }
            
    //         // For actions that don't transition, wait for user to continue
    //         Console.WriteLine("\n1. Continue");
    //         string continueInput = Console.ReadLine();
            
    //         if (continueInput == "1")
    //         {
    //             return null; // Stay on current dialogue
    //         }
            
    //         return null;
    //     }
    //     else
    //     {
    //         Console.WriteLine("Invalid choice!");
    //     }
        
    //     return null;
    // }

    private static string GetUserInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }

    private static bool TryParseChoice(string input, int maxOptions, out int choice)
    {
        return int.TryParse(input, out choice) && choice >= 1 && choice <= maxOptions;
    }
}

public class NPC
{
    public string ID { get; private set; }
    public List<Dialogue> Dialogues { get; private set; }

    public NPC(string id)
    {
        ID = id;
        Dialogues = new List<Dialogue>();
    }

    public void AddDialogue(Dialogue dialogue)
    {
        Dialogues.Add(dialogue);
    }

    public override string ToString()
    {
        return "ID: " + ID + "Dialogues: \n" + Dialogues;
    }
}

public class DialogueManager
{
    private Dictionary<string, NPC> m_Npcs = new();

    // public void LoadDialoguesFromCsv(string filePath)
    // {
    //     string[] allLines = File.ReadAllLines(filePath);

    //     for (int i = 1; i < allLines.Length; i++)
    //     {
    //         string[] fields = allLines[i].Split(',');
    //         string npcId = fields[0].Trim();
    //         string title = fields[1].Trim();
    //         string optionText = fields[2].Trim();
    //         string actionType = fields[3].Trim();

    //         iDialogueAction action = CreateActionFromString(actionType);

    //         var dialogueOption = new DialogueOption
    //         {
    //             text = optionText,
    //             m_DialogueAction = action
    //         };

    //         if (!m_Npcs.TryGetValue(npcId, out NPC npc))
    //         {
    //             npc = new NPC(npcId);
    //             m_Npcs[npcId] = npc;
    //         }

    //         Dialogue dialogue = npc.Dialogues.FirstOrDefault(d => d.title == title);
    //         if (dialogue == null)
    //         {
    //             dialogue = new Dialogue { title = title, options = new List<DialogueOption>() };
    //             npc.AddDialogue(dialogue);
    //         }

    //         dialogue.options.Add(dialogueOption);
    //     }
    // }

    public List<NPC> LoadDialoguesFromJson(string jsonName)
    {
        string jsonContent = File.ReadAllText(jsonName);
        List<NPC> NPCs = JsonConvert.DeserializeObject<List<NPC>>(jsonContent);

        return NPCs;

    }

    private iDialogueAction CreateActionFromString(string actionType)
    {
        return actionType.Trim().ToLower() switch
        {
            "openshop" => new OpenStoreDialogueAction(),
            "startcombat" => new CombatDialogueAction(),
            "changenpc" => new ChangeNPCDialogueAction(),
            "enddialogue" => new ExitDialogueAction(),
            "continue" => new ContinueDialogueAction(),
            "exit" => new ExitDialogueAction(),
            _ => new ContinueDialogueAction()
        };
    }

    public List<string> GetAllNpcIds()
    {
        return m_Npcs.Keys.ToList();
    }

    public NPC GetNPC(string npcID)
    {
        if (m_Npcs.TryGetValue(npcID, out NPC npc))
        {
            return npc;
        }
        return null;
    }
}

public class Dialogue
{
    public string title;
    public List<DialogueOption> options;

    public List<DialogueOption> GetOptions()
    {
        return options ?? new List<DialogueOption>();
    }

    public override string ToString()
    {
        string result = $"Title: {title}\nOptions:\n";

        if (options != null)
        {
            foreach (DialogueOption dialogueOption in options)
            {
                result += $"- {dialogueOption}\n";
            }
        }
        else
        {
            result += "- ! THERE ARE NO OPTIONS !";
        }

        return result;
    }
}

public class DialogueOption
{
    public string text;
    //public iDialogueAction m_DialogueAction;
    public string m_DialogueAction;

    // public string ExecuteAction()
    // {
    //     return m_DialogueAction?.onDialogueOptionChosen();
    // }

    public override string ToString()
    {
        return $"Text: {text} -> Action: {m_DialogueAction}";
    }
}

public interface iDialogueAction
{
    public string onDialogueOptionChosen();
}

public class CombatDialogueAction : iDialogueAction
{
    public string onDialogueOptionChosen()
    {
        Console.WriteLine("=== COMBAT MODE ===");
        Console.WriteLine("You are now in combat!");
        Console.WriteLine("Enemies are attacking...");
        return null;
    }
}

public class ContinueDialogueAction : iDialogueAction
{
    public string onDialogueOptionChosen()
    {
        Console.WriteLine("=== DIALOGUE CONTINUED ===");
        Console.WriteLine("The conversation continues...");
        return null;
    }
}

public class OpenStoreDialogueAction : iDialogueAction
{
    public string onDialogueOptionChosen()
    {
        Console.WriteLine("Entering the shop...");
        return "Welcome to the shop!";
    }
}

public class ChangeNPCDialogueAction : iDialogueAction
{
    public string onDialogueOptionChosen()
    {
        Console.WriteLine("Switching to Rose...");
        return "Rose";
    }
}

public class ExitDialogueAction : iDialogueAction
{
    public string onDialogueOptionChosen()
    {
        Console.WriteLine("Goodbye! Closing program...");
        Environment.Exit(0);
        return null;
    }
}