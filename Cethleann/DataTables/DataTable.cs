﻿using Cethleann.Structure;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Cethleann.DataTables
{
    /// <summary>
    /// Main blob bundle of the game.
    /// Can contain many things, from structures to art assets.
    /// </summary>
    public class DataTable
    {
        /// <summary>
        /// Lsof Entries found in the table
        /// </summary>
        public IEnumerable<Memory<byte>> Entries;
        
        /// <summary>
        /// Initialize with a span.
        /// </summary>
        /// <param name="buffer"></param>
        public DataTable(Span<byte> buffer)
        {
            var count = MemoryMarshal.Read<int>(buffer);
            var tableInfo = MemoryMarshal.Cast<byte, DataTableRecord>(buffer.Slice(4, 8 * count));
            var entries = new List<Memory<byte>>();
            foreach (var info in tableInfo)
            {
                entries.Add(buffer.Slice(info.Offset, info.Size).ToArray());
            }
            Entries = entries;
        }
    }
}