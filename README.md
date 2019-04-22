# Satispay Online .NET

Unofficial .NET package for satispay online API.

## Quick Start

```csharp
using SatispayOnline;

var satispayOnline = new SatispayOnlineClient("<SecurityBearerString>");

// for sandbox
var satispayOnline = new SatispayOnlineClient("<SecurityBearerString>", SatispayEnvironment.Sandbox);

// create a user
 SatispayUser user = await satispayOnline.CreateUserAsync("<PhoneNumber>");
```