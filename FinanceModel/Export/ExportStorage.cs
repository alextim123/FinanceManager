using System;
using System.IO;

namespace FinanceModel.Export
{
    public static class ExportStorage
    {
        public static readonly string Dir =
            Path.Combine(AppContext.BaseDirectory, "exports");

        public static string MakePath(string fileNameWithoutExt, string ext)
        {
            if (string.IsNullOrWhiteSpace(fileNameWithoutExt))
                fileNameWithoutExt = "export";

            var safe = Path.GetFileNameWithoutExtension(fileNameWithoutExt);
            Directory.CreateDirectory(Dir);
            return Path.Combine(Dir, safe + ext);
        }
    }
}
