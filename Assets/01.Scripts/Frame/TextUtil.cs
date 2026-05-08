using System.Collections.Generic;
using System.Collections;
using System;
using TMPro;
using UnityEngine;
public static class TextUtil
{
    public static IEnumerator TypeText(TMP_Text targetText, string fullText, float charsPerSecond, Action onComplete = null)
    {
        if (targetText == null)
            yield break;

        targetText.text = fullText;
        targetText.ForceMeshUpdate();

        int totalChars = targetText.textInfo.characterCount;
        targetText.maxVisibleCharacters = 0;

        float delay = charsPerSecond <= 0f ? 0f : 1f / charsPerSecond;

        if (delay <= 0f)
        {
            targetText.maxVisibleCharacters = totalChars;
            onComplete?.Invoke();
            yield break;
        }

        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(delay);

        for (int i = 1; i <= totalChars; i++)
        {
            targetText.maxVisibleCharacters = i;
            yield return wait;
        }


        onComplete?.Invoke();

    }

    public static void ShowAll(TMP_Text targetText)
    {
        if (targetText == null)
            return;

        targetText.ForceMeshUpdate();
        targetText.maxVisibleCharacters = targetText.textInfo.characterCount;
    }

    public static void Clear(TMP_Text targetText)
    {
        if (targetText == null)
            return;

        targetText.text = string.Empty;
        targetText.maxVisibleCharacters = 0;
    }
    

    //조사 나눠지는부분

    private static readonly Dictionary<string, KeyValuePair<string, string>> _koreanParticles
    = new Dictionary<string, KeyValuePair<string, string>>
    {
        { "을/를", new KeyValuePair<string, string>("을", "를") },
        { "이/가", new KeyValuePair<string, string>("이", "가") },
        { "은/는", new KeyValuePair<string, string>("은", "는") },
    };

    public static string ApplyKoreanParticles(string text)
    {
        foreach (KeyValuePair<string, KeyValuePair<string, string>> particle in _koreanParticles)
        {
            int markerIndex = text.IndexOf(particle.Key);
            while (markerIndex > 0)
            {
                int prevIndex = markerIndex - 1;
                char prevChar = text[prevIndex];

                bool hasFinalConsonant = prevChar >= 0xAC00 && prevChar <= 0xD7A3 && ((prevChar - 0xAC00) % 28 > 0);
                string replaced = hasFinalConsonant ? particle.Value.Key : particle.Value.Value;

                text = text.Remove(prevIndex + 1, particle.Key.Length).Insert(prevIndex + 1, replaced);
                markerIndex = text.IndexOf(particle.Key);
            }
        }

        return text;
    }
}
