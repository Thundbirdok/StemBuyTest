using UnityEngine;

namespace Lobby
{
    using System.Threading.Tasks;
    using Unity.Services.Authentication;
    using Unity.Services.Core;

    public static class UnityServicesInitializer
    {
        public async static Task Initialize()
        {
            var options = new InitializationOptions();
            options.SetProfile("Player" + Random.Range(int.MinValue, int.MaxValue));
            await UnityServices.InitializeAsync(options);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
}
