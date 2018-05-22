using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Media.Imaging;

namespace TestAppMacroscop.Controller
{
    public class VideoController
    {
        private Thread _thread;
        private long _frames;
        private string _uri;

        public event UpdateFrameHandler UpdateFrame;
        public delegate void UpdateFrameHandler(object sender, UpdateFrameEventArgs e);

        public class UpdateFrameEventArgs : EventArgs
        {
            private Bitmap bmp;

            public UpdateFrameEventArgs(Bitmap bmp)
            {
                this.bmp = bmp;
            }

            public Bitmap BitmapImage
            {
                get { return bmp; }
            }
        }

        public VideoController(string uri)
        {
            _thread = new Thread(VideoReading);
            _uri = uri;
            _frames = 0;
        }

        public void Start()
        {
            _thread.Start();
        }

        public void Stop()
        {
            _thread.Abort();
            _thread.Join();
        }

        private enum FrameState { FindStartFrame, FindEndFrame };

        private void VideoReading()
        {
            while (true)
            {
                var req = (HttpWebRequest)WebRequest.Create(_uri);
                var resp = req.GetResponse();

                byte[] readBuffer = new byte[1000000];
                byte[] frameBuffer = new byte[1000000];
                int offset = 0, read = 0;
                FrameState state = FrameState.FindStartFrame;
                using (Stream stream = resp.GetResponseStream())
                {
                    while ((read = stream.Read(readBuffer, offset, 1000)) > 0)
                    {
                        int posFrame = GetPosition(readBuffer, Encoding.UTF8.GetBytes("--myboundary"), offset, read);
                        offset += read;
                        if (posFrame > -1)
                        {
                            switch (state)
                            {
                                case FrameState.FindStartFrame:
                                    state = FrameState.FindEndFrame;
                                    break;
                                case FrameState.FindEndFrame:
                                    _frames++;
                                    Array.Copy(readBuffer, 181, frameBuffer, 0, posFrame);
                                    try
                                    {
                                        using (Image image = Image.FromStream(new MemoryStream(frameBuffer)))
                                        {
                                            UpdateFrame(this, new UpdateFrameEventArgs(image as Bitmap));
                                        }
                                    }
                                    catch (Exception e) { }
                                    //byte[] temp = new byte[read];
                                    Array.Copy(readBuffer, posFrame + 12, readBuffer, 0, read);
                                    offset = read;
                                    state = FrameState.FindStartFrame;
                                    break;
                            }
                        }

                    }
                }
            }
        }

        private int GetPosition(byte[] bytes, byte[] find, int startIndex, int count)
        {
            for (var i = startIndex; i + find.Length < startIndex + count; i++)
            {
                var contains = true;
                for (var j = 0; j < find.Length; j++)
                {
                    if (bytes[i + j] != find[j])
                    {
                        contains = false;
                        break;
                    }
                }

                if (contains)
                {
                    return i - find.Length;
                }
            }
            return -1;
        }
    }
}
