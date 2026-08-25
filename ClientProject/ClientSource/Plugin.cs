// Copyright (c) 2026 Retype15
// This file is licensed under the GNU GPLv3.
// See the LICENSE file in the project root for details.

#pragma warning disable IDE0130
#pragma warning disable IDE0290

using System.Diagnostics;
using SOS;

namespace DummySimulator
{
    // Client-specific code
    public partial class Plugin
    {
        [Conditional("CLIENT")]
        public static void InitClient()
        {
        }

        [Conditional("CLIENT")]
        public static void DisposeClient()
        {
        }
    }
}