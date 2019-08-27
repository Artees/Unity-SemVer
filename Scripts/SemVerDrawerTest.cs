using System.Diagnostics.CodeAnalysis;
using Artees.UnitySemVer;
using UnityEngine;

[SuppressMessage("ReSharper", "NotAccessedField.Local")]
public class SemVerDrawerTest : MonoBehaviour
{
    [SerializeField] private SemVer version;
    [SerializeField, SemVer] private string versionString;
}
