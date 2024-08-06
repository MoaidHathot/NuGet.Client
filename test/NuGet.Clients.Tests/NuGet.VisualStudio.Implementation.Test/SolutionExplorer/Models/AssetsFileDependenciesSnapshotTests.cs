// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Linq;
using NuGet.ProjectModel;
using NuGet.Versioning;
using NuGet.VisualStudio.SolutionExplorer.Models;
using Xunit;

namespace NuGet.VisualStudio.Implementation.Test.SolutionExplorer.Models
{
    public class AssetsFileDependenciesSnapshotTests
    {
        [Fact]
        public void TransitiveDependencyWarningsPropagate()
        {
            #region Test data

            var lockFileContent = """
                {
                  "version": 3,
                  "targets": {
                    "net8.0": {
                      "Fluid.Core/2.7.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.Extensions.FileProviders.Abstractions": "1.1.1",
                          "Parlot": "0.0.24",
                          "TimeZoneConverter": "6.1.0"
                        },
                        "compile": {
                          "lib/net8.0/Fluid.dll": {
                            "related": ".pdb;.xml"
                          }
                        },
                        "runtime": {
                          "lib/net8.0/Fluid.dll": {
                            "related": ".pdb;.xml"
                          }
                        }
                      },
                      "Microsoft.Extensions.FileProviders.Abstractions/1.1.1": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.Extensions.Primitives": "1.1.1",
                          "NETStandard.Library": "1.6.1"
                        },
                        "compile": {
                          "lib/netstandard1.0/Microsoft.Extensions.FileProviders.Abstractions.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.0/Microsoft.Extensions.FileProviders.Abstractions.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "Microsoft.Extensions.Primitives/1.1.1": {
                        "type": "package",
                        "dependencies": {
                          "NETStandard.Library": "1.6.1",
                          "System.Runtime.CompilerServices.Unsafe": "4.3.0"
                        },
                        "compile": {
                          "lib/netstandard1.0/Microsoft.Extensions.Primitives.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.0/Microsoft.Extensions.Primitives.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "Microsoft.NETCore.Platforms/1.1.0": {
                        "type": "package",
                        "compile": {
                          "lib/netstandard1.0/_._": {}
                        },
                        "runtime": {
                          "lib/netstandard1.0/_._": {}
                        }
                      },
                      "Microsoft.NETCore.Targets/1.1.0": {
                        "type": "package",
                        "compile": {
                          "lib/netstandard1.0/_._": {}
                        },
                        "runtime": {
                          "lib/netstandard1.0/_._": {}
                        }
                      },
                      "Microsoft.Win32.Primitives/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/Microsoft.Win32.Primitives.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "NETStandard.Library/1.6.1": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.Win32.Primitives": "4.3.0",
                          "System.AppContext": "4.3.0",
                          "System.Collections": "4.3.0",
                          "System.Collections.Concurrent": "4.3.0",
                          "System.Console": "4.3.0",
                          "System.Diagnostics.Debug": "4.3.0",
                          "System.Diagnostics.Tools": "4.3.0",
                          "System.Diagnostics.Tracing": "4.3.0",
                          "System.Globalization": "4.3.0",
                          "System.Globalization.Calendars": "4.3.0",
                          "System.IO": "4.3.0",
                          "System.IO.Compression": "4.3.0",
                          "System.IO.Compression.ZipFile": "4.3.0",
                          "System.IO.FileSystem": "4.3.0",
                          "System.IO.FileSystem.Primitives": "4.3.0",
                          "System.Linq": "4.3.0",
                          "System.Linq.Expressions": "4.3.0",
                          "System.Net.Http": "4.3.0",
                          "System.Net.Primitives": "4.3.0",
                          "System.Net.Sockets": "4.3.0",
                          "System.ObjectModel": "4.3.0",
                          "System.Reflection": "4.3.0",
                          "System.Reflection.Extensions": "4.3.0",
                          "System.Reflection.Primitives": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Extensions": "4.3.0",
                          "System.Runtime.Handles": "4.3.0",
                          "System.Runtime.InteropServices": "4.3.0",
                          "System.Runtime.InteropServices.RuntimeInformation": "4.3.0",
                          "System.Runtime.Numerics": "4.3.0",
                          "System.Security.Cryptography.Algorithms": "4.3.0",
                          "System.Security.Cryptography.Encoding": "4.3.0",
                          "System.Security.Cryptography.Primitives": "4.3.0",
                          "System.Security.Cryptography.X509Certificates": "4.3.0",
                          "System.Text.Encoding": "4.3.0",
                          "System.Text.Encoding.Extensions": "4.3.0",
                          "System.Text.RegularExpressions": "4.3.0",
                          "System.Threading": "4.3.0",
                          "System.Threading.Tasks": "4.3.0",
                          "System.Threading.Timer": "4.3.0",
                          "System.Xml.ReaderWriter": "4.3.0",
                          "System.Xml.XDocument": "4.3.0"
                        }
                      },
                      "Parlot/0.0.24": {
                        "type": "package",
                        "compile": {
                          "lib/netstandard2.1/Parlot.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard2.1/Parlot.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "runtime.debian.8-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                        "type": "package",
                        "runtimeTargets": {
                          "runtimes/debian.8-x64/native/System.Security.Cryptography.Native.OpenSsl.so": {
                            "assetType": "native",
                            "rid": "debian.8-x64"
                          }
                        }
                      },
                      "runtime.fedora.23-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                        "type": "package",
                        "runtimeTargets": {
                          "runtimes/fedora.23-x64/native/System.Security.Cryptography.Native.OpenSsl.so": {
                            "assetType": "native",
                            "rid": "fedora.23-x64"
                          }
                        }
                      },
                      "runtime.fedora.24-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                        "type": "package",
                        "runtimeTargets": {
                          "runtimes/fedora.24-x64/native/System.Security.Cryptography.Native.OpenSsl.so": {
                            "assetType": "native",
                            "rid": "fedora.24-x64"
                          }
                        }
                      },
                      "runtime.native.System/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0"
                        },
                        "compile": {
                          "lib/netstandard1.0/_._": {}
                        },
                        "runtime": {
                          "lib/netstandard1.0/_._": {}
                        }
                      },
                      "runtime.native.System.IO.Compression/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0"
                        },
                        "compile": {
                          "lib/netstandard1.0/_._": {}
                        },
                        "runtime": {
                          "lib/netstandard1.0/_._": {}
                        }
                      },
                      "runtime.native.System.Net.Http/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0"
                        },
                        "compile": {
                          "lib/netstandard1.0/_._": {}
                        },
                        "runtime": {
                          "lib/netstandard1.0/_._": {}
                        }
                      },
                      "runtime.native.System.Security.Cryptography.Apple/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "runtime.osx.10.10-x64.runtime.native.System.Security.Cryptography.Apple": "4.3.0"
                        },
                        "compile": {
                          "lib/netstandard1.0/_._": {}
                        },
                        "runtime": {
                          "lib/netstandard1.0/_._": {}
                        }
                      },
                      "runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "runtime.debian.8-x64.runtime.native.System.Security.Cryptography.OpenSsl": "4.3.0",
                          "runtime.fedora.23-x64.runtime.native.System.Security.Cryptography.OpenSsl": "4.3.0",
                          "runtime.fedora.24-x64.runtime.native.System.Security.Cryptography.OpenSsl": "4.3.0",
                          "runtime.opensuse.13.2-x64.runtime.native.System.Security.Cryptography.OpenSsl": "4.3.0",
                          "runtime.opensuse.42.1-x64.runtime.native.System.Security.Cryptography.OpenSsl": "4.3.0",
                          "runtime.osx.10.10-x64.runtime.native.System.Security.Cryptography.OpenSsl": "4.3.0",
                          "runtime.rhel.7-x64.runtime.native.System.Security.Cryptography.OpenSsl": "4.3.0",
                          "runtime.ubuntu.14.04-x64.runtime.native.System.Security.Cryptography.OpenSsl": "4.3.0",
                          "runtime.ubuntu.16.04-x64.runtime.native.System.Security.Cryptography.OpenSsl": "4.3.0",
                          "runtime.ubuntu.16.10-x64.runtime.native.System.Security.Cryptography.OpenSsl": "4.3.0"
                        },
                        "compile": {
                          "lib/netstandard1.0/_._": {}
                        },
                        "runtime": {
                          "lib/netstandard1.0/_._": {}
                        }
                      },
                      "runtime.opensuse.13.2-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                        "type": "package",
                        "runtimeTargets": {
                          "runtimes/opensuse.13.2-x64/native/System.Security.Cryptography.Native.OpenSsl.so": {
                            "assetType": "native",
                            "rid": "opensuse.13.2-x64"
                          }
                        }
                      },
                      "runtime.opensuse.42.1-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                        "type": "package",
                        "runtimeTargets": {
                          "runtimes/opensuse.42.1-x64/native/System.Security.Cryptography.Native.OpenSsl.so": {
                            "assetType": "native",
                            "rid": "opensuse.42.1-x64"
                          }
                        }
                      },
                      "runtime.osx.10.10-x64.runtime.native.System.Security.Cryptography.Apple/4.3.0": {
                        "type": "package",
                        "runtimeTargets": {
                          "runtimes/osx.10.10-x64/native/System.Security.Cryptography.Native.Apple.dylib": {
                            "assetType": "native",
                            "rid": "osx.10.10-x64"
                          }
                        }
                      },
                      "runtime.osx.10.10-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                        "type": "package",
                        "runtimeTargets": {
                          "runtimes/osx.10.10-x64/native/System.Security.Cryptography.Native.OpenSsl.dylib": {
                            "assetType": "native",
                            "rid": "osx.10.10-x64"
                          }
                        }
                      },
                      "runtime.rhel.7-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                        "type": "package",
                        "runtimeTargets": {
                          "runtimes/rhel.7-x64/native/System.Security.Cryptography.Native.OpenSsl.so": {
                            "assetType": "native",
                            "rid": "rhel.7-x64"
                          }
                        }
                      },
                      "runtime.ubuntu.14.04-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                        "type": "package",
                        "runtimeTargets": {
                          "runtimes/ubuntu.14.04-x64/native/System.Security.Cryptography.Native.OpenSsl.so": {
                            "assetType": "native",
                            "rid": "ubuntu.14.04-x64"
                          }
                        }
                      },
                      "runtime.ubuntu.16.04-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                        "type": "package",
                        "runtimeTargets": {
                          "runtimes/ubuntu.16.04-x64/native/System.Security.Cryptography.Native.OpenSsl.so": {
                            "assetType": "native",
                            "rid": "ubuntu.16.04-x64"
                          }
                        }
                      },
                      "runtime.ubuntu.16.10-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                        "type": "package",
                        "runtimeTargets": {
                          "runtimes/ubuntu.16.10-x64/native/System.Security.Cryptography.Native.OpenSsl.so": {
                            "assetType": "native",
                            "rid": "ubuntu.16.10-x64"
                          }
                        }
                      },
                      "System.AppContext/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.6/System.AppContext.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.6/System.AppContext.dll": {}
                        }
                      },
                      "System.Buffers/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Diagnostics.Debug": "4.3.0",
                          "System.Diagnostics.Tracing": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Threading": "4.3.0"
                        },
                        "compile": {
                          "lib/netstandard1.1/_._": {}
                        },
                        "runtime": {
                          "lib/netstandard1.1/System.Buffers.dll": {}
                        }
                      },
                      "System.Collections/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.Collections.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Collections.Concurrent/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Collections": "4.3.0",
                          "System.Diagnostics.Debug": "4.3.0",
                          "System.Diagnostics.Tracing": "4.3.0",
                          "System.Globalization": "4.3.0",
                          "System.Reflection": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Extensions": "4.3.0",
                          "System.Threading": "4.3.0",
                          "System.Threading.Tasks": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.Collections.Concurrent.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.3/System.Collections.Concurrent.dll": {}
                        }
                      },
                      "System.Console/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.IO": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Text.Encoding": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.Console.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Diagnostics.Debug/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.Diagnostics.Debug.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Diagnostics.DiagnosticSource/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Collections": "4.3.0",
                          "System.Diagnostics.Tracing": "4.3.0",
                          "System.Reflection": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Threading": "4.3.0"
                        },
                        "compile": {
                          "lib/netstandard1.3/_._": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.3/System.Diagnostics.DiagnosticSource.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Diagnostics.Tools/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.0/System.Diagnostics.Tools.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Diagnostics.Tracing/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.5/System.Diagnostics.Tracing.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Globalization/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.Globalization.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Globalization.Calendars/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.Globalization": "4.3.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.Globalization.Calendars.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Globalization.Extensions/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "System.Globalization": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Extensions": "4.3.0",
                          "System.Runtime.InteropServices": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/_._": {
                            "related": ".xml"
                          }
                        },
                        "runtimeTargets": {
                          "runtimes/unix/lib/netstandard1.3/System.Globalization.Extensions.dll": {
                            "assetType": "runtime",
                            "rid": "unix"
                          },
                          "runtimes/win/lib/netstandard1.3/System.Globalization.Extensions.dll": {
                            "assetType": "runtime",
                            "rid": "win"
                          }
                        }
                      },
                      "System.IO/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.Runtime": "4.3.0",
                          "System.Text.Encoding": "4.3.0",
                          "System.Threading.Tasks": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.5/System.IO.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.IO.Compression/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "System.Buffers": "4.3.0",
                          "System.Collections": "4.3.0",
                          "System.Diagnostics.Debug": "4.3.0",
                          "System.IO": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Extensions": "4.3.0",
                          "System.Runtime.Handles": "4.3.0",
                          "System.Runtime.InteropServices": "4.3.0",
                          "System.Text.Encoding": "4.3.0",
                          "System.Threading": "4.3.0",
                          "System.Threading.Tasks": "4.3.0",
                          "runtime.native.System": "4.3.0",
                          "runtime.native.System.IO.Compression": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.IO.Compression.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtimeTargets": {
                          "runtimes/unix/lib/netstandard1.3/System.IO.Compression.dll": {
                            "assetType": "runtime",
                            "rid": "unix"
                          },
                          "runtimes/win/lib/netstandard1.3/System.IO.Compression.dll": {
                            "assetType": "runtime",
                            "rid": "win"
                          }
                        }
                      },
                      "System.IO.Compression.ZipFile/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Buffers": "4.3.0",
                          "System.IO": "4.3.0",
                          "System.IO.Compression": "4.3.0",
                          "System.IO.FileSystem": "4.3.0",
                          "System.IO.FileSystem.Primitives": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Extensions": "4.3.0",
                          "System.Text.Encoding": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.IO.Compression.ZipFile.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.3/System.IO.Compression.ZipFile.dll": {}
                        }
                      },
                      "System.IO.FileSystem/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.IO": "4.3.0",
                          "System.IO.FileSystem.Primitives": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Handles": "4.3.0",
                          "System.Text.Encoding": "4.3.0",
                          "System.Threading.Tasks": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.IO.FileSystem.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.IO.FileSystem.Primitives/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.IO.FileSystem.Primitives.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.3/System.IO.FileSystem.Primitives.dll": {}
                        }
                      },
                      "System.Linq/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Collections": "4.3.0",
                          "System.Diagnostics.Debug": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Extensions": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.6/System.Linq.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.6/System.Linq.dll": {}
                        }
                      },
                      "System.Linq.Expressions/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Collections": "4.3.0",
                          "System.Diagnostics.Debug": "4.3.0",
                          "System.Globalization": "4.3.0",
                          "System.IO": "4.3.0",
                          "System.Linq": "4.3.0",
                          "System.ObjectModel": "4.3.0",
                          "System.Reflection": "4.3.0",
                          "System.Reflection.Emit": "4.3.0",
                          "System.Reflection.Emit.ILGeneration": "4.3.0",
                          "System.Reflection.Emit.Lightweight": "4.3.0",
                          "System.Reflection.Extensions": "4.3.0",
                          "System.Reflection.Primitives": "4.3.0",
                          "System.Reflection.TypeExtensions": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Extensions": "4.3.0",
                          "System.Threading": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.6/System.Linq.Expressions.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.6/System.Linq.Expressions.dll": {}
                        }
                      },
                      "System.Net.Http/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "System.Collections": "4.3.0",
                          "System.Diagnostics.Debug": "4.3.0",
                          "System.Diagnostics.DiagnosticSource": "4.3.0",
                          "System.Diagnostics.Tracing": "4.3.0",
                          "System.Globalization": "4.3.0",
                          "System.Globalization.Extensions": "4.3.0",
                          "System.IO": "4.3.0",
                          "System.IO.FileSystem": "4.3.0",
                          "System.Net.Primitives": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Extensions": "4.3.0",
                          "System.Runtime.Handles": "4.3.0",
                          "System.Runtime.InteropServices": "4.3.0",
                          "System.Security.Cryptography.Algorithms": "4.3.0",
                          "System.Security.Cryptography.Encoding": "4.3.0",
                          "System.Security.Cryptography.OpenSsl": "4.3.0",
                          "System.Security.Cryptography.Primitives": "4.3.0",
                          "System.Security.Cryptography.X509Certificates": "4.3.0",
                          "System.Text.Encoding": "4.3.0",
                          "System.Threading": "4.3.0",
                          "System.Threading.Tasks": "4.3.0",
                          "runtime.native.System": "4.3.0",
                          "runtime.native.System.Net.Http": "4.3.0",
                          "runtime.native.System.Security.Cryptography.OpenSsl": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.Net.Http.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtimeTargets": {
                          "runtimes/unix/lib/netstandard1.6/System.Net.Http.dll": {
                            "assetType": "runtime",
                            "rid": "unix"
                          },
                          "runtimes/win/lib/netstandard1.3/System.Net.Http.dll": {
                            "assetType": "runtime",
                            "rid": "win"
                          }
                        }
                      },
                      "System.Net.Primitives/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Handles": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.Net.Primitives.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Net.Sockets/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.IO": "4.3.0",
                          "System.Net.Primitives": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Threading.Tasks": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.Net.Sockets.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.ObjectModel/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Collections": "4.3.0",
                          "System.Diagnostics.Debug": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Threading": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.ObjectModel.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.3/System.ObjectModel.dll": {}
                        }
                      },
                      "System.Reflection/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.IO": "4.3.0",
                          "System.Reflection.Primitives": "4.3.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.5/System.Reflection.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Reflection.Emit/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.IO": "4.3.0",
                          "System.Reflection": "4.3.0",
                          "System.Reflection.Emit.ILGeneration": "4.3.0",
                          "System.Reflection.Primitives": "4.3.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.1/_._": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.3/System.Reflection.Emit.dll": {}
                        }
                      },
                      "System.Reflection.Emit.ILGeneration/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Reflection": "4.3.0",
                          "System.Reflection.Primitives": "4.3.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.0/_._": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.3/System.Reflection.Emit.ILGeneration.dll": {}
                        }
                      },
                      "System.Reflection.Emit.Lightweight/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Reflection": "4.3.0",
                          "System.Reflection.Emit.ILGeneration": "4.3.0",
                          "System.Reflection.Primitives": "4.3.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.0/_._": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.3/System.Reflection.Emit.Lightweight.dll": {}
                        }
                      },
                      "System.Reflection.Extensions/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.Reflection": "4.3.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.0/System.Reflection.Extensions.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Reflection.Primitives/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.0/System.Reflection.Primitives.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Reflection.TypeExtensions/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Reflection": "4.3.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.5/_._": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.5/System.Reflection.TypeExtensions.dll": {}
                        }
                      },
                      "System.Resources.ResourceManager/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.Globalization": "4.3.0",
                          "System.Reflection": "4.3.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.0/System.Resources.ResourceManager.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Runtime/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0"
                        },
                        "compile": {
                          "ref/netstandard1.5/System.Runtime.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Runtime.CompilerServices.Unsafe/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "lib/netstandard1.0/System.Runtime.CompilerServices.Unsafe.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.0/System.Runtime.CompilerServices.Unsafe.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Runtime.Extensions/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.5/System.Runtime.Extensions.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Runtime.Handles/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.Runtime.Handles.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Runtime.InteropServices/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.Reflection": "4.3.0",
                          "System.Reflection.Primitives": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Handles": "4.3.0"
                        },
                        "compile": {
                          "ref/netcoreapp1.1/System.Runtime.InteropServices.dll": {}
                        }
                      },
                      "System.Runtime.InteropServices.RuntimeInformation/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Reflection": "4.3.0",
                          "System.Reflection.Extensions": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.InteropServices": "4.3.0",
                          "System.Threading": "4.3.0",
                          "runtime.native.System": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.1/System.Runtime.InteropServices.RuntimeInformation.dll": {}
                        },
                        "runtime": {
                          "lib/netstandard1.1/System.Runtime.InteropServices.RuntimeInformation.dll": {}
                        },
                        "runtimeTargets": {
                          "runtimes/unix/lib/netstandard1.1/System.Runtime.InteropServices.RuntimeInformation.dll": {
                            "assetType": "runtime",
                            "rid": "unix"
                          },
                          "runtimes/win/lib/netstandard1.1/System.Runtime.InteropServices.RuntimeInformation.dll": {
                            "assetType": "runtime",
                            "rid": "win"
                          }
                        }
                      },
                      "System.Runtime.Numerics/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Globalization": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Extensions": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.1/System.Runtime.Numerics.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.3/System.Runtime.Numerics.dll": {}
                        }
                      },
                      "System.Security.Cryptography.Algorithms/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "System.Collections": "4.3.0",
                          "System.IO": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Extensions": "4.3.0",
                          "System.Runtime.Handles": "4.3.0",
                          "System.Runtime.InteropServices": "4.3.0",
                          "System.Runtime.Numerics": "4.3.0",
                          "System.Security.Cryptography.Encoding": "4.3.0",
                          "System.Security.Cryptography.Primitives": "4.3.0",
                          "System.Text.Encoding": "4.3.0",
                          "runtime.native.System.Security.Cryptography.Apple": "4.3.0",
                          "runtime.native.System.Security.Cryptography.OpenSsl": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.6/System.Security.Cryptography.Algorithms.dll": {}
                        },
                        "runtimeTargets": {
                          "runtimes/osx/lib/netstandard1.6/System.Security.Cryptography.Algorithms.dll": {
                            "assetType": "runtime",
                            "rid": "osx"
                          },
                          "runtimes/unix/lib/netstandard1.6/System.Security.Cryptography.Algorithms.dll": {
                            "assetType": "runtime",
                            "rid": "unix"
                          },
                          "runtimes/win/lib/netstandard1.6/System.Security.Cryptography.Algorithms.dll": {
                            "assetType": "runtime",
                            "rid": "win"
                          }
                        }
                      },
                      "System.Security.Cryptography.Cng/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "System.IO": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Extensions": "4.3.0",
                          "System.Runtime.Handles": "4.3.0",
                          "System.Runtime.InteropServices": "4.3.0",
                          "System.Security.Cryptography.Algorithms": "4.3.0",
                          "System.Security.Cryptography.Encoding": "4.3.0",
                          "System.Security.Cryptography.Primitives": "4.3.0",
                          "System.Text.Encoding": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.6/_._": {}
                        },
                        "runtimeTargets": {
                          "runtimes/unix/lib/netstandard1.6/System.Security.Cryptography.Cng.dll": {
                            "assetType": "runtime",
                            "rid": "unix"
                          },
                          "runtimes/win/lib/netstandard1.6/System.Security.Cryptography.Cng.dll": {
                            "assetType": "runtime",
                            "rid": "win"
                          }
                        }
                      },
                      "System.Security.Cryptography.Csp/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "System.IO": "4.3.0",
                          "System.Reflection": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Extensions": "4.3.0",
                          "System.Runtime.Handles": "4.3.0",
                          "System.Runtime.InteropServices": "4.3.0",
                          "System.Security.Cryptography.Algorithms": "4.3.0",
                          "System.Security.Cryptography.Encoding": "4.3.0",
                          "System.Security.Cryptography.Primitives": "4.3.0",
                          "System.Text.Encoding": "4.3.0",
                          "System.Threading": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/_._": {}
                        },
                        "runtimeTargets": {
                          "runtimes/unix/lib/netstandard1.3/System.Security.Cryptography.Csp.dll": {
                            "assetType": "runtime",
                            "rid": "unix"
                          },
                          "runtimes/win/lib/netstandard1.3/System.Security.Cryptography.Csp.dll": {
                            "assetType": "runtime",
                            "rid": "win"
                          }
                        }
                      },
                      "System.Security.Cryptography.Encoding/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "System.Collections": "4.3.0",
                          "System.Collections.Concurrent": "4.3.0",
                          "System.Linq": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Extensions": "4.3.0",
                          "System.Runtime.Handles": "4.3.0",
                          "System.Runtime.InteropServices": "4.3.0",
                          "System.Security.Cryptography.Primitives": "4.3.0",
                          "System.Text.Encoding": "4.3.0",
                          "runtime.native.System.Security.Cryptography.OpenSsl": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.Security.Cryptography.Encoding.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtimeTargets": {
                          "runtimes/unix/lib/netstandard1.3/System.Security.Cryptography.Encoding.dll": {
                            "assetType": "runtime",
                            "rid": "unix"
                          },
                          "runtimes/win/lib/netstandard1.3/System.Security.Cryptography.Encoding.dll": {
                            "assetType": "runtime",
                            "rid": "win"
                          }
                        }
                      },
                      "System.Security.Cryptography.OpenSsl/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Collections": "4.3.0",
                          "System.IO": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Extensions": "4.3.0",
                          "System.Runtime.Handles": "4.3.0",
                          "System.Runtime.InteropServices": "4.3.0",
                          "System.Runtime.Numerics": "4.3.0",
                          "System.Security.Cryptography.Algorithms": "4.3.0",
                          "System.Security.Cryptography.Encoding": "4.3.0",
                          "System.Security.Cryptography.Primitives": "4.3.0",
                          "System.Text.Encoding": "4.3.0",
                          "runtime.native.System.Security.Cryptography.OpenSsl": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.6/_._": {}
                        },
                        "runtime": {
                          "lib/netstandard1.6/System.Security.Cryptography.OpenSsl.dll": {}
                        },
                        "runtimeTargets": {
                          "runtimes/unix/lib/netstandard1.6/System.Security.Cryptography.OpenSsl.dll": {
                            "assetType": "runtime",
                            "rid": "unix"
                          }
                        }
                      },
                      "System.Security.Cryptography.Primitives/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Diagnostics.Debug": "4.3.0",
                          "System.Globalization": "4.3.0",
                          "System.IO": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Threading": "4.3.0",
                          "System.Threading.Tasks": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.Security.Cryptography.Primitives.dll": {}
                        },
                        "runtime": {
                          "lib/netstandard1.3/System.Security.Cryptography.Primitives.dll": {}
                        }
                      },
                      "System.Security.Cryptography.X509Certificates/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "System.Collections": "4.3.0",
                          "System.Diagnostics.Debug": "4.3.0",
                          "System.Globalization": "4.3.0",
                          "System.Globalization.Calendars": "4.3.0",
                          "System.IO": "4.3.0",
                          "System.IO.FileSystem": "4.3.0",
                          "System.IO.FileSystem.Primitives": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Extensions": "4.3.0",
                          "System.Runtime.Handles": "4.3.0",
                          "System.Runtime.InteropServices": "4.3.0",
                          "System.Runtime.Numerics": "4.3.0",
                          "System.Security.Cryptography.Algorithms": "4.3.0",
                          "System.Security.Cryptography.Cng": "4.3.0",
                          "System.Security.Cryptography.Csp": "4.3.0",
                          "System.Security.Cryptography.Encoding": "4.3.0",
                          "System.Security.Cryptography.OpenSsl": "4.3.0",
                          "System.Security.Cryptography.Primitives": "4.3.0",
                          "System.Text.Encoding": "4.3.0",
                          "System.Threading": "4.3.0",
                          "runtime.native.System": "4.3.0",
                          "runtime.native.System.Net.Http": "4.3.0",
                          "runtime.native.System.Security.Cryptography.OpenSsl": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.4/System.Security.Cryptography.X509Certificates.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtimeTargets": {
                          "runtimes/unix/lib/netstandard1.6/System.Security.Cryptography.X509Certificates.dll": {
                            "assetType": "runtime",
                            "rid": "unix"
                          },
                          "runtimes/win/lib/netstandard1.6/System.Security.Cryptography.X509Certificates.dll": {
                            "assetType": "runtime",
                            "rid": "win"
                          }
                        }
                      },
                      "System.Text.Encoding/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.Text.Encoding.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Text.Encoding.Extensions/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.Runtime": "4.3.0",
                          "System.Text.Encoding": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.Text.Encoding.Extensions.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Text.RegularExpressions/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netcoreapp1.1/System.Text.RegularExpressions.dll": {}
                        },
                        "runtime": {
                          "lib/netstandard1.6/System.Text.RegularExpressions.dll": {}
                        }
                      },
                      "System.Threading/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Runtime": "4.3.0",
                          "System.Threading.Tasks": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.Threading.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.3/System.Threading.dll": {}
                        }
                      },
                      "System.Threading.Tasks/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.Threading.Tasks.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Threading.Tasks.Extensions/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Collections": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Threading.Tasks": "4.3.0"
                        },
                        "compile": {
                          "lib/netstandard1.0/_._": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.0/System.Threading.Tasks.Extensions.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Threading.Timer/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "Microsoft.NETCore.Platforms": "1.1.0",
                          "Microsoft.NETCore.Targets": "1.1.0",
                          "System.Runtime": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.2/System.Threading.Timer.dll": {
                            "related": ".xml"
                          }
                        }
                      },
                      "System.Xml.ReaderWriter/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Collections": "4.3.0",
                          "System.Diagnostics.Debug": "4.3.0",
                          "System.Globalization": "4.3.0",
                          "System.IO": "4.3.0",
                          "System.IO.FileSystem": "4.3.0",
                          "System.IO.FileSystem.Primitives": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Extensions": "4.3.0",
                          "System.Runtime.InteropServices": "4.3.0",
                          "System.Text.Encoding": "4.3.0",
                          "System.Text.Encoding.Extensions": "4.3.0",
                          "System.Text.RegularExpressions": "4.3.0",
                          "System.Threading.Tasks": "4.3.0",
                          "System.Threading.Tasks.Extensions": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.Xml.ReaderWriter.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.3/System.Xml.ReaderWriter.dll": {}
                        }
                      },
                      "System.Xml.XDocument/4.3.0": {
                        "type": "package",
                        "dependencies": {
                          "System.Collections": "4.3.0",
                          "System.Diagnostics.Debug": "4.3.0",
                          "System.Diagnostics.Tools": "4.3.0",
                          "System.Globalization": "4.3.0",
                          "System.IO": "4.3.0",
                          "System.Reflection": "4.3.0",
                          "System.Resources.ResourceManager": "4.3.0",
                          "System.Runtime": "4.3.0",
                          "System.Runtime.Extensions": "4.3.0",
                          "System.Text.Encoding": "4.3.0",
                          "System.Threading": "4.3.0",
                          "System.Xml.ReaderWriter": "4.3.0"
                        },
                        "compile": {
                          "ref/netstandard1.3/System.Xml.XDocument.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/netstandard1.3/System.Xml.XDocument.dll": {}
                        }
                      },
                      "TimeZoneConverter/6.1.0": {
                        "type": "package",
                        "compile": {
                          "lib/net6.0/TimeZoneConverter.dll": {
                            "related": ".xml"
                          }
                        },
                        "runtime": {
                          "lib/net6.0/TimeZoneConverter.dll": {
                            "related": ".xml"
                          }
                        }
                      }
                    }
                  },
                  "libraries": {
                    "Fluid.Core/2.7.0": {
                      "sha512": "O18vhUy5HrWI0fiV6gf6slV8Ffbb91+2sLRqEg6lrpEWLD3xqb1loxfI9OQJGUPoPfSVnWZJgda116HADIsZ4g==",
                      "type": "package",
                      "path": "fluid.core/2.7.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "README.md",
                        "fluid.core.2.7.0.nupkg.sha512",
                        "fluid.core.nuspec",
                        "lib/net6.0/Fluid.dll",
                        "lib/net6.0/Fluid.pdb",
                        "lib/net6.0/Fluid.xml",
                        "lib/net7.0/Fluid.dll",
                        "lib/net7.0/Fluid.pdb",
                        "lib/net7.0/Fluid.xml",
                        "lib/net8.0/Fluid.dll",
                        "lib/net8.0/Fluid.pdb",
                        "lib/net8.0/Fluid.xml",
                        "lib/netstandard2.0/Fluid.dll",
                        "lib/netstandard2.0/Fluid.pdb",
                        "lib/netstandard2.0/Fluid.xml",
                        "lib/netstandard2.1/Fluid.dll",
                        "lib/netstandard2.1/Fluid.pdb",
                        "lib/netstandard2.1/Fluid.xml",
                        "logo_64x64.png"
                      ]
                    },
                    "Microsoft.Extensions.FileProviders.Abstractions/1.1.1": {
                      "sha512": "Z/XGMFZ8IuTcd8j5D/77jnajN5ZEs/NCH+LZkMUKKW2d9rpg10VtPhDsvMwKNhI+kcdTE00G/obXUF7Ruuwqlg==",
                      "type": "package",
                      "path": "microsoft.extensions.fileproviders.abstractions/1.1.1",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "lib/netstandard1.0/Microsoft.Extensions.FileProviders.Abstractions.dll",
                        "lib/netstandard1.0/Microsoft.Extensions.FileProviders.Abstractions.xml",
                        "microsoft.extensions.fileproviders.abstractions.1.1.1.nupkg.sha512",
                        "microsoft.extensions.fileproviders.abstractions.nuspec"
                      ]
                    },
                    "Microsoft.Extensions.Primitives/1.1.1": {
                      "sha512": "MrsHOyFpwT+LBzGWp/Oq3pV1Vku8FYE6hgO+2XR0WBRtoI9EaJcpRbtBabS7pXYrkIN1/LOXACpZ9Stqmbrs6A==",
                      "type": "package",
                      "path": "microsoft.extensions.primitives/1.1.1",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "lib/netstandard1.0/Microsoft.Extensions.Primitives.dll",
                        "lib/netstandard1.0/Microsoft.Extensions.Primitives.xml",
                        "microsoft.extensions.primitives.1.1.1.nupkg.sha512",
                        "microsoft.extensions.primitives.nuspec"
                      ]
                    },
                    "Microsoft.NETCore.Platforms/1.1.0": {
                      "sha512": "kz0PEW2lhqygehI/d6XsPCQzD7ff7gUJaVGPVETX611eadGsA3A877GdSlU0LRVMCTH/+P3o2iDTak+S08V2+A==",
                      "type": "package",
                      "path": "microsoft.netcore.platforms/1.1.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/netstandard1.0/_._",
                        "microsoft.netcore.platforms.1.1.0.nupkg.sha512",
                        "microsoft.netcore.platforms.nuspec",
                        "runtime.json"
                      ]
                    },
                    "Microsoft.NETCore.Targets/1.1.0": {
                      "sha512": "aOZA3BWfz9RXjpzt0sRJJMjAscAUm3Hoa4UWAfceV9UTYxgwZ1lZt5nO2myFf+/jetYQo4uTP7zS8sJY67BBxg==",
                      "type": "package",
                      "path": "microsoft.netcore.targets/1.1.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/netstandard1.0/_._",
                        "microsoft.netcore.targets.1.1.0.nupkg.sha512",
                        "microsoft.netcore.targets.nuspec",
                        "runtime.json"
                      ]
                    },
                    "Microsoft.Win32.Primitives/4.3.0": {
                      "sha512": "9ZQKCWxH7Ijp9BfahvL2Zyf1cJIk8XYLF6Yjzr2yi0b2cOut/HQ31qf1ThHAgCc3WiZMdnWcfJCgN82/0UunxA==",
                      "type": "package",
                      "path": "microsoft.win32.primitives/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net46/Microsoft.Win32.Primitives.dll",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "microsoft.win32.primitives.4.3.0.nupkg.sha512",
                        "microsoft.win32.primitives.nuspec",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net46/Microsoft.Win32.Primitives.dll",
                        "ref/netstandard1.3/Microsoft.Win32.Primitives.dll",
                        "ref/netstandard1.3/Microsoft.Win32.Primitives.xml",
                        "ref/netstandard1.3/de/Microsoft.Win32.Primitives.xml",
                        "ref/netstandard1.3/es/Microsoft.Win32.Primitives.xml",
                        "ref/netstandard1.3/fr/Microsoft.Win32.Primitives.xml",
                        "ref/netstandard1.3/it/Microsoft.Win32.Primitives.xml",
                        "ref/netstandard1.3/ja/Microsoft.Win32.Primitives.xml",
                        "ref/netstandard1.3/ko/Microsoft.Win32.Primitives.xml",
                        "ref/netstandard1.3/ru/Microsoft.Win32.Primitives.xml",
                        "ref/netstandard1.3/zh-hans/Microsoft.Win32.Primitives.xml",
                        "ref/netstandard1.3/zh-hant/Microsoft.Win32.Primitives.xml",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._"
                      ]
                    },
                    "NETStandard.Library/1.6.1": {
                      "sha512": "WcSp3+vP+yHNgS8EV5J7pZ9IRpeDuARBPN28by8zqff1wJQXm26PVU8L3/fYLBJVU7BtDyqNVWq2KlCVvSSR4A==",
                      "type": "package",
                      "path": "netstandard.library/1.6.1",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "netstandard.library.1.6.1.nupkg.sha512",
                        "netstandard.library.nuspec"
                      ]
                    },
                    "Parlot/0.0.24": {
                      "sha512": "jdhTa3NK1Vy8CdiWitfIPfQfmg9KvEdrVSYU6wquPQxF4FfGGtuTZyeduUDWDwU51ql6X9o/mDpHqHQQUy5k6w==",
                      "type": "package",
                      "path": "parlot/0.0.24",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "lib/netstandard2.0/Parlot.dll",
                        "lib/netstandard2.0/Parlot.xml",
                        "lib/netstandard2.1/Parlot.dll",
                        "lib/netstandard2.1/Parlot.xml",
                        "parlot.0.0.24.nupkg.sha512",
                        "parlot.nuspec"
                      ]
                    },
                    "runtime.debian.8-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                      "sha512": "HdSSp5MnJSsg08KMfZThpuLPJpPwE5hBXvHwoKWosyHHfe8Mh5WKT0ylEOf6yNzX6Ngjxe4Whkafh5q7Ymac4Q==",
                      "type": "package",
                      "path": "runtime.debian.8-x64.runtime.native.system.security.cryptography.openssl/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "runtime.debian.8-x64.runtime.native.system.security.cryptography.openssl.4.3.0.nupkg.sha512",
                        "runtime.debian.8-x64.runtime.native.system.security.cryptography.openssl.nuspec",
                        "runtimes/debian.8-x64/native/System.Security.Cryptography.Native.OpenSsl.so"
                      ]
                    },
                    "runtime.fedora.23-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                      "sha512": "+yH1a49wJMy8Zt4yx5RhJrxO/DBDByAiCzNwiETI+1S4mPdCu0OY4djdciC7Vssk0l22wQaDLrXxXkp+3+7bVA==",
                      "type": "package",
                      "path": "runtime.fedora.23-x64.runtime.native.system.security.cryptography.openssl/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "runtime.fedora.23-x64.runtime.native.system.security.cryptography.openssl.4.3.0.nupkg.sha512",
                        "runtime.fedora.23-x64.runtime.native.system.security.cryptography.openssl.nuspec",
                        "runtimes/fedora.23-x64/native/System.Security.Cryptography.Native.OpenSsl.so"
                      ]
                    },
                    "runtime.fedora.24-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                      "sha512": "c3YNH1GQJbfIPJeCnr4avseugSqPrxwIqzthYyZDN6EuOyNOzq+y2KSUfRcXauya1sF4foESTgwM5e1A8arAKw==",
                      "type": "package",
                      "path": "runtime.fedora.24-x64.runtime.native.system.security.cryptography.openssl/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "runtime.fedora.24-x64.runtime.native.system.security.cryptography.openssl.4.3.0.nupkg.sha512",
                        "runtime.fedora.24-x64.runtime.native.system.security.cryptography.openssl.nuspec",
                        "runtimes/fedora.24-x64/native/System.Security.Cryptography.Native.OpenSsl.so"
                      ]
                    },
                    "runtime.native.System/4.3.0": {
                      "sha512": "c/qWt2LieNZIj1jGnVNsE2Kl23Ya2aSTBuXMD6V7k9KWr6l16Tqdwq+hJScEpWER9753NWC8h96PaVNY5Ld7Jw==",
                      "type": "package",
                      "path": "runtime.native.system/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/netstandard1.0/_._",
                        "runtime.native.system.4.3.0.nupkg.sha512",
                        "runtime.native.system.nuspec"
                      ]
                    },
                    "runtime.native.System.IO.Compression/4.3.0": {
                      "sha512": "INBPonS5QPEgn7naufQFXJEp3zX6L4bwHgJ/ZH78aBTpeNfQMtf7C6VrAFhlq2xxWBveIOWyFzQjJ8XzHMhdOQ==",
                      "type": "package",
                      "path": "runtime.native.system.io.compression/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/netstandard1.0/_._",
                        "runtime.native.system.io.compression.4.3.0.nupkg.sha512",
                        "runtime.native.system.io.compression.nuspec"
                      ]
                    },
                    "runtime.native.System.Net.Http/4.3.0": {
                      "sha512": "ZVuZJqnnegJhd2k/PtAbbIcZ3aZeITq3sj06oKfMBSfphW3HDmk/t4ObvbOk/JA/swGR0LNqMksAh/f7gpTROg==",
                      "type": "package",
                      "path": "runtime.native.system.net.http/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/netstandard1.0/_._",
                        "runtime.native.system.net.http.4.3.0.nupkg.sha512",
                        "runtime.native.system.net.http.nuspec"
                      ]
                    },
                    "runtime.native.System.Security.Cryptography.Apple/4.3.0": {
                      "sha512": "DloMk88juo0OuOWr56QG7MNchmafTLYWvABy36izkrLI5VledI0rq28KGs1i9wbpeT9NPQrx/wTf8U2vazqQ3Q==",
                      "type": "package",
                      "path": "runtime.native.system.security.cryptography.apple/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/netstandard1.0/_._",
                        "runtime.native.system.security.cryptography.apple.4.3.0.nupkg.sha512",
                        "runtime.native.system.security.cryptography.apple.nuspec"
                      ]
                    },
                    "runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                      "sha512": "NS1U+700m4KFRHR5o4vo9DSlTmlCKu/u7dtE5sUHVIPB+xpXxYQvgBgA6wEIeCz6Yfn0Z52/72WYsToCEPJnrw==",
                      "type": "package",
                      "path": "runtime.native.system.security.cryptography.openssl/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/netstandard1.0/_._",
                        "runtime.native.system.security.cryptography.openssl.4.3.0.nupkg.sha512",
                        "runtime.native.system.security.cryptography.openssl.nuspec"
                      ]
                    },
                    "runtime.opensuse.13.2-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                      "sha512": "b3pthNgxxFcD+Pc0WSEoC0+md3MyhRS6aCEeenvNE3Fdw1HyJ18ZhRFVJJzIeR/O/jpxPboB805Ho0T3Ul7w8A==",
                      "type": "package",
                      "path": "runtime.opensuse.13.2-x64.runtime.native.system.security.cryptography.openssl/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "runtime.opensuse.13.2-x64.runtime.native.system.security.cryptography.openssl.4.3.0.nupkg.sha512",
                        "runtime.opensuse.13.2-x64.runtime.native.system.security.cryptography.openssl.nuspec",
                        "runtimes/opensuse.13.2-x64/native/System.Security.Cryptography.Native.OpenSsl.so"
                      ]
                    },
                    "runtime.opensuse.42.1-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                      "sha512": "KeLz4HClKf+nFS7p/6Fi/CqyLXh81FpiGzcmuS8DGi9lUqSnZ6Es23/gv2O+1XVGfrbNmviF7CckBpavkBoIFQ==",
                      "type": "package",
                      "path": "runtime.opensuse.42.1-x64.runtime.native.system.security.cryptography.openssl/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "runtime.opensuse.42.1-x64.runtime.native.system.security.cryptography.openssl.4.3.0.nupkg.sha512",
                        "runtime.opensuse.42.1-x64.runtime.native.system.security.cryptography.openssl.nuspec",
                        "runtimes/opensuse.42.1-x64/native/System.Security.Cryptography.Native.OpenSsl.so"
                      ]
                    },
                    "runtime.osx.10.10-x64.runtime.native.System.Security.Cryptography.Apple/4.3.0": {
                      "sha512": "kVXCuMTrTlxq4XOOMAysuNwsXWpYeboGddNGpIgNSZmv1b6r/s/DPk0fYMB7Q5Qo4bY68o48jt4T4y5BVecbCQ==",
                      "type": "package",
                      "path": "runtime.osx.10.10-x64.runtime.native.system.security.cryptography.apple/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "runtime.osx.10.10-x64.runtime.native.system.security.cryptography.apple.4.3.0.nupkg.sha512",
                        "runtime.osx.10.10-x64.runtime.native.system.security.cryptography.apple.nuspec",
                        "runtimes/osx.10.10-x64/native/System.Security.Cryptography.Native.Apple.dylib"
                      ]
                    },
                    "runtime.osx.10.10-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                      "sha512": "X7IdhILzr4ROXd8mI1BUCQMSHSQwelUlBjF1JyTKCjXaOGn2fB4EKBxQbCK2VjO3WaWIdlXZL3W6TiIVnrhX4g==",
                      "type": "package",
                      "path": "runtime.osx.10.10-x64.runtime.native.system.security.cryptography.openssl/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "runtime.osx.10.10-x64.runtime.native.system.security.cryptography.openssl.4.3.0.nupkg.sha512",
                        "runtime.osx.10.10-x64.runtime.native.system.security.cryptography.openssl.nuspec",
                        "runtimes/osx.10.10-x64/native/System.Security.Cryptography.Native.OpenSsl.dylib"
                      ]
                    },
                    "runtime.rhel.7-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                      "sha512": "nyFNiCk/r+VOiIqreLix8yN+q3Wga9+SE8BCgkf+2BwEKiNx6DyvFjCgkfV743/grxv8jHJ8gUK4XEQw7yzRYg==",
                      "type": "package",
                      "path": "runtime.rhel.7-x64.runtime.native.system.security.cryptography.openssl/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "runtime.rhel.7-x64.runtime.native.system.security.cryptography.openssl.4.3.0.nupkg.sha512",
                        "runtime.rhel.7-x64.runtime.native.system.security.cryptography.openssl.nuspec",
                        "runtimes/rhel.7-x64/native/System.Security.Cryptography.Native.OpenSsl.so"
                      ]
                    },
                    "runtime.ubuntu.14.04-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                      "sha512": "ytoewC6wGorL7KoCAvRfsgoJPJbNq+64k2SqW6JcOAebWsFUvCCYgfzQMrnpvPiEl4OrblUlhF2ji+Q1+SVLrQ==",
                      "type": "package",
                      "path": "runtime.ubuntu.14.04-x64.runtime.native.system.security.cryptography.openssl/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "runtime.ubuntu.14.04-x64.runtime.native.system.security.cryptography.openssl.4.3.0.nupkg.sha512",
                        "runtime.ubuntu.14.04-x64.runtime.native.system.security.cryptography.openssl.nuspec",
                        "runtimes/ubuntu.14.04-x64/native/System.Security.Cryptography.Native.OpenSsl.so"
                      ]
                    },
                    "runtime.ubuntu.16.04-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                      "sha512": "I8bKw2I8k58Wx7fMKQJn2R8lamboCAiHfHeV/pS65ScKWMMI0+wJkLYlEKvgW1D/XvSl/221clBoR2q9QNNM7A==",
                      "type": "package",
                      "path": "runtime.ubuntu.16.04-x64.runtime.native.system.security.cryptography.openssl/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "runtime.ubuntu.16.04-x64.runtime.native.system.security.cryptography.openssl.4.3.0.nupkg.sha512",
                        "runtime.ubuntu.16.04-x64.runtime.native.system.security.cryptography.openssl.nuspec",
                        "runtimes/ubuntu.16.04-x64/native/System.Security.Cryptography.Native.OpenSsl.so"
                      ]
                    },
                    "runtime.ubuntu.16.10-x64.runtime.native.System.Security.Cryptography.OpenSsl/4.3.0": {
                      "sha512": "VB5cn/7OzUfzdnC8tqAIMQciVLiq2epm2NrAm1E9OjNRyG4lVhfR61SMcLizejzQP8R8Uf/0l5qOIbUEi+RdEg==",
                      "type": "package",
                      "path": "runtime.ubuntu.16.10-x64.runtime.native.system.security.cryptography.openssl/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "runtime.ubuntu.16.10-x64.runtime.native.system.security.cryptography.openssl.4.3.0.nupkg.sha512",
                        "runtime.ubuntu.16.10-x64.runtime.native.system.security.cryptography.openssl.nuspec",
                        "runtimes/ubuntu.16.10-x64/native/System.Security.Cryptography.Native.OpenSsl.so"
                      ]
                    },
                    "System.AppContext/4.3.0": {
                      "sha512": "fKC+rmaLfeIzUhagxY17Q9siv/sPrjjKcfNg1Ic8IlQkZLipo8ljcaZQu4VtI4Jqbzjc2VTjzGLF6WmsRXAEgA==",
                      "type": "package",
                      "path": "system.appcontext/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net46/System.AppContext.dll",
                        "lib/net463/System.AppContext.dll",
                        "lib/netcore50/System.AppContext.dll",
                        "lib/netstandard1.6/System.AppContext.dll",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net46/System.AppContext.dll",
                        "ref/net463/System.AppContext.dll",
                        "ref/netstandard/_._",
                        "ref/netstandard1.3/System.AppContext.dll",
                        "ref/netstandard1.3/System.AppContext.xml",
                        "ref/netstandard1.3/de/System.AppContext.xml",
                        "ref/netstandard1.3/es/System.AppContext.xml",
                        "ref/netstandard1.3/fr/System.AppContext.xml",
                        "ref/netstandard1.3/it/System.AppContext.xml",
                        "ref/netstandard1.3/ja/System.AppContext.xml",
                        "ref/netstandard1.3/ko/System.AppContext.xml",
                        "ref/netstandard1.3/ru/System.AppContext.xml",
                        "ref/netstandard1.3/zh-hans/System.AppContext.xml",
                        "ref/netstandard1.3/zh-hant/System.AppContext.xml",
                        "ref/netstandard1.6/System.AppContext.dll",
                        "ref/netstandard1.6/System.AppContext.xml",
                        "ref/netstandard1.6/de/System.AppContext.xml",
                        "ref/netstandard1.6/es/System.AppContext.xml",
                        "ref/netstandard1.6/fr/System.AppContext.xml",
                        "ref/netstandard1.6/it/System.AppContext.xml",
                        "ref/netstandard1.6/ja/System.AppContext.xml",
                        "ref/netstandard1.6/ko/System.AppContext.xml",
                        "ref/netstandard1.6/ru/System.AppContext.xml",
                        "ref/netstandard1.6/zh-hans/System.AppContext.xml",
                        "ref/netstandard1.6/zh-hant/System.AppContext.xml",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "runtimes/aot/lib/netcore50/System.AppContext.dll",
                        "system.appcontext.4.3.0.nupkg.sha512",
                        "system.appcontext.nuspec"
                      ]
                    },
                    "System.Buffers/4.3.0": {
                      "sha512": "ratu44uTIHgeBeI0dE8DWvmXVBSo4u7ozRZZHOMmK/JPpYyo0dAfgSiHlpiObMQ5lEtEyIXA40sKRYg5J6A8uQ==",
                      "type": "package",
                      "path": "system.buffers/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/netstandard1.1/.xml",
                        "lib/netstandard1.1/System.Buffers.dll",
                        "system.buffers.4.3.0.nupkg.sha512",
                        "system.buffers.nuspec"
                      ]
                    },
                    "System.Collections/4.3.0": {
                      "sha512": "3Dcj85/TBdVpL5Zr+gEEBUuFe2icOnLalmEh9hfck1PTYbbyWuZgh4fmm2ysCLTrqLQw6t3TgTyJ+VLp+Qb+Lw==",
                      "type": "package",
                      "path": "system.collections/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/netcore50/System.Collections.dll",
                        "ref/netcore50/System.Collections.xml",
                        "ref/netcore50/de/System.Collections.xml",
                        "ref/netcore50/es/System.Collections.xml",
                        "ref/netcore50/fr/System.Collections.xml",
                        "ref/netcore50/it/System.Collections.xml",
                        "ref/netcore50/ja/System.Collections.xml",
                        "ref/netcore50/ko/System.Collections.xml",
                        "ref/netcore50/ru/System.Collections.xml",
                        "ref/netcore50/zh-hans/System.Collections.xml",
                        "ref/netcore50/zh-hant/System.Collections.xml",
                        "ref/netstandard1.0/System.Collections.dll",
                        "ref/netstandard1.0/System.Collections.xml",
                        "ref/netstandard1.0/de/System.Collections.xml",
                        "ref/netstandard1.0/es/System.Collections.xml",
                        "ref/netstandard1.0/fr/System.Collections.xml",
                        "ref/netstandard1.0/it/System.Collections.xml",
                        "ref/netstandard1.0/ja/System.Collections.xml",
                        "ref/netstandard1.0/ko/System.Collections.xml",
                        "ref/netstandard1.0/ru/System.Collections.xml",
                        "ref/netstandard1.0/zh-hans/System.Collections.xml",
                        "ref/netstandard1.0/zh-hant/System.Collections.xml",
                        "ref/netstandard1.3/System.Collections.dll",
                        "ref/netstandard1.3/System.Collections.xml",
                        "ref/netstandard1.3/de/System.Collections.xml",
                        "ref/netstandard1.3/es/System.Collections.xml",
                        "ref/netstandard1.3/fr/System.Collections.xml",
                        "ref/netstandard1.3/it/System.Collections.xml",
                        "ref/netstandard1.3/ja/System.Collections.xml",
                        "ref/netstandard1.3/ko/System.Collections.xml",
                        "ref/netstandard1.3/ru/System.Collections.xml",
                        "ref/netstandard1.3/zh-hans/System.Collections.xml",
                        "ref/netstandard1.3/zh-hant/System.Collections.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.collections.4.3.0.nupkg.sha512",
                        "system.collections.nuspec"
                      ]
                    },
                    "System.Collections.Concurrent/4.3.0": {
                      "sha512": "ztl69Xp0Y/UXCL+3v3tEU+lIy+bvjKNUmopn1wep/a291pVPK7dxBd6T7WnlQqRog+d1a/hSsgRsmFnIBKTPLQ==",
                      "type": "package",
                      "path": "system.collections.concurrent/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/netcore50/System.Collections.Concurrent.dll",
                        "lib/netstandard1.3/System.Collections.Concurrent.dll",
                        "lib/portable-net45+win8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/netcore50/System.Collections.Concurrent.dll",
                        "ref/netcore50/System.Collections.Concurrent.xml",
                        "ref/netcore50/de/System.Collections.Concurrent.xml",
                        "ref/netcore50/es/System.Collections.Concurrent.xml",
                        "ref/netcore50/fr/System.Collections.Concurrent.xml",
                        "ref/netcore50/it/System.Collections.Concurrent.xml",
                        "ref/netcore50/ja/System.Collections.Concurrent.xml",
                        "ref/netcore50/ko/System.Collections.Concurrent.xml",
                        "ref/netcore50/ru/System.Collections.Concurrent.xml",
                        "ref/netcore50/zh-hans/System.Collections.Concurrent.xml",
                        "ref/netcore50/zh-hant/System.Collections.Concurrent.xml",
                        "ref/netstandard1.1/System.Collections.Concurrent.dll",
                        "ref/netstandard1.1/System.Collections.Concurrent.xml",
                        "ref/netstandard1.1/de/System.Collections.Concurrent.xml",
                        "ref/netstandard1.1/es/System.Collections.Concurrent.xml",
                        "ref/netstandard1.1/fr/System.Collections.Concurrent.xml",
                        "ref/netstandard1.1/it/System.Collections.Concurrent.xml",
                        "ref/netstandard1.1/ja/System.Collections.Concurrent.xml",
                        "ref/netstandard1.1/ko/System.Collections.Concurrent.xml",
                        "ref/netstandard1.1/ru/System.Collections.Concurrent.xml",
                        "ref/netstandard1.1/zh-hans/System.Collections.Concurrent.xml",
                        "ref/netstandard1.1/zh-hant/System.Collections.Concurrent.xml",
                        "ref/netstandard1.3/System.Collections.Concurrent.dll",
                        "ref/netstandard1.3/System.Collections.Concurrent.xml",
                        "ref/netstandard1.3/de/System.Collections.Concurrent.xml",
                        "ref/netstandard1.3/es/System.Collections.Concurrent.xml",
                        "ref/netstandard1.3/fr/System.Collections.Concurrent.xml",
                        "ref/netstandard1.3/it/System.Collections.Concurrent.xml",
                        "ref/netstandard1.3/ja/System.Collections.Concurrent.xml",
                        "ref/netstandard1.3/ko/System.Collections.Concurrent.xml",
                        "ref/netstandard1.3/ru/System.Collections.Concurrent.xml",
                        "ref/netstandard1.3/zh-hans/System.Collections.Concurrent.xml",
                        "ref/netstandard1.3/zh-hant/System.Collections.Concurrent.xml",
                        "ref/portable-net45+win8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.collections.concurrent.4.3.0.nupkg.sha512",
                        "system.collections.concurrent.nuspec"
                      ]
                    },
                    "System.Console/4.3.0": {
                      "sha512": "DHDrIxiqk1h03m6khKWV2X8p/uvN79rgSqpilL6uzpmSfxfU5ng8VcPtW4qsDsQDHiTv6IPV9TmD5M/vElPNLg==",
                      "type": "package",
                      "path": "system.console/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net46/System.Console.dll",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net46/System.Console.dll",
                        "ref/netstandard1.3/System.Console.dll",
                        "ref/netstandard1.3/System.Console.xml",
                        "ref/netstandard1.3/de/System.Console.xml",
                        "ref/netstandard1.3/es/System.Console.xml",
                        "ref/netstandard1.3/fr/System.Console.xml",
                        "ref/netstandard1.3/it/System.Console.xml",
                        "ref/netstandard1.3/ja/System.Console.xml",
                        "ref/netstandard1.3/ko/System.Console.xml",
                        "ref/netstandard1.3/ru/System.Console.xml",
                        "ref/netstandard1.3/zh-hans/System.Console.xml",
                        "ref/netstandard1.3/zh-hant/System.Console.xml",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.console.4.3.0.nupkg.sha512",
                        "system.console.nuspec"
                      ]
                    },
                    "System.Diagnostics.Debug/4.3.0": {
                      "sha512": "ZUhUOdqmaG5Jk3Xdb8xi5kIyQYAA4PnTNlHx1mu9ZY3qv4ELIdKbnL/akbGaKi2RnNUWaZsAs31rvzFdewTj2g==",
                      "type": "package",
                      "path": "system.diagnostics.debug/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/netcore50/System.Diagnostics.Debug.dll",
                        "ref/netcore50/System.Diagnostics.Debug.xml",
                        "ref/netcore50/de/System.Diagnostics.Debug.xml",
                        "ref/netcore50/es/System.Diagnostics.Debug.xml",
                        "ref/netcore50/fr/System.Diagnostics.Debug.xml",
                        "ref/netcore50/it/System.Diagnostics.Debug.xml",
                        "ref/netcore50/ja/System.Diagnostics.Debug.xml",
                        "ref/netcore50/ko/System.Diagnostics.Debug.xml",
                        "ref/netcore50/ru/System.Diagnostics.Debug.xml",
                        "ref/netcore50/zh-hans/System.Diagnostics.Debug.xml",
                        "ref/netcore50/zh-hant/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.0/System.Diagnostics.Debug.dll",
                        "ref/netstandard1.0/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.0/de/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.0/es/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.0/fr/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.0/it/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.0/ja/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.0/ko/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.0/ru/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.0/zh-hans/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.0/zh-hant/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.3/System.Diagnostics.Debug.dll",
                        "ref/netstandard1.3/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.3/de/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.3/es/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.3/fr/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.3/it/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.3/ja/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.3/ko/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.3/ru/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.3/zh-hans/System.Diagnostics.Debug.xml",
                        "ref/netstandard1.3/zh-hant/System.Diagnostics.Debug.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.diagnostics.debug.4.3.0.nupkg.sha512",
                        "system.diagnostics.debug.nuspec"
                      ]
                    },
                    "System.Diagnostics.DiagnosticSource/4.3.0": {
                      "sha512": "tD6kosZnTAGdrEa0tZSuFyunMbt/5KYDnHdndJYGqZoNy00XVXyACd5d6KnE1YgYv3ne2CjtAfNXo/fwEhnKUA==",
                      "type": "package",
                      "path": "system.diagnostics.diagnosticsource/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/net46/System.Diagnostics.DiagnosticSource.dll",
                        "lib/net46/System.Diagnostics.DiagnosticSource.xml",
                        "lib/netstandard1.1/System.Diagnostics.DiagnosticSource.dll",
                        "lib/netstandard1.1/System.Diagnostics.DiagnosticSource.xml",
                        "lib/netstandard1.3/System.Diagnostics.DiagnosticSource.dll",
                        "lib/netstandard1.3/System.Diagnostics.DiagnosticSource.xml",
                        "lib/portable-net45+win8+wpa81/System.Diagnostics.DiagnosticSource.dll",
                        "lib/portable-net45+win8+wpa81/System.Diagnostics.DiagnosticSource.xml",
                        "system.diagnostics.diagnosticsource.4.3.0.nupkg.sha512",
                        "system.diagnostics.diagnosticsource.nuspec"
                      ]
                    },
                    "System.Diagnostics.Tools/4.3.0": {
                      "sha512": "UUvkJfSYJMM6x527dJg2VyWPSRqIVB0Z7dbjHst1zmwTXz5CcXSYJFWRpuigfbO1Lf7yfZiIaEUesfnl/g5EyA==",
                      "type": "package",
                      "path": "system.diagnostics.tools/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/netcore50/System.Diagnostics.Tools.dll",
                        "ref/netcore50/System.Diagnostics.Tools.xml",
                        "ref/netcore50/de/System.Diagnostics.Tools.xml",
                        "ref/netcore50/es/System.Diagnostics.Tools.xml",
                        "ref/netcore50/fr/System.Diagnostics.Tools.xml",
                        "ref/netcore50/it/System.Diagnostics.Tools.xml",
                        "ref/netcore50/ja/System.Diagnostics.Tools.xml",
                        "ref/netcore50/ko/System.Diagnostics.Tools.xml",
                        "ref/netcore50/ru/System.Diagnostics.Tools.xml",
                        "ref/netcore50/zh-hans/System.Diagnostics.Tools.xml",
                        "ref/netcore50/zh-hant/System.Diagnostics.Tools.xml",
                        "ref/netstandard1.0/System.Diagnostics.Tools.dll",
                        "ref/netstandard1.0/System.Diagnostics.Tools.xml",
                        "ref/netstandard1.0/de/System.Diagnostics.Tools.xml",
                        "ref/netstandard1.0/es/System.Diagnostics.Tools.xml",
                        "ref/netstandard1.0/fr/System.Diagnostics.Tools.xml",
                        "ref/netstandard1.0/it/System.Diagnostics.Tools.xml",
                        "ref/netstandard1.0/ja/System.Diagnostics.Tools.xml",
                        "ref/netstandard1.0/ko/System.Diagnostics.Tools.xml",
                        "ref/netstandard1.0/ru/System.Diagnostics.Tools.xml",
                        "ref/netstandard1.0/zh-hans/System.Diagnostics.Tools.xml",
                        "ref/netstandard1.0/zh-hant/System.Diagnostics.Tools.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.diagnostics.tools.4.3.0.nupkg.sha512",
                        "system.diagnostics.tools.nuspec"
                      ]
                    },
                    "System.Diagnostics.Tracing/4.3.0": {
                      "sha512": "rswfv0f/Cqkh78rA5S8eN8Neocz234+emGCtTF3lxPY96F+mmmUen6tbn0glN6PMvlKQb9bPAY5e9u7fgPTkKw==",
                      "type": "package",
                      "path": "system.diagnostics.tracing/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/net462/System.Diagnostics.Tracing.dll",
                        "lib/portable-net45+win8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/net462/System.Diagnostics.Tracing.dll",
                        "ref/netcore50/System.Diagnostics.Tracing.dll",
                        "ref/netcore50/System.Diagnostics.Tracing.xml",
                        "ref/netcore50/de/System.Diagnostics.Tracing.xml",
                        "ref/netcore50/es/System.Diagnostics.Tracing.xml",
                        "ref/netcore50/fr/System.Diagnostics.Tracing.xml",
                        "ref/netcore50/it/System.Diagnostics.Tracing.xml",
                        "ref/netcore50/ja/System.Diagnostics.Tracing.xml",
                        "ref/netcore50/ko/System.Diagnostics.Tracing.xml",
                        "ref/netcore50/ru/System.Diagnostics.Tracing.xml",
                        "ref/netcore50/zh-hans/System.Diagnostics.Tracing.xml",
                        "ref/netcore50/zh-hant/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.1/System.Diagnostics.Tracing.dll",
                        "ref/netstandard1.1/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.1/de/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.1/es/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.1/fr/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.1/it/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.1/ja/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.1/ko/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.1/ru/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.1/zh-hans/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.1/zh-hant/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.2/System.Diagnostics.Tracing.dll",
                        "ref/netstandard1.2/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.2/de/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.2/es/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.2/fr/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.2/it/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.2/ja/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.2/ko/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.2/ru/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.2/zh-hans/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.2/zh-hant/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.3/System.Diagnostics.Tracing.dll",
                        "ref/netstandard1.3/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.3/de/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.3/es/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.3/fr/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.3/it/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.3/ja/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.3/ko/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.3/ru/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.3/zh-hans/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.3/zh-hant/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.5/System.Diagnostics.Tracing.dll",
                        "ref/netstandard1.5/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.5/de/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.5/es/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.5/fr/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.5/it/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.5/ja/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.5/ko/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.5/ru/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.5/zh-hans/System.Diagnostics.Tracing.xml",
                        "ref/netstandard1.5/zh-hant/System.Diagnostics.Tracing.xml",
                        "ref/portable-net45+win8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.diagnostics.tracing.4.3.0.nupkg.sha512",
                        "system.diagnostics.tracing.nuspec"
                      ]
                    },
                    "System.Globalization/4.3.0": {
                      "sha512": "kYdVd2f2PAdFGblzFswE4hkNANJBKRmsfa2X5LG2AcWE1c7/4t0pYae1L8vfZ5xvE2nK/R9JprtToA61OSHWIg==",
                      "type": "package",
                      "path": "system.globalization/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/netcore50/System.Globalization.dll",
                        "ref/netcore50/System.Globalization.xml",
                        "ref/netcore50/de/System.Globalization.xml",
                        "ref/netcore50/es/System.Globalization.xml",
                        "ref/netcore50/fr/System.Globalization.xml",
                        "ref/netcore50/it/System.Globalization.xml",
                        "ref/netcore50/ja/System.Globalization.xml",
                        "ref/netcore50/ko/System.Globalization.xml",
                        "ref/netcore50/ru/System.Globalization.xml",
                        "ref/netcore50/zh-hans/System.Globalization.xml",
                        "ref/netcore50/zh-hant/System.Globalization.xml",
                        "ref/netstandard1.0/System.Globalization.dll",
                        "ref/netstandard1.0/System.Globalization.xml",
                        "ref/netstandard1.0/de/System.Globalization.xml",
                        "ref/netstandard1.0/es/System.Globalization.xml",
                        "ref/netstandard1.0/fr/System.Globalization.xml",
                        "ref/netstandard1.0/it/System.Globalization.xml",
                        "ref/netstandard1.0/ja/System.Globalization.xml",
                        "ref/netstandard1.0/ko/System.Globalization.xml",
                        "ref/netstandard1.0/ru/System.Globalization.xml",
                        "ref/netstandard1.0/zh-hans/System.Globalization.xml",
                        "ref/netstandard1.0/zh-hant/System.Globalization.xml",
                        "ref/netstandard1.3/System.Globalization.dll",
                        "ref/netstandard1.3/System.Globalization.xml",
                        "ref/netstandard1.3/de/System.Globalization.xml",
                        "ref/netstandard1.3/es/System.Globalization.xml",
                        "ref/netstandard1.3/fr/System.Globalization.xml",
                        "ref/netstandard1.3/it/System.Globalization.xml",
                        "ref/netstandard1.3/ja/System.Globalization.xml",
                        "ref/netstandard1.3/ko/System.Globalization.xml",
                        "ref/netstandard1.3/ru/System.Globalization.xml",
                        "ref/netstandard1.3/zh-hans/System.Globalization.xml",
                        "ref/netstandard1.3/zh-hant/System.Globalization.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.globalization.4.3.0.nupkg.sha512",
                        "system.globalization.nuspec"
                      ]
                    },
                    "System.Globalization.Calendars/4.3.0": {
                      "sha512": "GUlBtdOWT4LTV3I+9/PJW+56AnnChTaOqqTLFtdmype/L500M2LIyXgmtd9X2P2VOkmJd5c67H5SaC2QcL1bFA==",
                      "type": "package",
                      "path": "system.globalization.calendars/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net46/System.Globalization.Calendars.dll",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net46/System.Globalization.Calendars.dll",
                        "ref/netstandard1.3/System.Globalization.Calendars.dll",
                        "ref/netstandard1.3/System.Globalization.Calendars.xml",
                        "ref/netstandard1.3/de/System.Globalization.Calendars.xml",
                        "ref/netstandard1.3/es/System.Globalization.Calendars.xml",
                        "ref/netstandard1.3/fr/System.Globalization.Calendars.xml",
                        "ref/netstandard1.3/it/System.Globalization.Calendars.xml",
                        "ref/netstandard1.3/ja/System.Globalization.Calendars.xml",
                        "ref/netstandard1.3/ko/System.Globalization.Calendars.xml",
                        "ref/netstandard1.3/ru/System.Globalization.Calendars.xml",
                        "ref/netstandard1.3/zh-hans/System.Globalization.Calendars.xml",
                        "ref/netstandard1.3/zh-hant/System.Globalization.Calendars.xml",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.globalization.calendars.4.3.0.nupkg.sha512",
                        "system.globalization.calendars.nuspec"
                      ]
                    },
                    "System.Globalization.Extensions/4.3.0": {
                      "sha512": "FhKmdR6MPG+pxow6wGtNAWdZh7noIOpdD5TwQ3CprzgIE1bBBoim0vbR1+AWsWjQmU7zXHgQo4TWSP6lCeiWcQ==",
                      "type": "package",
                      "path": "system.globalization.extensions/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net46/System.Globalization.Extensions.dll",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net46/System.Globalization.Extensions.dll",
                        "ref/netstandard1.3/System.Globalization.Extensions.dll",
                        "ref/netstandard1.3/System.Globalization.Extensions.xml",
                        "ref/netstandard1.3/de/System.Globalization.Extensions.xml",
                        "ref/netstandard1.3/es/System.Globalization.Extensions.xml",
                        "ref/netstandard1.3/fr/System.Globalization.Extensions.xml",
                        "ref/netstandard1.3/it/System.Globalization.Extensions.xml",
                        "ref/netstandard1.3/ja/System.Globalization.Extensions.xml",
                        "ref/netstandard1.3/ko/System.Globalization.Extensions.xml",
                        "ref/netstandard1.3/ru/System.Globalization.Extensions.xml",
                        "ref/netstandard1.3/zh-hans/System.Globalization.Extensions.xml",
                        "ref/netstandard1.3/zh-hant/System.Globalization.Extensions.xml",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "runtimes/unix/lib/netstandard1.3/System.Globalization.Extensions.dll",
                        "runtimes/win/lib/net46/System.Globalization.Extensions.dll",
                        "runtimes/win/lib/netstandard1.3/System.Globalization.Extensions.dll",
                        "system.globalization.extensions.4.3.0.nupkg.sha512",
                        "system.globalization.extensions.nuspec"
                      ]
                    },
                    "System.IO/4.3.0": {
                      "sha512": "3qjaHvxQPDpSOYICjUoTsmoq5u6QJAFRUITgeT/4gqkF1bajbSmb1kwSxEA8AHlofqgcKJcM8udgieRNhaJ5Cg==",
                      "type": "package",
                      "path": "system.io/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/net462/System.IO.dll",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/net462/System.IO.dll",
                        "ref/netcore50/System.IO.dll",
                        "ref/netcore50/System.IO.xml",
                        "ref/netcore50/de/System.IO.xml",
                        "ref/netcore50/es/System.IO.xml",
                        "ref/netcore50/fr/System.IO.xml",
                        "ref/netcore50/it/System.IO.xml",
                        "ref/netcore50/ja/System.IO.xml",
                        "ref/netcore50/ko/System.IO.xml",
                        "ref/netcore50/ru/System.IO.xml",
                        "ref/netcore50/zh-hans/System.IO.xml",
                        "ref/netcore50/zh-hant/System.IO.xml",
                        "ref/netstandard1.0/System.IO.dll",
                        "ref/netstandard1.0/System.IO.xml",
                        "ref/netstandard1.0/de/System.IO.xml",
                        "ref/netstandard1.0/es/System.IO.xml",
                        "ref/netstandard1.0/fr/System.IO.xml",
                        "ref/netstandard1.0/it/System.IO.xml",
                        "ref/netstandard1.0/ja/System.IO.xml",
                        "ref/netstandard1.0/ko/System.IO.xml",
                        "ref/netstandard1.0/ru/System.IO.xml",
                        "ref/netstandard1.0/zh-hans/System.IO.xml",
                        "ref/netstandard1.0/zh-hant/System.IO.xml",
                        "ref/netstandard1.3/System.IO.dll",
                        "ref/netstandard1.3/System.IO.xml",
                        "ref/netstandard1.3/de/System.IO.xml",
                        "ref/netstandard1.3/es/System.IO.xml",
                        "ref/netstandard1.3/fr/System.IO.xml",
                        "ref/netstandard1.3/it/System.IO.xml",
                        "ref/netstandard1.3/ja/System.IO.xml",
                        "ref/netstandard1.3/ko/System.IO.xml",
                        "ref/netstandard1.3/ru/System.IO.xml",
                        "ref/netstandard1.3/zh-hans/System.IO.xml",
                        "ref/netstandard1.3/zh-hant/System.IO.xml",
                        "ref/netstandard1.5/System.IO.dll",
                        "ref/netstandard1.5/System.IO.xml",
                        "ref/netstandard1.5/de/System.IO.xml",
                        "ref/netstandard1.5/es/System.IO.xml",
                        "ref/netstandard1.5/fr/System.IO.xml",
                        "ref/netstandard1.5/it/System.IO.xml",
                        "ref/netstandard1.5/ja/System.IO.xml",
                        "ref/netstandard1.5/ko/System.IO.xml",
                        "ref/netstandard1.5/ru/System.IO.xml",
                        "ref/netstandard1.5/zh-hans/System.IO.xml",
                        "ref/netstandard1.5/zh-hant/System.IO.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.io.4.3.0.nupkg.sha512",
                        "system.io.nuspec"
                      ]
                    },
                    "System.IO.Compression/4.3.0": {
                      "sha512": "YHndyoiV90iu4iKG115ibkhrG+S3jBm8Ap9OwoUAzO5oPDAWcr0SFwQFm0HjM8WkEZWo0zvLTyLmbvTkW1bXgg==",
                      "type": "package",
                      "path": "system.io.compression/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/net46/System.IO.Compression.dll",
                        "lib/portable-net45+win8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/net46/System.IO.Compression.dll",
                        "ref/netcore50/System.IO.Compression.dll",
                        "ref/netcore50/System.IO.Compression.xml",
                        "ref/netcore50/de/System.IO.Compression.xml",
                        "ref/netcore50/es/System.IO.Compression.xml",
                        "ref/netcore50/fr/System.IO.Compression.xml",
                        "ref/netcore50/it/System.IO.Compression.xml",
                        "ref/netcore50/ja/System.IO.Compression.xml",
                        "ref/netcore50/ko/System.IO.Compression.xml",
                        "ref/netcore50/ru/System.IO.Compression.xml",
                        "ref/netcore50/zh-hans/System.IO.Compression.xml",
                        "ref/netcore50/zh-hant/System.IO.Compression.xml",
                        "ref/netstandard1.1/System.IO.Compression.dll",
                        "ref/netstandard1.1/System.IO.Compression.xml",
                        "ref/netstandard1.1/de/System.IO.Compression.xml",
                        "ref/netstandard1.1/es/System.IO.Compression.xml",
                        "ref/netstandard1.1/fr/System.IO.Compression.xml",
                        "ref/netstandard1.1/it/System.IO.Compression.xml",
                        "ref/netstandard1.1/ja/System.IO.Compression.xml",
                        "ref/netstandard1.1/ko/System.IO.Compression.xml",
                        "ref/netstandard1.1/ru/System.IO.Compression.xml",
                        "ref/netstandard1.1/zh-hans/System.IO.Compression.xml",
                        "ref/netstandard1.1/zh-hant/System.IO.Compression.xml",
                        "ref/netstandard1.3/System.IO.Compression.dll",
                        "ref/netstandard1.3/System.IO.Compression.xml",
                        "ref/netstandard1.3/de/System.IO.Compression.xml",
                        "ref/netstandard1.3/es/System.IO.Compression.xml",
                        "ref/netstandard1.3/fr/System.IO.Compression.xml",
                        "ref/netstandard1.3/it/System.IO.Compression.xml",
                        "ref/netstandard1.3/ja/System.IO.Compression.xml",
                        "ref/netstandard1.3/ko/System.IO.Compression.xml",
                        "ref/netstandard1.3/ru/System.IO.Compression.xml",
                        "ref/netstandard1.3/zh-hans/System.IO.Compression.xml",
                        "ref/netstandard1.3/zh-hant/System.IO.Compression.xml",
                        "ref/portable-net45+win8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "runtimes/unix/lib/netstandard1.3/System.IO.Compression.dll",
                        "runtimes/win/lib/net46/System.IO.Compression.dll",
                        "runtimes/win/lib/netstandard1.3/System.IO.Compression.dll",
                        "system.io.compression.4.3.0.nupkg.sha512",
                        "system.io.compression.nuspec"
                      ]
                    },
                    "System.IO.Compression.ZipFile/4.3.0": {
                      "sha512": "G4HwjEsgIwy3JFBduZ9quBkAu+eUwjIdJleuNSgmUojbH6O3mlvEIme+GHx/cLlTAPcrnnL7GqvB9pTlWRfhOg==",
                      "type": "package",
                      "path": "system.io.compression.zipfile/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net46/System.IO.Compression.ZipFile.dll",
                        "lib/netstandard1.3/System.IO.Compression.ZipFile.dll",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net46/System.IO.Compression.ZipFile.dll",
                        "ref/netstandard1.3/System.IO.Compression.ZipFile.dll",
                        "ref/netstandard1.3/System.IO.Compression.ZipFile.xml",
                        "ref/netstandard1.3/de/System.IO.Compression.ZipFile.xml",
                        "ref/netstandard1.3/es/System.IO.Compression.ZipFile.xml",
                        "ref/netstandard1.3/fr/System.IO.Compression.ZipFile.xml",
                        "ref/netstandard1.3/it/System.IO.Compression.ZipFile.xml",
                        "ref/netstandard1.3/ja/System.IO.Compression.ZipFile.xml",
                        "ref/netstandard1.3/ko/System.IO.Compression.ZipFile.xml",
                        "ref/netstandard1.3/ru/System.IO.Compression.ZipFile.xml",
                        "ref/netstandard1.3/zh-hans/System.IO.Compression.ZipFile.xml",
                        "ref/netstandard1.3/zh-hant/System.IO.Compression.ZipFile.xml",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.io.compression.zipfile.4.3.0.nupkg.sha512",
                        "system.io.compression.zipfile.nuspec"
                      ]
                    },
                    "System.IO.FileSystem/4.3.0": {
                      "sha512": "3wEMARTnuio+ulnvi+hkRNROYwa1kylvYahhcLk4HSoVdl+xxTFVeVlYOfLwrDPImGls0mDqbMhrza8qnWPTdA==",
                      "type": "package",
                      "path": "system.io.filesystem/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net46/System.IO.FileSystem.dll",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net46/System.IO.FileSystem.dll",
                        "ref/netstandard1.3/System.IO.FileSystem.dll",
                        "ref/netstandard1.3/System.IO.FileSystem.xml",
                        "ref/netstandard1.3/de/System.IO.FileSystem.xml",
                        "ref/netstandard1.3/es/System.IO.FileSystem.xml",
                        "ref/netstandard1.3/fr/System.IO.FileSystem.xml",
                        "ref/netstandard1.3/it/System.IO.FileSystem.xml",
                        "ref/netstandard1.3/ja/System.IO.FileSystem.xml",
                        "ref/netstandard1.3/ko/System.IO.FileSystem.xml",
                        "ref/netstandard1.3/ru/System.IO.FileSystem.xml",
                        "ref/netstandard1.3/zh-hans/System.IO.FileSystem.xml",
                        "ref/netstandard1.3/zh-hant/System.IO.FileSystem.xml",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.io.filesystem.4.3.0.nupkg.sha512",
                        "system.io.filesystem.nuspec"
                      ]
                    },
                    "System.IO.FileSystem.Primitives/4.3.0": {
                      "sha512": "6QOb2XFLch7bEc4lIcJH49nJN2HV+OC3fHDgsLVsBVBk3Y4hFAnOBGzJ2lUu7CyDDFo9IBWkSsnbkT6IBwwiMw==",
                      "type": "package",
                      "path": "system.io.filesystem.primitives/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net46/System.IO.FileSystem.Primitives.dll",
                        "lib/netstandard1.3/System.IO.FileSystem.Primitives.dll",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net46/System.IO.FileSystem.Primitives.dll",
                        "ref/netstandard1.3/System.IO.FileSystem.Primitives.dll",
                        "ref/netstandard1.3/System.IO.FileSystem.Primitives.xml",
                        "ref/netstandard1.3/de/System.IO.FileSystem.Primitives.xml",
                        "ref/netstandard1.3/es/System.IO.FileSystem.Primitives.xml",
                        "ref/netstandard1.3/fr/System.IO.FileSystem.Primitives.xml",
                        "ref/netstandard1.3/it/System.IO.FileSystem.Primitives.xml",
                        "ref/netstandard1.3/ja/System.IO.FileSystem.Primitives.xml",
                        "ref/netstandard1.3/ko/System.IO.FileSystem.Primitives.xml",
                        "ref/netstandard1.3/ru/System.IO.FileSystem.Primitives.xml",
                        "ref/netstandard1.3/zh-hans/System.IO.FileSystem.Primitives.xml",
                        "ref/netstandard1.3/zh-hant/System.IO.FileSystem.Primitives.xml",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.io.filesystem.primitives.4.3.0.nupkg.sha512",
                        "system.io.filesystem.primitives.nuspec"
                      ]
                    },
                    "System.Linq/4.3.0": {
                      "sha512": "5DbqIUpsDp0dFftytzuMmc0oeMdQwjcP/EWxsksIz/w1TcFRkZ3yKKz0PqiYFMmEwPSWw+qNVqD7PJ889JzHbw==",
                      "type": "package",
                      "path": "system.linq/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/net463/System.Linq.dll",
                        "lib/netcore50/System.Linq.dll",
                        "lib/netstandard1.6/System.Linq.dll",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/net463/System.Linq.dll",
                        "ref/netcore50/System.Linq.dll",
                        "ref/netcore50/System.Linq.xml",
                        "ref/netcore50/de/System.Linq.xml",
                        "ref/netcore50/es/System.Linq.xml",
                        "ref/netcore50/fr/System.Linq.xml",
                        "ref/netcore50/it/System.Linq.xml",
                        "ref/netcore50/ja/System.Linq.xml",
                        "ref/netcore50/ko/System.Linq.xml",
                        "ref/netcore50/ru/System.Linq.xml",
                        "ref/netcore50/zh-hans/System.Linq.xml",
                        "ref/netcore50/zh-hant/System.Linq.xml",
                        "ref/netstandard1.0/System.Linq.dll",
                        "ref/netstandard1.0/System.Linq.xml",
                        "ref/netstandard1.0/de/System.Linq.xml",
                        "ref/netstandard1.0/es/System.Linq.xml",
                        "ref/netstandard1.0/fr/System.Linq.xml",
                        "ref/netstandard1.0/it/System.Linq.xml",
                        "ref/netstandard1.0/ja/System.Linq.xml",
                        "ref/netstandard1.0/ko/System.Linq.xml",
                        "ref/netstandard1.0/ru/System.Linq.xml",
                        "ref/netstandard1.0/zh-hans/System.Linq.xml",
                        "ref/netstandard1.0/zh-hant/System.Linq.xml",
                        "ref/netstandard1.6/System.Linq.dll",
                        "ref/netstandard1.6/System.Linq.xml",
                        "ref/netstandard1.6/de/System.Linq.xml",
                        "ref/netstandard1.6/es/System.Linq.xml",
                        "ref/netstandard1.6/fr/System.Linq.xml",
                        "ref/netstandard1.6/it/System.Linq.xml",
                        "ref/netstandard1.6/ja/System.Linq.xml",
                        "ref/netstandard1.6/ko/System.Linq.xml",
                        "ref/netstandard1.6/ru/System.Linq.xml",
                        "ref/netstandard1.6/zh-hans/System.Linq.xml",
                        "ref/netstandard1.6/zh-hant/System.Linq.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.linq.4.3.0.nupkg.sha512",
                        "system.linq.nuspec"
                      ]
                    },
                    "System.Linq.Expressions/4.3.0": {
                      "sha512": "PGKkrd2khG4CnlyJwxwwaWWiSiWFNBGlgXvJpeO0xCXrZ89ODrQ6tjEWS/kOqZ8GwEOUATtKtzp1eRgmYNfclg==",
                      "type": "package",
                      "path": "system.linq.expressions/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/net463/System.Linq.Expressions.dll",
                        "lib/netcore50/System.Linq.Expressions.dll",
                        "lib/netstandard1.6/System.Linq.Expressions.dll",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/net463/System.Linq.Expressions.dll",
                        "ref/netcore50/System.Linq.Expressions.dll",
                        "ref/netcore50/System.Linq.Expressions.xml",
                        "ref/netcore50/de/System.Linq.Expressions.xml",
                        "ref/netcore50/es/System.Linq.Expressions.xml",
                        "ref/netcore50/fr/System.Linq.Expressions.xml",
                        "ref/netcore50/it/System.Linq.Expressions.xml",
                        "ref/netcore50/ja/System.Linq.Expressions.xml",
                        "ref/netcore50/ko/System.Linq.Expressions.xml",
                        "ref/netcore50/ru/System.Linq.Expressions.xml",
                        "ref/netcore50/zh-hans/System.Linq.Expressions.xml",
                        "ref/netcore50/zh-hant/System.Linq.Expressions.xml",
                        "ref/netstandard1.0/System.Linq.Expressions.dll",
                        "ref/netstandard1.0/System.Linq.Expressions.xml",
                        "ref/netstandard1.0/de/System.Linq.Expressions.xml",
                        "ref/netstandard1.0/es/System.Linq.Expressions.xml",
                        "ref/netstandard1.0/fr/System.Linq.Expressions.xml",
                        "ref/netstandard1.0/it/System.Linq.Expressions.xml",
                        "ref/netstandard1.0/ja/System.Linq.Expressions.xml",
                        "ref/netstandard1.0/ko/System.Linq.Expressions.xml",
                        "ref/netstandard1.0/ru/System.Linq.Expressions.xml",
                        "ref/netstandard1.0/zh-hans/System.Linq.Expressions.xml",
                        "ref/netstandard1.0/zh-hant/System.Linq.Expressions.xml",
                        "ref/netstandard1.3/System.Linq.Expressions.dll",
                        "ref/netstandard1.3/System.Linq.Expressions.xml",
                        "ref/netstandard1.3/de/System.Linq.Expressions.xml",
                        "ref/netstandard1.3/es/System.Linq.Expressions.xml",
                        "ref/netstandard1.3/fr/System.Linq.Expressions.xml",
                        "ref/netstandard1.3/it/System.Linq.Expressions.xml",
                        "ref/netstandard1.3/ja/System.Linq.Expressions.xml",
                        "ref/netstandard1.3/ko/System.Linq.Expressions.xml",
                        "ref/netstandard1.3/ru/System.Linq.Expressions.xml",
                        "ref/netstandard1.3/zh-hans/System.Linq.Expressions.xml",
                        "ref/netstandard1.3/zh-hant/System.Linq.Expressions.xml",
                        "ref/netstandard1.6/System.Linq.Expressions.dll",
                        "ref/netstandard1.6/System.Linq.Expressions.xml",
                        "ref/netstandard1.6/de/System.Linq.Expressions.xml",
                        "ref/netstandard1.6/es/System.Linq.Expressions.xml",
                        "ref/netstandard1.6/fr/System.Linq.Expressions.xml",
                        "ref/netstandard1.6/it/System.Linq.Expressions.xml",
                        "ref/netstandard1.6/ja/System.Linq.Expressions.xml",
                        "ref/netstandard1.6/ko/System.Linq.Expressions.xml",
                        "ref/netstandard1.6/ru/System.Linq.Expressions.xml",
                        "ref/netstandard1.6/zh-hans/System.Linq.Expressions.xml",
                        "ref/netstandard1.6/zh-hant/System.Linq.Expressions.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "runtimes/aot/lib/netcore50/System.Linq.Expressions.dll",
                        "system.linq.expressions.4.3.0.nupkg.sha512",
                        "system.linq.expressions.nuspec"
                      ]
                    },
                    "System.Net.Http/4.3.0": {
                      "sha512": "sYg+FtILtRQuYWSIAuNOELwVuVsxVyJGWQyOnlAzhV4xvhyFnON1bAzYYC+jjRW8JREM45R0R5Dgi8MTC5sEwA==",
                      "type": "package",
                      "path": "system.net.http/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/Xamarinmac20/_._",
                        "lib/monoandroid10/_._",
                        "lib/monotouch10/_._",
                        "lib/net45/_._",
                        "lib/net46/System.Net.Http.dll",
                        "lib/portable-net45+win8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/Xamarinmac20/_._",
                        "ref/monoandroid10/_._",
                        "ref/monotouch10/_._",
                        "ref/net45/_._",
                        "ref/net46/System.Net.Http.dll",
                        "ref/net46/System.Net.Http.xml",
                        "ref/net46/de/System.Net.Http.xml",
                        "ref/net46/es/System.Net.Http.xml",
                        "ref/net46/fr/System.Net.Http.xml",
                        "ref/net46/it/System.Net.Http.xml",
                        "ref/net46/ja/System.Net.Http.xml",
                        "ref/net46/ko/System.Net.Http.xml",
                        "ref/net46/ru/System.Net.Http.xml",
                        "ref/net46/zh-hans/System.Net.Http.xml",
                        "ref/net46/zh-hant/System.Net.Http.xml",
                        "ref/netcore50/System.Net.Http.dll",
                        "ref/netcore50/System.Net.Http.xml",
                        "ref/netcore50/de/System.Net.Http.xml",
                        "ref/netcore50/es/System.Net.Http.xml",
                        "ref/netcore50/fr/System.Net.Http.xml",
                        "ref/netcore50/it/System.Net.Http.xml",
                        "ref/netcore50/ja/System.Net.Http.xml",
                        "ref/netcore50/ko/System.Net.Http.xml",
                        "ref/netcore50/ru/System.Net.Http.xml",
                        "ref/netcore50/zh-hans/System.Net.Http.xml",
                        "ref/netcore50/zh-hant/System.Net.Http.xml",
                        "ref/netstandard1.1/System.Net.Http.dll",
                        "ref/netstandard1.1/System.Net.Http.xml",
                        "ref/netstandard1.1/de/System.Net.Http.xml",
                        "ref/netstandard1.1/es/System.Net.Http.xml",
                        "ref/netstandard1.1/fr/System.Net.Http.xml",
                        "ref/netstandard1.1/it/System.Net.Http.xml",
                        "ref/netstandard1.1/ja/System.Net.Http.xml",
                        "ref/netstandard1.1/ko/System.Net.Http.xml",
                        "ref/netstandard1.1/ru/System.Net.Http.xml",
                        "ref/netstandard1.1/zh-hans/System.Net.Http.xml",
                        "ref/netstandard1.1/zh-hant/System.Net.Http.xml",
                        "ref/netstandard1.3/System.Net.Http.dll",
                        "ref/netstandard1.3/System.Net.Http.xml",
                        "ref/netstandard1.3/de/System.Net.Http.xml",
                        "ref/netstandard1.3/es/System.Net.Http.xml",
                        "ref/netstandard1.3/fr/System.Net.Http.xml",
                        "ref/netstandard1.3/it/System.Net.Http.xml",
                        "ref/netstandard1.3/ja/System.Net.Http.xml",
                        "ref/netstandard1.3/ko/System.Net.Http.xml",
                        "ref/netstandard1.3/ru/System.Net.Http.xml",
                        "ref/netstandard1.3/zh-hans/System.Net.Http.xml",
                        "ref/netstandard1.3/zh-hant/System.Net.Http.xml",
                        "ref/portable-net45+win8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "runtimes/unix/lib/netstandard1.6/System.Net.Http.dll",
                        "runtimes/win/lib/net46/System.Net.Http.dll",
                        "runtimes/win/lib/netcore50/System.Net.Http.dll",
                        "runtimes/win/lib/netstandard1.3/System.Net.Http.dll",
                        "system.net.http.4.3.0.nupkg.sha512",
                        "system.net.http.nuspec"
                      ]
                    },
                    "System.Net.Primitives/4.3.0": {
                      "sha512": "qOu+hDwFwoZPbzPvwut2qATe3ygjeQBDQj91xlsaqGFQUI5i4ZnZb8yyQuLGpDGivEPIt8EJkd1BVzVoP31FXA==",
                      "type": "package",
                      "path": "system.net.primitives/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/netcore50/System.Net.Primitives.dll",
                        "ref/netcore50/System.Net.Primitives.xml",
                        "ref/netcore50/de/System.Net.Primitives.xml",
                        "ref/netcore50/es/System.Net.Primitives.xml",
                        "ref/netcore50/fr/System.Net.Primitives.xml",
                        "ref/netcore50/it/System.Net.Primitives.xml",
                        "ref/netcore50/ja/System.Net.Primitives.xml",
                        "ref/netcore50/ko/System.Net.Primitives.xml",
                        "ref/netcore50/ru/System.Net.Primitives.xml",
                        "ref/netcore50/zh-hans/System.Net.Primitives.xml",
                        "ref/netcore50/zh-hant/System.Net.Primitives.xml",
                        "ref/netstandard1.0/System.Net.Primitives.dll",
                        "ref/netstandard1.0/System.Net.Primitives.xml",
                        "ref/netstandard1.0/de/System.Net.Primitives.xml",
                        "ref/netstandard1.0/es/System.Net.Primitives.xml",
                        "ref/netstandard1.0/fr/System.Net.Primitives.xml",
                        "ref/netstandard1.0/it/System.Net.Primitives.xml",
                        "ref/netstandard1.0/ja/System.Net.Primitives.xml",
                        "ref/netstandard1.0/ko/System.Net.Primitives.xml",
                        "ref/netstandard1.0/ru/System.Net.Primitives.xml",
                        "ref/netstandard1.0/zh-hans/System.Net.Primitives.xml",
                        "ref/netstandard1.0/zh-hant/System.Net.Primitives.xml",
                        "ref/netstandard1.1/System.Net.Primitives.dll",
                        "ref/netstandard1.1/System.Net.Primitives.xml",
                        "ref/netstandard1.1/de/System.Net.Primitives.xml",
                        "ref/netstandard1.1/es/System.Net.Primitives.xml",
                        "ref/netstandard1.1/fr/System.Net.Primitives.xml",
                        "ref/netstandard1.1/it/System.Net.Primitives.xml",
                        "ref/netstandard1.1/ja/System.Net.Primitives.xml",
                        "ref/netstandard1.1/ko/System.Net.Primitives.xml",
                        "ref/netstandard1.1/ru/System.Net.Primitives.xml",
                        "ref/netstandard1.1/zh-hans/System.Net.Primitives.xml",
                        "ref/netstandard1.1/zh-hant/System.Net.Primitives.xml",
                        "ref/netstandard1.3/System.Net.Primitives.dll",
                        "ref/netstandard1.3/System.Net.Primitives.xml",
                        "ref/netstandard1.3/de/System.Net.Primitives.xml",
                        "ref/netstandard1.3/es/System.Net.Primitives.xml",
                        "ref/netstandard1.3/fr/System.Net.Primitives.xml",
                        "ref/netstandard1.3/it/System.Net.Primitives.xml",
                        "ref/netstandard1.3/ja/System.Net.Primitives.xml",
                        "ref/netstandard1.3/ko/System.Net.Primitives.xml",
                        "ref/netstandard1.3/ru/System.Net.Primitives.xml",
                        "ref/netstandard1.3/zh-hans/System.Net.Primitives.xml",
                        "ref/netstandard1.3/zh-hant/System.Net.Primitives.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.net.primitives.4.3.0.nupkg.sha512",
                        "system.net.primitives.nuspec"
                      ]
                    },
                    "System.Net.Sockets/4.3.0": {
                      "sha512": "m6icV6TqQOAdgt5N/9I5KNpjom/5NFtkmGseEH+AK/hny8XrytLH3+b5M8zL/Ycg3fhIocFpUMyl/wpFnVRvdw==",
                      "type": "package",
                      "path": "system.net.sockets/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net46/System.Net.Sockets.dll",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net46/System.Net.Sockets.dll",
                        "ref/netstandard1.3/System.Net.Sockets.dll",
                        "ref/netstandard1.3/System.Net.Sockets.xml",
                        "ref/netstandard1.3/de/System.Net.Sockets.xml",
                        "ref/netstandard1.3/es/System.Net.Sockets.xml",
                        "ref/netstandard1.3/fr/System.Net.Sockets.xml",
                        "ref/netstandard1.3/it/System.Net.Sockets.xml",
                        "ref/netstandard1.3/ja/System.Net.Sockets.xml",
                        "ref/netstandard1.3/ko/System.Net.Sockets.xml",
                        "ref/netstandard1.3/ru/System.Net.Sockets.xml",
                        "ref/netstandard1.3/zh-hans/System.Net.Sockets.xml",
                        "ref/netstandard1.3/zh-hant/System.Net.Sockets.xml",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.net.sockets.4.3.0.nupkg.sha512",
                        "system.net.sockets.nuspec"
                      ]
                    },
                    "System.ObjectModel/4.3.0": {
                      "sha512": "bdX+80eKv9bN6K4N+d77OankKHGn6CH711a6fcOpMQu2Fckp/Ft4L/kW9WznHpyR0NRAvJutzOMHNNlBGvxQzQ==",
                      "type": "package",
                      "path": "system.objectmodel/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/netcore50/System.ObjectModel.dll",
                        "lib/netstandard1.3/System.ObjectModel.dll",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/netcore50/System.ObjectModel.dll",
                        "ref/netcore50/System.ObjectModel.xml",
                        "ref/netcore50/de/System.ObjectModel.xml",
                        "ref/netcore50/es/System.ObjectModel.xml",
                        "ref/netcore50/fr/System.ObjectModel.xml",
                        "ref/netcore50/it/System.ObjectModel.xml",
                        "ref/netcore50/ja/System.ObjectModel.xml",
                        "ref/netcore50/ko/System.ObjectModel.xml",
                        "ref/netcore50/ru/System.ObjectModel.xml",
                        "ref/netcore50/zh-hans/System.ObjectModel.xml",
                        "ref/netcore50/zh-hant/System.ObjectModel.xml",
                        "ref/netstandard1.0/System.ObjectModel.dll",
                        "ref/netstandard1.0/System.ObjectModel.xml",
                        "ref/netstandard1.0/de/System.ObjectModel.xml",
                        "ref/netstandard1.0/es/System.ObjectModel.xml",
                        "ref/netstandard1.0/fr/System.ObjectModel.xml",
                        "ref/netstandard1.0/it/System.ObjectModel.xml",
                        "ref/netstandard1.0/ja/System.ObjectModel.xml",
                        "ref/netstandard1.0/ko/System.ObjectModel.xml",
                        "ref/netstandard1.0/ru/System.ObjectModel.xml",
                        "ref/netstandard1.0/zh-hans/System.ObjectModel.xml",
                        "ref/netstandard1.0/zh-hant/System.ObjectModel.xml",
                        "ref/netstandard1.3/System.ObjectModel.dll",
                        "ref/netstandard1.3/System.ObjectModel.xml",
                        "ref/netstandard1.3/de/System.ObjectModel.xml",
                        "ref/netstandard1.3/es/System.ObjectModel.xml",
                        "ref/netstandard1.3/fr/System.ObjectModel.xml",
                        "ref/netstandard1.3/it/System.ObjectModel.xml",
                        "ref/netstandard1.3/ja/System.ObjectModel.xml",
                        "ref/netstandard1.3/ko/System.ObjectModel.xml",
                        "ref/netstandard1.3/ru/System.ObjectModel.xml",
                        "ref/netstandard1.3/zh-hans/System.ObjectModel.xml",
                        "ref/netstandard1.3/zh-hant/System.ObjectModel.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.objectmodel.4.3.0.nupkg.sha512",
                        "system.objectmodel.nuspec"
                      ]
                    },
                    "System.Reflection/4.3.0": {
                      "sha512": "KMiAFoW7MfJGa9nDFNcfu+FpEdiHpWgTcS2HdMpDvt9saK3y/G4GwprPyzqjFH9NTaGPQeWNHU+iDlDILj96aQ==",
                      "type": "package",
                      "path": "system.reflection/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/net462/System.Reflection.dll",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/net462/System.Reflection.dll",
                        "ref/netcore50/System.Reflection.dll",
                        "ref/netcore50/System.Reflection.xml",
                        "ref/netcore50/de/System.Reflection.xml",
                        "ref/netcore50/es/System.Reflection.xml",
                        "ref/netcore50/fr/System.Reflection.xml",
                        "ref/netcore50/it/System.Reflection.xml",
                        "ref/netcore50/ja/System.Reflection.xml",
                        "ref/netcore50/ko/System.Reflection.xml",
                        "ref/netcore50/ru/System.Reflection.xml",
                        "ref/netcore50/zh-hans/System.Reflection.xml",
                        "ref/netcore50/zh-hant/System.Reflection.xml",
                        "ref/netstandard1.0/System.Reflection.dll",
                        "ref/netstandard1.0/System.Reflection.xml",
                        "ref/netstandard1.0/de/System.Reflection.xml",
                        "ref/netstandard1.0/es/System.Reflection.xml",
                        "ref/netstandard1.0/fr/System.Reflection.xml",
                        "ref/netstandard1.0/it/System.Reflection.xml",
                        "ref/netstandard1.0/ja/System.Reflection.xml",
                        "ref/netstandard1.0/ko/System.Reflection.xml",
                        "ref/netstandard1.0/ru/System.Reflection.xml",
                        "ref/netstandard1.0/zh-hans/System.Reflection.xml",
                        "ref/netstandard1.0/zh-hant/System.Reflection.xml",
                        "ref/netstandard1.3/System.Reflection.dll",
                        "ref/netstandard1.3/System.Reflection.xml",
                        "ref/netstandard1.3/de/System.Reflection.xml",
                        "ref/netstandard1.3/es/System.Reflection.xml",
                        "ref/netstandard1.3/fr/System.Reflection.xml",
                        "ref/netstandard1.3/it/System.Reflection.xml",
                        "ref/netstandard1.3/ja/System.Reflection.xml",
                        "ref/netstandard1.3/ko/System.Reflection.xml",
                        "ref/netstandard1.3/ru/System.Reflection.xml",
                        "ref/netstandard1.3/zh-hans/System.Reflection.xml",
                        "ref/netstandard1.3/zh-hant/System.Reflection.xml",
                        "ref/netstandard1.5/System.Reflection.dll",
                        "ref/netstandard1.5/System.Reflection.xml",
                        "ref/netstandard1.5/de/System.Reflection.xml",
                        "ref/netstandard1.5/es/System.Reflection.xml",
                        "ref/netstandard1.5/fr/System.Reflection.xml",
                        "ref/netstandard1.5/it/System.Reflection.xml",
                        "ref/netstandard1.5/ja/System.Reflection.xml",
                        "ref/netstandard1.5/ko/System.Reflection.xml",
                        "ref/netstandard1.5/ru/System.Reflection.xml",
                        "ref/netstandard1.5/zh-hans/System.Reflection.xml",
                        "ref/netstandard1.5/zh-hant/System.Reflection.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.reflection.4.3.0.nupkg.sha512",
                        "system.reflection.nuspec"
                      ]
                    },
                    "System.Reflection.Emit/4.3.0": {
                      "sha512": "228FG0jLcIwTVJyz8CLFKueVqQK36ANazUManGaJHkO0icjiIypKW7YLWLIWahyIkdh5M7mV2dJepllLyA1SKg==",
                      "type": "package",
                      "path": "system.reflection.emit/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/monotouch10/_._",
                        "lib/net45/_._",
                        "lib/netcore50/System.Reflection.Emit.dll",
                        "lib/netstandard1.3/System.Reflection.Emit.dll",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/net45/_._",
                        "ref/netstandard1.1/System.Reflection.Emit.dll",
                        "ref/netstandard1.1/System.Reflection.Emit.xml",
                        "ref/netstandard1.1/de/System.Reflection.Emit.xml",
                        "ref/netstandard1.1/es/System.Reflection.Emit.xml",
                        "ref/netstandard1.1/fr/System.Reflection.Emit.xml",
                        "ref/netstandard1.1/it/System.Reflection.Emit.xml",
                        "ref/netstandard1.1/ja/System.Reflection.Emit.xml",
                        "ref/netstandard1.1/ko/System.Reflection.Emit.xml",
                        "ref/netstandard1.1/ru/System.Reflection.Emit.xml",
                        "ref/netstandard1.1/zh-hans/System.Reflection.Emit.xml",
                        "ref/netstandard1.1/zh-hant/System.Reflection.Emit.xml",
                        "ref/xamarinmac20/_._",
                        "system.reflection.emit.4.3.0.nupkg.sha512",
                        "system.reflection.emit.nuspec"
                      ]
                    },
                    "System.Reflection.Emit.ILGeneration/4.3.0": {
                      "sha512": "59tBslAk9733NXLrUJrwNZEzbMAcu8k344OYo+wfSVygcgZ9lgBdGIzH/nrg3LYhXceynyvTc8t5/GD4Ri0/ng==",
                      "type": "package",
                      "path": "system.reflection.emit.ilgeneration/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/netcore50/System.Reflection.Emit.ILGeneration.dll",
                        "lib/netstandard1.3/System.Reflection.Emit.ILGeneration.dll",
                        "lib/portable-net45+wp8/_._",
                        "lib/wp80/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/netstandard1.0/System.Reflection.Emit.ILGeneration.dll",
                        "ref/netstandard1.0/System.Reflection.Emit.ILGeneration.xml",
                        "ref/netstandard1.0/de/System.Reflection.Emit.ILGeneration.xml",
                        "ref/netstandard1.0/es/System.Reflection.Emit.ILGeneration.xml",
                        "ref/netstandard1.0/fr/System.Reflection.Emit.ILGeneration.xml",
                        "ref/netstandard1.0/it/System.Reflection.Emit.ILGeneration.xml",
                        "ref/netstandard1.0/ja/System.Reflection.Emit.ILGeneration.xml",
                        "ref/netstandard1.0/ko/System.Reflection.Emit.ILGeneration.xml",
                        "ref/netstandard1.0/ru/System.Reflection.Emit.ILGeneration.xml",
                        "ref/netstandard1.0/zh-hans/System.Reflection.Emit.ILGeneration.xml",
                        "ref/netstandard1.0/zh-hant/System.Reflection.Emit.ILGeneration.xml",
                        "ref/portable-net45+wp8/_._",
                        "ref/wp80/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "runtimes/aot/lib/netcore50/_._",
                        "system.reflection.emit.ilgeneration.4.3.0.nupkg.sha512",
                        "system.reflection.emit.ilgeneration.nuspec"
                      ]
                    },
                    "System.Reflection.Emit.Lightweight/4.3.0": {
                      "sha512": "oadVHGSMsTmZsAF864QYN1t1QzZjIcuKU3l2S9cZOwDdDueNTrqq1yRj7koFfIGEnKpt6NjpL3rOzRhs4ryOgA==",
                      "type": "package",
                      "path": "system.reflection.emit.lightweight/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/netcore50/System.Reflection.Emit.Lightweight.dll",
                        "lib/netstandard1.3/System.Reflection.Emit.Lightweight.dll",
                        "lib/portable-net45+wp8/_._",
                        "lib/wp80/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/netstandard1.0/System.Reflection.Emit.Lightweight.dll",
                        "ref/netstandard1.0/System.Reflection.Emit.Lightweight.xml",
                        "ref/netstandard1.0/de/System.Reflection.Emit.Lightweight.xml",
                        "ref/netstandard1.0/es/System.Reflection.Emit.Lightweight.xml",
                        "ref/netstandard1.0/fr/System.Reflection.Emit.Lightweight.xml",
                        "ref/netstandard1.0/it/System.Reflection.Emit.Lightweight.xml",
                        "ref/netstandard1.0/ja/System.Reflection.Emit.Lightweight.xml",
                        "ref/netstandard1.0/ko/System.Reflection.Emit.Lightweight.xml",
                        "ref/netstandard1.0/ru/System.Reflection.Emit.Lightweight.xml",
                        "ref/netstandard1.0/zh-hans/System.Reflection.Emit.Lightweight.xml",
                        "ref/netstandard1.0/zh-hant/System.Reflection.Emit.Lightweight.xml",
                        "ref/portable-net45+wp8/_._",
                        "ref/wp80/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "runtimes/aot/lib/netcore50/_._",
                        "system.reflection.emit.lightweight.4.3.0.nupkg.sha512",
                        "system.reflection.emit.lightweight.nuspec"
                      ]
                    },
                    "System.Reflection.Extensions/4.3.0": {
                      "sha512": "rJkrJD3kBI5B712aRu4DpSIiHRtr6QlfZSQsb0hYHrDCZORXCFjQfoipo2LaMUHoT9i1B7j7MnfaEKWDFmFQNQ==",
                      "type": "package",
                      "path": "system.reflection.extensions/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/netcore50/System.Reflection.Extensions.dll",
                        "ref/netcore50/System.Reflection.Extensions.xml",
                        "ref/netcore50/de/System.Reflection.Extensions.xml",
                        "ref/netcore50/es/System.Reflection.Extensions.xml",
                        "ref/netcore50/fr/System.Reflection.Extensions.xml",
                        "ref/netcore50/it/System.Reflection.Extensions.xml",
                        "ref/netcore50/ja/System.Reflection.Extensions.xml",
                        "ref/netcore50/ko/System.Reflection.Extensions.xml",
                        "ref/netcore50/ru/System.Reflection.Extensions.xml",
                        "ref/netcore50/zh-hans/System.Reflection.Extensions.xml",
                        "ref/netcore50/zh-hant/System.Reflection.Extensions.xml",
                        "ref/netstandard1.0/System.Reflection.Extensions.dll",
                        "ref/netstandard1.0/System.Reflection.Extensions.xml",
                        "ref/netstandard1.0/de/System.Reflection.Extensions.xml",
                        "ref/netstandard1.0/es/System.Reflection.Extensions.xml",
                        "ref/netstandard1.0/fr/System.Reflection.Extensions.xml",
                        "ref/netstandard1.0/it/System.Reflection.Extensions.xml",
                        "ref/netstandard1.0/ja/System.Reflection.Extensions.xml",
                        "ref/netstandard1.0/ko/System.Reflection.Extensions.xml",
                        "ref/netstandard1.0/ru/System.Reflection.Extensions.xml",
                        "ref/netstandard1.0/zh-hans/System.Reflection.Extensions.xml",
                        "ref/netstandard1.0/zh-hant/System.Reflection.Extensions.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.reflection.extensions.4.3.0.nupkg.sha512",
                        "system.reflection.extensions.nuspec"
                      ]
                    },
                    "System.Reflection.Primitives/4.3.0": {
                      "sha512": "5RXItQz5As4xN2/YUDxdpsEkMhvw3e6aNveFXUn4Hl/udNTCNhnKp8lT9fnc3MhvGKh1baak5CovpuQUXHAlIA==",
                      "type": "package",
                      "path": "system.reflection.primitives/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/netcore50/System.Reflection.Primitives.dll",
                        "ref/netcore50/System.Reflection.Primitives.xml",
                        "ref/netcore50/de/System.Reflection.Primitives.xml",
                        "ref/netcore50/es/System.Reflection.Primitives.xml",
                        "ref/netcore50/fr/System.Reflection.Primitives.xml",
                        "ref/netcore50/it/System.Reflection.Primitives.xml",
                        "ref/netcore50/ja/System.Reflection.Primitives.xml",
                        "ref/netcore50/ko/System.Reflection.Primitives.xml",
                        "ref/netcore50/ru/System.Reflection.Primitives.xml",
                        "ref/netcore50/zh-hans/System.Reflection.Primitives.xml",
                        "ref/netcore50/zh-hant/System.Reflection.Primitives.xml",
                        "ref/netstandard1.0/System.Reflection.Primitives.dll",
                        "ref/netstandard1.0/System.Reflection.Primitives.xml",
                        "ref/netstandard1.0/de/System.Reflection.Primitives.xml",
                        "ref/netstandard1.0/es/System.Reflection.Primitives.xml",
                        "ref/netstandard1.0/fr/System.Reflection.Primitives.xml",
                        "ref/netstandard1.0/it/System.Reflection.Primitives.xml",
                        "ref/netstandard1.0/ja/System.Reflection.Primitives.xml",
                        "ref/netstandard1.0/ko/System.Reflection.Primitives.xml",
                        "ref/netstandard1.0/ru/System.Reflection.Primitives.xml",
                        "ref/netstandard1.0/zh-hans/System.Reflection.Primitives.xml",
                        "ref/netstandard1.0/zh-hant/System.Reflection.Primitives.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.reflection.primitives.4.3.0.nupkg.sha512",
                        "system.reflection.primitives.nuspec"
                      ]
                    },
                    "System.Reflection.TypeExtensions/4.3.0": {
                      "sha512": "7u6ulLcZbyxB5Gq0nMkQttcdBTx57ibzw+4IOXEfR+sXYQoHvjW5LTLyNr8O22UIMrqYbchJQJnos4eooYzYJA==",
                      "type": "package",
                      "path": "system.reflection.typeextensions/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net46/System.Reflection.TypeExtensions.dll",
                        "lib/net462/System.Reflection.TypeExtensions.dll",
                        "lib/netcore50/System.Reflection.TypeExtensions.dll",
                        "lib/netstandard1.5/System.Reflection.TypeExtensions.dll",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net46/System.Reflection.TypeExtensions.dll",
                        "ref/net462/System.Reflection.TypeExtensions.dll",
                        "ref/netstandard1.3/System.Reflection.TypeExtensions.dll",
                        "ref/netstandard1.3/System.Reflection.TypeExtensions.xml",
                        "ref/netstandard1.3/de/System.Reflection.TypeExtensions.xml",
                        "ref/netstandard1.3/es/System.Reflection.TypeExtensions.xml",
                        "ref/netstandard1.3/fr/System.Reflection.TypeExtensions.xml",
                        "ref/netstandard1.3/it/System.Reflection.TypeExtensions.xml",
                        "ref/netstandard1.3/ja/System.Reflection.TypeExtensions.xml",
                        "ref/netstandard1.3/ko/System.Reflection.TypeExtensions.xml",
                        "ref/netstandard1.3/ru/System.Reflection.TypeExtensions.xml",
                        "ref/netstandard1.3/zh-hans/System.Reflection.TypeExtensions.xml",
                        "ref/netstandard1.3/zh-hant/System.Reflection.TypeExtensions.xml",
                        "ref/netstandard1.5/System.Reflection.TypeExtensions.dll",
                        "ref/netstandard1.5/System.Reflection.TypeExtensions.xml",
                        "ref/netstandard1.5/de/System.Reflection.TypeExtensions.xml",
                        "ref/netstandard1.5/es/System.Reflection.TypeExtensions.xml",
                        "ref/netstandard1.5/fr/System.Reflection.TypeExtensions.xml",
                        "ref/netstandard1.5/it/System.Reflection.TypeExtensions.xml",
                        "ref/netstandard1.5/ja/System.Reflection.TypeExtensions.xml",
                        "ref/netstandard1.5/ko/System.Reflection.TypeExtensions.xml",
                        "ref/netstandard1.5/ru/System.Reflection.TypeExtensions.xml",
                        "ref/netstandard1.5/zh-hans/System.Reflection.TypeExtensions.xml",
                        "ref/netstandard1.5/zh-hant/System.Reflection.TypeExtensions.xml",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "runtimes/aot/lib/netcore50/System.Reflection.TypeExtensions.dll",
                        "system.reflection.typeextensions.4.3.0.nupkg.sha512",
                        "system.reflection.typeextensions.nuspec"
                      ]
                    },
                    "System.Resources.ResourceManager/4.3.0": {
                      "sha512": "/zrcPkkWdZmI4F92gL/TPumP98AVDu/Wxr3CSJGQQ+XN6wbRZcyfSKVoPo17ilb3iOr0cCRqJInGwNMolqhS8A==",
                      "type": "package",
                      "path": "system.resources.resourcemanager/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/netcore50/System.Resources.ResourceManager.dll",
                        "ref/netcore50/System.Resources.ResourceManager.xml",
                        "ref/netcore50/de/System.Resources.ResourceManager.xml",
                        "ref/netcore50/es/System.Resources.ResourceManager.xml",
                        "ref/netcore50/fr/System.Resources.ResourceManager.xml",
                        "ref/netcore50/it/System.Resources.ResourceManager.xml",
                        "ref/netcore50/ja/System.Resources.ResourceManager.xml",
                        "ref/netcore50/ko/System.Resources.ResourceManager.xml",
                        "ref/netcore50/ru/System.Resources.ResourceManager.xml",
                        "ref/netcore50/zh-hans/System.Resources.ResourceManager.xml",
                        "ref/netcore50/zh-hant/System.Resources.ResourceManager.xml",
                        "ref/netstandard1.0/System.Resources.ResourceManager.dll",
                        "ref/netstandard1.0/System.Resources.ResourceManager.xml",
                        "ref/netstandard1.0/de/System.Resources.ResourceManager.xml",
                        "ref/netstandard1.0/es/System.Resources.ResourceManager.xml",
                        "ref/netstandard1.0/fr/System.Resources.ResourceManager.xml",
                        "ref/netstandard1.0/it/System.Resources.ResourceManager.xml",
                        "ref/netstandard1.0/ja/System.Resources.ResourceManager.xml",
                        "ref/netstandard1.0/ko/System.Resources.ResourceManager.xml",
                        "ref/netstandard1.0/ru/System.Resources.ResourceManager.xml",
                        "ref/netstandard1.0/zh-hans/System.Resources.ResourceManager.xml",
                        "ref/netstandard1.0/zh-hant/System.Resources.ResourceManager.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.resources.resourcemanager.4.3.0.nupkg.sha512",
                        "system.resources.resourcemanager.nuspec"
                      ]
                    },
                    "System.Runtime/4.3.0": {
                      "sha512": "JufQi0vPQ0xGnAczR13AUFglDyVYt4Kqnz1AZaiKZ5+GICq0/1MH/mO/eAJHt/mHW1zjKBJd7kV26SrxddAhiw==",
                      "type": "package",
                      "path": "system.runtime/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/net462/System.Runtime.dll",
                        "lib/portable-net45+win8+wp80+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/net462/System.Runtime.dll",
                        "ref/netcore50/System.Runtime.dll",
                        "ref/netcore50/System.Runtime.xml",
                        "ref/netcore50/de/System.Runtime.xml",
                        "ref/netcore50/es/System.Runtime.xml",
                        "ref/netcore50/fr/System.Runtime.xml",
                        "ref/netcore50/it/System.Runtime.xml",
                        "ref/netcore50/ja/System.Runtime.xml",
                        "ref/netcore50/ko/System.Runtime.xml",
                        "ref/netcore50/ru/System.Runtime.xml",
                        "ref/netcore50/zh-hans/System.Runtime.xml",
                        "ref/netcore50/zh-hant/System.Runtime.xml",
                        "ref/netstandard1.0/System.Runtime.dll",
                        "ref/netstandard1.0/System.Runtime.xml",
                        "ref/netstandard1.0/de/System.Runtime.xml",
                        "ref/netstandard1.0/es/System.Runtime.xml",
                        "ref/netstandard1.0/fr/System.Runtime.xml",
                        "ref/netstandard1.0/it/System.Runtime.xml",
                        "ref/netstandard1.0/ja/System.Runtime.xml",
                        "ref/netstandard1.0/ko/System.Runtime.xml",
                        "ref/netstandard1.0/ru/System.Runtime.xml",
                        "ref/netstandard1.0/zh-hans/System.Runtime.xml",
                        "ref/netstandard1.0/zh-hant/System.Runtime.xml",
                        "ref/netstandard1.2/System.Runtime.dll",
                        "ref/netstandard1.2/System.Runtime.xml",
                        "ref/netstandard1.2/de/System.Runtime.xml",
                        "ref/netstandard1.2/es/System.Runtime.xml",
                        "ref/netstandard1.2/fr/System.Runtime.xml",
                        "ref/netstandard1.2/it/System.Runtime.xml",
                        "ref/netstandard1.2/ja/System.Runtime.xml",
                        "ref/netstandard1.2/ko/System.Runtime.xml",
                        "ref/netstandard1.2/ru/System.Runtime.xml",
                        "ref/netstandard1.2/zh-hans/System.Runtime.xml",
                        "ref/netstandard1.2/zh-hant/System.Runtime.xml",
                        "ref/netstandard1.3/System.Runtime.dll",
                        "ref/netstandard1.3/System.Runtime.xml",
                        "ref/netstandard1.3/de/System.Runtime.xml",
                        "ref/netstandard1.3/es/System.Runtime.xml",
                        "ref/netstandard1.3/fr/System.Runtime.xml",
                        "ref/netstandard1.3/it/System.Runtime.xml",
                        "ref/netstandard1.3/ja/System.Runtime.xml",
                        "ref/netstandard1.3/ko/System.Runtime.xml",
                        "ref/netstandard1.3/ru/System.Runtime.xml",
                        "ref/netstandard1.3/zh-hans/System.Runtime.xml",
                        "ref/netstandard1.3/zh-hant/System.Runtime.xml",
                        "ref/netstandard1.5/System.Runtime.dll",
                        "ref/netstandard1.5/System.Runtime.xml",
                        "ref/netstandard1.5/de/System.Runtime.xml",
                        "ref/netstandard1.5/es/System.Runtime.xml",
                        "ref/netstandard1.5/fr/System.Runtime.xml",
                        "ref/netstandard1.5/it/System.Runtime.xml",
                        "ref/netstandard1.5/ja/System.Runtime.xml",
                        "ref/netstandard1.5/ko/System.Runtime.xml",
                        "ref/netstandard1.5/ru/System.Runtime.xml",
                        "ref/netstandard1.5/zh-hans/System.Runtime.xml",
                        "ref/netstandard1.5/zh-hant/System.Runtime.xml",
                        "ref/portable-net45+win8+wp80+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.runtime.4.3.0.nupkg.sha512",
                        "system.runtime.nuspec"
                      ]
                    },
                    "System.Runtime.CompilerServices.Unsafe/4.3.0": {
                      "sha512": "rcnXA1U9W3QUtMSGoyoNHH6w4V5Rxa/EKXmzpORUYlDAlDB34hIQoU57ATXl8xHa83VvzRm6PcElEizgUd7U5w==",
                      "type": "package",
                      "path": "system.runtime.compilerservices.unsafe/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/netstandard1.0/System.Runtime.CompilerServices.Unsafe.dll",
                        "lib/netstandard1.0/System.Runtime.CompilerServices.Unsafe.xml",
                        "system.runtime.compilerservices.unsafe.4.3.0.nupkg.sha512",
                        "system.runtime.compilerservices.unsafe.nuspec"
                      ]
                    },
                    "System.Runtime.Extensions/4.3.0": {
                      "sha512": "guW0uK0fn5fcJJ1tJVXYd7/1h5F+pea1r7FLSOz/f8vPEqbR2ZAknuRDvTQ8PzAilDveOxNjSfr0CHfIQfFk8g==",
                      "type": "package",
                      "path": "system.runtime.extensions/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/net462/System.Runtime.Extensions.dll",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/net462/System.Runtime.Extensions.dll",
                        "ref/netcore50/System.Runtime.Extensions.dll",
                        "ref/netcore50/System.Runtime.Extensions.xml",
                        "ref/netcore50/de/System.Runtime.Extensions.xml",
                        "ref/netcore50/es/System.Runtime.Extensions.xml",
                        "ref/netcore50/fr/System.Runtime.Extensions.xml",
                        "ref/netcore50/it/System.Runtime.Extensions.xml",
                        "ref/netcore50/ja/System.Runtime.Extensions.xml",
                        "ref/netcore50/ko/System.Runtime.Extensions.xml",
                        "ref/netcore50/ru/System.Runtime.Extensions.xml",
                        "ref/netcore50/zh-hans/System.Runtime.Extensions.xml",
                        "ref/netcore50/zh-hant/System.Runtime.Extensions.xml",
                        "ref/netstandard1.0/System.Runtime.Extensions.dll",
                        "ref/netstandard1.0/System.Runtime.Extensions.xml",
                        "ref/netstandard1.0/de/System.Runtime.Extensions.xml",
                        "ref/netstandard1.0/es/System.Runtime.Extensions.xml",
                        "ref/netstandard1.0/fr/System.Runtime.Extensions.xml",
                        "ref/netstandard1.0/it/System.Runtime.Extensions.xml",
                        "ref/netstandard1.0/ja/System.Runtime.Extensions.xml",
                        "ref/netstandard1.0/ko/System.Runtime.Extensions.xml",
                        "ref/netstandard1.0/ru/System.Runtime.Extensions.xml",
                        "ref/netstandard1.0/zh-hans/System.Runtime.Extensions.xml",
                        "ref/netstandard1.0/zh-hant/System.Runtime.Extensions.xml",
                        "ref/netstandard1.3/System.Runtime.Extensions.dll",
                        "ref/netstandard1.3/System.Runtime.Extensions.xml",
                        "ref/netstandard1.3/de/System.Runtime.Extensions.xml",
                        "ref/netstandard1.3/es/System.Runtime.Extensions.xml",
                        "ref/netstandard1.3/fr/System.Runtime.Extensions.xml",
                        "ref/netstandard1.3/it/System.Runtime.Extensions.xml",
                        "ref/netstandard1.3/ja/System.Runtime.Extensions.xml",
                        "ref/netstandard1.3/ko/System.Runtime.Extensions.xml",
                        "ref/netstandard1.3/ru/System.Runtime.Extensions.xml",
                        "ref/netstandard1.3/zh-hans/System.Runtime.Extensions.xml",
                        "ref/netstandard1.3/zh-hant/System.Runtime.Extensions.xml",
                        "ref/netstandard1.5/System.Runtime.Extensions.dll",
                        "ref/netstandard1.5/System.Runtime.Extensions.xml",
                        "ref/netstandard1.5/de/System.Runtime.Extensions.xml",
                        "ref/netstandard1.5/es/System.Runtime.Extensions.xml",
                        "ref/netstandard1.5/fr/System.Runtime.Extensions.xml",
                        "ref/netstandard1.5/it/System.Runtime.Extensions.xml",
                        "ref/netstandard1.5/ja/System.Runtime.Extensions.xml",
                        "ref/netstandard1.5/ko/System.Runtime.Extensions.xml",
                        "ref/netstandard1.5/ru/System.Runtime.Extensions.xml",
                        "ref/netstandard1.5/zh-hans/System.Runtime.Extensions.xml",
                        "ref/netstandard1.5/zh-hant/System.Runtime.Extensions.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.runtime.extensions.4.3.0.nupkg.sha512",
                        "system.runtime.extensions.nuspec"
                      ]
                    },
                    "System.Runtime.Handles/4.3.0": {
                      "sha512": "OKiSUN7DmTWeYb3l51A7EYaeNMnvxwE249YtZz7yooT4gOZhmTjIn48KgSsw2k2lYdLgTKNJw/ZIfSElwDRVgg==",
                      "type": "package",
                      "path": "system.runtime.handles/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net46/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net46/_._",
                        "ref/netstandard1.3/System.Runtime.Handles.dll",
                        "ref/netstandard1.3/System.Runtime.Handles.xml",
                        "ref/netstandard1.3/de/System.Runtime.Handles.xml",
                        "ref/netstandard1.3/es/System.Runtime.Handles.xml",
                        "ref/netstandard1.3/fr/System.Runtime.Handles.xml",
                        "ref/netstandard1.3/it/System.Runtime.Handles.xml",
                        "ref/netstandard1.3/ja/System.Runtime.Handles.xml",
                        "ref/netstandard1.3/ko/System.Runtime.Handles.xml",
                        "ref/netstandard1.3/ru/System.Runtime.Handles.xml",
                        "ref/netstandard1.3/zh-hans/System.Runtime.Handles.xml",
                        "ref/netstandard1.3/zh-hant/System.Runtime.Handles.xml",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.runtime.handles.4.3.0.nupkg.sha512",
                        "system.runtime.handles.nuspec"
                      ]
                    },
                    "System.Runtime.InteropServices/4.3.0": {
                      "sha512": "uv1ynXqiMK8mp1GM3jDqPCFN66eJ5w5XNomaK2XD+TuCroNTLFGeZ+WCmBMcBDyTFKou3P6cR6J/QsaqDp7fGQ==",
                      "type": "package",
                      "path": "system.runtime.interopservices/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/net462/System.Runtime.InteropServices.dll",
                        "lib/net463/System.Runtime.InteropServices.dll",
                        "lib/portable-net45+win8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/net462/System.Runtime.InteropServices.dll",
                        "ref/net463/System.Runtime.InteropServices.dll",
                        "ref/netcore50/System.Runtime.InteropServices.dll",
                        "ref/netcore50/System.Runtime.InteropServices.xml",
                        "ref/netcore50/de/System.Runtime.InteropServices.xml",
                        "ref/netcore50/es/System.Runtime.InteropServices.xml",
                        "ref/netcore50/fr/System.Runtime.InteropServices.xml",
                        "ref/netcore50/it/System.Runtime.InteropServices.xml",
                        "ref/netcore50/ja/System.Runtime.InteropServices.xml",
                        "ref/netcore50/ko/System.Runtime.InteropServices.xml",
                        "ref/netcore50/ru/System.Runtime.InteropServices.xml",
                        "ref/netcore50/zh-hans/System.Runtime.InteropServices.xml",
                        "ref/netcore50/zh-hant/System.Runtime.InteropServices.xml",
                        "ref/netcoreapp1.1/System.Runtime.InteropServices.dll",
                        "ref/netstandard1.1/System.Runtime.InteropServices.dll",
                        "ref/netstandard1.1/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.1/de/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.1/es/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.1/fr/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.1/it/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.1/ja/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.1/ko/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.1/ru/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.1/zh-hans/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.1/zh-hant/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.2/System.Runtime.InteropServices.dll",
                        "ref/netstandard1.2/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.2/de/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.2/es/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.2/fr/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.2/it/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.2/ja/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.2/ko/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.2/ru/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.2/zh-hans/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.2/zh-hant/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.3/System.Runtime.InteropServices.dll",
                        "ref/netstandard1.3/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.3/de/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.3/es/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.3/fr/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.3/it/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.3/ja/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.3/ko/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.3/ru/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.3/zh-hans/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.3/zh-hant/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.5/System.Runtime.InteropServices.dll",
                        "ref/netstandard1.5/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.5/de/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.5/es/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.5/fr/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.5/it/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.5/ja/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.5/ko/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.5/ru/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.5/zh-hans/System.Runtime.InteropServices.xml",
                        "ref/netstandard1.5/zh-hant/System.Runtime.InteropServices.xml",
                        "ref/portable-net45+win8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.runtime.interopservices.4.3.0.nupkg.sha512",
                        "system.runtime.interopservices.nuspec"
                      ]
                    },
                    "System.Runtime.InteropServices.RuntimeInformation/4.3.0": {
                      "sha512": "cbz4YJMqRDR7oLeMRbdYv7mYzc++17lNhScCX0goO2XpGWdvAt60CGN+FHdePUEHCe/Jy9jUlvNAiNdM+7jsOw==",
                      "type": "package",
                      "path": "system.runtime.interopservices.runtimeinformation/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/System.Runtime.InteropServices.RuntimeInformation.dll",
                        "lib/netstandard1.1/System.Runtime.InteropServices.RuntimeInformation.dll",
                        "lib/win8/System.Runtime.InteropServices.RuntimeInformation.dll",
                        "lib/wpa81/System.Runtime.InteropServices.RuntimeInformation.dll",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/netstandard1.1/System.Runtime.InteropServices.RuntimeInformation.dll",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "runtimes/aot/lib/netcore50/System.Runtime.InteropServices.RuntimeInformation.dll",
                        "runtimes/unix/lib/netstandard1.1/System.Runtime.InteropServices.RuntimeInformation.dll",
                        "runtimes/win/lib/net45/System.Runtime.InteropServices.RuntimeInformation.dll",
                        "runtimes/win/lib/netcore50/System.Runtime.InteropServices.RuntimeInformation.dll",
                        "runtimes/win/lib/netstandard1.1/System.Runtime.InteropServices.RuntimeInformation.dll",
                        "system.runtime.interopservices.runtimeinformation.4.3.0.nupkg.sha512",
                        "system.runtime.interopservices.runtimeinformation.nuspec"
                      ]
                    },
                    "System.Runtime.Numerics/4.3.0": {
                      "sha512": "yMH+MfdzHjy17l2KESnPiF2dwq7T+xLnSJar7slyimAkUh/gTrS9/UQOtv7xarskJ2/XDSNvfLGOBQPjL7PaHQ==",
                      "type": "package",
                      "path": "system.runtime.numerics/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/netcore50/System.Runtime.Numerics.dll",
                        "lib/netstandard1.3/System.Runtime.Numerics.dll",
                        "lib/portable-net45+win8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/netcore50/System.Runtime.Numerics.dll",
                        "ref/netcore50/System.Runtime.Numerics.xml",
                        "ref/netcore50/de/System.Runtime.Numerics.xml",
                        "ref/netcore50/es/System.Runtime.Numerics.xml",
                        "ref/netcore50/fr/System.Runtime.Numerics.xml",
                        "ref/netcore50/it/System.Runtime.Numerics.xml",
                        "ref/netcore50/ja/System.Runtime.Numerics.xml",
                        "ref/netcore50/ko/System.Runtime.Numerics.xml",
                        "ref/netcore50/ru/System.Runtime.Numerics.xml",
                        "ref/netcore50/zh-hans/System.Runtime.Numerics.xml",
                        "ref/netcore50/zh-hant/System.Runtime.Numerics.xml",
                        "ref/netstandard1.1/System.Runtime.Numerics.dll",
                        "ref/netstandard1.1/System.Runtime.Numerics.xml",
                        "ref/netstandard1.1/de/System.Runtime.Numerics.xml",
                        "ref/netstandard1.1/es/System.Runtime.Numerics.xml",
                        "ref/netstandard1.1/fr/System.Runtime.Numerics.xml",
                        "ref/netstandard1.1/it/System.Runtime.Numerics.xml",
                        "ref/netstandard1.1/ja/System.Runtime.Numerics.xml",
                        "ref/netstandard1.1/ko/System.Runtime.Numerics.xml",
                        "ref/netstandard1.1/ru/System.Runtime.Numerics.xml",
                        "ref/netstandard1.1/zh-hans/System.Runtime.Numerics.xml",
                        "ref/netstandard1.1/zh-hant/System.Runtime.Numerics.xml",
                        "ref/portable-net45+win8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.runtime.numerics.4.3.0.nupkg.sha512",
                        "system.runtime.numerics.nuspec"
                      ]
                    },
                    "System.Security.Cryptography.Algorithms/4.3.0": {
                      "sha512": "W1kd2Y8mYSCgc3ULTAZ0hOP2dSdG5YauTb1089T0/kRcN2MpSAW1izOFROrJgxSlMn3ArsgHXagigyi+ibhevg==",
                      "type": "package",
                      "path": "system.security.cryptography.algorithms/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net46/System.Security.Cryptography.Algorithms.dll",
                        "lib/net461/System.Security.Cryptography.Algorithms.dll",
                        "lib/net463/System.Security.Cryptography.Algorithms.dll",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net46/System.Security.Cryptography.Algorithms.dll",
                        "ref/net461/System.Security.Cryptography.Algorithms.dll",
                        "ref/net463/System.Security.Cryptography.Algorithms.dll",
                        "ref/netstandard1.3/System.Security.Cryptography.Algorithms.dll",
                        "ref/netstandard1.4/System.Security.Cryptography.Algorithms.dll",
                        "ref/netstandard1.6/System.Security.Cryptography.Algorithms.dll",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "runtimes/osx/lib/netstandard1.6/System.Security.Cryptography.Algorithms.dll",
                        "runtimes/unix/lib/netstandard1.6/System.Security.Cryptography.Algorithms.dll",
                        "runtimes/win/lib/net46/System.Security.Cryptography.Algorithms.dll",
                        "runtimes/win/lib/net461/System.Security.Cryptography.Algorithms.dll",
                        "runtimes/win/lib/net463/System.Security.Cryptography.Algorithms.dll",
                        "runtimes/win/lib/netcore50/System.Security.Cryptography.Algorithms.dll",
                        "runtimes/win/lib/netstandard1.6/System.Security.Cryptography.Algorithms.dll",
                        "system.security.cryptography.algorithms.4.3.0.nupkg.sha512",
                        "system.security.cryptography.algorithms.nuspec"
                      ]
                    },
                    "System.Security.Cryptography.Cng/4.3.0": {
                      "sha512": "03idZOqFlsKRL4W+LuCpJ6dBYDUWReug6lZjBa3uJWnk5sPCUXckocevTaUA8iT/MFSrY/2HXkOt753xQ/cf8g==",
                      "type": "package",
                      "path": "system.security.cryptography.cng/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/net46/System.Security.Cryptography.Cng.dll",
                        "lib/net461/System.Security.Cryptography.Cng.dll",
                        "lib/net463/System.Security.Cryptography.Cng.dll",
                        "ref/net46/System.Security.Cryptography.Cng.dll",
                        "ref/net461/System.Security.Cryptography.Cng.dll",
                        "ref/net463/System.Security.Cryptography.Cng.dll",
                        "ref/netstandard1.3/System.Security.Cryptography.Cng.dll",
                        "ref/netstandard1.4/System.Security.Cryptography.Cng.dll",
                        "ref/netstandard1.6/System.Security.Cryptography.Cng.dll",
                        "runtimes/unix/lib/netstandard1.6/System.Security.Cryptography.Cng.dll",
                        "runtimes/win/lib/net46/System.Security.Cryptography.Cng.dll",
                        "runtimes/win/lib/net461/System.Security.Cryptography.Cng.dll",
                        "runtimes/win/lib/net463/System.Security.Cryptography.Cng.dll",
                        "runtimes/win/lib/netstandard1.4/System.Security.Cryptography.Cng.dll",
                        "runtimes/win/lib/netstandard1.6/System.Security.Cryptography.Cng.dll",
                        "system.security.cryptography.cng.4.3.0.nupkg.sha512",
                        "system.security.cryptography.cng.nuspec"
                      ]
                    },
                    "System.Security.Cryptography.Csp/4.3.0": {
                      "sha512": "X4s/FCkEUnRGnwR3aSfVIkldBmtURMhmexALNTwpjklzxWU7yjMk7GHLKOZTNkgnWnE0q7+BCf9N2LVRWxewaA==",
                      "type": "package",
                      "path": "system.security.cryptography.csp/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net46/System.Security.Cryptography.Csp.dll",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net46/System.Security.Cryptography.Csp.dll",
                        "ref/netstandard1.3/System.Security.Cryptography.Csp.dll",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "runtimes/unix/lib/netstandard1.3/System.Security.Cryptography.Csp.dll",
                        "runtimes/win/lib/net46/System.Security.Cryptography.Csp.dll",
                        "runtimes/win/lib/netcore50/_._",
                        "runtimes/win/lib/netstandard1.3/System.Security.Cryptography.Csp.dll",
                        "system.security.cryptography.csp.4.3.0.nupkg.sha512",
                        "system.security.cryptography.csp.nuspec"
                      ]
                    },
                    "System.Security.Cryptography.Encoding/4.3.0": {
                      "sha512": "1DEWjZZly9ae9C79vFwqaO5kaOlI5q+3/55ohmq/7dpDyDfc8lYe7YVxJUZ5MF/NtbkRjwFRo14yM4OEo9EmDw==",
                      "type": "package",
                      "path": "system.security.cryptography.encoding/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net46/System.Security.Cryptography.Encoding.dll",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net46/System.Security.Cryptography.Encoding.dll",
                        "ref/netstandard1.3/System.Security.Cryptography.Encoding.dll",
                        "ref/netstandard1.3/System.Security.Cryptography.Encoding.xml",
                        "ref/netstandard1.3/de/System.Security.Cryptography.Encoding.xml",
                        "ref/netstandard1.3/es/System.Security.Cryptography.Encoding.xml",
                        "ref/netstandard1.3/fr/System.Security.Cryptography.Encoding.xml",
                        "ref/netstandard1.3/it/System.Security.Cryptography.Encoding.xml",
                        "ref/netstandard1.3/ja/System.Security.Cryptography.Encoding.xml",
                        "ref/netstandard1.3/ko/System.Security.Cryptography.Encoding.xml",
                        "ref/netstandard1.3/ru/System.Security.Cryptography.Encoding.xml",
                        "ref/netstandard1.3/zh-hans/System.Security.Cryptography.Encoding.xml",
                        "ref/netstandard1.3/zh-hant/System.Security.Cryptography.Encoding.xml",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "runtimes/unix/lib/netstandard1.3/System.Security.Cryptography.Encoding.dll",
                        "runtimes/win/lib/net46/System.Security.Cryptography.Encoding.dll",
                        "runtimes/win/lib/netstandard1.3/System.Security.Cryptography.Encoding.dll",
                        "system.security.cryptography.encoding.4.3.0.nupkg.sha512",
                        "system.security.cryptography.encoding.nuspec"
                      ]
                    },
                    "System.Security.Cryptography.OpenSsl/4.3.0": {
                      "sha512": "h4CEgOgv5PKVF/HwaHzJRiVboL2THYCou97zpmhjghx5frc7fIvlkY1jL+lnIQyChrJDMNEXS6r7byGif8Cy4w==",
                      "type": "package",
                      "path": "system.security.cryptography.openssl/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/netstandard1.6/System.Security.Cryptography.OpenSsl.dll",
                        "ref/netstandard1.6/System.Security.Cryptography.OpenSsl.dll",
                        "runtimes/unix/lib/netstandard1.6/System.Security.Cryptography.OpenSsl.dll",
                        "system.security.cryptography.openssl.4.3.0.nupkg.sha512",
                        "system.security.cryptography.openssl.nuspec"
                      ]
                    },
                    "System.Security.Cryptography.Primitives/4.3.0": {
                      "sha512": "7bDIyVFNL/xKeFHjhobUAQqSpJq9YTOpbEs6mR233Et01STBMXNAc/V+BM6dwYGc95gVh/Zf+iVXWzj3mE8DWg==",
                      "type": "package",
                      "path": "system.security.cryptography.primitives/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net46/System.Security.Cryptography.Primitives.dll",
                        "lib/netstandard1.3/System.Security.Cryptography.Primitives.dll",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net46/System.Security.Cryptography.Primitives.dll",
                        "ref/netstandard1.3/System.Security.Cryptography.Primitives.dll",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.security.cryptography.primitives.4.3.0.nupkg.sha512",
                        "system.security.cryptography.primitives.nuspec"
                      ]
                    },
                    "System.Security.Cryptography.X509Certificates/4.3.0": {
                      "sha512": "t2Tmu6Y2NtJ2um0RtcuhP7ZdNNxXEgUm2JeoA/0NvlMjAhKCnM1NX07TDl3244mVp3QU6LPEhT3HTtH1uF7IYw==",
                      "type": "package",
                      "path": "system.security.cryptography.x509certificates/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net46/System.Security.Cryptography.X509Certificates.dll",
                        "lib/net461/System.Security.Cryptography.X509Certificates.dll",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net46/System.Security.Cryptography.X509Certificates.dll",
                        "ref/net461/System.Security.Cryptography.X509Certificates.dll",
                        "ref/netstandard1.3/System.Security.Cryptography.X509Certificates.dll",
                        "ref/netstandard1.3/System.Security.Cryptography.X509Certificates.xml",
                        "ref/netstandard1.3/de/System.Security.Cryptography.X509Certificates.xml",
                        "ref/netstandard1.3/es/System.Security.Cryptography.X509Certificates.xml",
                        "ref/netstandard1.3/fr/System.Security.Cryptography.X509Certificates.xml",
                        "ref/netstandard1.3/it/System.Security.Cryptography.X509Certificates.xml",
                        "ref/netstandard1.3/ja/System.Security.Cryptography.X509Certificates.xml",
                        "ref/netstandard1.3/ko/System.Security.Cryptography.X509Certificates.xml",
                        "ref/netstandard1.3/ru/System.Security.Cryptography.X509Certificates.xml",
                        "ref/netstandard1.3/zh-hans/System.Security.Cryptography.X509Certificates.xml",
                        "ref/netstandard1.3/zh-hant/System.Security.Cryptography.X509Certificates.xml",
                        "ref/netstandard1.4/System.Security.Cryptography.X509Certificates.dll",
                        "ref/netstandard1.4/System.Security.Cryptography.X509Certificates.xml",
                        "ref/netstandard1.4/de/System.Security.Cryptography.X509Certificates.xml",
                        "ref/netstandard1.4/es/System.Security.Cryptography.X509Certificates.xml",
                        "ref/netstandard1.4/fr/System.Security.Cryptography.X509Certificates.xml",
                        "ref/netstandard1.4/it/System.Security.Cryptography.X509Certificates.xml",
                        "ref/netstandard1.4/ja/System.Security.Cryptography.X509Certificates.xml",
                        "ref/netstandard1.4/ko/System.Security.Cryptography.X509Certificates.xml",
                        "ref/netstandard1.4/ru/System.Security.Cryptography.X509Certificates.xml",
                        "ref/netstandard1.4/zh-hans/System.Security.Cryptography.X509Certificates.xml",
                        "ref/netstandard1.4/zh-hant/System.Security.Cryptography.X509Certificates.xml",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "runtimes/unix/lib/netstandard1.6/System.Security.Cryptography.X509Certificates.dll",
                        "runtimes/win/lib/net46/System.Security.Cryptography.X509Certificates.dll",
                        "runtimes/win/lib/net461/System.Security.Cryptography.X509Certificates.dll",
                        "runtimes/win/lib/netcore50/System.Security.Cryptography.X509Certificates.dll",
                        "runtimes/win/lib/netstandard1.6/System.Security.Cryptography.X509Certificates.dll",
                        "system.security.cryptography.x509certificates.4.3.0.nupkg.sha512",
                        "system.security.cryptography.x509certificates.nuspec"
                      ]
                    },
                    "System.Text.Encoding/4.3.0": {
                      "sha512": "BiIg+KWaSDOITze6jGQynxg64naAPtqGHBwDrLaCtixsa5bKiR8dpPOHA7ge3C0JJQizJE+sfkz1wV+BAKAYZw==",
                      "type": "package",
                      "path": "system.text.encoding/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/netcore50/System.Text.Encoding.dll",
                        "ref/netcore50/System.Text.Encoding.xml",
                        "ref/netcore50/de/System.Text.Encoding.xml",
                        "ref/netcore50/es/System.Text.Encoding.xml",
                        "ref/netcore50/fr/System.Text.Encoding.xml",
                        "ref/netcore50/it/System.Text.Encoding.xml",
                        "ref/netcore50/ja/System.Text.Encoding.xml",
                        "ref/netcore50/ko/System.Text.Encoding.xml",
                        "ref/netcore50/ru/System.Text.Encoding.xml",
                        "ref/netcore50/zh-hans/System.Text.Encoding.xml",
                        "ref/netcore50/zh-hant/System.Text.Encoding.xml",
                        "ref/netstandard1.0/System.Text.Encoding.dll",
                        "ref/netstandard1.0/System.Text.Encoding.xml",
                        "ref/netstandard1.0/de/System.Text.Encoding.xml",
                        "ref/netstandard1.0/es/System.Text.Encoding.xml",
                        "ref/netstandard1.0/fr/System.Text.Encoding.xml",
                        "ref/netstandard1.0/it/System.Text.Encoding.xml",
                        "ref/netstandard1.0/ja/System.Text.Encoding.xml",
                        "ref/netstandard1.0/ko/System.Text.Encoding.xml",
                        "ref/netstandard1.0/ru/System.Text.Encoding.xml",
                        "ref/netstandard1.0/zh-hans/System.Text.Encoding.xml",
                        "ref/netstandard1.0/zh-hant/System.Text.Encoding.xml",
                        "ref/netstandard1.3/System.Text.Encoding.dll",
                        "ref/netstandard1.3/System.Text.Encoding.xml",
                        "ref/netstandard1.3/de/System.Text.Encoding.xml",
                        "ref/netstandard1.3/es/System.Text.Encoding.xml",
                        "ref/netstandard1.3/fr/System.Text.Encoding.xml",
                        "ref/netstandard1.3/it/System.Text.Encoding.xml",
                        "ref/netstandard1.3/ja/System.Text.Encoding.xml",
                        "ref/netstandard1.3/ko/System.Text.Encoding.xml",
                        "ref/netstandard1.3/ru/System.Text.Encoding.xml",
                        "ref/netstandard1.3/zh-hans/System.Text.Encoding.xml",
                        "ref/netstandard1.3/zh-hant/System.Text.Encoding.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.text.encoding.4.3.0.nupkg.sha512",
                        "system.text.encoding.nuspec"
                      ]
                    },
                    "System.Text.Encoding.Extensions/4.3.0": {
                      "sha512": "YVMK0Bt/A43RmwizJoZ22ei2nmrhobgeiYwFzC4YAN+nue8RF6djXDMog0UCn+brerQoYVyaS+ghy9P/MUVcmw==",
                      "type": "package",
                      "path": "system.text.encoding.extensions/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/netcore50/System.Text.Encoding.Extensions.dll",
                        "ref/netcore50/System.Text.Encoding.Extensions.xml",
                        "ref/netcore50/de/System.Text.Encoding.Extensions.xml",
                        "ref/netcore50/es/System.Text.Encoding.Extensions.xml",
                        "ref/netcore50/fr/System.Text.Encoding.Extensions.xml",
                        "ref/netcore50/it/System.Text.Encoding.Extensions.xml",
                        "ref/netcore50/ja/System.Text.Encoding.Extensions.xml",
                        "ref/netcore50/ko/System.Text.Encoding.Extensions.xml",
                        "ref/netcore50/ru/System.Text.Encoding.Extensions.xml",
                        "ref/netcore50/zh-hans/System.Text.Encoding.Extensions.xml",
                        "ref/netcore50/zh-hant/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.0/System.Text.Encoding.Extensions.dll",
                        "ref/netstandard1.0/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.0/de/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.0/es/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.0/fr/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.0/it/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.0/ja/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.0/ko/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.0/ru/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.0/zh-hans/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.0/zh-hant/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.3/System.Text.Encoding.Extensions.dll",
                        "ref/netstandard1.3/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.3/de/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.3/es/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.3/fr/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.3/it/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.3/ja/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.3/ko/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.3/ru/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.3/zh-hans/System.Text.Encoding.Extensions.xml",
                        "ref/netstandard1.3/zh-hant/System.Text.Encoding.Extensions.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.text.encoding.extensions.4.3.0.nupkg.sha512",
                        "system.text.encoding.extensions.nuspec"
                      ]
                    },
                    "System.Text.RegularExpressions/4.3.0": {
                      "sha512": "RpT2DA+L660cBt1FssIE9CAGpLFdFPuheB7pLpKpn6ZXNby7jDERe8Ua/Ne2xGiwLVG2JOqziiaVCGDon5sKFA==",
                      "type": "package",
                      "path": "system.text.regularexpressions/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/net463/System.Text.RegularExpressions.dll",
                        "lib/netcore50/System.Text.RegularExpressions.dll",
                        "lib/netstandard1.6/System.Text.RegularExpressions.dll",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/net463/System.Text.RegularExpressions.dll",
                        "ref/netcore50/System.Text.RegularExpressions.dll",
                        "ref/netcore50/System.Text.RegularExpressions.xml",
                        "ref/netcore50/de/System.Text.RegularExpressions.xml",
                        "ref/netcore50/es/System.Text.RegularExpressions.xml",
                        "ref/netcore50/fr/System.Text.RegularExpressions.xml",
                        "ref/netcore50/it/System.Text.RegularExpressions.xml",
                        "ref/netcore50/ja/System.Text.RegularExpressions.xml",
                        "ref/netcore50/ko/System.Text.RegularExpressions.xml",
                        "ref/netcore50/ru/System.Text.RegularExpressions.xml",
                        "ref/netcore50/zh-hans/System.Text.RegularExpressions.xml",
                        "ref/netcore50/zh-hant/System.Text.RegularExpressions.xml",
                        "ref/netcoreapp1.1/System.Text.RegularExpressions.dll",
                        "ref/netstandard1.0/System.Text.RegularExpressions.dll",
                        "ref/netstandard1.0/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.0/de/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.0/es/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.0/fr/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.0/it/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.0/ja/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.0/ko/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.0/ru/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.0/zh-hans/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.0/zh-hant/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.3/System.Text.RegularExpressions.dll",
                        "ref/netstandard1.3/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.3/de/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.3/es/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.3/fr/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.3/it/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.3/ja/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.3/ko/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.3/ru/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.3/zh-hans/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.3/zh-hant/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.6/System.Text.RegularExpressions.dll",
                        "ref/netstandard1.6/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.6/de/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.6/es/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.6/fr/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.6/it/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.6/ja/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.6/ko/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.6/ru/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.6/zh-hans/System.Text.RegularExpressions.xml",
                        "ref/netstandard1.6/zh-hant/System.Text.RegularExpressions.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.text.regularexpressions.4.3.0.nupkg.sha512",
                        "system.text.regularexpressions.nuspec"
                      ]
                    },
                    "System.Threading/4.3.0": {
                      "sha512": "VkUS0kOBcUf3Wwm0TSbrevDDZ6BlM+b/HRiapRFWjM5O0NS0LviG0glKmFK+hhPDd1XFeSdU1GmlLhb2CoVpIw==",
                      "type": "package",
                      "path": "system.threading/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/netcore50/System.Threading.dll",
                        "lib/netstandard1.3/System.Threading.dll",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/netcore50/System.Threading.dll",
                        "ref/netcore50/System.Threading.xml",
                        "ref/netcore50/de/System.Threading.xml",
                        "ref/netcore50/es/System.Threading.xml",
                        "ref/netcore50/fr/System.Threading.xml",
                        "ref/netcore50/it/System.Threading.xml",
                        "ref/netcore50/ja/System.Threading.xml",
                        "ref/netcore50/ko/System.Threading.xml",
                        "ref/netcore50/ru/System.Threading.xml",
                        "ref/netcore50/zh-hans/System.Threading.xml",
                        "ref/netcore50/zh-hant/System.Threading.xml",
                        "ref/netstandard1.0/System.Threading.dll",
                        "ref/netstandard1.0/System.Threading.xml",
                        "ref/netstandard1.0/de/System.Threading.xml",
                        "ref/netstandard1.0/es/System.Threading.xml",
                        "ref/netstandard1.0/fr/System.Threading.xml",
                        "ref/netstandard1.0/it/System.Threading.xml",
                        "ref/netstandard1.0/ja/System.Threading.xml",
                        "ref/netstandard1.0/ko/System.Threading.xml",
                        "ref/netstandard1.0/ru/System.Threading.xml",
                        "ref/netstandard1.0/zh-hans/System.Threading.xml",
                        "ref/netstandard1.0/zh-hant/System.Threading.xml",
                        "ref/netstandard1.3/System.Threading.dll",
                        "ref/netstandard1.3/System.Threading.xml",
                        "ref/netstandard1.3/de/System.Threading.xml",
                        "ref/netstandard1.3/es/System.Threading.xml",
                        "ref/netstandard1.3/fr/System.Threading.xml",
                        "ref/netstandard1.3/it/System.Threading.xml",
                        "ref/netstandard1.3/ja/System.Threading.xml",
                        "ref/netstandard1.3/ko/System.Threading.xml",
                        "ref/netstandard1.3/ru/System.Threading.xml",
                        "ref/netstandard1.3/zh-hans/System.Threading.xml",
                        "ref/netstandard1.3/zh-hant/System.Threading.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "runtimes/aot/lib/netcore50/System.Threading.dll",
                        "system.threading.4.3.0.nupkg.sha512",
                        "system.threading.nuspec"
                      ]
                    },
                    "System.Threading.Tasks/4.3.0": {
                      "sha512": "LbSxKEdOUhVe8BezB/9uOGGppt+nZf6e1VFyw6v3DN6lqitm0OSn2uXMOdtP0M3W4iMcqcivm2J6UgqiwwnXiA==",
                      "type": "package",
                      "path": "system.threading.tasks/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/netcore50/System.Threading.Tasks.dll",
                        "ref/netcore50/System.Threading.Tasks.xml",
                        "ref/netcore50/de/System.Threading.Tasks.xml",
                        "ref/netcore50/es/System.Threading.Tasks.xml",
                        "ref/netcore50/fr/System.Threading.Tasks.xml",
                        "ref/netcore50/it/System.Threading.Tasks.xml",
                        "ref/netcore50/ja/System.Threading.Tasks.xml",
                        "ref/netcore50/ko/System.Threading.Tasks.xml",
                        "ref/netcore50/ru/System.Threading.Tasks.xml",
                        "ref/netcore50/zh-hans/System.Threading.Tasks.xml",
                        "ref/netcore50/zh-hant/System.Threading.Tasks.xml",
                        "ref/netstandard1.0/System.Threading.Tasks.dll",
                        "ref/netstandard1.0/System.Threading.Tasks.xml",
                        "ref/netstandard1.0/de/System.Threading.Tasks.xml",
                        "ref/netstandard1.0/es/System.Threading.Tasks.xml",
                        "ref/netstandard1.0/fr/System.Threading.Tasks.xml",
                        "ref/netstandard1.0/it/System.Threading.Tasks.xml",
                        "ref/netstandard1.0/ja/System.Threading.Tasks.xml",
                        "ref/netstandard1.0/ko/System.Threading.Tasks.xml",
                        "ref/netstandard1.0/ru/System.Threading.Tasks.xml",
                        "ref/netstandard1.0/zh-hans/System.Threading.Tasks.xml",
                        "ref/netstandard1.0/zh-hant/System.Threading.Tasks.xml",
                        "ref/netstandard1.3/System.Threading.Tasks.dll",
                        "ref/netstandard1.3/System.Threading.Tasks.xml",
                        "ref/netstandard1.3/de/System.Threading.Tasks.xml",
                        "ref/netstandard1.3/es/System.Threading.Tasks.xml",
                        "ref/netstandard1.3/fr/System.Threading.Tasks.xml",
                        "ref/netstandard1.3/it/System.Threading.Tasks.xml",
                        "ref/netstandard1.3/ja/System.Threading.Tasks.xml",
                        "ref/netstandard1.3/ko/System.Threading.Tasks.xml",
                        "ref/netstandard1.3/ru/System.Threading.Tasks.xml",
                        "ref/netstandard1.3/zh-hans/System.Threading.Tasks.xml",
                        "ref/netstandard1.3/zh-hant/System.Threading.Tasks.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.threading.tasks.4.3.0.nupkg.sha512",
                        "system.threading.tasks.nuspec"
                      ]
                    },
                    "System.Threading.Tasks.Extensions/4.3.0": {
                      "sha512": "npvJkVKl5rKXrtl1Kkm6OhOUaYGEiF9wFbppFRWSMoApKzt2PiPHT2Bb8a5sAWxprvdOAtvaARS9QYMznEUtug==",
                      "type": "package",
                      "path": "system.threading.tasks.extensions/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/netstandard1.0/System.Threading.Tasks.Extensions.dll",
                        "lib/netstandard1.0/System.Threading.Tasks.Extensions.xml",
                        "lib/portable-net45+win8+wp8+wpa81/System.Threading.Tasks.Extensions.dll",
                        "lib/portable-net45+win8+wp8+wpa81/System.Threading.Tasks.Extensions.xml",
                        "system.threading.tasks.extensions.4.3.0.nupkg.sha512",
                        "system.threading.tasks.extensions.nuspec"
                      ]
                    },
                    "System.Threading.Timer/4.3.0": {
                      "sha512": "Z6YfyYTCg7lOZjJzBjONJTFKGN9/NIYKSxhU5GRd+DTwHSZyvWp1xuI5aR+dLg+ayyC5Xv57KiY4oJ0tMO89fQ==",
                      "type": "package",
                      "path": "system.threading.timer/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net451/_._",
                        "lib/portable-net451+win81+wpa81/_._",
                        "lib/win81/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net451/_._",
                        "ref/netcore50/System.Threading.Timer.dll",
                        "ref/netcore50/System.Threading.Timer.xml",
                        "ref/netcore50/de/System.Threading.Timer.xml",
                        "ref/netcore50/es/System.Threading.Timer.xml",
                        "ref/netcore50/fr/System.Threading.Timer.xml",
                        "ref/netcore50/it/System.Threading.Timer.xml",
                        "ref/netcore50/ja/System.Threading.Timer.xml",
                        "ref/netcore50/ko/System.Threading.Timer.xml",
                        "ref/netcore50/ru/System.Threading.Timer.xml",
                        "ref/netcore50/zh-hans/System.Threading.Timer.xml",
                        "ref/netcore50/zh-hant/System.Threading.Timer.xml",
                        "ref/netstandard1.2/System.Threading.Timer.dll",
                        "ref/netstandard1.2/System.Threading.Timer.xml",
                        "ref/netstandard1.2/de/System.Threading.Timer.xml",
                        "ref/netstandard1.2/es/System.Threading.Timer.xml",
                        "ref/netstandard1.2/fr/System.Threading.Timer.xml",
                        "ref/netstandard1.2/it/System.Threading.Timer.xml",
                        "ref/netstandard1.2/ja/System.Threading.Timer.xml",
                        "ref/netstandard1.2/ko/System.Threading.Timer.xml",
                        "ref/netstandard1.2/ru/System.Threading.Timer.xml",
                        "ref/netstandard1.2/zh-hans/System.Threading.Timer.xml",
                        "ref/netstandard1.2/zh-hant/System.Threading.Timer.xml",
                        "ref/portable-net451+win81+wpa81/_._",
                        "ref/win81/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.threading.timer.4.3.0.nupkg.sha512",
                        "system.threading.timer.nuspec"
                      ]
                    },
                    "System.Xml.ReaderWriter/4.3.0": {
                      "sha512": "GrprA+Z0RUXaR4N7/eW71j1rgMnEnEVlgii49GZyAjTH7uliMnrOU3HNFBr6fEDBCJCIdlVNq9hHbaDR621XBA==",
                      "type": "package",
                      "path": "system.xml.readerwriter/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/net46/System.Xml.ReaderWriter.dll",
                        "lib/netcore50/System.Xml.ReaderWriter.dll",
                        "lib/netstandard1.3/System.Xml.ReaderWriter.dll",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/net46/System.Xml.ReaderWriter.dll",
                        "ref/netcore50/System.Xml.ReaderWriter.dll",
                        "ref/netcore50/System.Xml.ReaderWriter.xml",
                        "ref/netcore50/de/System.Xml.ReaderWriter.xml",
                        "ref/netcore50/es/System.Xml.ReaderWriter.xml",
                        "ref/netcore50/fr/System.Xml.ReaderWriter.xml",
                        "ref/netcore50/it/System.Xml.ReaderWriter.xml",
                        "ref/netcore50/ja/System.Xml.ReaderWriter.xml",
                        "ref/netcore50/ko/System.Xml.ReaderWriter.xml",
                        "ref/netcore50/ru/System.Xml.ReaderWriter.xml",
                        "ref/netcore50/zh-hans/System.Xml.ReaderWriter.xml",
                        "ref/netcore50/zh-hant/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.0/System.Xml.ReaderWriter.dll",
                        "ref/netstandard1.0/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.0/de/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.0/es/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.0/fr/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.0/it/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.0/ja/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.0/ko/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.0/ru/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.0/zh-hans/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.0/zh-hant/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.3/System.Xml.ReaderWriter.dll",
                        "ref/netstandard1.3/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.3/de/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.3/es/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.3/fr/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.3/it/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.3/ja/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.3/ko/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.3/ru/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.3/zh-hans/System.Xml.ReaderWriter.xml",
                        "ref/netstandard1.3/zh-hant/System.Xml.ReaderWriter.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.xml.readerwriter.4.3.0.nupkg.sha512",
                        "system.xml.readerwriter.nuspec"
                      ]
                    },
                    "System.Xml.XDocument/4.3.0": {
                      "sha512": "5zJ0XDxAIg8iy+t4aMnQAu0MqVbqyvfoUVl1yDV61xdo3Vth45oA2FoY4pPkxYAH5f8ixpmTqXeEIya95x0aCQ==",
                      "type": "package",
                      "path": "system.xml.xdocument/4.3.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "ThirdPartyNotices.txt",
                        "dotnet_library_license.txt",
                        "lib/MonoAndroid10/_._",
                        "lib/MonoTouch10/_._",
                        "lib/net45/_._",
                        "lib/netcore50/System.Xml.XDocument.dll",
                        "lib/netstandard1.3/System.Xml.XDocument.dll",
                        "lib/portable-net45+win8+wp8+wpa81/_._",
                        "lib/win8/_._",
                        "lib/wp80/_._",
                        "lib/wpa81/_._",
                        "lib/xamarinios10/_._",
                        "lib/xamarinmac20/_._",
                        "lib/xamarintvos10/_._",
                        "lib/xamarinwatchos10/_._",
                        "ref/MonoAndroid10/_._",
                        "ref/MonoTouch10/_._",
                        "ref/net45/_._",
                        "ref/netcore50/System.Xml.XDocument.dll",
                        "ref/netcore50/System.Xml.XDocument.xml",
                        "ref/netcore50/de/System.Xml.XDocument.xml",
                        "ref/netcore50/es/System.Xml.XDocument.xml",
                        "ref/netcore50/fr/System.Xml.XDocument.xml",
                        "ref/netcore50/it/System.Xml.XDocument.xml",
                        "ref/netcore50/ja/System.Xml.XDocument.xml",
                        "ref/netcore50/ko/System.Xml.XDocument.xml",
                        "ref/netcore50/ru/System.Xml.XDocument.xml",
                        "ref/netcore50/zh-hans/System.Xml.XDocument.xml",
                        "ref/netcore50/zh-hant/System.Xml.XDocument.xml",
                        "ref/netstandard1.0/System.Xml.XDocument.dll",
                        "ref/netstandard1.0/System.Xml.XDocument.xml",
                        "ref/netstandard1.0/de/System.Xml.XDocument.xml",
                        "ref/netstandard1.0/es/System.Xml.XDocument.xml",
                        "ref/netstandard1.0/fr/System.Xml.XDocument.xml",
                        "ref/netstandard1.0/it/System.Xml.XDocument.xml",
                        "ref/netstandard1.0/ja/System.Xml.XDocument.xml",
                        "ref/netstandard1.0/ko/System.Xml.XDocument.xml",
                        "ref/netstandard1.0/ru/System.Xml.XDocument.xml",
                        "ref/netstandard1.0/zh-hans/System.Xml.XDocument.xml",
                        "ref/netstandard1.0/zh-hant/System.Xml.XDocument.xml",
                        "ref/netstandard1.3/System.Xml.XDocument.dll",
                        "ref/netstandard1.3/System.Xml.XDocument.xml",
                        "ref/netstandard1.3/de/System.Xml.XDocument.xml",
                        "ref/netstandard1.3/es/System.Xml.XDocument.xml",
                        "ref/netstandard1.3/fr/System.Xml.XDocument.xml",
                        "ref/netstandard1.3/it/System.Xml.XDocument.xml",
                        "ref/netstandard1.3/ja/System.Xml.XDocument.xml",
                        "ref/netstandard1.3/ko/System.Xml.XDocument.xml",
                        "ref/netstandard1.3/ru/System.Xml.XDocument.xml",
                        "ref/netstandard1.3/zh-hans/System.Xml.XDocument.xml",
                        "ref/netstandard1.3/zh-hant/System.Xml.XDocument.xml",
                        "ref/portable-net45+win8+wp8+wpa81/_._",
                        "ref/win8/_._",
                        "ref/wp80/_._",
                        "ref/wpa81/_._",
                        "ref/xamarinios10/_._",
                        "ref/xamarinmac20/_._",
                        "ref/xamarintvos10/_._",
                        "ref/xamarinwatchos10/_._",
                        "system.xml.xdocument.4.3.0.nupkg.sha512",
                        "system.xml.xdocument.nuspec"
                      ]
                    },
                    "TimeZoneConverter/6.1.0": {
                      "sha512": "UGdtyKWJqXXinyvGB9X6NVoIYbTAidoZYmn3aXzxeEYC9+OL8vF36eDt1qjb6RqBkWDl4v7iE84ecI+dFhA80A==",
                      "type": "package",
                      "path": "timezoneconverter/6.1.0",
                      "files": [
                        ".nupkg.metadata",
                        ".signature.p7s",
                        "lib/net462/TimeZoneConverter.dll",
                        "lib/net462/TimeZoneConverter.xml",
                        "lib/net6.0/TimeZoneConverter.dll",
                        "lib/net6.0/TimeZoneConverter.xml",
                        "lib/netstandard2.0/TimeZoneConverter.dll",
                        "lib/netstandard2.0/TimeZoneConverter.xml",
                        "timezoneconverter.6.1.0.nupkg.sha512",
                        "timezoneconverter.nuspec"
                      ]
                    }
                  },
                  "projectFileDependencyGroups": {
                    "net8.0": [
                      "Fluid.Core >= 2.7.0"
                    ]
                  },
                  "packageFolders": {
                    "D:\\NuGetPackages": {},
                    "C:\\Program Files\\dotnet\\sdk\\NuGetFallbackFolder": {}
                  },
                  "project": {
                    "version": "1.0.0",
                    "restore": {
                      "projectUniqueName": "C:\\Users\\drnoakes\\source\\repos\\TransitiveAuditDemo\\TransitiveAuditDemo\\TransitiveAuditDemo.csproj",
                      "projectName": "TransitiveAuditDemo",
                      "projectPath": "C:\\Users\\drnoakes\\source\\repos\\TransitiveAuditDemo\\TransitiveAuditDemo\\TransitiveAuditDemo.csproj",
                      "packagesPath": "D:\\NuGetPackages",
                      "outputPath": "C:\\Users\\drnoakes\\source\\repos\\TransitiveAuditDemo\\TransitiveAuditDemo\\obj\\",
                      "projectStyle": "PackageReference",
                      "fallbackFolders": [
                        "C:\\Program Files\\dotnet\\sdk\\NuGetFallbackFolder"
                      ],
                      "configFilePaths": [
                        "C:\\Users\\drnoakes\\AppData\\Roaming\\NuGet\\NuGet.Config",
                        "C:\\Program Files (x86)\\NuGet\\Config\\Microsoft.VisualStudio.Offline.config",
                        "C:\\Program Files (x86)\\NuGet\\Config\\Xamarin.Offline.config"
                      ],
                      "originalTargetFrameworks": [
                        "net8.0"
                      ],
                      "sources": {
                        "C:\\Program Files (x86)\\Microsoft SDKs\\NuGetPackages\\": {},
                        "C:\\Program Files\\dotnet\\library-packs": {},
                        "D:\\nuget-local": {},
                        "https://api.nuget.org/v3/index.json": {}
                      },
                      "frameworks": {
                        "net8.0": {
                          "targetAlias": "net8.0",
                          "projectReferences": {}
                        }
                      },
                      "warningProperties": {
                        "warnAsError": [
                          "NU1605"
                        ]
                      },
                      "restoreAuditProperties": {
                        "enableAudit": "true",
                        "auditLevel": "low",
                        "auditMode": "all"
                      }
                    },
                    "frameworks": {
                      "net8.0": {
                        "targetAlias": "net8.0",
                        "dependencies": {
                          "Fluid.Core": {
                            "target": "Package",
                            "version": "[2.7.0, )"
                          }
                        },
                        "imports": [
                          "net461",
                          "net462",
                          "net47",
                          "net471",
                          "net472",
                          "net48",
                          "net481"
                        ],
                        "assetTargetFallback": true,
                        "warn": true,
                        "frameworkReferences": {
                          "Microsoft.NETCore.App": {
                            "privateAssets": "all"
                          }
                        },
                        "runtimeIdentifierGraphPath": "C:\\Program Files\\dotnet\\sdk\\8.0.400-preview.0.24324.5/PortableRuntimeIdentifierGraph.json"
                      }
                    }
                  },
                  "logs": [
                    {
                      "code": "NU1903",
                      "level": "Warning",
                      "warningLevel": 1,
                      "message": "Package 'System.Net.Http' 4.3.0 has a known high severity vulnerability, https://github.com/advisories/GHSA-7jgj-8wvc-jh57",
                      "libraryId": "System.Net.Http",
                      "targetGraphs": [
                        "net8.0"
                      ]
                    },
                    {
                      "code": "NU1903",
                      "level": "Warning",
                      "warningLevel": 1,
                      "message": "Package 'System.Text.RegularExpressions' 4.3.0 has a known high severity vulnerability, https://github.com/advisories/GHSA-cmhx-cq75-c4mj",
                      "libraryId": "System.Text.RegularExpressions",
                      "targetGraphs": [
                        "net8.0"
                      ]
                    }
                  ]
                }
                """;

            #endregion

            var lockFilePath = """C:\repo\obj\project.assets.json""";

            var lockFile = new LockFileFormat().Parse(lockFileContent, lockFilePath);

            var snapshot = AssetsFileDependenciesSnapshot.FromLockFile(lockFile);

            var dependencies = snapshot.DataByTarget.Single().Value.LibraryByName;

            // This has a NU1903 warning directly upon it
            Assert.True(dependencies.TryGetValue("System.Text.RegularExpressions", out var systemTextRegex));
            Assert.Equal(NuGet.Common.LogLevel.Warning, systemTextRegex.LogLevel);

            // This has a NU1903 warning directly upon it
            Assert.True(dependencies.TryGetValue("System.Net.Http", out var systemNetHttp));
            Assert.Equal(NuGet.Common.LogLevel.Warning, systemNetHttp.LogLevel);

            // Inherits a warning, as it depends upon System.Net.Http
            Assert.True(dependencies.TryGetValue("NETStandard.Library", out var netStandardLibrary));
            Assert.Equal(NuGet.Common.LogLevel.Warning, netStandardLibrary.LogLevel);

            // Inherits a warning, as it depends upon NETStandard.Library
            Assert.True(dependencies.TryGetValue("Microsoft.Extensions.FileProviders.Abstractions", out var extensions));
            Assert.Equal(NuGet.Common.LogLevel.Warning, extensions.LogLevel);

            // Inherits a warning, as it depends upon Microsoft.Extensions.FileProviders.Abstractions
            Assert.True(dependencies.TryGetValue("Fluid.Core", out var fluidCore));
            Assert.Equal(NuGet.Common.LogLevel.Warning, fluidCore.LogLevel);

            // No warning, as doesn't inherit anything with a warning
            Assert.True(dependencies.TryGetValue("System.Linq", out var systemLinq));
            Assert.Null(systemLinq.LogLevel);
        }

        [Fact]
        public void ParseLibraries_IgnoreCaseInDependenciesTree_Succeeds()
        {
            var lockFileContent = """
                {
                  "version": 3,
                  "targets": {
                    "net5.0": {
                      "System.Runtime/4.0.20-beta-22927": {
                        "type": "package",
                        "dependencies": {
                          "Frob": "4.0.20"
                        },
                        "compile": {
                          "ref/dotnet/System.Runtime.dll": {}
                        }
                      }
                    }
                  },
                  "libraries": {
                    "System.Runtime/4.0.20-beta-22927": {
                      "sha512": "sup3rs3cur3",
                      "type": "package",
                      "files": [
                        "System.Runtime.nuspec"
                      ]
                    }
                  },
                  "projectFileDependencyGroups": {
                    "": [
                      "System.Runtime [4.0.10-beta-*, )"
                    ],
                    "net5.0": []
                  },
                  "logs": [
                    {
                      "code": "NU1000",
                      "level": "Error",
                      "message": "test log message"
                    }
                  ]
                }
                """;

            var lockFilePath = """C:\repo\obj\project.assets.json""";

            var lockFile = new LockFileFormat().Parse(lockFileContent, lockFilePath);

            var dependencies = AssetsFileDependenciesSnapshot.ParseLibraries(lockFile, lockFile.Targets.First(), []);

            var dependency = Assert.Single(dependencies);
            Assert.Equal("System.Runtime", dependency.Key);
        }

        [Fact]
        public void ParseLibraries_LogForUnknownLibrary_AddsUnknownLibraryType()
        {
            var lockFileContent = """
                {
                  "version": 3,
                  "targets": {
                    "net5.0": {
                    }
                  },
                  "libraries": {
                  },
                  "projectFileDependencyGroups": {
                    "": [
                      "System.Runtime [4.0.10-beta-*, )"
                    ],
                    "net5.0": []
                  },
                  "logs": [
                    {
                      "code": "NU1000",
                      "level": "Error",
                      "message": "test log message",
                      "libraryId": "UnknownLibraryId"
                    }
                  ]
                }
                """;

            var lockFilePath = """C:\repo\obj\project.assets.json""";

            var lockFile = new LockFileFormat().Parse(lockFileContent, lockFilePath);

            var logMessages = ImmutableArray.Create(new AssetsFileLogMessage(lockFilePath, lockFile.LogMessages.Single()));

            var dependencies = AssetsFileDependenciesSnapshot.ParseLibraries(lockFile, lockFile.Targets.First(), logMessages);

            var dependency = Assert.Single(dependencies);
            Assert.Equal("UnknownLibraryId", dependency.Key);
            Assert.Equal("UnknownLibraryId", dependency.Value.Name);
            Assert.Null(dependency.Value.Version);
            Assert.Empty(dependency.Value.BuildFiles);
            Assert.Empty(dependency.Value.BuildMultiTargetingFiles);
            Assert.Empty(dependency.Value.CompileTimeAssemblies);
            Assert.Empty(dependency.Value.Dependencies);
            Assert.Empty(dependency.Value.DocumentationFiles);
            Assert.Empty(dependency.Value.FrameworkAssemblies);
            Assert.Equal(AssetsFileLibraryType.Unknown, dependency.Value.Type);
        }

        [Fact]
        public void ParseLibraries_LogForUnknownLibrary_WithAbsolutePath_AddsUnknownLibraryType()
        {
            var lockFileContent = """
                {
                  "version": 3,
                  "targets": {
                    "net5.0": {
                    }
                  },
                  "libraries": {
                  },
                  "projectFileDependencyGroups": {
                    "": [
                      "System.Runtime [4.0.10-beta-*, )"
                    ],
                    "net5.0": []
                  },
                  "logs": [
                    {
                      "code": "NU1000",
                      "level": "Error",
                      "message": "test log message",
                      "libraryId": "C:\\repo\\OtherProject\\OtherProject.csproj"
                    }
                  ]
                }
                """;

            var lockFilePath = """C:\repo\obj\project.assets.json""";

            var lockFile = new LockFileFormat().Parse(lockFileContent, lockFilePath);

            var logMessages = ImmutableArray.Create(new AssetsFileLogMessage(lockFilePath, lockFile.LogMessages.Single()));

            var dependencies = AssetsFileDependenciesSnapshot.ParseLibraries(lockFile, lockFile.Targets.First(), logMessages);

            var dependency = Assert.Single(dependencies);
            Assert.Equal("""..\OtherProject\OtherProject.csproj""", dependency.Key);
            Assert.Equal("""..\OtherProject\OtherProject.csproj""", dependency.Value.Name);
            Assert.Null(dependency.Value.Version);
            Assert.Empty(dependency.Value.BuildFiles);
            Assert.Empty(dependency.Value.BuildMultiTargetingFiles);
            Assert.Empty(dependency.Value.CompileTimeAssemblies);
            Assert.Empty(dependency.Value.Dependencies);
            Assert.Empty(dependency.Value.DocumentationFiles);
            Assert.Empty(dependency.Value.FrameworkAssemblies);
            Assert.Equal(AssetsFileLibraryType.Unknown, dependency.Value.Type);
        }

        [Fact]
        public void ParseLibraries_LockFileTargetLibrariesWithDifferentCase_Throws()
        {
            var lockFileTarget = new LockFileTarget
            {
                Libraries =
                [
                    new()
                    {
                        Name = "packageA",
                        Type = "package",
                        Version = NuGetVersion.Parse("1.0.0")
                    },
                    new()
                    {
                        Name = "PackageA",
                        Type = "package",
                        Version = NuGetVersion.Parse("1.0.0")
                    }
                ]
            };

            var exception = Assert.Throws<ArgumentException>(() => AssetsFileDependenciesSnapshot.ParseLibraries(new LockFile(), lockFileTarget, []));

            Assert.Contains("PackageA", exception.Message);
        }

        [Fact]
        public void ParseLibraries_LockFileTargetLibrariesMatchesDependencies_Succeeds()
        {
            var lockFileTarget = new LockFileTarget
            {
                Libraries =
                [
                    new()
                    {
                        Name = "packageA",
                        Type = "package",
                        Version = NuGetVersion.Parse("1.0.0")
                    },
                    new()
                    {
                        Name = "packageB",
                        Type = "package",
                        Version = NuGetVersion.Parse("1.0.0")
                    },
                    new()
                    {
                        Name = "projectA",
                        Type = "project",
                        Version = NuGetVersion.Parse("1.0.0")
                    },
                    new()
                    {
                        Name = "projectB",
                        Type = "project",
                        Version = NuGetVersion.Parse("1.0.0")
                    }
                ]
            };

            ImmutableDictionary<string, AssetsFileTargetLibrary> dependencies = AssetsFileDependenciesSnapshot.ParseLibraries(new LockFile(), lockFileTarget, []);

            Assert.Equal(lockFileTarget.Libraries.Count, dependencies.Count);
            Assert.All(lockFileTarget.Libraries,
                source =>
                {
                    Assert.True(dependencies.ContainsKey(source.Name));

                    AssetsFileTargetLibrary target = dependencies[source.Name];
                    Assert.Equal(source.Name, target.Name);
                    Assert.Equal(source.Version.ToNormalizedString(), target.Version);

                    Assert.True(Enum.TryParse(source.Type, ignoreCase: true, out AssetsFileLibraryType sourceType));
                    Assert.Equal(sourceType, target.Type);
                });
        }
    }
}
