using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CrouMetro.Core.Entity;
using CrouMetro.Core.Tools;

namespace CrouMetro.Core.Managers
{
    public class FavoriteManager
    {
        public static async Task<HttpStatusCode> CreateFavorite(long id, bool? trim, UserAccountEntity userAccountEntity)
        {
            try
            {
                var param = new Dictionary<String, String>();
                if (trim == true) param.Add("trim_user", true.ToString());
                var theAuthClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, (String.Format(EndPoints.FAVORITE_CREATE, id)));

                if (userAccountEntity.GetAccessToken().Equals("refresh"))
                {
                    AuthenticationManager authManager = new AuthenticationManager();
                    await authManager.RefreshAccessToken(userAccountEntity);
                }

                string accessToken = userAccountEntity.GetAccessToken();

                HttpContent header = new FormUrlEncodedContent(param);
                request.Content = header;
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage response = await theAuthClient.SendAsync(request);
                return response.StatusCode;
            }
            catch (Exception)
            {
                return HttpStatusCode.GatewayTimeout;
            }
        }
    }
}