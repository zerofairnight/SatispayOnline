using System;
using NUnit.Framework;

namespace SatispayOnline.Tests
{
    [TestFixture]
    public abstract class TestFixtureBase
    {
        public SatispayOnlineClient CreateSatispayOnline() =>
            new SatispayOnlineClient(
                TestContext.Parameters["SatispaySecurityBearer"],
                SatispayEnvironment.Sandbox
            );
    }
}
