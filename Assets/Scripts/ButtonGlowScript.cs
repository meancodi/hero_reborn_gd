using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonGlowScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("BG + Colors")]
    public Image BGImage;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.blue;

    [Header("Glow")]
    public Outline outline;
    public Color glowColor = Color.blue;
    public float pulseSpeed = 2f;

    [Header("Hover Move")]
    public float moveDistance = 10f;   // how much to move to the right
    public float moveSpeed = 10f;      // how fast to move

    private Color originalGlowColor;
    private bool isHovered = false;

    private RectTransform rectTransform;
    private Vector2 originalPos;
    private bool canMoveRight = false;

    private IEnumerator RightMoveDelay()
    {
        yield return new WaitForSeconds(0.5f); // half-second delay
        canMoveRight = true;
    }

    void Start()
    {
        // Cache RectTransform + original position
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            originalPos = rectTransform.anchoredPosition;
        }

        if (outline != null)
        {
            isHovered = false;
            originalGlowColor = outline.effectColor;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        canMoveRight = false;  // reset
        StartCoroutine(RightMoveDelay());

        // Optional: change background color too
        if (BGImage != null)
            BGImage.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;

        // Optional: revert background color
        if (BGImage != null)
            BGImage.color = normalColor;
    }

    void Update()
    {
        // --- Glow logic ---
        if (outline != null)
        {
            if (isHovered)
            {
                float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
                outline.effectColor = Color.Lerp(originalGlowColor, glowColor, pulse);
            }
            else
            {
                outline.effectColor = Color.Lerp(outline.effectColor, originalGlowColor, Time.deltaTime * 5);
            }
        }

        // --- Move-on-hover logic ---
        if (rectTransform != null)
        {
            Vector2 targetPos = originalPos;

            if (isHovered)
            {
                // Step 1: Always move up a little
                targetPos += Vector2.up * (moveDistance / 5f);

                // Step 2: After delay, move right
                if (canMoveRight)
                    targetPos += Vector2.right * moveDistance;
            }

            rectTransform.anchoredPosition = Vector2.Lerp(
                rectTransform.anchoredPosition,
                targetPos,
                Time.deltaTime * moveSpeed
            );
        }

    }
}
