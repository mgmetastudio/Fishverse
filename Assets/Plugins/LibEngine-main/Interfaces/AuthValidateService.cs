using System;
using Cysharp.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace LibEngine.Auth
{
    public interface IAuthValidateService
    {
        bool IsValid(Action<string> onNotValidateMessage = null);
        UniTask<bool> IsValidAsync(Action<string> onNotValidateMessage = null);

        void Reset();
    }

    public class AuthValidateService : IAuthValidateService
    {
        private readonly HttpClient httpClient;
        private readonly string endpoint;
        private bool isValidCache;
        private string lastFailMessage;
        private bool isHandlingRequest;

        private const string NotValidExceptionMessage = "Not valid exception";
        private const string EndpointUrl = "https://api.hunter.metashooter.gg/user/validate/";

        public AuthValidateService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            endpoint = EndpointUrl;
        }

        public bool IsValid(Action<string> onNotValidateMessage = null)
        {
#if DEVELOP
            return true;
#endif

            if (isValidCache)
            {
                return true;
            }

            if (!string.IsNullOrEmpty(lastFailMessage))
            {
                onNotValidateMessage?.Invoke(lastFailMessage);
                return false;
            }

            if (!isHandlingRequest)
            {
                isHandlingRequest = true;
                bool isValid = IsValidInternal(onNotValidateMessage);
                isHandlingRequest = false;
                return isValid;
            }

            return false;
        }

        public async UniTask<bool> IsValidAsync(Action<string> onNotValidateMessage = null)
        {
#if DEVELOP
            return true;
#endif

            if (isValidCache)
            {
                return true;
            }

            if (!isHandlingRequest)
            {
                isHandlingRequest = true;
                bool isValid = await IsValidInternalAsync(onNotValidateMessage);
                isHandlingRequest = false;
                return isValid;
            }

            return false;
        }

        private bool IsValidInternal(Action<string> onNotValidateMessage)
        {
            try
            {
                HttpResponseMessage response = httpClient.GetAsync(endpoint).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                AuthValidationResponseDto responseDto = JsonConvert.DeserializeObject<AuthValidationResponseDto>(responseBody);

                if (responseDto != null && responseDto.Access)
                {
                    isValidCache = true;
                    return true;
                }
                else
                {
                    lastFailMessage = responseDto?.AccessMessage;
                    onNotValidateMessage?.Invoke(lastFailMessage);
                    return false;
                }
            }
            catch (Exception)
            {
                lastFailMessage = NotValidExceptionMessage;
                return false;
            }
        }

        private async UniTask<bool> IsValidInternalAsync(Action<string> onNotValidateMessage)
        {
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                AuthValidationResponseDto responseDto = JsonConvert.DeserializeObject<AuthValidationResponseDto>(responseBody);

                if (responseDto != null && responseDto.Access)
                {
                    isValidCache = true;
                    return true;
                }
                else
                {
                    lastFailMessage = responseDto?.AccessMessage;
                    string text = "Not valid: " + lastFailMessage;
                    onNotValidateMessage?.Invoke(text);
                    return false;
                }
            }
            catch (Exception)
            {
                lastFailMessage = NotValidExceptionMessage;
                return false;
            }
        }

        public void Reset()
        {
            isValidCache = false;
        }
    }
}