using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestAppMacroscop.Model
{
    public class Camera
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Camera(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
