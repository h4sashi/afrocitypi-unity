



using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayFabManager : MonoBehaviour
{
    private static PlayFabManager instance;

    [SerializeField]
    private string nextSceneName = "MainGame"; // Change this to your scene name

    [SerializeField]
    private bool useDeviceId = true;

    private GoogleLogin googleLoginScript;
    private GoogleUserProfile currentUserProfile;

    // Add this field near currentUserProfile
    public Texture2D cachedProfileTexture;

    public static PlayFabManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = UnityEngine.Object.FindFirstObjectByType<PlayFabManager>();
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
        googleLoginScript = UnityEngine.Object.FindFirstObjectByType<GoogleLogin>() ?? GoogleLogin.Instance;

        if (googleLoginScript == null)
        {
            Debug.LogError("GoogleLogin script not found in the scene!");
            return;
        }

        googleLoginScript.OnProfileAvailable += OnGoogleLoginSuccess;

        if (googleLoginScript.UserProfile != null)
        {
            OnGoogleLoginSuccess(googleLoginScript.UserProfile);
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

        if (currentUserProfile != null && currentUserProfile.id == userProfile.id)
        {
            Debug.Log("Google login already processed for this user profile.");
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
            googleLoginScript = UnityEngine.Object.FindFirstObjectByType<GoogleLogin>() ?? GoogleLogin.Instance;
        }

        var profile = googleLoginScript?.UserProfile;
        if (profile != null)
        {
            OnGoogleLoginSuccess(profile);
            return;
        }

        Debug.LogError("Google user profile not available yet!");
    }

    private void RegisterOrLoginWithPlayFab(GoogleUserProfile userProfile)
    {
        // Validate user profile
        if (string.IsNullOrEmpty(userProfile.id))
        {
            Debug.LogError("Invalid user profile - missing Google ID");
            return;
        }

        // Use Google ID as unique username (invisible to user), first name as display name
        string customUsername =
            "g"
            + userProfile
                .id.Replace("-", "")
                .Replace(".", "")
                .Substring(0, Math.Min(19, userProfile.id.Length));
        string password = GenerateSecurePassword(userProfile.id);

        Debug.Log($"Attempting login first - Username: {customUsername}");

        // Try login first
        var loginRequest = new LoginWithPlayFabRequest
        {
            Username = customUsername,
            Password = password,
        };

        PlayFabClientAPI.LoginWithPlayFab(
            loginRequest,
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
            }
        );
    }

    private void RegisterNewUser(
        GoogleUserProfile userProfile,
        string customUsername,
        string password
    )
    {
        Debug.Log($"Attempting registration - Username: {customUsername}");

        var request = new RegisterPlayFabUserRequest
        {
            Username = customUsername,
            Password = password,
            DisplayName = userProfile.given_name ?? "Player", // Use first name as display name
            RequireBothUsernameAndEmail = false,
        };

        PlayFabClientAPI.RegisterPlayFabUser(
            request,
            success =>
            {
                Debug.Log("PlayFab registration successful!");
                UpdatePlayerProfile(userProfile);
                SetUserCustomData(userProfile);
                LoadNextScene();
            },
            error =>
            {
                Debug.LogError($"Registration failed - Full error: {error.GenerateErrorReport()}");
                Debug.LogError($"Error code: {error.Error}, Error message: {error.ErrorMessage}");
            }
        );
    }

    private void UpdatePlayerProfile(GoogleUserProfile userProfile)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = userProfile.given_name ?? "Player",
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(
            request,
            success =>
            {
                Debug.Log("Player profile updated successfully!");
            },
            error =>
            {
                Debug.LogWarning($"Failed to update profile: {error.GenerateErrorReport()}");
            }
        );
    }

    private void SetUserCustomData(GoogleUserProfile userProfile)
    {
        // Create custom data to differentiate users with the same display name
        var customData = new Dictionary<string, string>
        {
            { "googleId", userProfile.id },
            { "email", userProfile.email },
            { "fullName", userProfile.name },
            { "givenName", userProfile.given_name ?? "" },
            { "familyName", userProfile.family_name ?? "" },
            { "registrationDate", System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") },
        };

        var request = new UpdateUserDataRequest
        {
            Data = customData,
            Permission = UserDataPermission.Private, // Only the user can see this data
        };

        PlayFabClientAPI.UpdateUserData(
            request,
            success =>
            {
                Debug.Log("User custom data set successfully!");
            },
            error =>
            {
                Debug.LogWarning($"Failed to set custom data: {error.GenerateErrorReport()}");
            }
        );
    }

    private string GenerateSecurePassword(string googleId)
    {
        // Generate a deterministic but secure password from Google ID
        // This ensures the same user always gets the same password
        string basePassword = googleId + PlayFabSettings.staticSettings.TitleId;

        // Hash it to create a more secure password
        using (
            System.Security.Cryptography.SHA256 sha256 =
                System.Security.Cryptography.SHA256.Create()
        )
        {
            byte[] hashedBytes = sha256.ComputeHash(
                System.Text.Encoding.UTF8.GetBytes(basePassword)
            );
            return System.Convert.ToBase64String(hashedBytes).Substring(0, 20); // Use first 20 chars
        }
    }

    private void LoadNextScene()
    {
        Debug.Log($"Loading scene: {nextSceneName}");

        // Load player's level progress before loading the scene
        LoadPlayerLevelProgress(() =>
        {
            SceneManager.LoadScene(nextSceneName);
        });
    }

    public GoogleUserProfile GetCurrentUserProfile()
    {
        return currentUserProfile;
    }

    /// <summary>
    /// Get user custom data to differentiate users with the same display name
    /// </summary>
    public void GetUserCustomData(
        System.Action<Dictionary<string, string>> onSuccess,
        System.Action<string> onError = null
    )
    {
        var request = new GetUserDataRequest();

        PlayFabClientAPI.GetUserData(
            request,
            result =>
            {
                // Convert UserDataRecord to string dictionary
                var stringData = new Dictionary<string, string>();
                foreach (var kvp in result.Data)
                {
                    stringData[kvp.Key] = kvp.Value.Value;
                }
                onSuccess?.Invoke(stringData);
            },
            error =>
            {
                Debug.LogError($"Failed to get custom data: {error.GenerateErrorReport()}");
                onError?.Invoke(error.ErrorMessage);
            }
        );
    }

    /// <summary>
    /// Get a unique identifier for the current user (useful for differentiating users with same display name)
    /// </summary>
    public string GetUserUniqueIdentifier()
    {
        return currentUserProfile?.id ?? "unknown";
    }

    /// <summary>
    /// Save player's unlocked level to PlayFab and local storage
    /// </summary>
    public void SavePlayerLevelProgress(int unlockedLevel)
    {
        Debug.Log($"Saving level progress: {unlockedLevel}");

        // Save to local storage immediately
        UnityEngine.PlayerPrefs.SetInt("currentLevel", unlockedLevel);
        UnityEngine.PlayerPrefs.SetInt("UnlockedLevels", unlockedLevel);
        UnityEngine.PlayerPrefs.Save();

        // Save to PlayFab custom data
        var levelData = new Dictionary<string, string>
        {
            { "unlockedLevel", unlockedLevel.ToString() },
            { "lastUpdated", System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") },
        };

        var request = new UpdateUserDataRequest
        {
            Data = levelData,
            Permission = UserDataPermission.Private,
        };

        PlayFabClientAPI.UpdateUserData(
            request,
            success =>
            {
                Debug.Log($"✓ Level progress saved to PlayFab: Level {unlockedLevel}");
            },
            error =>
            {
                Debug.LogError(
                    $"✗ Failed to save level progress to PlayFab: {error.GenerateErrorReport()}"
                );
            }
        );
    }

    /// <summary>
    /// Load player's level progress from PlayFab
    /// </summary>
    public void LoadPlayerLevelProgress(System.Action onComplete = null)
    {
        var request = new GetUserDataRequest();

        PlayFabClientAPI.GetUserData(
            request,
            result =>
            {
                int unlockedLevel = 1; // Default level

                // Try to get the unlocked level from custom data
                if (result.Data != null && result.Data.ContainsKey("unlockedLevel"))
                {
                    string levelValue = result.Data["unlockedLevel"].Value;
                    if (
                        !string.IsNullOrEmpty(levelValue) && int.TryParse(levelValue, out int level)
                    )
                    {
                        unlockedLevel = level;
                        Debug.Log($"✓ Level progress loaded from PlayFab: Level {unlockedLevel}");
                    }
                    else
                    {
                        Debug.LogWarning($"Could not parse level value: {levelValue}");
                    }
                }
                else
                {
                    Debug.Log("No unlocked level found in PlayFab data, using default Level 1");
                }

                // Set the level in local storage and LevelManager
                UnityEngine.PlayerPrefs.SetInt("currentLevel", unlockedLevel);
                UnityEngine.PlayerPrefs.SetInt("UnlockedLevels", unlockedLevel);
                UnityEngine.PlayerPrefs.Save();

                onComplete?.Invoke();
            },
            error =>
            {
                Debug.LogError($"✗ Failed to load level progress: {error.GenerateErrorReport()}");
                // Use default level if fetch fails
                onComplete?.Invoke();
            }
        );
    }

    // Add this public method
    public void DownloadProfilePicture(System.Action<Texture2D> onComplete)
    {
        if (cachedProfileTexture != null)
        {
            onComplete?.Invoke(cachedProfileTexture);
            return;
        }

        if (currentUserProfile == null || string.IsNullOrEmpty(currentUserProfile.picture))
        {
            Debug.LogWarning("No profile picture URL available.");
            onComplete?.Invoke(null);
            return;
        }

        StartCoroutine(DownloadTextureCoroutine(currentUserProfile.picture, onComplete));
    }

    private IEnumerator DownloadTextureCoroutine(string url, System.Action<Texture2D> onComplete)
    {
        using (
            UnityEngine.Networking.UnityWebRequest request =
                UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url)
        )
        {
            yield return request.SendWebRequest();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                cachedProfileTexture = UnityEngine.Networking.DownloadHandlerTexture.GetContent(
                    request
                );
                Debug.Log("Profile picture downloaded successfully.");
                onComplete?.Invoke(cachedProfileTexture);
            }
            else
            {
                Debug.LogWarning($"Failed to download profile picture: {request.error}");
                onComplete?.Invoke(null);
            }
        }
    }

    /// <summary>
    /// Send leaderboard score to PlayFab
    /// </summary>
    public void SendLeaderBoard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "LevelProgress",
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(
            request,
            success =>
            {
                Debug.Log($"✓ Leaderboard score sent successfully: {score}");
            },
            error =>
            {
                Debug.LogError($"✗ Failed to send leaderboard score: {error.GenerateErrorReport()}");
            }
        );
    }

    /// <summary>
    /// Get leaderboard data from PlayFab
    /// </summary>
    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "LevelProgress",
            StartPosition = 0,
            MaxResultsCount = 10
        };

        PlayFabClientAPI.GetLeaderboard(
            request,
            result =>
            {
                // Clear existing data
                // LeaderboardManager.manager.list_playerData.Clear();

                // Populate with new data
                for (int i = 0; i < result.Leaderboard.Count; i++)
                {
                    var entry = result.Leaderboard[i];
                    // LeaderboardManager.manager.list_playerData.Add(new LeaderboardManager.PlayerData
                    // {
                    //     number = entry.Position + 1, // Position is 0-based, but we want 1-based ranking
                    //     name = entry.DisplayName ?? "Player",
                    //     level = entry.StatValue
                    // });
                }

                Debug.Log($"✓ Leaderboard data retrieved successfully: {result.Leaderboard.Count} entries");
            },
            error =>
            {
                Debug.LogError($"✗ Failed to get leaderboard: {error.GenerateErrorReport()}");
            }
        );
    }
}

