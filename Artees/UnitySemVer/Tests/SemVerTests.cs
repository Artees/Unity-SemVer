using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Artees.UnitySemVer.Tests
{
    [TestFixture]
    internal class SemVerTests
    {
        [TestCaseSource(typeof(Data), nameof(Data.ValidVersionsTestCases))]
        public void MajorMinorPatchTest(SemVer semVer)
        {
            Assert.That(semVer.major, Is.Not.Negative);
            Assert.That(semVer.minor, Is.Not.Negative);
            Assert.That(semVer.patch, Is.Not.Negative);
        }

        [TestCaseSource(typeof(Data), nameof(Data.ValidVersionsTestCases))]
        public void PreReleaseTest(SemVer semVer)
        {
            foreach (var identifier in semVer.preRelease.Split('.'))
            {
                Assert.That(identifier, Is.Empty.Or.Match("[0-9A-Za-z-]"));
                int.TryParse(identifier, out var numericIdentifier);
                if (numericIdentifier > 0)
                {
                    Assert.That(identifier, Does.Not.StartsWith("0"));
                }
            }
        }

        [TestCaseSource(typeof(Data), nameof(Data.ValidVersionsTestCases))]
        public void BuildTest(SemVer semVer)
        {
            foreach (var identifier in semVer.Build.Split('.'))
            {
                Assert.That(identifier, Is.Empty.Or.Match("[0-9A-Za-z-]"));
            }
        }

        [TestCaseSource(typeof(Data), nameof(Data.ValidateTestCases))]
        public ReadOnlyCollection<string> ValidateTest(SemVer invalid, SemVer corrected)
        {
            var original = invalid.Clone();
            var result = invalid.Validate();
            EqualsTest(original, invalid);
            EqualsTest(result.Corrected, corrected);
            MajorMinorPatchTest(result.Corrected);
            PreReleaseTest(result.Corrected);
            BuildTest(result.Corrected);
            var expression = result.IsValid ? new ConstraintExpression() : Does.Not;
            Assert.That(original.ToString(), expression.Match(SuggestedRegEx.Pattern));
            Assert.That(result.Corrected.ToString(), Does.Match(SuggestedRegEx.Pattern));
            return result.Errors;
        }

        [TestCaseSource(typeof(Data), nameof(Data.EqualsTestCases))]
        public void EqualsTest(SemVer left, SemVer right)
        {
            Assert.That(left, Is.EqualTo(right));
        }

        [TestCaseSource(typeof(Data), nameof(Data.NotEqualTestCases))]
        public void NotEqualTest(SemVer left, SemVer right)
        {
            Assert.That(left, Is.Not.EqualTo(right));
        }

        [TestCaseSource(typeof(Data), nameof(Data.CompareTestCases))]
        public void CompareTest(SemVer big, SemVer small)
        {
            Assert.That(big, Is.GreaterThan(small));
            Assert.That(small, Is.LessThan(big));
        }

        [TestCaseSource(typeof(Data), nameof(Data.ConvertToStringTestCases))]
        public string ConvertToStringTest(SemVer semVer)
        {
            return semVer;
        }

        [TestCaseSource(typeof(Data), nameof(Data.ConvertFromStringTestCases))]
        public SemVer ConvertFromStringTest(string semVer)
        {
            return semVer;
        }

        [TestCaseSource(typeof(Data), nameof(Data.AutoBuildTestCases))]
        public string AutoBuildTest(SemVer semVer)
        {
            Assert.That(SemVerAutoBuild.Instances.Keys, Does.Contain(semVer.autoBuild));
            return semVer.Build;
        }

        [TestCaseSource(typeof(Data), nameof(Data.CoreTestCases))]
        public string CoreTest(SemVer semVer)
        {
            return semVer.Core;
        }

        [TestCaseSource(typeof(Data), nameof(Data.AndroidBundleVersionCode))]
        public int AndroidBundleVersionCodeTest(SemVer semVer)
        {
            return semVer.AndroidBundleVersionCode;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private class Data
        {
            private static IEnumerable<SemVer> ValidVersions
            {
                get
                {
                    yield return new SemVer();
                    yield return new SemVer
                    {
                        major = 1,
                        minor = 2,
                        patch = 3
                    };
                    yield return new SemVer
                    {
                        major = 1,
                        minor = 2,
                        patch = 3,
                        preRelease = "alpha"
                    };
                    yield return new SemVer
                    {
                        major = 1,
                        minor = 2,
                        patch = 3,
                        preRelease = "alpha",
                        Build = "CustomBuild1"
                    };
                    yield return new SemVer
                    {
                        major = 1,
                        minor = 2,
                        patch = 3,
                        preRelease = "alpha",
                        Build = "CustomBuild2"
                    };
                    yield return new SemVer
                    {
                        preRelease = "ALPHA"
                    };
                    yield return new SemVer
                    {
                        preRelease = "alpha.1"
                    };
                    yield return new SemVer
                    {
                        preRelease = "0.3.7",
                        Build = "20130313144700"
                    };
                    yield return new SemVer
                    {
                        preRelease = "x.7.z.92",
                        Build = "exp.sha.5114f85"
                    };
                    yield return new SemVer
                    {
                        major = 1,
                        minor = 0,
                        patch = 0,
                        preRelease = "alpha",
                        Build = "001"
                    };
                }
            }

            public static IEnumerable<TestCaseData> ValidVersionsTestCases
            {
                get { return ValidVersions.Select(semVer => new TestCaseData(semVer)); }
            }

            public static IEnumerable<TestCaseData> ValidateTestCases
            {
                get
                {
                    yield return new TestCaseData(new SemVer
                    {
                        preRelease = ".",
                        Build = "."
                    }, new SemVer()).Returns(new[]
                    {
                        SemVerErrorMessage.Empty,
                        SemVerErrorMessage.Empty
                    });
                    yield return new TestCaseData(new SemVer
                    {
                        preRelease = "a..a",
                        Build = "a..a"
                    }, new SemVer
                    {
                        preRelease = "a.a",
                        Build = "a.a"
                    }).Returns(new[]
                    {
                        SemVerErrorMessage.Empty,
                        SemVerErrorMessage.Empty
                    });
                    yield return new TestCaseData(new SemVer
                    {
                        preRelease = "a a",
                        Build = "a a"
                    }, new SemVer
                    {
                        preRelease = "a-a",
                        Build = "a-a"
                    }).Returns(new[]
                    {
                        SemVerErrorMessage.Invalid,
                        SemVerErrorMessage.Invalid
                    });
                    yield return new TestCaseData(new SemVer
                    {
                        preRelease = "$",
                        Build = "$"
                    }, new SemVer
                    {
                        preRelease = "-",
                        Build = "-"
                    }).Returns(new[]
                    {
                        SemVerErrorMessage.Invalid,
                        SemVerErrorMessage.Invalid
                    });
                    yield return new TestCaseData(new SemVer
                    {
                        preRelease = "01"
                    }, new SemVer
                    {
                        preRelease = "1"
                    }).Returns(new[]
                    {
                        SemVerErrorMessage.LeadingZero
                    });
                }
            }

            public static IEnumerable<TestCaseData> EqualsTestCases
            {
                get
                {
                    foreach (var semVer in ValidVersions)
                    {
                        yield return new TestCaseData(semVer, semVer.Clone());
                    }

                    yield return new TestCaseData(
                        new SemVer
                        {
                            major = 1,
                            minor = 7,
                            patch = 3,
                            preRelease = "alpha",
                            Build = "001"
                        },
                        new SemVer
                        {
                            major = 1,
                            minor = 7,
                            patch = 3,
                            preRelease = "alpha",
                            Build = "2"
                        });
                }
            }

            public static IEnumerable<TestCaseData> NotEqualTestCases
            {
                get
                {
                    foreach (var semVer in ValidVersions)
                    {
                        var other = semVer.Clone();
                        other.IncrementMajor();
                        yield return new TestCaseData(semVer, other);
                    }
                }
            }

            public static IEnumerable<TestCaseData> CompareTestCases
            {
                get
                {
                    yield return new TestCaseData(
                        new SemVer
                        {
                            major = 2,
                            minor = 1,
                            patch = 1
                        },
                        new SemVer
                        {
                            major = 2,
                            minor = 1,
                            patch = 0
                        });
                    yield return new TestCaseData(
                        new SemVer
                        {
                            major = 2,
                            minor = 1,
                            patch = 0
                        },
                        new SemVer
                        {
                            major = 2,
                            minor = 0,
                            patch = 0
                        });
                    yield return new TestCaseData(
                        new SemVer
                        {
                            major = 2,
                            minor = 0,
                            patch = 0
                        },
                        new SemVer
                        {
                            major = 1,
                            minor = 0,
                            patch = 0
                        });
                    yield return new TestCaseData(
                        new SemVer
                        {
                            major = 1,
                            minor = 7,
                            patch = 3,
                            preRelease = string.Empty
                        },
                        new SemVer
                        {
                            major = 1,
                            minor = 7,
                            patch = 3,
                            preRelease = "alpha"
                        });
                    yield return new TestCaseData(
                        new SemVer
                        {
                            major = 1,
                            minor = 0,
                            patch = 0
                        },
                        new SemVer
                        {
                            major = 1,
                            minor = 0,
                            patch = 0,
                            preRelease = "rc.1"
                        });
                    yield return new TestCaseData(
                        new SemVer
                        {
                            major = 1,
                            minor = 0,
                            patch = 0,
                            preRelease = "rc.1"
                        },
                        new SemVer
                        {
                            major = 1,
                            minor = 0,
                            patch = 0,
                            preRelease = "beta.11"
                        });
                    yield return new TestCaseData(
                        new SemVer
                        {
                            major = 1,
                            minor = 0,
                            patch = 0,
                            preRelease = "beta.11"
                        },
                        new SemVer
                        {
                            major = 1,
                            minor = 0,
                            patch = 0,
                            preRelease = "beta.2"
                        });
                    yield return new TestCaseData(
                        new SemVer
                        {
                            major = 1,
                            minor = 0,
                            patch = 0,
                            preRelease = "beta.2"
                        },
                        new SemVer
                        {
                            major = 1,
                            minor = 0,
                            patch = 0,
                            preRelease = "beta"
                        });
                    yield return new TestCaseData(
                        new SemVer
                        {
                            major = 1,
                            minor = 0,
                            patch = 0,
                            preRelease = "beta"
                        },
                        new SemVer
                        {
                            major = 1,
                            minor = 0,
                            patch = 0,
                            preRelease = "alpha.beta"
                        });
                    yield return new TestCaseData(
                        new SemVer
                        {
                            major = 1,
                            minor = 0,
                            patch = 0,
                            preRelease = "alpha.beta"
                        },
                        new SemVer
                        {
                            major = 1,
                            minor = 0,
                            patch = 0,
                            preRelease = "alpha.1"
                        });
                    yield return new TestCaseData(
                        new SemVer
                        {
                            major = 1,
                            minor = 0,
                            patch = 0,
                            preRelease = "alpha.1"
                        },
                        new SemVer
                        {
                            major = 1,
                            minor = 0,
                            patch = 0,
                            preRelease = "alpha"
                        });
                }
            }

            public static IEnumerable<TestCaseData> ConvertToStringTestCases
            {
                get
                {
                    yield return new TestCaseData(new SemVer
                    {
                        major = 1,
                        minor = 2,
                        patch = 3,
                        preRelease = "pr",
                        Build = "b"
                    }).Returns("1.2.3-pr+b");
                    yield return new TestCaseData(new SemVer
                    {
                        preRelease = "pre-alpha"
                    }).Returns("0.1.0-pre-alpha");
                }
            }

            public static IEnumerable<TestCaseData> ConvertFromStringTestCases
            {
                get
                {
                    yield return new TestCaseData("1.2.3-pr+b").Returns(new SemVer
                    {
                        major = 1,
                        minor = 2,
                        patch = 3,
                        preRelease = "pr",
                        Build = "b"
                    });
                    yield return new TestCaseData("1.2.3+b").Returns(new SemVer
                    {
                        major = 1,
                        minor = 2,
                        patch = 3,
                        Build = "b"
                    });
                    yield return new TestCaseData("1.2.3-pr").Returns(new SemVer
                    {
                        major = 1,
                        minor = 2,
                        patch = 3,
                        preRelease = "pr"
                    });
                    yield return new TestCaseData("0.1.0-pre-alpha").Returns(new SemVer
                    {
                        preRelease = "pre-alpha"
                    });
                }
            }

            public static IEnumerable<TestCaseData> AutoBuildTestCases
            {
                get
                {
                    yield return new TestCaseData(new SemVer
                    {
                        autoBuild = SemVerAutoBuild.Type.Manual,
                        Build = "auto-build"
                    }).Returns("auto-build");
                }
            }

            public static IEnumerable<TestCaseData> CoreTestCases
            {
                get
                {
                    yield return new TestCaseData(new SemVer
                    {
                        major = 4,
                        minor = 5,
                        patch = 6,
                        preRelease = "alpha",
                        Build = "CustomBuild3"
                    }).Returns("4.5.6");
                }
            }

            public static IEnumerable<TestCaseData> AndroidBundleVersionCode
            {
                get
                {
                    yield return new TestCaseData(new SemVer
                    {
                        major = 1,
                        minor = 2,
                        patch = 3,
                        preRelease = "alpha",
                        Build = "CustomBuild4"
                    }).Returns(10203);
                }
            }
        }
    }
}