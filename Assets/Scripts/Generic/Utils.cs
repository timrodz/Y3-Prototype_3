using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Multi-purpose utilities
/// </summary>
public class Utils : MonoBehaviourSingletonPersistent<Utils>
// public static class Utils
{

    /// <summary>
    /// Debug with an x amount of parameters
    /// </summary>
    /// <param name="names"></param>
    public static void Print(params string[] names)
    {
        string s = "";

        for (int i = 0; i < names.Length - 1; i++)
            s += names[i] + " - ";
        s += names[names.Length - 1];

        Debug.Log(s);
    }

    public static Color ChangeOpacity(Color c, float alpha)
    {
        if (alpha >= 1)
        {
            alpha = 1;
        }
        else if (alpha < 0)
        {
            alpha = 0;
        }
        
        return (new Color(c.r, c.g, c.b, alpha));
    }

    public static void FadeCanvasGroupIn(CanvasGroup cg, float fadeDuration)
    {
        cg.DOFade(1, fadeDuration);
    }

    public static void FadeCanvasGroupOut(CanvasGroup cg, float fadeDuration)
    {
        cg.DOFade(0, fadeDuration);
    }

    public static void FadeTransformIn(Transform t, float fadeDuration)
    {
        t.DOScale(1, fadeDuration);
    }

    public static void FadeTransformOut(Transform t, float fadeDuration)
    {
        t.DOScale(0, fadeDuration);
    }

}