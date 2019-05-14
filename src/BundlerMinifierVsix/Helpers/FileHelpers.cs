using System;
using System.IO;
using BundlerMinifier;

namespace BundlerMinifierVsix
{
    public static class FileHelpers
    {
        public static bool HasMinFile(string file, string bundleMinFile, out string minFile)
        {
            minFile = GetMinFileName(file, bundleMinFile);
            return File.Exists(minFile);
        }

        public static string GetMinFileName(string file, string minFile = "")
        {            
            if (string.IsNullOrEmpty(minFile))
            {
                string fileName = Path.GetFileName(file);
                if (fileName.IndexOf(".min.", StringComparison.OrdinalIgnoreCase) > 0)
                    return file;

                string ext = Path.GetExtension(file);
                return file.Substring(0, file.LastIndexOf(ext, StringComparison.OrdinalIgnoreCase)) + ".min" + ext;
            }
            else
            {
                return minFile;
            }
        }

        public static bool HasSourceMap(string file, out string sourceMap)
        {
            if (File.Exists(file + ".map"))
            {
                sourceMap = file + ".map";
                return true;
            }

            sourceMap = null;

            return false;
        }
    }
}
