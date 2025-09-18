using UnityEngine;

public class AvatarAppearance : MonoBehaviour
{

    public Transform avatarHair;

    void Start()
    {
        SetHair();
    }

    void SetHair()
    {
        if (PlayerPrefs.HasKey("hairSelection"))
        {
            int hair = PlayerPrefs.GetInt("hairSelection");
            avatarHair.GetChild(hair).gameObject.SetActive(true);
        }
        else
        {
            avatarHair.GetChild(0).gameObject.SetActive(true);
        }
    }

    
}
