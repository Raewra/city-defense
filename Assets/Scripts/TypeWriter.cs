using System.Collections;
using TMPro;
using UnityEngine;

public class TypeWriter : MonoBehaviour
{ 
    public TextMeshProUGUI dialogText;
    public float typingSpeed = 0.05f;
    public bool IsTyping { get; private set; }
    public void StartTyping(string sentence)
    {
        StopAllCoroutines(); 
        StartCoroutine(TypeSentence(sentence));
    }

    
    private IEnumerator TypeSentence(string sentence)
    {
        IsTyping = true;
        dialogText.text = "";
        foreach (char letter in sentence)
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        IsTyping = false;
    }
}
