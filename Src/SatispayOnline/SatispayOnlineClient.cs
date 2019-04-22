using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SatispayOnline
{
    public class SatispayUser
    {
        public string Id { get; set; }
        public string UUID { get; set; }
        public string PhoneNumber { get; set; }
    }
    
    public class SatispayUsersList
    {
        public bool HasMore { get; set; }
        public List<SatispayUser> List { get; set; } = new List<SatispayUser>();
    }
    
    public class SatispayCharge
    {
        public string Id { get; set; }
        public string Currency { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string StatusDetail { get; set; }
        public string UserId { get; set; }
        public string UserShortName { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
        public bool Paid { get; set; }
        public string RequiredSuccessEmail { get; set; }
        public string ExpireDate { get; set; }
        public string ChargeDate { get; set; }
        public string CallbackUrl { get; set; }
        public int RefundAmount { get; set; }

        public bool Required() => Status == SatispayChargeStatus.Required;
        public bool Success() => Status == SatispayChargeStatus.Success;
        public bool Faiulire() => Status == SatispayChargeStatus.Failure;

        public bool Declined() => StatusDetail == SatispayChargeStatusDetails.DeclinedByPayer || StatusDetail == SatispayChargeStatusDetails.DeclinedByPayerNotRequired;
        public bool DeclinedByPayer() => StatusDetail == SatispayChargeStatusDetails.DeclinedByPayerNotRequired;
        public bool DeclinedByPayerNotRequired() => StatusDetail == SatispayChargeStatusDetails.DeclinedByPayerNotRequired;
        public bool CancelByNewCharge() => StatusDetail == SatispayChargeStatusDetails.CancelByNewCharge;
        public bool InternalFailure() => StatusDetail == SatispayChargeStatusDetails.InternalFailure;
        public bool Expired() => StatusDetail == SatispayChargeStatusDetails.Expired;
    }

    public class SatispayResponseList<T>
    {
        public bool HasMore { get; set; }
        public List<T> List { get; set; } = new List<T>();
    }
    
    public class SatispayChargesList : SatispayResponseList<SatispayCharge>
    {

    }
    
    public class SatispayRefund
    {
        public string Id { get; set; }
        public string ChargeId { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
        public int Amount { get; set; }
        public string Created { get; set; }
        public string Reason { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
    
    public class SatispayRefundsList : SatispayResponseList<SatispayRefund>
    {

    }

    class SatispayErrorData
    {
        public int code;
        public string message;
        public string wlt;
    }

    public partial class SatispayOnlineClient : IDisposable
    {
        private HttpClient httpClient;
        private HttpClientHandler httpClientHandler;

        private readonly string securityBearer;
        private readonly SatispayEnvironment environment;

        /// <summary>
        /// Gets or sets proxy information used by the client.
        /// </summary>
        public WebProxy Proxy
        {
            get => (WebProxy)httpClientHandler.Proxy;
            set => httpClientHandler.Proxy = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SatispayOnlineClient"/> class with a specific security bearer and environment.
        /// </summary>
        /// <param name="securityBearer">The Satispay security bearer.</param>
        /// <param name="environment">The enviorment, default: <see cref="SatispayEnvironment.Production"/></param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="securityBearer"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="securityBearer"/> is an empty string.</exception>
        public SatispayOnlineClient(string securityBearer, SatispayEnvironment environment = SatispayEnvironment.Production)
        {
            if (securityBearer == null)
                throw new ArgumentNullException(nameof(securityBearer));
            if (securityBearer.Length == 0)
                throw new ArgumentException("SecurityBearer must not be empty.", nameof(securityBearer));
            Contract.EndContractBlock();

            // set the fields
            this.securityBearer = securityBearer;
            this.environment = environment;

            // creat the http client
            httpClient = new HttpClient(httpClientHandler = new HttpClientHandler());
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", securityBearer);
        }

        public async void CheckAuthorizationAsync()
        {
            await RequestAsync<dynamic>(HttpMethod.Get, "/wally-services/protocol/authenticated", null, default(CancellationToken)).ConfigureAwait(false);
        }

        //
        T Request<T>(HttpMethod method, string endpoint, object data)
        {
            // hazzard
            return RequestAsync<T>(method, endpoint, data, default(CancellationToken)).GetAwaiter().GetResult();
        }

        async Task<T> RequestAsync<T>(HttpMethod method, string endpoint, object data, CancellationToken cancellationToken)
        {
            if (httpClient == null)
                throw new ObjectDisposedException(nameof(SatispayOnlineClient));

            // resolve the uri here, so we dont have to deal with threading
            string url = (environment == SatispayEnvironment.Production ? "https://authservices.satispay.com" : "https://staging.authservices.satispay.com") + endpoint;

            HttpRequestMessage message = new HttpRequestMessage(method, url);

            // serialize the content
            if (data != null)
            {
                var serializedData = JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });

                message.Content = new StringContent(serializedData, Encoding.UTF8, "application/json");
            }

            var response = await httpClient.SendAsync(message, cancellationToken).ConfigureAwait(false);

            var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // this is a critical error
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                //
                throw new SatispayException { Response = response };
            }

            if (response.IsSuccessStatusCode)
            {
                var value = JsonConvert.DeserializeObject<T>(responseBody, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy()
                    }
                });

                return value;
            }

            // we have a satispay error
            var ex = JsonConvert.DeserializeObject<SatispayErrorData>(responseBody);

            // 401 is unauthorized/unauthenticated exception
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new SatispayUnauthorizedException(ex.code, ex.message, ex.wlt) { Response = response };
            }

            // 400 is a validation exception
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new SatispayValidationException(ex.code, ex.message, ex.wlt) { Response = response };
            }

            throw new SatispayException(ex.code, ex.message, ex.wlt) { Response = response };
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (httpClient != null)
                {
                    httpClient.Dispose();
                    httpClient = null;
                }
            }
        }

        #endregion
    }
}
