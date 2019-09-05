# Unity SemVer
A convenient way to edit and compare version numbers according to the [Semantic Versioning 2.0.0](https://semver.org/) specification. Also includes a property drawer for Unity.

![Property drawer](https://github.com/Artees/Unity-SemVer/raw/master/SemVerDrawer.png)

# Installation
Install the package **games.artees.semver** using [my package registry](https://artees.games/upm).

# Usage
Use the `Artees.UnitySemVer.SemVer` class or apply the `Artees.UnitySemVer.SemVerAttribute` attribute to a string field.
```
public SemVer version = new SemVer {major = 1, minor = 2, patch = 3};
[SemVer] public string versionString = "1.2.3";
```

Parsing:
```
var version = SemVer.Parse("2.0.0-rc.1+build.123");
```

Comparing:
```
Debug.Log("2.1.0"  > version);
```

Validating:
```
var result = version.Validate();
version = result.Corrected;
foreach (var message in result.Errors)
{
    Debug.LogWarning(message);
}
```