
namespace Constellation.Package
{
    //
    // Summary:
    //     Provides the base implementation used to create Constellation package.
    public abstract class PackageBase : IPackage
    {
        protected PackageBase()
        { }

        //
        // Summary:
        //     Called before shutdown the package (the package is still connected to Constellation).
        public virtual void OnPreShutdown()
        { }
        //
        // Summary:
        //     Called when the package is shutdown (disconnected from Constellation)
        public virtual void OnShutdown()
        { }
        //
        // Summary:
        //     Called when the package is started.
        public virtual void OnStart()
        { }
    }
}