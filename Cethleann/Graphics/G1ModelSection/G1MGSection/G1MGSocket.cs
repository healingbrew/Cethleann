﻿using System;
using System.Runtime.InteropServices;
using Cethleann.Structure.Resource.Model;
using JetBrains.Annotations;

namespace Cethleann.Graphics.G1ModelSection.G1MGSection
{
    /// <summary>
    ///     "Lattice" is an assumption.
    ///     Just a list of float data, used for bones maybe?
    /// </summary>
    /// <inheritdoc />
    [PublicAPI]
    public class G1MGSocket : IG1MGSection
    {
        internal G1MGSocket(Span<byte> data, ModelSection sectionInfo)
        {
            Section = sectionInfo;
            Sockets = MemoryMarshal.Cast<byte, ModelGeometrySocket>(data).ToArray();
        }

        /// <summary>
        ///     Sockets
        /// </summary>
        public ModelGeometrySocket[] Sockets { get; }

        /// <inheritdoc />
        public ModelSection Section { get; }

        /// <inheritdoc />
        public ModelGeometrySectionType Type => ModelGeometrySectionType.Socket;
    }
}
