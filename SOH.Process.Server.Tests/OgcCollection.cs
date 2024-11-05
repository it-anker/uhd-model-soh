namespace SOH.Process.Server.Tests;

// These classes has no code, and is never created. Its purpose is simply
// to be the place to apply [CollectionDefinition] and all the
// ICollectionFixture<> interfaces.

[CollectionDefinition("Database collection")]
public class OgcCollection : ICollectionFixture<OgcIntegration>;