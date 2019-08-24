namespace Artees.UnitySemVer.Tests
{
    internal static class SuggestedRegEx
    {
        /// <summary>
        /// This is one of the
        /// <a href="https://semver.org/#is-there-a-suggested-regular-expression-regex-to-check-a-semver-string">
        /// suggested regular expressions to check a SemVer string</a>.
        /// </summary>
        public const string Pattern = @"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-((?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+([0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$";
    }
}