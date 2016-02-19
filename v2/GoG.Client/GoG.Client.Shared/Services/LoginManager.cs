using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using GoG.Client.Models;
using System.Threading.Tasks;
using LiveSDKHelper;
using Microsoft.Live;

namespace GoG.Client.Services
{
    public interface ILoginManager
    {
        Task<Session> LoginAsync();
    }

    public class LiveSessionManager : ILoginManager
    {
        #region ISessionManager Members

        public async Task<Session> LoginAsync()
        {
            try
            {
                var authClient = new LiveAuthClient();
                var result = await authClient.LoginAsync(new[]
                {
                    Scope.SignIn.ToStringScope(), 
                    Scope.SkyDrive.ToStringScope()
                });

                if (result.Status == LiveConnectSessionStatus.Connected)
                {
                    var connectClient = new LiveConnectClient(result.Session);
                    var meResult = await connectClient.GetAsync("me");
                    if (connectClient.Session != null)
                    {
                        var dcs = new DataContractJsonSerializer(typeof (MeDetails));
                        var obj = dcs.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(meResult.RawResult)));
                        var me = obj as MeDetails;
                        if (me != null)
                        {
                            return new Session
                            {
                                SessionType = SessionType.Microsoft,
                                UserProfile = new UserProfile
                                {
                                    Id = me.Id,
                                    Name =
                                        String.IsNullOrWhiteSpace(me.FirstName)
                                            ? me.Name
                                            : me.FirstName + (me.LastName ?? String.Empty),
                                    FirstName = me.FirstName,
                                    LastName = me.LastName,
                                    Link = me.Link,
                                    Gender = me.Gender,
                                    Locale = me.Locale,
                                    UpdatedTime = me.UpdatedTime,
                                },
                                AccessToken = connectClient.Session.AccessToken,
                                AuthenticationToken = connectClient.Session.AuthenticationToken
                            };
                        }
                    }
                }
            }
            catch (LiveAuthException laex)
            {
                // Display an error message.
            }
            catch (LiveConnectException lcex)
            {
                // Display an error message.
            }
            return null;
        }

        #endregion
    }
}
