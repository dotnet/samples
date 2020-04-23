//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Microsoft.Drawing
{
    public struct PixelData
    {
        public byte B;
        public byte G;
        public byte R;
    }

    public unsafe class FastBitmap : IDisposable
    {
        private readonly Bitmap _bitmap;
        private int _width;
        private BitmapData _bitmapData = null;
        private byte* _pBase = null;
        private PixelData* _pInitPixel = null;
        private Point _size;
        private bool _locked = false;

        public FastBitmap(Bitmap bmp)
        {
            _bitmap = bmp ?? throw new ArgumentNullException(nameof(bmp));
            _size = new Point(bmp.Width, bmp.Height);

            LockBitmap();
        }

        public PixelData* GetInitialPixelForRow(int rowNumber) => (PixelData*)(_pBase + rowNumber * _width);

        public PixelData* this[int x, int y] => (PixelData*)(_pBase + y * _width + x * sizeof(PixelData));

        public Color GetColor(int x, int y)
        {
            PixelData* data = this[x, y];
            return Color.FromArgb(data->R, data->G, data->B);
        }

        public void SetColor(int x, int y, Color c)
        {
            PixelData* data = this[x, y];
            data->R = c.R;
            data->G = c.G;
            data->B = c.B;
        }

        private void LockBitmap()
        {
            if (_locked) throw new InvalidOperationException("Already locked");

            var bounds = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);

            // Figure out the number of bytes in a row. This is rounded up to be a multiple 
            // of 4 bytes, since a scan line in an image must always be a multiple of 4 bytes
            // in length. 
            _width = bounds.Width * sizeof(PixelData);
            if (_width % 4 != 0) _width = 4 * (_width / 4 + 1);

            _bitmapData = _bitmap.LockBits(bounds, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            _pBase = (byte*)_bitmapData.Scan0.ToPointer();
            _locked = true;
        }

        private void InitCurrentPixel() => _pInitPixel = (PixelData*)_pBase;

        private void UnlockBitmap()
        {
            if (!_locked) throw new InvalidOperationException("Not currently locked");

            _bitmap.UnlockBits(_bitmapData);
            _bitmapData = null;
            _pBase = null;
            _locked = false;
        }

        public void Dispose()
        {
            if (_locked) UnlockBitmap();
        }
    }
}
