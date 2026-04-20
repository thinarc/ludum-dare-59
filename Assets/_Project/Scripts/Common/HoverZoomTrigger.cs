using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverZoomTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Image (этот объект)")]
    public Image hoverImage;

    [Header("Список изображений для зума")]
    public List<RectTransform> targetImages;

    [Header("Параметры зума")]
    public float zoomScale = 1.15f;
    public float animationDuration = 0.15f;

    [Header("Кривая анимации")]
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Dictionary<RectTransform, Vector3> originalScales = new Dictionary<RectTransform, Vector3>();
    private Dictionary<RectTransform, Coroutine> runningCoroutines = new Dictionary<RectTransform, Coroutine>();

    // цвет по умолчанию и целевой
    private Color originalColor;
    public Color hoverColor = Color.red;

    public GameObject text;
    public Image binded;

    private void Awake()
    {
        if (hoverImage == null)
        {
            hoverImage = GetComponent<Image>();
            if (hoverImage) targetImages.Add(hoverImage.rectTransform);
            if (binded) targetImages.Add(binded.rectTransform);
        }
        

        if (hoverImage != null)
        {
            originalColor = hoverImage.color;
        }
        

        foreach (var rt in targetImages)
        {
            if (rt == null) continue;
            originalScales[rt] = rt.localScale;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // смена цвета
        if (hoverImage != null)
        {
            hoverImage.color = hoverColor;
            if (binded) binded.color = hoverColor;
            if (text != null) text.SetActive(true);
        }

        // зум других элементов
        foreach (var rt in targetImages)
        {
            if (rt == null) continue;
            StartScale(rt, originalScales[rt] * zoomScale);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // возврат цвета
        if (hoverImage != null)
        {
            hoverImage.color = originalColor;
            if (binded) binded.color = originalColor;
            if (text != null) text.SetActive(false);
        }

        // возврат масштаба
        foreach (var rt in targetImages)
        {
            if (rt == null) continue;
            StartScale(rt, originalScales[rt]);
        }
    }

    private void StartScale(RectTransform rt, Vector3 targetScale)
    {
        if (runningCoroutines.ContainsKey(rt) && runningCoroutines[rt] != null)
            StopCoroutine(runningCoroutines[rt]);

        runningCoroutines[rt] = StartCoroutine(ScaleRoutine(rt, targetScale));
    }

    private IEnumerator ScaleRoutine(RectTransform rt, Vector3 targetScale)
    {
        Vector3 start = rt.localScale;
        float t = 0f;

        while (t < animationDuration)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / animationDuration);
            float c = animationCurve.Evaluate(k);

            rt.localScale = Vector3.LerpUnclamped(start, targetScale, c);
            yield return null;
        }

        rt.localScale = targetScale;
    }
}
