using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Steamworks;
using Newtonsoft.Json;

namespace LegionLords.API
{
    public class API : MonoBehaviour
    {

        /// <summary>Singleton</summary>
        public static API S;

        private const string URL = "https://api.legionlords.com/api/v1/";
        private const string CONTENT_TYPE = "application/json";

        public class Payload<T>
        {
            public UnityWebRequest.Result Result { get; set; }
            public T Data { get; set; }

            public Payload(UnityWebRequest.Result result, T data)
            {
                Result = result;
                Data = data;
            }
        }

        private void Awake()
        {
            if (S == null)
            {
                S = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        public async Task<Payload<AccountModel>> CreateAccount()
        {
            using UnityWebRequest webRequest = UnityWebRequest.Post(URL + "account/" + SteamClient.SteamId, null, CONTENT_TYPE);

            await Awaitable.FromAsyncOperation(webRequest.SendWebRequest());

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("API Error: " + webRequest.error);
                return null;
            }
            else
            {
                AccountModel data = JsonConvert.DeserializeObject<AccountModel>(webRequest.downloadHandler.text);
                return new Payload<AccountModel>(webRequest.result, data);
            }
        }
 
        public async Task<Payload<AccountModel>> GetAccount()
        {
            using UnityWebRequest webRequest = UnityWebRequest.Get(URL + "account/" + SteamClient.SteamId);

            await Awaitable.FromAsyncOperation(webRequest.SendWebRequest());

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("API Error: " + webRequest.error);
                return null;
            }
            else
            {
                AccountModel data = JsonConvert.DeserializeObject<AccountModel>(webRequest.downloadHandler.text);
                return new Payload<AccountModel>(webRequest.result, data);
            }
        }

        public async Task<Payload<LegionModel[]>> GetLegions()
        {
            using UnityWebRequest webRequest = UnityWebRequest.Get(URL + "account/" + Client.S.AccountID + "/legion");

            await Awaitable.FromAsyncOperation(webRequest.SendWebRequest());

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("API Error: " + webRequest.error);
                return new Payload<LegionModel[]>(webRequest.result, null);
            }
            else
            {
                LegionModel[] data = JsonConvert.DeserializeObject<LegionModel[]>(webRequest.downloadHandler.text);
                return new Payload<LegionModel[]>(webRequest.result, data);
            }
        }

    }
}

