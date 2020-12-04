﻿using System;
using System.Runtime.InteropServices;

namespace ERSB.Modules
{
    static class NativeMethods
    {
        [DllImport("shell32")]
#pragma warning disable CA5392 // Use DefaultDllImportSearchPaths attribute for P/Invokes
        private static extern int SHGetKnownFolderPath(ref Guid rfid, uint dwFlags, IntPtr hToken, ref IntPtr np);
#pragma warning restore CA5392 // Use DefaultDllImportSearchPaths attribute for P/Invokes

        public static string GetDownloadFolder()
        {
            IntPtr np = IntPtr.Zero;
            var downloadsFolder = new Guid("374DE290-123F-4565-9164-39C4925E467B");
            _ = SHGetKnownFolderPath(ref downloadsFolder, 0, IntPtr.Zero, ref np);
            string path = Marshal.PtrToStringUni(np);
            Marshal.FreeCoTaskMem(np);
            return path;
        }
    }
}
