using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using TestAppMacroscop.Model;

namespace TestAppMacroscop.Controller
{
    class CameraController
    {
        private static string _xmlinfo = @"camerainfo.xml";

        public static List<Camera> GetCameraList()
        {
            List<Camera> cameraList = new List<Camera>();

            WebClient wc = new WebClient();
            wc.DownloadFile("http://demo.macroscop.com:8080/configex?login=root", _xmlinfo);
            XDocument doc;
            using (StreamReader sr = new StreamReader(_xmlinfo, true))
            {
                doc = XDocument.Load(sr);
            }
            foreach(var node in doc.Descendants("ChannelInfo"))
            {
                string id = node.Attribute("Id").Value;
                string name = node.Attribute("Name").Value;
                var cam = new Camera(id, name);
                cameraList.Add(cam);
            }
            return cameraList;
        }
    }
}
