using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Results;
using Sylvanas.Core.Async;
using Zio;

namespace Sylvanas
{
    public class ContentService
    {

        /// <summary>
        /// Gets the virtual filesystem that encapsulates the content.
        /// </summary>
        public IFileSystem FileSystem { get; }

        /// <summary>
        /// Gets the path to the database credentials.
        /// </summary>
        private readonly UPath _databaseCredentialsPath;

        /// <summary>
        /// Gets the path to the discord token.
        /// </summary>
        private readonly UPath _tokenPath;
        
        public ContentService(IFileSystem fileSystem)
        {
            FileSystem = fileSystem;

            _databaseCredentialsPath = UPath.Combine(UPath.Root, "Database", "database.credentials");
            _tokenPath = UPath.Combine(UPath.Root, "Discord", "bot.token");
        }
        
        /// <summary>
        /// Loads the bot token from disk.
        /// </summary>
        /// <exception cref="FileNotFoundException">Thrown if the bot token file can't be found.</exception>
        /// <exception cref="InvalidDataException">Thrown if no token exists in the file.</exception>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        public async Task<RetrieveEntityResult<string>> GetBotTokenAsync()
        {
            if (!FileSystem.FileExists(_tokenPath))
            {
                return RetrieveEntityResult<string>.FromError("The token file could not be found.");
            }

            var getTokenStream = OpenLocalStream(_tokenPath);
            if (!getTokenStream.IsSuccess)
            {
                return RetrieveEntityResult<string>.FromError("The token file could not be opened.");
            }

            await using var tokenStream = getTokenStream.Entity;
            var token = await AsyncIO.ReadAllTextAsync(tokenStream);

            if (string.IsNullOrEmpty(token))
            {
                return RetrieveEntityResult<string>.FromError("The token file did not contain a valid token.");
            }

            return RetrieveEntityResult<string>.FromSuccess(token);
        }
        
        /// <summary>
        /// Gets the stream of a local content file.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <param name="fileMode">The mode with which to open the stream.</param>
        /// <param name="fileAccess">The access rights with which to open the stream.</param>
        /// <param name="fileShare">The sharing rights with which to open the stream.</param>
        /// <returns>A <see cref="FileStream"/> with the file data.</returns>
        [Pure]
        [MustUseReturnValue("The resulting file stream must be disposed.")]
        public RetrieveEntityResult<Stream> OpenLocalStream
        (
            [PathReference] UPath path,
            FileMode fileMode = FileMode.Open,
            FileAccess fileAccess = FileAccess.Read,
            FileShare fileShare = FileShare.Read
        )
        {
            if (!path.IsAbsolute)
            {
                return RetrieveEntityResult<Stream>.FromError("Content paths must be absolute.");
            }

            try
            {
                var file = FileSystem.OpenFile(path, fileMode, fileAccess, fileShare);
                return RetrieveEntityResult<Stream>.FromSuccess(file);
            }
            catch (IOException iex)
            {
                return RetrieveEntityResult<Stream>.FromError(iex);
            }
        }
    }
}