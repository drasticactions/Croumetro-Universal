using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Storage;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Tools;
using Newtonsoft.Json.Linq;

namespace CrouMetro.Core.Managers
{
    public class AuthenticationManager
    {
        private ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
        public async Task<UserAccountEntity> RequestAccessToken(String code)
        {
            string consumerKey;
            string consumerSecret;
            var dic = new Dictionary<String, String>();
            dic["grant_type"] = "authorization_code";
            dic["client_id"] = Constants.CONSUMER_KEY;
            dic["client_secret"] = Constants.CONSUMER_SECRET;
            dic["code"] = code;

            var theAuthClient = new HttpClient();
            HttpContent header = new FormUrlEncodedContent(dic);
            HttpResponseMessage response = await theAuthClient.PostAsync(EndPoints.OAUTH_TOKEN, header);
            string responseContent = await response.Content.ReadAsStringAsync();
            JObject o = JObject.Parse(responseContent);

            return new UserAccountEntity((String)o["access_token"], (String)o["refresh_token"],
                int.Parse((String)o["expires_in"]));
        }

        public async Task<bool> RefreshAccessToken(UserAccountEntity account)
        {
            var dic = new Dictionary<String, String>();
            dic["grant_type"] = "refresh_token";
            dic["client_id"] = Constants.CONSUMER_KEY;
            dic["client_secret"] = Constants.CONSUMER_SECRET;
            dic["refresh_token"] = account.GetRefreshToken();

            account.SetAccessToken("updating", null);
            account.SetRefreshTime(1000);
            var theAuthClient = new HttpClient();
            HttpContent header = new FormUrlEncodedContent(dic);
            HttpResponseMessage response;
            try
            {
                response = await theAuthClient.PostAsync(EndPoints.OAUTH_TOKEN, header);
            }
            catch (WebException)
            {
                return false;
            }

            try
            {
                if (response.StatusCode != HttpStatusCode.OK) return false;
                string responseContent = await response.Content.ReadAsStringAsync();
                JObject o = JObject.Parse(responseContent);
                account.SetAccessToken((String)o["access_token"], (String)o["refresh_token"]);
                account.SetRefreshTime(long.Parse((String)o["expires_in"]));

                _localSettings.Values["AccessToken"] = (String)o["access_token"];
                _localSettings.Values["RefreshToken"] = (String)o["refresh_token"];
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> VerifyAccount(UserAccountEntity userAccountEntity)
        {
            var theAuthClient = new HttpClient();
            var requestMsg = new HttpRequestMessage(new HttpMethod("GET"), EndPoints.ACCOUNT_VERIFY);
            requestMsg.Headers.Authorization = new AuthenticationHeaderValue("Bearer",
                userAccountEntity.GetAccessToken());
            HttpResponseMessage response;
            try
            {
                response = await theAuthClient.SendAsync(requestMsg);
            }
            catch (WebException)
            {
                return false;
            }
            if (response.StatusCode != HttpStatusCode.OK) return false;
            string responseContent = await response.Content.ReadAsStringAsync();
            userAccountEntity.SetUserEntity(UserEntity.Parse(responseContent, userAccountEntity));
            SaveUserCredentials(userAccountEntity);
            return true;
        }

        public void SaveUserCredentials(UserAccountEntity userAccountEntity)
        {
            _localSettings.Values["AccessToken"] = userAccountEntity.GetAccessToken();
            _localSettings.Values["RefreshToken"] = userAccountEntity.GetRefreshToken();
        }
    }
}