using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SatispayOnline.Tests
{
    [TestFixture]
    public class ChargeAPITest : TestFixtureBase
    {
        [Test]
        public async Task CreateChargeAsyncIsValid()
        {
            var satispayOnline = CreateSatispayOnline();

            SatispayUser user = await satispayOnline.CreateUserAsync(TestContext.Parameters["SatispayPhoneNumber"]);

            SatispayCharge charge = await satispayOnline.CreateChargeAsync(user.Id, SatispayCurrency.EUR, 1);

            Assert.IsNotNull(charge.Id);
            Assert.IsNotNull(charge.ExpireDate);
            Assert.IsTrue(charge.UserId == user.Id);
            Assert.IsTrue(charge.Amount == 1);
            Assert.IsTrue(charge.Status == SatispayChargeStatus.Required);
            Assert.IsNull(charge.StatusDetail);
            Assert.IsTrue(charge.Currency == SatispayCurrency.EUR);
        }

        [Test]
        public async Task GetChargesAsyncIsValid()
        {
            var satispayOnline = CreateSatispayOnline();

            SatispayChargesList charges = await satispayOnline.GetChargesAsync();

            Assert.IsTrue(charges.List.Count > 0);
        }

        [Test]
        public async Task GetChargeAsyncIsValid()
        {
            var satispayOnline = CreateSatispayOnline();
            
            SatispayUser user = await satispayOnline.CreateUserAsync(TestContext.Parameters["SatispayPhoneNumber"]);

            SatispayCharge charge = await satispayOnline.CreateChargeAsync(user.Id, SatispayCurrency.EUR, 1);

            SatispayCharge newCharge = await satispayOnline.GetChargeAsync(charge.Id);

            Assert.IsTrue(charge.Id == newCharge.Id);
        }
    }
}
