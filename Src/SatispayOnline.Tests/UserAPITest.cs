using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SatispayOnline.Tests
{
    [TestFixture]
    public class UserAPITest : TestFixtureBase
    {
        [Test]
        public async Task CreateUserAsyncIsValid()
        {
            var satispayOnline = CreateSatispayOnline();

            SatispayUser user = await satispayOnline.CreateUserAsync(TestContext.Parameters["SatispayPhoneNumber"]);

            Assert.IsNotNull(user.Id);
            Assert.IsNotNull(user.UUID);
            Assert.IsNotNull(user.PhoneNumber);
        }

        [Test]
        public async Task GetUserAsyncIsValid()
        {
            var satispayOnline = CreateSatispayOnline();

            SatispayUser user = await satispayOnline.CreateUserAsync(TestContext.Parameters["SatispayPhoneNumber"]);


            SatispayUser newUser = await satispayOnline.GetUserAsync(user.Id);

            Assert.IsNotNull(newUser.Id);
            Assert.IsNotNull(newUser.UUID);
            Assert.IsNotNull(newUser.PhoneNumber);

            Assert.IsTrue(newUser.Id == user.Id);
            Assert.IsNotNull(newUser.UUID == user.UUID);
            Assert.IsNotNull(newUser.PhoneNumber == user.PhoneNumber);
        }

        [Test]
        public async Task GetUsersAsyncIsValid()
        {
            var satispayOnline = CreateSatispayOnline();

            SatispayUsersList users = await satispayOnline.GetUsersAsync();

            Assert.IsTrue(users.List.Count > 0);
        }

        [Test]
        public async Task GetUsersAsyncHasMore()
        {
            var satispayOnline = CreateSatispayOnline();

            SatispayUsersList users = await satispayOnline.GetUsersAsync(1);

            Assert.IsTrue(users.List.Count == 1);
            Assert.IsTrue(users.HasMore == true);
        }
    }
}