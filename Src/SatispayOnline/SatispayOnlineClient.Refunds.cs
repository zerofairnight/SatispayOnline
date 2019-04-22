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
        /// To create a refund, you must specify the Charge to create it on.
        /// </summary>
        /// <param name="chargeId">ID of a Charge</param>
        /// <param name="currency">amount currency (eg. EUR).</param>
        /// <param name="amount">Expressed in the smallest currency units (Ex: 1.15 € = 115 units).</param>
        /// <param name="description">Refund description (max 255 chars).</param>
        /// <param name="reason">String indicating the reason for the refund. If set, possible values are DUPLICATE, FRAUDULENT, and REQUESTED_BY_CUSTOMER.</param>
        /// <param name="metadata">Object with max 20 keys. Key length 45 characters and values 500 characters. This param can be used</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns></returns>
        public Task<SatispayRefund> RefundChargeAsync(string chargeId, string currency, int amount, string description = null, string reason = null, Dictionary<string, string> metadata = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (chargeId == null)
                throw new ArgumentNullException(nameof(chargeId));
            if (currency == null)
                throw new ArgumentNullException(nameof(currency));
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            Contract.EndContractBlock();

            var data = new
            {
                charge_id = chargeId,
                currency,
                amount,
                description,
                metadata,
                reason,
            };

            return RequestAsync<SatispayRefund>(HttpMethod.Post, "/online/v1/refunds", data, cancellationToken);
        }
        
        /// <summary>
        /// Returns a list of all refunds you’ve previously created
        /// </summary>
        /// <param name="limit">Max number of returned items</param>
        /// <param name="startingAfterRefund">is a user id</param>
        /// <param name="endingBeforeRefund">is a user id</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns></returns>
        public Task<SatispayRefundsList> GetRefundsAsync(int limit = 20, string startingAfterRefund = null, string endingBeforeRefund = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (limit < 1)
                throw new ArgumentOutOfRangeException(nameof(limit));
            Contract.EndContractBlock();

            string url = $"/online/v1/users?limit={limit}";

            if (startingAfterRefund != null)
            {
                url += $"&starting_after={startingAfterRefund}";
            }

            if (endingBeforeRefund != null)
            {
                url += $"&ending_before={endingBeforeRefund}";
            }

            return RequestAsync<SatispayRefundsList>(HttpMethod.Get, url, null, cancellationToken);
        }
        
        /// <summary>
        /// get a refund by id.
        /// </summary>
        /// <param name="id">The id of a refund.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns></returns>
        public Task<SatispayRefund> GetRefundAsync(string refundId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (refundId == null)
                throw new ArgumentNullException(nameof(refundId));
            Contract.EndContractBlock();

            return RequestAsync<SatispayRefund>(HttpMethod.Get, $"/online/v1/refunds/{refundId}", null, cancellationToken);
        }
        
        /// <summary>
        /// update a refund by id.
        /// </summary>
        /// <param name="id">The id of a refund.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns></returns>
        public Task<SatispayRefund> UpdateRefundAsync(string refundId, Dictionary<string, string> metadata, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (refundId == null)
                throw new ArgumentNullException(nameof(refundId));
            if (metadata == null)
                throw new ArgumentNullException(nameof(metadata));
            Contract.EndContractBlock();

            var data = new
            {
                metadata
            };

            return RequestAsync<SatispayRefund>(HttpMethod.Put, $"/online/v1/refunds/{refundId}", data, cancellationToken);
        }
    }
}
