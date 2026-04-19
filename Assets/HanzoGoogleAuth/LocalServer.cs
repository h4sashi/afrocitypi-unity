using System;
using System.Net;
using System.IO;
using UnityEngine;

public class LocalServer : MonoBehaviour
{
    private HttpListener httpListener;
    public string code;

    void Start()
    {
        // Ensure the Unity main thread dispatcher is initialized (if you are using one).
        UnityMainThreadDispatcher.Instance();
        StartLocalServer();
    }

    void StartLocalServer()
    {
        httpListener = new HttpListener();
        httpListener.Prefixes.Add("http://+:3000/");
        httpListener.Start();

        Debug.Log("Local server is running on http://localhost:3000/");
        httpListener.BeginGetContext(OnRequestReceived, null);
    }

    private void OnRequestReceived(IAsyncResult result)
    {
        try
        {
            if (httpListener == null) return;

            var context = httpListener.EndGetContext(result);
            var request = context.Request;
            var response = context.Response;

            // Extract the query string to get the code parameter.
            string query = request.Url.Query;
            if (query.Contains("code="))
            {
                // Extract and URL-decode the code.
                string _code = query.Split(new string[] { "code=" }, StringSplitOptions.None)[1].Split('&')[0];
                _code = WWW.UnEscapeURL(_code);  // Decodes URL-encoded characters.
                code = _code;

                // Run CallProfile() on the main thread.
                UnityMainThreadDispatcher.Instance().Enqueue(() => CallProfile());
            }

            Debug.Log("Authorization Code received: " + code);

            // Send a simple response to the browser.
            string responseString = "<html><body>Google Auth Successful! You can close this window now.</body></html>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();

            // Continue listening for the next request.
            httpListener.BeginGetContext(OnRequestReceived, null);
        }
        catch (Exception ex)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => Debug.LogError("Error in OnRequestReceived: " + ex.Message));
        }
    }


    // This method now calls the GoogleLogin callback.
    private void CallProfile()
    {
        Debug.Log("Calling GoogleLogin callback from LocalServer");
        GoogleLogin googleLogin = FindObjectOfType<GoogleLogin>();
        if (googleLogin != null)
        {
            // googleLogin.OnGoogleCallback(code);
        }
        else
        {
            Debug.LogError("GoogleLogin component not found in the scene!");
        }
    }

    void OnApplicationQuit()
    {
        httpListener?.Stop();
    }
}
