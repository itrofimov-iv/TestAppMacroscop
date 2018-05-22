using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text;
using TestAppMacroscop.Controller;
using System.Windows.Threading;
using TestAppMacroscop.Model;

namespace TestAppMacroscop.View
{
    /// <summary>
    /// Логика взаимодействия для TestApp.xaml
    /// </summary>
    public partial class TestApp : Window
    {
        public TestApp()
        {
            InitializeComponent();
            lvCams.ItemsSource = CameraController.GetCameraList();
            lvCams.SelectedIndex = 0;
        }

        private VideoController cameraController;

        private void lvCams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cameraController != null)
                cameraController.Stop();
            Camera selCam = lvCams.SelectedItem as Camera;
            string uri = "http://demo.macroscop.com:8080/mobile?login=root&channelid=" + selCam.Id + "&resolutionX=640&resolutionY=480&fps=25";
            cameraController = new VideoController(uri);
            cameraController.UpdateFrame += GetFrame;
            cameraController.Start();
        }

        public void GetFrame(object sender, VideoController.UpdateFrameEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    video.Source = BitmapToImageSource(e.BitmapImage);
                }
                catch(Exception ex) { }
            }), DispatcherPriority.ContextIdle);
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                    memory.Position = 0;
                    BitmapImage bitmapimage = new BitmapImage();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memory;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();
                    return bitmapimage;
                }
            
        }
    }
}
