using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Xbox_Live_Save_Exporter
{
    internal class ContainerHelper
    {
        /// <summary>
        /// Try to decode a Base64 string if it looks like Base64
        /// </summary>
        internal static string TryDecodeBase64(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length % 4 != 0)
                return input;

            try
            {
                byte[] decodedBytes = Convert.FromBase64String(input);
                string decoded = Encoding.UTF8.GetString(decodedBytes);

                // Check if decoded result looks valid (mostly printable characters)
                if (decoded.All(c => !char.IsControl(c) || c == '\n' || c == '\r' || c == '\t'))
                    return decoded;
            }
            catch { }

            return input;
        }

        /// <summary>
        /// Choose the best name from available options
        /// Prefers readable names over Base64-encoded or suspicious ones
        /// </summary>
        /// <param name="primaryName">The primary name (often Base64 encoded)</param>
        /// <param name="fallbackNames">Optional fallback names to consider</param>
        /// <returns>The best available name</returns>
        internal static string GetBestName(string primaryName, params string[] fallbackNames)
        {
            // Try to decode the primary name first
            string decodedPrimary = TryDecodeBase64(primaryName);

            // If decoding improved the name (it's different and looks good), use it
            if (decodedPrimary != primaryName && IsGoodName(decodedPrimary))
                return decodedPrimary;

            // Check fallback names - prefer non-empty, good-looking ones
            foreach (var name in fallbackNames ?? Array.Empty<string>())
            {
                if (IsGoodName(name))
                    return name;
            }

            // If nothing else works, return the decoded primary (or original if decoding failed)
            return !string.IsNullOrEmpty(decodedPrimary) ? decodedPrimary : primaryName;
        }

        /// <summary>
        /// Check if a name looks "good" (readable and valid)
        /// </summary>
        private static bool IsGoodName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            // Trim and check length
            name = name.Trim();
            if (name.Length == 0)
                return false;

            // A good name should contain mostly printable characters
            int printableCount = name.Count(c => !char.IsControl(c));
            return printableCount > (name.Length * 0.9); // At least 90% printable
        }

        internal static async Task<StorageFolder> CreateNestedFolderAsync(StorageFolder root, string relativePath)
        {
            var parts = relativePath.Split(
                new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
                StringSplitOptions.RemoveEmptyEntries);

            StorageFolder current = root;

            foreach (var part in parts)
                current = await current.CreateFolderAsync(PathHelper.Sanitize(part), CreationCollisionOption.OpenIfExists);

            return current;
        }
    }
}

