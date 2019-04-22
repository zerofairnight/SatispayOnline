using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SatispayOnline
{
    public partial class SatispayOnlineClient
    {
        /// <summary>
        /// Create a charge for the specified user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="currency">The currency code (EUR, SatispayOnlineCurrency.EUR).</param>
        /// <param name="amount">The charge amount in the smallest currency units (Ex: 1.15 € = 115 units).</param>
        /// <param name="description">The charge description.</param>
        /// <param name="expireInSeconds">The charge expiration date in seconds from now. Default 15 minutes. When this amount of time elapses, the Charge fails with EXPIRED as state detail.</param>
        /// <param name="metadata">The metadata object that will be returned in the callback url (Max 20 keys. Key length 45 characters and values 500 characters).</param>
        /// <param name="callbackUrl">The url that will be called when the Charge will change state. Make sure to include the string {uuid} so that when the callback is executed you get the Charge-uid.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="userId"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="currency"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="expireInSeconds"/> is less than 60.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when <see cref="SatispayOnlineClient"/> has been disposed.</exception>
        public Task<SatispayCharge> CreateChargeAsync(string userId, string currency, int amount, string description = null, int expireInSeconds = 900, Dictionary<string, string> metadata = null, string callbackUrl = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));
            if (expireInSeconds < 60)
                throw new ArgumentOutOfRangeException(nameof(expireInSeconds));
            Contract.EndContractBlock();

            var data = new
            {
                user_id = userId,
                currency,
                amount,
                description,
                expire_in = expireInSeconds,
                metadata,
                callback_url = callbackUrl,
            };

            return RequestAsync<SatispayCharge>(HttpMethod.Post, "/online/v1/charges", data, cancellationToken);
        }

        /// <summary>
        /// Get a charge by the specified id.
        /// </summary>
        /// <param name="chargeId">The charge id.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns></returns>
        public Task<SatispayCharge> GetChargeAsync(string chargeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (chargeId == null)
                throw new ArgumentNullException(nameof(chargeId));
            Contract.EndContractBlock();

            return RequestAsync<SatispayCharge>(HttpMethod.Get, $"/online/v1/charges/{chargeId}", null, cancellationToken);
        }

        /// <summary>
        /// Get a list of charges ordered by creation.
        /// Note: starting_after and ending_before cannot be used together.
        /// </summary>
        /// <param name="limit">Max number of returned items</param>
        /// <param name="startingAfterCharge">The charge id.</param>
        /// <param name="endingBeforeCharge">The charge id.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns></returns>
        public Task<SatispayChargesList> GetChargesAsync(int limit = 20, string startingAfterCharge = null, string endingBeforeCharge = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (limit < 1 || limit > 100)
                throw new ArgumentOutOfRangeException(nameof(limit));
            Contract.EndContractBlock();

            string url = $"/online/v1/charges?limit={limit}";

            if (startingAfterCharge != null)
            {
                url += $"&starting_after={startingAfterCharge}";
            }

            if (endingBeforeCharge != null)
            {
                url += $"&ending_before={endingBeforeCharge}";
            }

            return RequestAsync<SatispayChargesList>(HttpMethod.Get, url, null, cancellationToken);
        }

        /// <summary>
        /// Update a Charge, only metadata, description and state can be updated.
        /// </summary>
        /// <param name="id">The charge ID</param>
        /// <param name="description">a Charge description</param>
        /// <param name="metadata">Object with max 20 keys. Key length 45 characters and values 500 characters. New keys will be added, the existing keys will be updated and keys set with null value will be removed.</param>
        /// <param name="chargeState">a string that can contain only the CANCELED value. If set to CANCELED, the target Charge gets canceled; the staus will be set to FAILURE and the status_detail will be set to DECLINED_BY_PAYER.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns></returns>
        public Task<SatispayCharge> UpdateChargeAsync(string chargeId, string description = null, Dictionary<string, string> metadata = null, string chargeState = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (chargeId == null)
                throw new ArgumentNullException(nameof(chargeId));
            Contract.EndContractBlock();

            var data = new
            {
                description,
                metadata,
                charge_state = chargeState
            };

            return RequestAsync<SatispayCharge>(HttpMethod.Put, $"/online/v1/charges/{chargeId}", data, cancellationToken);
        }
    }
}
