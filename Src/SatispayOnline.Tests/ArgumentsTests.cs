using System;
using NUnit.Framework;

namespace SatispayOnline.Tests
{
    [TestFixture]
    public class ArgumentsTests : TestFixtureBase
    {
        [Test(Description = "SatispayOnlineClient constructor dont accept null.")]
        public void NullSecurityBearerInConstructor()
        {
            Assert.Throws<ArgumentNullException>(() => new SatispayOnlineClient(null));
        }

        [Test(Description = "SatispayOnlineClient constructor dont accept empty string.")]
        public void EmptySecurityBearerInConstructor()
        {
            Assert.Throws<ArgumentException>(() => new SatispayOnlineClient(string.Empty));
        }

        [Test(Description = "SatispayOnlineClient CreateUser dont accept empty string.")]
        public void NullPhoneNumberInCreateUser()
        {
            var satispayOnlineClient = CreateSatispayOnline();

            Assert.ThrowsAsync<ArgumentNullException>(async () => await satispayOnlineClient.CreateUserAsync(null));
        }

        [Test(Description = "SatispayOnlineClient GetUser dont accept empty string.")]
        public void NullUserIdInGetUser()
        {
            var satispayOnlineClient = CreateSatispayOnline();

            Assert.ThrowsAsync<ArgumentNullException>(async () => await satispayOnlineClient.GetUserAsync(null));
        }

        [Test(Description = "SatispayOnlineClient GetUsers dont accept zero or negative limit.")]
        public void InvalidLimitInGetUsers()
        {
            var satispayOnlineClient = CreateSatispayOnline();

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await satispayOnlineClient.GetUsersAsync(0));
        }
    }
}
