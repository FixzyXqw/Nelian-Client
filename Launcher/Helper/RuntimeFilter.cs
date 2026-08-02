using System;
using System.Collections.Generic;
using System.IO;

namespace Shared
{
    public static class RuntimeFilter
    {
        private static readonly HashSet<string> DotNetRuntimeFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Temel runtime dosyaları
            "coreclr.dll",
            "clrjit.dll",
            "clrgc.dll",
            "clretwrc.dll",
            "mscordaccore.dll",
            "mscordbi.dll",
            "mscorlib.dll",
            "mscorrc.dll",
            "hostfxr.dll",
            "hostpolicy.dll",
            "vcruntime140_cor3.dll",
            "wpfgfx_cor3.dll",
            "PresentationNative_cor3.dll",
            "D3DCompiler_47_cor3.dll",
            "PenImc_cor3.dll",
            "msquic.dll",
            "createdump.exe",
            
            // System.* dosyaları
            "System.dll",
            "System.Core.dll",
            "System.Data.dll",
            "System.Drawing.dll",
            "System.Net.Http.dll",
            "System.Numerics.dll",
            "System.Runtime.Serialization.dll",
            "System.Security.dll",
            "System.Transactions.dll",
            "System.Web.dll",
            "System.Windows.Forms.dll",
            "System.Xml.dll",
            "System.Xaml.dll",
            "System.Configuration.dll",
            "System.Management.dll",
            "System.Design.dll",
            "System.ServiceProcess.dll",
            "System.EnterpriseServices.dll",
            "System.DirectoryServices.dll",
            "System.Printing.dll",
            "System.ComponentModel.DataAnnotations.dll",
            "System.Data.DataSetExtensions.dll",
            "System.Drawing.Design.dll",
            "System.Net.dll",
            "System.Net.Http.Json.dll",
            "System.Net.HttpListener.dll",
            "System.Net.Mail.dll",
            "System.Net.NameResolution.dll",
            "System.Net.NetworkInformation.dll",
            "System.Net.Ping.dll",
            "System.Net.Primitives.dll",
            "System.Net.Quic.dll",
            "System.Net.Requests.dll",
            "System.Net.Security.dll",
            "System.Net.ServicePoint.dll",
            "System.Net.Sockets.dll",
            "System.Net.WebClient.dll",
            "System.Net.WebHeaderCollection.dll",
            "System.Net.WebProxy.dll",
            "System.Net.WebSockets.Client.dll",
            "System.Net.WebSockets.dll",
            "System.Numerics.Vectors.dll",
            "System.Reflection.Emit.dll",
            "System.Reflection.Emit.ILGeneration.dll",
            "System.Reflection.Emit.Lightweight.dll",
            "System.Reflection.Metadata.dll",
            "System.Runtime.CompilerServices.Unsafe.dll",
            "System.Runtime.Intrinsics.dll",
            "System.Security.Cryptography.Cng.dll",
            "System.Security.Cryptography.Csp.dll",
            "System.Security.Cryptography.OpenSsl.dll",
            "System.Security.Cryptography.Pkcs.dll",
            "System.Security.Cryptography.ProtectedData.dll",
            "System.Security.Cryptography.Xml.dll",
            "System.Security.Permissions.dll",
            "System.Security.Principal.Windows.dll",
            "System.Text.Encoding.CodePages.dll",
            "System.Text.Encodings.Web.dll",
            "System.Text.Json.dll",
            "System.Threading.Channels.dll",
            "System.Threading.Tasks.Extensions.dll",
            "System.ValueTuple.dll",
            "System.Windows.Controls.Ribbon.dll",
            "System.Windows.Extensions.dll",
            "System.Windows.Input.Manipulations.dll",
            "System.Windows.Presentation.dll",
            "System.CodeDom.dll",
            "System.Configuration.ConfigurationManager.dll",
            "System.Diagnostics.EventLog.dll",
            "System.Diagnostics.PerformanceCounter.dll",
            "System.Drawing.Common.dll",
            "System.IO.Packaging.dll",
            "System.Resources.Extensions.dll",
            
            // Microsoft.* dosyaları
            "Microsoft.CSharp.dll",
            "Microsoft.VisualBasic.dll",
            "Microsoft.VisualBasic.Core.dll",
            "Microsoft.VisualBasic.Forms.dll",
            "Microsoft.Win32.Primitives.dll",
            "Microsoft.Win32.Registry.dll",
            "Microsoft.Win32.Registry.AccessControl.dll",
            "Microsoft.Win32.SystemEvents.dll",
            "Microsoft.Extensions.DependencyInjection.Abstractions.dll",
            "Microsoft.Extensions.Logging.Abstractions.dll",
            
            // WPF ve UI dosyaları
            "WindowsBase.dll",
            "PresentationCore.dll",
            "PresentationFramework.dll",
            "PresentationUI.dll",
            "ReachFramework.dll",
            "UIAutomationClient.dll",
            "UIAutomationProvider.dll",
            "UIAutomationTypes.dll",
            "UIAutomationClientSideProviders.dll",
            "WindowsFormsIntegration.dll",
            "System.Windows.Forms.Primitives.dll",
            "PresentationFramework.Aero.dll",
            "PresentationFramework.Aero2.dll",
            "PresentationFramework.AeroLite.dll",
            "PresentationFramework.Classic.dll",
            "PresentationFramework.Luna.dll",
            "PresentationFramework.Royale.dll",
            "PresentationFramework-SystemCore.dll",
            "PresentationFramework-SystemData.dll",
            "PresentationFramework-SystemDrawing.dll",
            "PresentationFramework-SystemXml.dll",
            "PresentationFramework-SystemXmlLinq.dll"
        };

        // Özel istisnalar (uygulamaya ait System.* veya Microsoft.* dosyaları)
        private static readonly HashSet<string> Exceptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "System.CodeDom.dll",
            "System.Configuration.ConfigurationManager.dll",
            "System.Diagnostics.EventLog.dll",
            "System.Drawing.Common.dll",
            "System.Resources.Extensions.dll",
            "System.Security.Permissions.dll",
            "System.Windows.Extensions.dll",
            "System.Diagnostics.PerformanceCounter.dll",
            "System.IO.Packaging.dll"
        };

        public static bool IsDotNetRuntimeFile(string filePath)
        {
            string fileName = Path.GetFileName(filePath);

            // Direkt .NET runtime dosyaları
            if (DotNetRuntimeFiles.Contains(fileName))
                return true;

            // System.* ve Microsoft.* dosyaları (kök dizinde)
            if (!filePath.Contains("/") && !filePath.Contains("\\"))
            {
                if (fileName.StartsWith("System.", StringComparison.OrdinalIgnoreCase) ||
                    fileName.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase))
                {
                    if (!Exceptions.Contains(fileName))
                        return true;
                }
            }

            // Dil klasörlerindeki framework dosyaları
            string[] separators = { "/", "\\" };
            string[] parts = filePath.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 2)
            {
                string dirName = parts[0];
                bool isLanguageFolder = dirName.Length == 2 ||
                                       dirName == "zh-Hans" ||
                                       dirName == "zh-Hant" ||
                                       dirName == "pt-BR";

                if (isLanguageFolder)
                {
                    string fileName2 = parts[parts.Length - 1];
                    if (fileName2.StartsWith("System.", StringComparison.OrdinalIgnoreCase) ||
                        fileName2.StartsWith("Microsoft.", StringComparison.OrdinalIgnoreCase) ||
                        fileName2.StartsWith("Presentation", StringComparison.OrdinalIgnoreCase) ||
                        fileName2.StartsWith("Windows", StringComparison.OrdinalIgnoreCase) ||
                        fileName2.StartsWith("UIAutomation", StringComparison.OrdinalIgnoreCase) ||
                        fileName2.StartsWith("ReachFramework", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
