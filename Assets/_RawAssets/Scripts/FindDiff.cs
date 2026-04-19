using UnityEngine;
using UnityEngine.UI;

public class FindDiff : MonoBehaviour
{
    public static bool OverCollider;

    private void Start()
    {

        GetComponent<Image>().enabled = false;
    }

    bool Selected
    {
        get
        {
            if (transform.parent.GetChild(0).GetComponent<Image>().enabled == false ||
                transform.parent.GetChild(1).GetComponent<Image>().enabled == false)
            {
                return false;
            }
            return true;
        }
    }

    private void OnMouseDown()
    {
        if (Selected == false && !UIManager.instance.IsAnyPopUpOpen())
        {
            LevelManager.instance.CurlevelDetail.rightscount++;
            GameManager.instance.TxtCorrect.text = LevelManager.instance.CurlevelDetail.rightscount + "/" + LevelManager.instance.CurlevelDetail.totalrights;
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                SoundHapticManager.Instance.playClip(SoundHapticManager.Instance.findMistake, 1);
                SoundHapticManager.Instance.Haptic();
                transform.parent.GetChild(i).GetComponent<Image>().sprite = LevelManager.instance.greenColor;
                transform.parent.GetChild(i).GetComponent<Image>().enabled = true;
                if (transform.parent.GetChild(i).GetComponent<Animator>())
                {
                    LevelManager.instance.isHintClickable = true;
                    Destroy(transform.parent.GetChild(i).GetComponent<Animator>());
                }
                transform.parent.GetChild(i).GetComponent<Image>().color = Color.white;
            }

            LevelManager.instance.Index.Remove(transform.parent.GetSiblingIndex());
            LevelDetails.winGameCheck?.Invoke();
        }
    }

    private void OnMouseOver()
    {
        OverCollider = true;
    }

    private void OnMouseExit()
    {
        OverCollider = false;
    }
}
