using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static CameraManager;

public class PopUpCanvasController : MonoBehaviour
{
    Transform mainCam;
    [Header("References")]
    public Image premadeImage;
    [SerializeField] Sprite happy;
    [SerializeField] Sprite sad;

    [Header("Debug")]
    CharacterHandler parentCharacter;

    private void Start()
    {
        mainCam = CameraManager.instance.mainCam.transform;
        premadeImage.gameObject.SetActive(false);
        parentCharacter = transform.parent.GetComponent<CharacterHandler>();

        InputManager.instance.NoWalkableNodesNearCarEvent += NoWalkablePathExists;
        Pathfinding.instance.NoPathCanBeFoundEvent += NoWalkablePathExists;
    }


    private void LateUpdate()
    {
        transform.forward = mainCam.forward;
    }
    private void NoWalkablePathExists()
    {
        if (InputManager.instance.GetSelectedCharacter() == parentCharacter)
            PopTheEmojiUp(false);
    }

    public void PopTheEmojiUp(bool isHappy)
    {
        Image clone = Instantiate(premadeImage, premadeImage.transform.parent);

        if (isHappy) clone.sprite = happy;
        else clone.sprite = sad;

        clone.gameObject.SetActive(true);
        clone.transform.localPosition = Vector3.zero;
        Vector3 initScale = clone.transform.lossyScale;

        Tween tween = clone.transform.DOScale(initScale / 1.5f, .5f).SetLoops(-1, LoopType.Yoyo);


        IEnumerator DestroyRoutine()
        {
            yield return new WaitForSeconds(1.5f);
            tween.Kill();
            Destroy(clone.gameObject);
        }

        StartCoroutine(DestroyRoutine());
    }
}
