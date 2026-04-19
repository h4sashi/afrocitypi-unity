using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;

public class PlayFabManager : MonoBehaviour
{
    private static PlayFabManager instance;
    
    [SerializeField] private string nextSceneName = "MainGame"; // Change this to your scene name
    [SerializeField] private bool useDeviceId = true;

    private GoogleLogin googleLoginScript;
    private GoogleUserProfile currentUserProfile;

    public static PlayFabManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayFabManager>();
                if (instance == null)
                {
                    GameObject managerObject = new GameObject("PlayFabManager");
                    instance = managerObject.AddComponent<PlayFabManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Find the GoogleLogin script in the scene
        googleLoginScript = FindObjectOfType<GoogleLogin>();
        
        if (googleLoginScript == null)
        {
            Debug.LogError("GoogleLogin script not found in the scene!");
        }
    }

    /// <summary>
    /// Call this method from GoogleLogin after successful Google authentication
    /// </summary>
    public void OnGoogleLoginSuccess(GoogleUserProfile userProfile)
    {
        currentUserProfile = userProfile;
        Debug.Log($"Received Google profile: {userProfile.email}");
        
        // Register or login with PlayFab using Google profile data
        RegisterOrLoginWithPlayFab(userProfile);
    }

    /// <summary>
    /// Alternative method: Call this directly from GoogleLogin
    /// </summary>
    public void AuthenticateWithGoogle()
    {
        if (googleLoginScript == null)
        {
            googleLoginScript = FindObjectOfType<GoogleLogin>();
        }

        if (googleLoginScript != null && googleLoginScript.userProfile != null)
        {
            OnGoogleLoginSuccess(googleLoginScript.userProfile);
        }
        else
        {
            Debug.LogError("Google user profile not available yet!");
        }
    }

    private void RegisterOrLoginWithPlayFab(GoogleUserProfile userProfile)
    {
        // Create a custom username from email (remove domain)
        string customUsername = userProfile.email.Split('@')[0] + "_" + userProfile.id.Substring(0, 8);
        
        var request = new RegisterPlayFabUserRequest
        {
            Username = customUsername,
            Email = userProfile.email,
            Password = GenerateSecurePassword(userProfile.id),
            DisplayName = userProfile.name,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request,
            success =>
            {
                Debug.Log("PlayFab registration successful!");
                UpdatePlayerProfile(userProfile);
                LoadNextScene();
            },
            error =>
            {
                // If registration fails (user might already exist), try login
                Debug.LogWarning($"Registration failed: {error.GenerateErrorReport()}");
                LoginWithPlayFab(userProfile);
            });
    }

    private void LoginWithPlayFab(GoogleUserProfile userProfile)
    {
        string customUsername = userProfile.email.Split('@')[0] + "_" + userProfile.id.Substring(0, 8);
        string password = GenerateSecurePassword(userProfile.id);

        var request = new LoginWithPlayFabRequest
        {
            Username = customUsername,
            Password = password
        };

        PlayFabClientAPI.LoginWithPlayFab(request,
            success =>
            {
                Debug.Log("PlayFab login successful!");
                LoadNextScene();
            },
            error =>
            {
                Debug.LogError($"PlayFab login failed: {error.GenerateErrorReport()}");
            });
    }

    private void UpdatePlayerProfile(GoogleUserProfile userProfile)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = userProfile.name
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request,
            success =>
            {
                Debug.Log("Player profile updated successfully!");
            },
            error =>
            {
                Debug.LogWarning($"Failed to update profile: {error.GenerateErrorReport()}");
            });
    }

    private string GenerateSecurePassword(string googleId)
    {
        // Generate a deterministic but secure password from Google ID
        // This ensures the same user always gets the same password
        string basePassword = googleId + PlayFabSettings.staticSettings.TitleId;
        
        // Hash it to create a more secure password
        using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(basePassword));
            return System.Convert.ToBase64String(hashedBytes).Substring(0, 20); // Use first 20 chars
        }
    }

    private void LoadNextScene()
    {
        Debug.Log($"Loading scene: {nextSceneName}");
        SceneManager.LoadScene(nextSceneName);
    }

    public GoogleUserProfile GetCurrentUserProfile()
    {
        return currentUserProfile;
    }
}
