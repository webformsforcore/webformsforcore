// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorQuantizer.cs" company="Microsoft">
//   Copyright Microsoft Corporation, all rights reserved
// </copyright>
// <auto-generated />
// <summary>
//   The color quantizer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WebGrease.ImageAssemble
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;

    using WebGrease.Extensions;

    /// <summary>The color quantizer.</summary>
    public static class ColorQuantizer
    {
        #region Quantize methods

        /// <summary>The quantize.</summary>
        /// <param name="image">The image.</param>
        /// <param name="bitmapPixelFormat">The bitmap pixel format.</param>
        /// <returns></returns>
        public static Bitmap Quantize(Image image, PixelFormat bitmapPixelFormat)
        {
            // use dither by default
            return Quantize(image, bitmapPixelFormat, true);
        }

        /// <summary>The quantize.</summary>
        /// <param name="image">The image.</param>
        /// <param name="pixelFormat">The pixel format.</param>
        /// <param name="useDither">The use dither.</param>
        /// <returns></returns>
        public static Bitmap Quantize(Image image, PixelFormat pixelFormat, bool useDither)
        {
            var tryBitmap = image as Bitmap;
            if (tryBitmap != null && tryBitmap.PixelFormat == PixelFormat.Format32bppArgb)
            {
                // the image passed to us is ALREADY a bitmap in the right format. No need to create
                // a copy and work from there.
                return DoQuantize(tryBitmap, pixelFormat, useDither);
            }
            else
            {
                // we use these values a lot
                var width = image.Width;
                var height = image.Height;
                var sourceRect = Rectangle.FromLTRB(0, 0, width, height);

                // create a 24-bit rgb version of the source image
                using (var bitmapSource = new Bitmap(width, height, PixelFormat.Format32bppArgb))
                {
                    using (var grfx = Graphics.FromImage(bitmapSource))
                    {
                        grfx.DrawImage(image, sourceRect, 0, 0, width, height, GraphicsUnit.Pixel);
                    }

                    return DoQuantize(bitmapSource, pixelFormat, useDither);
                }
            }
        }

        private static Bitmap DoQuantize(Bitmap bitmapSource, PixelFormat pixelFormat, bool useDither)
        {
            // we use these values a lot
            var width = bitmapSource.Width;
            var height = bitmapSource.Height;
            var sourceRect = Rectangle.FromLTRB(0, 0, width, height);

            Bitmap bitmapOptimized = null;
            try
            {
                // create a bitmap with the same dimensions and the desired format
                bitmapOptimized = new Bitmap(width, height, pixelFormat);

                // lock the bits of the source image for reading.
                // we will need to write if we do the dither.
                var bitmapDataSource = bitmapSource.LockBits(
                    sourceRect,
                    ImageLockMode.ReadWrite,
                    PixelFormat.Format32bppArgb
                    );
                try
                {
                    // perform the first pass, which generates the octree data

                    // create an octree
                    var octree = new Octree(pixelFormat);

                    // stride might be negative, indicating inverted row order.
                    // allocate a managed buffer for the pixel data, and copy it from the unmanaged pointer.
                    var strideSource = Math.Abs(bitmapDataSource.Stride);
                    var sourceDataBuffer = new byte[strideSource * height];
                    Marshal.Copy(bitmapDataSource.Scan0, sourceDataBuffer, 0, sourceDataBuffer.Length);

                    // we could skip every other row and/or every other column when sampling the colors
                    // of the source image, rather than hitting every other pixel. It doesn't seem to
                    // degrade the resulting image too much. But it doesn't really help the performance
                    // too much because the majority of the time seems to be spent in other places.

                    // for every row
                    int rowStartSource = 0;
                    for (var ndxRow = 0; ndxRow < height; ndxRow += 1)
                    {
                        // for each column
                        for (var ndxCol = 0; ndxCol < width; ndxCol += 1)
                        {
                            // add the color (4 bytes per pixel - ARGB)
                            var pixel = GetSourcePixel(sourceDataBuffer, rowStartSource, ndxCol);
                            octree.AddColor(pixel);
                        }

                        rowStartSource += strideSource;
                    }

                    // get the optimized colors
                    var colors = octree.GetPaletteColors();

                    // set the palette from the octree
                    var palette = bitmapOptimized.Palette;
                    for (var ndx = 0; ndx < palette.Entries.Length; ++ndx)
                    {
                        // use the colors we calculated
                        // for the rest, just set to transparent
                        palette.Entries[ndx] = (ndx < colors.Length)
                            ? colors[ndx]
                            : Color.Transparent;
                    }

                    bitmapOptimized.Palette = palette;

                    // lock the bits of the optimized bitmap for writing.
                    // we will also need to read if we are doing 1bpp or 4bpp
                    var bitmapDataOutput = bitmapOptimized.LockBits(sourceRect, ImageLockMode.ReadWrite, pixelFormat);
                    try
                    {
                        // create a managed array for the destination bytes given the desired color depth
                        // and marshal the unmanaged data to the managed array
                        var strideOutput = Math.Abs(bitmapDataOutput.Stride);
                        var bitmapOutputBuffer = new byte[strideOutput * height];

                        // for each source pixel, compute the appropriate color index
                        rowStartSource = 0;
                        var rowStartOutput = 0;
                        for (var ndxRow = 0; ndxRow < height; ++ndxRow)
                        {
                            // for each column
                            for (var ndxCol = 0; ndxCol < width; ++ndxCol)
                            {
                                // get the source color
                                var pixel = GetSourcePixel(sourceDataBuffer, rowStartSource, ndxCol);

                                // get the closest palette index
                                var paletteIndex = octree.GetPaletteIndex(pixel);

                                // if we want to dither and this isn't the transparent pixel
                                if (useDither && pixel.Alpha != 0)
                                {
                                    // calculate the error
                                    var paletteColor = colors[paletteIndex];
                                    var deltaRed = pixel.Red - paletteColor.R;
                                    var deltaGreen = pixel.Green - paletteColor.G;
                                    var deltaBlue = pixel.Blue - paletteColor.B;

                                    // propagate the dither error. 
                                    // we'll use a standard Floyd-Steinberg matrix (1/16):
                                    // | 0 0 0 |
                                    // | 0 x 7 |
                                    // | 3 5 1 |

                                    // make sure we're not on the right-hand edge
                                    if (ndxCol + 1 < width)
                                    {
                                        DitherSourcePixel(sourceDataBuffer, rowStartSource, ndxCol + 1, deltaRed, deltaGreen, deltaBlue, 7);
                                    }

                                    // make sure we're not already on the bottom row
                                    if (ndxRow + 1 < height)
                                    {
                                        var nextRow = rowStartSource + strideSource;

                                        // make sure we're not on the left-hand column
                                        if (ndxCol > 0)
                                        {
                                            // down one row, but back one pixel
                                            DitherSourcePixel(sourceDataBuffer, nextRow, ndxCol - 1, deltaRed, deltaGreen, deltaBlue, 3);
                                        }

                                        // pixel directly below us
                                        DitherSourcePixel(sourceDataBuffer, nextRow, ndxCol, deltaRed, deltaGreen, deltaBlue, 5);

                                        // make sure we're not on the right-hand column
                                        if (ndxCol + 1 < width)
                                        {
                                            // down one row, but right one pixel
                                            DitherSourcePixel(sourceDataBuffer, nextRow, ndxCol + 1, deltaRed, deltaGreen, deltaBlue, 1);
                                        }
                                    }
                                }

                                // set the bitmap index based on the format
                                switch (pixelFormat)
                                {
                                    case PixelFormat.Format8bppIndexed:
                                        // each byte is a palette index
                                        bitmapOutputBuffer[rowStartOutput + ndxCol] = (byte)paletteIndex;
                                        break;

                                    case PixelFormat.Format4bppIndexed:
                                        // each byte contains two pixels
                                        bitmapOutputBuffer[rowStartOutput + (ndxCol >> 1)] |= ((ndxCol & 1) == 1)
                                            ? (byte)(paletteIndex & 0x0f) // lower nibble
                                            : (byte)(paletteIndex << 4);  // upper nibble
                                        break;

                                    case PixelFormat.Format1bppIndexed:
                                        // each byte contains eight pixels
                                        if (paletteIndex != 0)
                                        {
                                            bitmapOutputBuffer[rowStartOutput + (ndxCol >> 3)] |= (byte)(0x80 >> (ndxCol & 0x07));
                                        }

                                        break;
                                }
                            }

                            rowStartSource += strideSource;
                            rowStartOutput += strideOutput;
                        }

                        // now copy the calculated pixel bytes from the managed array to the unmanaged bitmap
                        Marshal.Copy(bitmapOutputBuffer, 0, bitmapDataOutput.Scan0, bitmapOutputBuffer.Length);
                    }
                    finally
                    {
                        bitmapOptimized.UnlockBits(bitmapDataOutput);
                        bitmapDataOutput = null;
                    }
                }
                finally
                {
                    bitmapSource.UnlockBits(bitmapDataSource);
                    bitmapDataSource = null;
                }
            }
            catch (Exception)
            {
                // if any exception is thrown, dispose of the bitmap object
                // we've been working on before we rethrow and bail
                if (bitmapOptimized != null)
                {
                    bitmapOptimized.Dispose();
                }

                throw;
            }

            // caller is responsible for disposing of this bitmap!
            return bitmapOptimized;
        }

        private static void DitherSourcePixel(byte[] buffer, int rowStart, int col, int deltaRed, int deltaGreen, int deltaBlue, int weight)
        {
            var colorIndex = rowStart + col * 4;
            buffer[colorIndex + 2] = ChannelAdjustment(buffer[colorIndex + 2], (deltaRed * weight) >> 4);
            buffer[colorIndex + 1] = ChannelAdjustment(buffer[colorIndex + 1], (deltaGreen * weight) >> 4);
            buffer[colorIndex] = ChannelAdjustment(buffer[colorIndex], (deltaBlue * weight) >> 4);
        }

        private static Pixel GetSourcePixel(byte[] buffer, int rowStart, int col)
        {
            var colorIndex = rowStart + col * 4;
            return new Pixel
            {
                Alpha = buffer[colorIndex + 3],
                Red = buffer[colorIndex + 2],
                Green = buffer[colorIndex + 1],
                Blue = buffer[colorIndex]
            };
        }

        #endregion

        /// <summary>The channel adjustment.</summary>
        /// <param name="current">The current.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The channel adjustment.</returns>
        private static byte ChannelAdjustment(byte current, int offset)
        {
            return (byte)Math.Min(255, Math.Max(0, current + offset));
        }

        #region Octree class

        /// <summary>data structure for storing and reducing colors used in the source image</summary>
        private class Octree
        {
            /// <summary>The m_max colors.</summary>
            private readonly int m_maxColors;

            /// <summary>The m_reducible nodes.</summary>
            private readonly OctreeNode[] m_reducibleNodes;

            /// <summary>The m_color count.</summary>
            private int m_colorCount;

            /// <summary>The m_has transparent.</summary>
            private bool m_hasTransparent;

            /// <summary>The m_last argb.</summary>
            private int m_lastArgb;

            /// <summary>The m_last node.</summary>
            private OctreeNode m_lastNode;

            /// <summary>The m_palette.</summary>
            private Color[] m_palette;

            /// <summary>The m_root.</summary>
            private OctreeNode m_root;

            /// <summary>Initializes a new instance of the <see cref="Octree"/> class. Constructor</summary>
            /// <param name="pixelFormat">desired pixel format</param>
            internal Octree(PixelFormat pixelFormat)
            {
                // figure out the maximum colors from the pixel format passed in
                switch (pixelFormat)
                {
                    case PixelFormat.Format1bppIndexed:
                        this.m_maxColors = 2;
                        break;

                    case PixelFormat.Format4bppIndexed:
                        this.m_maxColors = 16;
                        break;

                    case PixelFormat.Format8bppIndexed:
                        this.m_maxColors = 256;
                        break;

                    default:
                        throw new ArgumentException("Invalid Pixel Format", "pixelFormat");
                }

                // we need a list for each level that may have reducible nodes.
                // since the last level (level 7) is only made up of leaf nodes,
                // we don't need an array entry for it.
                this.m_reducibleNodes = new OctreeNode[7];

                // add the initial level-0 root node
                this.m_root = new OctreeNode(0, this);
            }

            /// <summary>Add the given pixel color to the octree</summary>
            /// <param name="pPixel">points to the pixel color we want to add</param>
            internal void AddColor(Pixel pixel)
            {
                // if the A value is non-zero (ignore the transparent color)
                if (pixel.Alpha > 0)
                {
                    // if we have a previous node and this color is the same as the last...
                    if (this.m_lastNode != null && pixel.ARGB == this.m_lastArgb)
                    {
                        // just add this color to the same last node
                        this.m_lastNode.AddColor(pixel);
                    }
                    else
                    {
                        // just start at the root. If a new color is added,
                        // add one to the count (otherwise 0).
                        this.m_colorCount += this.m_root.AddColor(pixel) ? 1 : 0;
                    }
                }
                else
                {
                    // flag that we have a transparent color
                    this.m_hasTransparent = true;
                }
            }

            /// <summary>Given a pixel color, return the index of the palette entry
            /// we want to use in the reduced image. If the color is not in the 
            /// octree, OctreeNode.GetPaletteIndex will return a negative number.
            /// In that case, we will have to caluclate the palette index the brute-force
            /// method by computing the least distance to each color in the palette array.</summary>
            /// <param name="pPixel">pointer to the pixel color we want to look up</param>
            /// <returns>index of the palette entry we want to use for this color</returns>
            internal int GetPaletteIndex(Pixel pixel)
            {
                var paletteIndex = 0;

                // transparent is always the first entry, so if this is transparent,
                // don't do anything.
                if (pixel.Alpha > 0)
                {
                    paletteIndex = this.m_root.GetPaletteIndex(pixel);

                    // returns -1 if this value isn't in the octree.
                    if (paletteIndex < 0)
                    {
                        // use the brute-force method of calculating the closest color
                        // in the palette to the one we want
                        var minDistance = int.MaxValue;
                        for (var ndx = 0; ndx < this.m_palette.Length; ++ndx)
                        {
                            var paletteColor = this.m_palette[ndx];

                            // calculate the delta for each channel
                            var deltaRed = pixel.Red - paletteColor.R;
                            var deltaGreen = pixel.Green - paletteColor.G;
                            var deltaBlue = pixel.Blue - paletteColor.B;

                            // calculate the distance-squared by summing each channel's square
                            var distance = deltaRed * deltaRed + deltaGreen * deltaGreen + deltaBlue * deltaBlue;
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                paletteIndex = ndx;
                            }
                        }
                    }
                }

                return paletteIndex;
            }

            /// <summary>Return a color palette for the computed octree</summary>
            /// <returns></returns>
            internal Color[] GetPaletteColors()
            {
                // if we haven't already computed it, compute it now
                if (this.m_palette == null)
                {
                    // start at the second-to-last reducible level
                    var reductionLevel = this.m_reducibleNodes.Length - 1;

                    // save this for debugging purposes
                    var numColorsBefore = this.m_colorCount;

                    // we want to subtract one from the target if we have a transparent
                    // bit because we want to save room for that special color
                    var targetCount = this.m_maxColors - (this.m_hasTransparent ? 1 : 0);

                    // while we still have more colors than the target...
                    while (this.m_colorCount > targetCount)
                    {
                        // find the first reducible node, starting with the last level
                        // that can have reducible nodes
                        while (reductionLevel > 0 && this.m_reducibleNodes[reductionLevel] == null)
                        {
                            --reductionLevel;
                        }

                        if (this.m_reducibleNodes[reductionLevel] == null)
                        {
                            // shouldn't get here
                            break;
                        }
                        else
                        {
                            // we should have a node now
                            var newLeaf = this.m_reducibleNodes[reductionLevel];
                            this.m_reducibleNodes[reductionLevel] = newLeaf.NextReducibleNode;
                            this.m_colorCount -= newLeaf.Reduce() - 1;
                        }
                    }

                    Debug.WriteLine("Reduced from {0} colors to {1}".InvariantFormat(numColorsBefore, this.m_colorCount));

                    if (reductionLevel == 0 && !this.m_hasTransparent)
                    {
                        // if this was the top-most level, we now only have a single color
                        // representing the average. That's not what we want.
                        // use just black and white
                        this.m_palette = new Color[2];
                        this.m_palette[0] = Color.Black;
                        this.m_palette[1] = Color.White;

                        // and empty the octree so it always picks the closer of the black and white entries
                        this.m_root = new OctreeNode(0, this);
                    }
                    else
                    {
                        // now walk the tree, adding all the remaining colors to the list
                        var paletteIndex = 0;
                        this.m_palette = new Color[this.m_colorCount + (this.m_hasTransparent ? 1 : 0)];

                        // add the transparent color if we need it
                        if (this.m_hasTransparent)
                        {
                            this.m_palette[paletteIndex++] = Color.Transparent;
                        }

                        // have the nodes insert their leaf colors
                        this.m_root.AddColorsToPalette(this.m_palette, ref paletteIndex);
                    }
                }

                return this.m_palette;
            }

            /// <summary>set up the values we need to reuse the given pointer if the next color is argb</summary>
            /// <param name="node">last node to which we added a color</param>
            /// <param name="argb">last color we added</param>
            private void SetLastNode(OctreeNode node, int argb)
            {
                this.m_lastNode = node;
                this.m_lastArgb = argb;
            }

            /// <summary>When a reducible node is added, this method is called to add it to the appropriate
            /// reducible node list (given its level)</summary>
            /// <param name="reducibleNode">node to add to a reducible list</param>
            private void AddReducibleNode(OctreeNode reducibleNode)
            {
                // hook this node into the front of the list. 
                // this means the last one added will be the first in the list.
                reducibleNode.NextReducibleNode = this.m_reducibleNodes[reducibleNode.Level];
                this.m_reducibleNodes[reducibleNode.Level] = reducibleNode;
            }

            #region OctreeNode class

            /// <summary>Node for an Octree structure</summary>
            private class OctreeNode
            {
                // the containing octree

                // the level for this node
                /// <summary>The s_level masks.</summary>
                private static readonly byte[] s_levelMasks = { 0x80, 0x40, 0x20, 0x10, 0x08, 0x04, 0x02, 0x01 };

                /// <summary>The m_level.</summary>
                private readonly int m_level;

                /// <summary>The m_octree.</summary>
                private readonly Octree m_octree;

                /// <summary>The m_blue sum.</summary>
                private int m_blueSum;

                /// <summary>The m_child nodes.</summary>
                private OctreeNode[] m_childNodes;

                /// <summary>The m_green sum.</summary>
                private int m_greenSum;

                /// <summary>The m_is leaf.</summary>
                private bool m_isLeaf;

                /// <summary>The m_palette index.</summary>
                private int m_paletteIndex;

                // information we need to calculate the average color
                // for a set of pixels
                /// <summary>The m_pixel count.</summary>
                private int m_pixelCount;

                /// <summary>The m_red sum.</summary>
                private int m_redSum;

                /// <summary>Initializes a new instance of the <see cref="OctreeNode"/> class. Constructor</summary>
                /// <param name="level">level for this node</param>
                /// <param name="octree">owning octree</param>
                internal OctreeNode(int level, Octree octree)
                {
                    this.m_octree = octree;
                    this.m_level = level;

                    // since there are only eight levels, if we get to level 7
                    // we automatically make this a leaf node
                    this.m_isLeaf = level == 7;

                    if (!this.m_isLeaf)
                    {
                        // create the child array
                        this.m_childNodes = new OctreeNode[8];

                        // add it to the tree's reducible node list
                        this.m_octree.AddReducibleNode(this);
                    }
                }

                /// <summary>Gets Level.</summary>
                internal int Level
                {
                    get { return this.m_level; }
                }

                // returns the average color for this node
                /// <summary>Gets NodeColor.</summary>
                internal Color NodeColor
                {
                    get
                    {
                        // average color is the sum of each channel divided by the pixel count
                        return Color.FromArgb(
                            this.m_redSum / this.m_pixelCount,
                            this.m_greenSum / this.m_pixelCount,
                            this.m_blueSum / this.m_pixelCount
                            );
                    }
                }

                // once we compute a palette, this will be set
                // to the palette index associated with this leaf node

                // nodes are arranged in linked lists of reducible nodes for a given level.
                // this field and property is used to traverse that list.
                /// <summary>Gets or sets NextReducibleNode.</summary>
                internal OctreeNode NextReducibleNode { get; set; }

                // child node array for this node

                /// <summary>Add the given color to this node if it is a leaf, otherwise recurse 
                /// down the appropriate child</summary>
                /// <param name="pPixel">color to add</param>
                /// <returns>true if a new color was added to the octree</returns>
                internal bool AddColor(Pixel pixel)
                {
                    var colorAdded = false;
                    if (this.m_isLeaf)
                    {
                        // increase the pixel count for this node, and if
                        // the result is 1, then this is a new color
                        colorAdded = ++this.m_pixelCount == 1;

                        // add the color to the running sum for this node
                        this.m_redSum += pixel.Red;
                        this.m_greenSum += pixel.Green;
                        this.m_blueSum += pixel.Blue;

                        // set the last node so we can quickly process adjacent pixels
                        // with the same color
                        this.m_octree.SetLastNode(this, pixel.ARGB);
                    }
                    else
                    {
                        // get the index at this level for the rgb values
                        var childIndex = this.GetChildIndex(pixel);

                        // if there is no child, add one now to the next level
                        if (this.m_childNodes[childIndex] == null)
                        {
                            this.m_childNodes[childIndex] = new OctreeNode(this.m_level + 1, this.m_octree);
                        }

                        // recurse...
                        colorAdded = this.m_childNodes[childIndex].AddColor(pixel);
                    }

                    return colorAdded;
                }

                /// <summary>Given a source color, return the palette index to use for the reduced image.
                /// Returns -1 if the color is not represented in the octree (this happens if
                /// the color has been dithered into a new color that did not appear in the 
                /// original image when the octree was formed in pass 1</summary>
                /// <param name="pPixel">source color to look up</param>
                /// <returns>palette index to use</returns>
                internal int GetPaletteIndex(Pixel pixel)
                {
                    var paletteIndex = -1;
                    if (this.m_isLeaf)
                    {
                        // use this leaf node's palette index
                        paletteIndex = this.m_paletteIndex;
                    }
                    else
                    {
                        // get the index at this level for the rgb values
                        var childIndex = this.GetChildIndex(pixel);
                        if (this.m_childNodes[childIndex] != null)
                        {
                            // recurse...
                            paletteIndex = this.m_childNodes[childIndex].GetPaletteIndex(pixel);
                        }
                    }

                    return paletteIndex;
                }

                /// <summary>Reduce this node by combining all child nodes</summary>
                /// <returns>number of nodes removed</returns>
                internal int Reduce()
                {
                    var numReduced = 0;
                    if (!this.m_isLeaf)
                    {
                        // for each child
                        for (var ndx = 0; ndx < this.m_childNodes.Length; ++ndx)
                        {
                            if (this.m_childNodes[ndx] != null)
                            {
                                var childNode = this.m_childNodes[ndx];

                                // add the pixel count from the child
                                this.m_pixelCount += childNode.m_pixelCount;

                                // add the running color sums from the child
                                this.m_redSum += childNode.m_redSum;
                                this.m_greenSum += childNode.m_greenSum;
                                this.m_blueSum += childNode.m_blueSum;

                                ++numReduced;
                            }
                        }

                        this.m_childNodes = null;
                        this.m_isLeaf = true;
                    }

                    return numReduced;
                }

                /// <summary>If this is a leaf node, add its color to the palette array at the given index
                /// and increment the index. If not a leaf, recurse the children nodes.</summary>
                /// <param name="colorArray">array of colors</param>
                /// <param name="paletteIndex">index of the next empty slot in the array</param>
                internal void AddColorsToPalette(Color[] colorArray, ref int paletteIndex)
                {
                    if (this.m_isLeaf)
                    {
                        // save our index and increment the running index
                        this.m_paletteIndex = paletteIndex++;

                        // the color for this node is the average color, which is created by
                        // dividing the running sums for each channel by the pixel count
                        colorArray[this.m_paletteIndex] = this.NodeColor;
                    }
                    else
                    {
                        // just run through all the non-null children and recurse
                        for (var ndx = 0; ndx < this.m_childNodes.Length; ++ndx)
                        {
                            if (this.m_childNodes[ndx] != null)
                            {
                                this.m_childNodes[ndx].AddColorsToPalette(colorArray, ref paletteIndex);
                            }
                        }
                    }
                }

                /// <summary>return the child index for a given color.
                /// Depends on which level this node is in.</summary>
                /// <param name="pPixel">color pixel to compute</param>
                /// <returns>child index (0-7)</returns>
                private int GetChildIndex(Pixel pixel)
                {
                    // lvl: 0 1 2 3 4 5 6 7
                    // bit: 7 6 5 4 3 2 1 0
                    var shift = 7 - this.m_level;
                    int mask = s_levelMasks[this.m_level];
                    return ((pixel.Red & mask) >> shift - 2) |
                           ((pixel.Green & mask) >> shift - 1) |
                           ((pixel.Blue & mask) >> shift);
                }
            }

            #endregion
        }

        #endregion

        #region Pixel class for ARGB values

        /// <summary>structure of a Format32bppArgb pixel in memory</summary>
        private class Pixel
        {
            /// <summary>The blue</summary>
            public byte Blue { get; set; }

            /// <summary>The green</summary>
            public byte Green { get; set; }

            /// <summary>The red</summary>
            public byte Red { get; set; }

            /// <summary>The alpha</summary>
            public byte Alpha { get; set; }

            /// <summary>The argb combination</summary>
            public int ARGB
            {
                get
                {
                    return Alpha << 24 | Red << 16 | Green << 8 | Blue;
                }
            }
        }
        #endregion
    }
}
