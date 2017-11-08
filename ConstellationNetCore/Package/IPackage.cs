
namespace Constellation.Package
{
    //
    // Summary:
    //     Defines a Constellation package
    public interface IPackage
    {
        //
        // Summary:
        //     Called before shutdown the package (the package is still connected to Constellation).
        void OnPreShutdown();
        //
        // Summary:
        //     Called when the package is shutdown (disconnected from Constellation)
        void OnShutdown();
        //
        // Summary:
        //     Called when the package is started.
        void OnStart();
    }
}