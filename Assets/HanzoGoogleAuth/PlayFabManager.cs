using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using System;
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
        // Initialize PlayFab with settings
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            Debug.LogError("PlayFab Title ID is not set! Configure it in PlayFab Settings.");
            return;
        }

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
        if (userProfile == null)
        {
            Debug.LogError("Received null user profile!");
            return;
        }

        currentUserProfile = userProfile;
        Debug.Log($"Received Google profile: {userProfile.email}");
        
        // Check if PlayFab is properly initialized
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            Debug.LogError("PlayFab Title ID not configured!");
            return;
        }
        
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
        // Validate user profile
        if (string.IsNullOrEmpty(userProfile.id))
        {
            Debug.LogError("Invalid user profile - missing Google ID");
            return;
        }

        // Use Google ID as unique username (guaranteed to be unique)
        // Create a valid username: alphanumeric + underscore, max 20 chars
        string baseUsername = "g" + userProfile.id.Replace("-", "").Replace(".", "").Substring(0, Math.Min(19, userProfile.id.Length));
        string customUsername = baseUsername;
        string password = GenerateSecurePassword(userProfile.id);

        Debug.Log($"Attempting login first - Username: {customUsername}");

        // Try login first
        var loginRequest = new LoginWithPlayFabRequest
        {
            Username = customUsername,
            Password = password
        };

        PlayFabClientAPI.LoginWithPlayFab(loginRequest,
            success =>
            {
                Debug.Log("PlayFab login successful!");
                UpdatePlayerProfile(userProfile);
                LoadNextScene();
            },
            error =>
            {
                // If login fails, try registration
                Debug.Log($"Login failed, attempting registration - Error: {error.ErrorMessage}");
                RegisterNewUser(userProfile, customUsername, password);
            });
    }

    private void RegisterNewUser(GoogleUserProfile userProfile, string customUsername, string password)
    {
        Debug.Log($"Attempting registration - Username: {customUsername}");

        var request = new RegisterPlayFabUserRequest
        {
            Username = customUsername,
            Password = password,
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
                Debug.LogError($"Registration failed - Full error: {error.GenerateErrorReport()}");
                Debug.LogError($"Error code: {error.Error}, Error message: {error.ErrorMessage}");

                // If registration fails due to username taken (unlikely with Google ID), try with timestamp
                if (error.Error == PlayFabErrorCode.UsernameNotAvailable)
                {
                    string timestampUsername = "g" + System.DateTime.Now.Ticks.ToString().Substring(10, 8);
                    Debug.Log($"Username taken, trying with timestamp: {timestampUsername}");
                    RegisterNewUser(userProfile, timestampUsername, password);
                }
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
