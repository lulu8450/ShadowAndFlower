using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    // Image associée au dialogue (personnage qui parle)
    public Sprite sprite;
    // Le nom du personnage qui parle (Hadès, Perséphone, Narrateur...)
    public string speakerName;

    // Le texte affiché à l'écran pour ce personnage
    [TextArea(3, 10)]
    public List<string> sentences;

    // True si ce dialogue ouvre sur un choix (Joute Verbale)
    public bool hasChoices = false;

    // Liste des choix (Joutes verbales) possibles si hasChoices est true
    public List<Choice> choices;

    // Référence au prochain dialogue à afficher après cette phrase (si pas de choix)
    public DialogueData nextDialogue;
}

// Structure des choix (Joute Verbale)
[System.Serializable]
public struct Choice
{
    // Le texte que le joueur voit et sélectionne (ex: "Vulnérabilité", "Défi")
    public string choiceText; 
    
    // Impact sur la relation avec le personnage (valeur positive ou négative)
    public float linkImpact; 
    
    // Référence au dialogue qui suit si ce choix est fait
    public DialogueData nextDialogue; 
}
