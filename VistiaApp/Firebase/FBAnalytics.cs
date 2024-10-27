using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class FBAnalytics : Singleton<FBAnalytics>
{
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    protected bool firebaseInitialized = false;

    // When the app starts, check to make sure that we have
    // the required dependencies to use Firebase, and if not,
    // add them if possible.
    public void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                  "FB: Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });

    }

    // Handle initialization of the necessary firebase modules:
    void InitializeFirebase()
    {

        string userID = SystemInfo.deviceUniqueIdentifier;
        Debug.Log("FB: Enabling data collection.");
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        Debug.Log("FB: Set user properties.");
        // Set the user's sign up method.
        FirebaseAnalytics.SetUserProperty(
          FirebaseAnalytics.UserPropertySignUpMethod,
          "Google");
        // Set the user ID.
        FirebaseAnalytics.SetUserId(userID);
        // Set default session duration values.
        FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));
        firebaseInitialized = true;
        AnalyticsLogin();
    }

    public void AnalyticsLogin()
    {
        // Log an event with no parameters.
        Debug.Log("FB: Logging a login event.");
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
    }

    public void LogEvent(string eventName)
    {
        Debug.Log($"Logging {eventName} event.");
        FirebaseAnalytics.LogEvent(
          eventName);
    }

    public void LogEvent(string eventName, string parameter, float value)
    {
        Debug.Log($"Logging {eventName} event.");
        FirebaseAnalytics.LogEvent(
          eventName,
          parameter,
          value);
    }

    public void LogEvent(string eventName, string parameter, string value)
    {
        Debug.Log($"Logging {eventName} event.");
        FirebaseAnalytics.LogEvent(
          eventName,
          parameter,
          value);
    }

    public void LogEvent(string eventName, string parameter, int value)
    {
        Debug.Log($"Logging {eventName} event.");
        FirebaseAnalytics.LogEvent(
        eventName,
        parameter,
        value);
    }

    public void LogEventWithParameter(string event_name, Hashtable hash)
    {

        Parameter[] parameter = new Parameter[hash.Count];
        if (hash != null && hash.Count > 0)
        {
            int i = 0;
            foreach (DictionaryEntry item in hash)
            {
                if (item.Equals((DictionaryEntry)default)) continue;

                parameter[i] = new Parameter(item.Key.ToString(), item.Key.ToString());
                Debug.Log($"==> LogEvent " + event_name + "- Key = " + item.Key.ToString() + " -  Value =" + item.Key.ToString() + " <==");
                i++;
            }

            FirebaseAnalytics.LogEvent(event_name, parameter);
        }
    }

    public void LogEventWithParams(string name, params Parameter[] parameters)
    {
        // Log an event with multiple parameters.
        Debug.Log($"Logging a {name} event.");
        FirebaseAnalytics.LogEvent(
          name,
          parameters);
    }

    // Reset analytics data for this app instance.
    public void ResetAnalyticsData()
    {
        Debug.Log("Reset analytics data.");
        FirebaseAnalytics.ResetAnalyticsData();
    }

    // Get the current app instance ID.
    public Task<string> DisplayAnalyticsInstanceId()
    {
        return FirebaseAnalytics.GetAnalyticsInstanceIdAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.Log("App instance ID fetch was canceled.");
            }
            else if (task.IsFaulted)
            {
                Debug.Log(String.Format("Encounted an error fetching app instance ID {0}",
                                        task.Exception.ToString()));
            }
            else if (task.IsCompleted)
            {
                Debug.Log(String.Format("App instance ID: {0}", task.Result));
            }
            return task;
        }).Unwrap();
    }

}
