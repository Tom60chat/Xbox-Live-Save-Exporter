using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Xbox_Live_Save_Exporter
{
    static class PathHelper
    {
        private static readonly HashSet<string> ReservedNames = new HashSet<string>(
            new[] { "CON", "PRN", "AUX", "NUL",
                    "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
                    "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" });

        /// <summary>
        /// Turns an arbitrary string into a name that is legal as a single path
        /// component. Game display names may contain characters Windows forbids
        /// in paths (for instance "Commandos: Origins (Win)"), which used to make
        /// CreateFolderAsync throw.
        /// </summary>
        /// <param name="name">The name to make safe.</param>
        /// <param name="fallback">Name to use when nothing usable is left.</param>
        /// <returns>A name usable as a folder or file name.</returns>
        public static string Sanitize(string name, string fallback = "unnamed")
        {
            if (string.IsNullOrWhiteSpace(name)) return fallback;

            var invalid = Path.GetInvalidFileNameChars();
            var safe = new string(name.Select(c => invalid.Contains(c) ? '_' : c).ToArray())
                .Trim()
                .TrimEnd('.');

            if (safe.Length == 0) return fallback;

            // Device names are not usable even with an extension
            var stem = safe.Split('.')[0].ToUpperInvariant();
            if (ReservedNames.Contains(stem)) safe = "_" + safe;

            // Keep well clear of MAX_PATH
            return safe.Length > 120 ? safe.Substring(0, 120) : safe;
        }
    }
}
