using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ProfileUI : MonoBehaviour
{
    public GoogleLogin googleLogin; // Reference to the GoogleLogin script

    public TextMeshProUGUI userNameText;
    public TextMeshProUGUI emailText;
    public TextMeshProUGUI idText;
    public Image profilePicture;

    void Start()
    {
        // Delay to give enough time for GoogleLogin.cs to fetch the profile
        StartCoroutine(WaitForUserProfile());
    }

    IEnumerator WaitForUserProfile()
    {
        // Wait until user profile data is available
        while (googleLogin.userProfile == null)
        {
            yield return null;
        }

        // Populate the UI with user profile data
        userNameText.text = $"{googleLogin.userProfile.name}";
        emailText.text = $"{googleLogin.userProfile.email}";
        idText.text = $"{googleLogin.userProfile.id}";

        // Load and display profile picture from URL
        StartCoroutine(LoadProfilePicture(googleLogin.userProfile.picture));
    }

    IEnumerator LoadProfilePicture(string url)
    {
        using (WWW www = new WWW(url))
        {
            yield return www;
            profilePicture.sprite = Sprite.Create(
                www.texture,
                new Rect(0, 0, www.texture.width, www.texture.height),
                Vector2.one * 0.5f);
        }
    }
}
