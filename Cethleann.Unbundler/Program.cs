﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Cethleann.DataTables;
using Cethleann.G1;
using DragonLib.IO;
using static Koei.DataExporter.Program;

namespace Cethleann.Unbundler
{
    static class Program
    {
        static void Main(string[] args)
        {
            var files = new List<string>();
            foreach (var arg in args)
            {
                if (!Directory.Exists(arg))
                {
                    files.Add(arg);
                    continue;
                }

                if (File.Exists(Path.Combine(arg, "originaltype.cethleann")))
                {
                    Logger.Info("CETH", arg);
                    var ext = File.ReadAllText(Path.Combine(arg, "originaltype.cethleann"));
                    var originalName = arg + ext;
                    switch (ext)
                    {
                        case ".datatable":
                        case ".bin":
                            TryPackDatatable(Path.ChangeExtension(originalName, "mod.datatable"), arg);
                            break;
                        case ".g1m":
                            TryPackG1M(Path.ChangeExtension(originalName, "mod.g1m"), arg);
                            break;
                    }

                    continue;
                }

                files.AddRange(Directory.GetFiles(arg, "*", SearchOption.TopDirectoryOnly));
            }

            foreach (var arg in files)
            {
                Logger.Info("CETH", arg);
                var data = new Memory<byte>(File.ReadAllBytes(arg));
                var pathBase = arg;
                if (!arg.EndsWith(".text"))
                {
                    if (Path.GetFileName(arg) == Path.GetFileNameWithoutExtension(arg))
                        pathBase += "_contents";
                    else
                        pathBase = Path.Combine(Path.GetDirectoryName(arg), Path.GetFileNameWithoutExtension(arg));
                }
                
                TryExtractBlob(pathBase, data, true, true, false);

                if (!Directory.Exists(pathBase) || Path.GetFileName(arg) == Path.GetFileNameWithoutExtension(arg)) continue;
                // TODO: Get TryExtractBlob to write this file with relevant file metadata.
                File.WriteAllText(Path.Combine(pathBase, "originaltype.cethleann"), Path.GetExtension(arg));
                Logger.Info("CETH", $"Writing meta file {Path.Combine(pathBase, "originaltype.cethleann")}");
            }
        }

        public static void TryPackG1M(string path, string dir)
        {
            var files = Directory.GetFiles(dir).OrderBy(x => int.Parse(Path.GetFileNameWithoutExtension(x).Split('_')[0], NumberStyles.Integer)).ToArray();
            var model = new G1Model();
            foreach (var file in files) model.SectionRoot.Sections.Add(new Memory<byte>(File.ReadAllBytes(file)));
            File.WriteAllBytes(path, model.WriteFromRoot().ToArray());
        }

        private static void TryPackDatatable(string path, string dir)
        {
            var files = Directory.GetFiles(dir).OrderBy(x => int.Parse(Path.GetFileNameWithoutExtension(x), NumberStyles.HexNumber)).ToArray();
            var dataTable = new DataTable
            {
                Entries = files.Select(x => new Memory<byte>(File.ReadAllBytes(x))).ToList()
            }.Write().ToArray();
            File.WriteAllBytes(path, dataTable);
        }
    }
}
