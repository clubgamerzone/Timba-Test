
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ClubGamerZone.Data.Remote
{
    public class Rest : MonoBehaviour
    {
        public static Rest Instance { get; private set; }
        public string URL = "https://farmgame-2553a-default-rtdb.firebaseio.com";
        public string LeaderBoardParms = "/Leaderboards/";

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
            DontDestroyOnLoad(gameObject);
        }
        public void Post(string url, string json, Action<string> onSuccess = null, Action<object> onFailure = null)
        {
            UnityWebRequest req = new UnityWebRequest(url, "POST");

            byte[] rawJson = Encoding.UTF8.GetBytes(json);
            req.uploadHandler = new UploadHandlerRaw(rawJson);
            StartCoroutine(SendRequest(req, onSuccess, onFailure));
        }

        public void Delete(string url, Action<string> onSuccess, Action<object> onFailure)
        {
            UnityWebRequest req = UnityWebRequest.Delete(url);
            StartCoroutine(SendRequest(req, onSuccess, onFailure));
        }
        public void Get(string url, Action<string> onSuccess, Action<object> onFailure)
        {

            UnityWebRequest req = new UnityWebRequest(url, "GET");
            req.downloadHandler = new DownloadHandlerBuffer();
            StartCoroutine(SendRequest(req, onSuccess, onFailure));

        }
        public void Patch(string url, string json, Action<string> onSuccess=null, Action<object> onFailure=null)
        {
            byte[] rawJson = Encoding.UTF8.GetBytes(json);
            UnityWebRequest req = UnityWebRequest.Put(url, rawJson);
            req.method = "PATCH";
            req.SetRequestHeader("Content-Type", "application/json");
            StartCoroutine(SendRequest(req, onSuccess, onFailure));
        }

        private IEnumerator RetryRequest(UnityWebRequest request, Action<string> onSuccess, Action<object> onFailure)
        {
            UnityWebRequest retryRequest = new UnityWebRequest();
            retryRequest.url = request.url;
            retryRequest.method = request.method;
            if (request.uploadHandler != null) retryRequest.uploadHandler = new UploadHandlerRaw(request.uploadHandler.data);
            if (request.downloadHandler != null) retryRequest.downloadHandler = new DownloadHandlerBuffer();

            yield return retryRequest.SendWebRequest();
            if (retryRequest.error == null)
            {
                onSuccess?.Invoke(retryRequest.downloadHandler.text);
            }
            else
            {
                onFailure?.Invoke(retryRequest.downloadHandler.text);
            }
        }
        private IEnumerator SendRequest(UnityWebRequest request, Action<string> onSuccess, Action<object> onFailure)
        {
            request.timeout = 5;
            yield return request.SendWebRequest();
            if (request.error == null)
            {
                onSuccess?.Invoke(request.downloadHandler?.text);
            }
            else if (request.responseCode == 401)
            {
                //TODO
            }
            else
            {
                string raw = request.downloadHandler?.text;
                onFailure?.Invoke(raw);
                RetryRequest(request, onSuccess, onFailure);
            }
        }
    }
}
