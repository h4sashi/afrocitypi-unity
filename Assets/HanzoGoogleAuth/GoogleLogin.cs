using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[System.Serializable]
public class GoogleUserProfile
{
    public string id;
    public string email;
    public bool verified_email;
    public string name;
    public string given_name;
    public string family_name;
    public string picture;
}

public class GoogleLogin : MonoBehaviour
{
    // Your Google Client ID
    private string clientId = "131729982261-5qba02r0rtendlb9reps1edo2ccjq6rk.apps.googleusercontent.com";
    private string redirectUri = "https://afrocity-auth-google.onrender.com/auth/google/callback";

    private string state; // Unique state for each login
    private string authUrl;
    public ProfileUI profileUI;

    public GoogleUserProfile userProfile;

    public void LoginWithGoogle()
    {
        // Generate a unique state for this login session (can use GUID)
        state = System.Guid.NewGuid().ToString();

        authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                  $"client_id={clientId}" +
                  $"&redirect_uri={redirectUri}" +
                  $"&response_type=code" +
                  $"&scope=email%20profile" +
                  $"&access_type=offline" +
                  $"&state={state}"; // Pass state to the server

        Application.OpenURL(authUrl);

        // Start polling for user profile after redirect
        StartCoroutine(PollForUserProfile());
    }

    IEnumerator PollForUserProfile()
    {
        string profileEndpoint = $"https://afrocity-auth-google.onrender.com/getProfile?state={state}";

        while (true)
        {
            UnityWebRequest request = UnityWebRequest.Get(profileEndpoint);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Parse the JSON response to GoogleUserProfile class
                userProfile = JsonUtility.FromJson<GoogleUserProfile>(request.downloadHandler.text);
                Debug.Log($"User Profile: {userProfile.name}, {userProfile.email}");
                profileUI.enabled = true;

                // Break out of the loop once profile is retrieved
                break;
            }

            // If profile not found, retry after 2 seconds
            yield return new WaitForSeconds(2f);
        }
    }
}
