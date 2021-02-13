using System;
using System.Runtime.InteropServices;

namespace ERSB.Modules
{
    static class NativeMethods
    {
        [DllImport("shell32")]
        private static extern int SHGetKnownFolderPath(ref Guid rfid, uint dwFlags, IntPtr hToken, ref IntPtr np);

        public static string GetDownloadFolder()
        {
            var np = IntPtr.Zero;
            var downloadsFolder = new Guid("374DE290-123F-4565-9164-39C4925E467B");
            _ = SHGetKnownFolderPath(ref downloadsFolder, 0, IntPtr.Zero, ref np);
            var path = Marshal.PtrToStringUni(np);
            Marshal.FreeCoTaskMem(np);
            return path;
        }
    }
}
