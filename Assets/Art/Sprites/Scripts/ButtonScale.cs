using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class ButtonScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject parentObject;
    public Vector3 targetScale;
    public float scaleDuration = 0.3f;

    private Vector3 initialScale;
    private bool isScaling = false;
    private Coroutine scaleCoroutine;
    public Button myButton;
    Animator Anim;
    private bool isHighlighted = false;

    private void Awake()
    {
        initialScale = parentObject.transform.localScale;
    }
    public void Start()
    {
        Anim = GetComponent<Animator>();
        myButton = GetComponent<Button>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
            isHighlighted = true;
      //  if (scaleCoroutine != null)
          //  StopCoroutine(scaleCoroutine);
      //  scaleCoroutine = StartCoroutine(ScaleAnimation(targetScale));   
    }

    public void OnPointerExit(PointerEventData eventData)
    {
            isHighlighted = false;
            //if (scaleCoroutine != null)
               // StopCoroutine(scaleCoroutine);
           // scaleCoroutine = StartCoroutine(ScaleAnimation(initialScale));
       
    }

    private IEnumerator ScaleAnimation(Vector3 target)
    {
        isScaling = true;

        float elapsedTime = 0f;
        Vector3 startScale = parentObject.transform.localScale;
        while (elapsedTime < scaleDuration)
        {
            float t = elapsedTime / scaleDuration;
            parentObject.transform.localScale = Vector3.Lerp(startScale, target, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        parentObject.transform.localScale = target;
        isScaling = false;
    }
    private void Update()
    {
        if (myButton != null && Anim != null)
        {
            if (myButton.interactable)
            {
                if (myButton == EventSystem.current.currentSelectedGameObject && isHighlighted)
                {
                    PlayAnimation("Selected");
                }
                else if (isHighlighted)
                {
                    PlayAnimation("Highlighted");
                }
                else if (Input.GetMouseButton(0) && isHighlighted)
                {
                    PlayAnimation("Pressed");
                }
                else
                {
                    PlayAnimation("Normal");
                }
            }
            else if (!isHighlighted)
            {
                PlayAnimation("Disabled");
            }
        }
    }
    public void PlayAnimation(string animationName)
    {
        if (Anim != null)
        {
            Anim.Play(animationName);
        }
    }

}
