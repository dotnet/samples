//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raytracer
{
    public partial class MainForm : Form
    {
        public MainForm() => InitializeComponent();

        private bool _showThreads;
        private bool _isParallel;
        private int _degreeOfParallelism = Environment.ProcessorCount;
        private CancellationTokenSource _cancellation;

        private int _width, _height;
        private Bitmap _bitmap;
        private Rectangle _rect;
        private ObjectPool<int[]> _freeBuffers;

        private void OnStartButtonClick(object sender, EventArgs e)
        {
            // If we already have the rendering task created, then we're currently running.
            // In that case, stop the renderer.
            if (_cancellation != null)
            {
                _startButton.Enabled = false;
                _cancellation.Cancel();
            }
            else
            {
                // Set up the image in the picture box and start the rendering loop with a new rendering task
                ConfigureImage();
                _showThreads = _showThreadsCheckBox.Checked;
                _cancellation = new CancellationTokenSource();
                Task.Factory.StartNew(RenderLoop,
                    _cancellation.Token, _cancellation.Token)
                    .ContinueWith(antedecent =>
                    {
                        _isParallelCheckBox.Enabled = true;
                        _showThreadsCheckBox.Enabled = _isParallelCheckBox.Checked;
                        _startButton.Enabled = true;
                        _startButton.Text = "Start";
                        _cancellation = null;
                    }, TaskScheduler.FromCurrentSynchronizationContext());

                _showThreadsCheckBox.Enabled = false;
                _isParallelCheckBox.Enabled = false;
                _startButton.Text = "Stop";
            }
        }

        private void ConfigureImage()
        {
            // If we need to create a new bitmap, do so
            if (_bitmap == null || _bitmap.Width != _renderedImage.Width || _bitmap.Height != _renderedImage.Height)
            {
                // Dispose of the old one if one exists
                if (_bitmap != null)
                {
                    _renderedImage.Image = null;
                    _bitmap.Dispose();
                }

                // We always render a square even if the window isn't square
                _width = _height = Math.Min(_renderedImage.Width, _renderedImage.Height);

                // Create a new object pool for the rendering arrays
                _freeBuffers = new ObjectPool<int[]>(() => new int[_width * _height]);

                // Create a new Bitmap and set it into the picture box
                _bitmap = new Bitmap(_width, _height, PixelFormat.Format32bppRgb);
                _rect = new Rectangle(0, 0, _width, _height);
                _renderedImage.Image = _bitmap;
            }
        }

        private void RenderLoop(object boxedToken)
        {
            var cancellationToken = (CancellationToken)boxedToken;

            // Create a ray tracer, and create a reference to "sphere2" that we are going to bounce
            var rayTracer = new RayTracer(_width, _height);
            var scene = rayTracer._defaultScene;
            var sphere2 = (Sphere)scene.Things[0]; // The first item is assumed to be our sphere
            var baseY = sphere2.Radius;
            sphere2.Center.Y = sphere2.Radius;

            // Timing determines how fast the ball bounces as well as diagnostics frames/second info
            var renderingTime = new Stopwatch();
            var totalTime = Stopwatch.StartNew();

            // Keep rendering until the rendering task has been canceled
            while (!cancellationToken.IsCancellationRequested)
            {
                // Get the next buffer
                var rgb = _freeBuffers.GetObject();

                // Determine the new position of the sphere based on the current time elapsed
                double dy2 = 0.8 * Math.Abs(Math.Sin(totalTime.ElapsedMilliseconds * Math.PI / 3000));
                sphere2.Center.Y = baseY + dy2;

                // Render the scene
                renderingTime.Reset();
                renderingTime.Start();

                var options = new ParallelOptions
                {
                    MaxDegreeOfParallelism = _degreeOfParallelism,
                    CancellationToken = _cancellation.Token
                };
                if (!_isParallel)
                {
                    rayTracer.RenderSequential(scene, rgb);
                }
                else if (_showThreads)
                {
                    rayTracer.RenderParallelShowingThreads(scene, rgb, options);
                }
                else
                {
                    rayTracer.RenderParallel(scene, rgb, options);
                }

                renderingTime.Stop();

                // Update the bitmap in the UI thread
                //var framesPerSecond = (++frame * 1000.0 / renderingTime.ElapsedMilliseconds);
                var framesPerSecond = 1000.0 / renderingTime.ElapsedMilliseconds;
                BeginInvoke((Action)delegate
                {
                    // Copy the pixel array into the bitmap
                    var bmpData = _bitmap.LockBits(_rect, ImageLockMode.WriteOnly, _bitmap.PixelFormat);
                    Marshal.Copy(rgb, 0, bmpData.Scan0, rgb.Length);
                    _bitmap.UnlockBits(bmpData);
                    _freeBuffers.PutObject(rgb);

                    // Refresh the UI
                    _renderedImage.Invalidate();
                    Text = $"Ray Tracer - FPS: {framesPerSecond:F1}";
                });
            }
        }

        private void OnIsParallelCheckBoxChanged(object sender, EventArgs e) =>
            _isParallel =
                _numberOfProcsLabel.Enabled =
                _numberOfProcsTrackBar.Enabled =
                _showThreadsCheckBox.Enabled =
                _isParallelCheckBox.Checked;

        private void OnNumberOfProcsTrackBarChanged(object sender, EventArgs e)
        {
            _numberOfProcsLabel.Text = _numberOfProcsTrackBar.Value.ToString();
            _degreeOfParallelism = _numberOfProcsTrackBar.Value;
        }

        private void OnMainFormLoaded(object sender, EventArgs e)
        {
            _numberOfProcsTrackBar.Minimum = 1;
            _numberOfProcsTrackBar.Maximum = Environment.ProcessorCount;
            _numberOfProcsTrackBar.Value = _numberOfProcsTrackBar.Maximum;
            _numberOfProcsLabel.Text = _numberOfProcsTrackBar.Value.ToString();
        }
    }
}
