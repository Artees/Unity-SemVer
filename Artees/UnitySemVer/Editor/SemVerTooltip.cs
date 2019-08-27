using System.Collections.Generic;

namespace Artees.UnitySemVer.Editor
{
    internal static class SemVerTooltip
    {
        public const string Major = "Major";
        public const string Minor = "Minor";
        public const string Patch = "Patch";
        public const string PreRelease = "Pre-Release";
        public const string Build = "Build";

        public static readonly IReadOnlyDictionary<string, string> Field = new Dictionary<string, string>
        {
            {
                Major, "Major version X (X.y.z | X > 0) MUST be incremented if any backwards incompatible changes " +
                       "are introduced to the public API. It MAY include minor and patch level changes. Patch and " +
                       "minor version MUST be reset to 0 when major version is incremented."
            },
            {
                Minor, "Minor version Y (x.Y.z | x > 0) MUST be incremented if new, backwards compatible " +
                       "functionality is introduced to the public API. It MUST be incremented if any public API " +
                       "functionality is marked as deprecated. It MAY be incremented if substantial new " +
                       "functionality or improvements are introduced within the private code. It MAY include patch " +
                       "level changes. Patch version MUST be reset to 0 when minor version is incremented."
            },
            {
                Patch, "Patch version Z (x.y.Z | x > 0) MUST be incremented if only backwards compatible bug fixes " +
                       "are introduced."
            },
            {
                PreRelease, "A pre-release version indicates that the version is unstable and might not satisfy " +
                            "the intended compatibility requirements as denoted by its associated normal version."
            },
            {
                Build, "Build metadata MUST be ignored when determining version precedence. Thus two versions that " +
                       "differ only in the build metadata, have the same precedence."
            }
        };

        public static readonly IReadOnlyDictionary<string, string> Increment = new Dictionary<string, string>
        {
            {
                Major, "Increment the major version, reset the patch and the minor version to 0."
            },
            {
                Minor, "Increment the minor version, reset the patch version to 0."
            },
            {
                Patch, "Increment the patch version."
            }
        };
    }
}