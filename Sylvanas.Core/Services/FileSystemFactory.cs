using System.IO;
using System.Reflection;
using Zio;
using Zio.FileSystems;

namespace Sylvanas.Core.Services
{
    /// <summary>
    /// Serves as a factory class for abstract file systems.
    /// </summary>
    public static class FileSystemFactory
    {
        /// <summary>
        /// Creates a rooted sub-filesystem for the local content folder.
        /// </summary>
        /// <returns>The rooted filesystem.</returns>
        public static IFileSystem CreateContentFileSystem()
        {
            var realFileSystem = new PhysicalFileSystem();

            var executingAssemblyLocation = Assembly.GetExecutingAssembly().Location;
            var executingAssemblyDirectory = Directory.GetParent(executingAssemblyLocation).FullName;

            var realContentPath = Path.GetFullPath(Path.Combine(executingAssemblyDirectory, "Content"));
            var zioContentPath = realFileSystem.ConvertPathFromInternal(realContentPath);
            
            if (!realFileSystem.DirectoryExists(zioContentPath))
            {
                realFileSystem.CreateDirectory(zioContentPath);
            }

            return new SubFileSystem(realFileSystem, zioContentPath);
        }
    }
}