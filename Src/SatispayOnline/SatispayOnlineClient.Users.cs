using System;
using System.Diagnostics.Contracts;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SatispayOnline
{
    public partial class SatispayOnlineClient
    {
        /// <summary>
        /// Create a user you want to send Charge request. The user must be subscribed to the satispay service.
        /// </summary>
        /// <param name="phoneNumber">The phone number of a Satispay user.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns></returns>
        public Task<SatispayUser> CreateUserAsync(string phoneNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (phoneNumber == null)
                throw new ArgumentNullException(nameof(phoneNumber));
            Contract.EndContractBlock();

            var data = new
            {
                phone_number = phoneNumber,
            };

            return RequestAsync<SatispayUser>(HttpMethod.Post, "/online/v1/users", data, cancellationToken);
        }

        /// <summary>
        /// Get an user by id, by the way the only new information returned from this call is the user phone_number.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns></returns>
        public Task<SatispayUser> GetUserAsync(string userId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId));
            Contract.EndContractBlock();

            return RequestAsync<SatispayUser>(HttpMethod.Get, $"/online/v1/users/{userId}", null, cancellationToken);
        }

        /// <summary>
        /// Get the list of shop Users of a online shop.
        /// </summary>
        /// <param name="limit">Max number of returned items</param>
        /// <param name="startingAfterUser"></param>
        /// <param name="endingBbeforeUser"></param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns></returns>
        public Task<SatispayUsersList> GetUsersAsync(int limit = 20, string startingAfterUser = null, string endingBbeforeUser = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (limit < 1)
                throw new ArgumentOutOfRangeException(nameof(limit));
            Contract.EndContractBlock();

            string url = $"/online/v1/users?limit={limit}";

            if (startingAfterUser != null)
            {
                url += $"&starting_after={startingAfterUser}";
            }

            if (endingBbeforeUser != null)
            {
                url += $"&ending_before={endingBbeforeUser}";
            }

            return RequestAsync<SatispayUsersList>(HttpMethod.Get, url, null, cancellationToken);
        }
    }
}
