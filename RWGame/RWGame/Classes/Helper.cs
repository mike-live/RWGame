﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RWGame.Classes
{
    class Helper
    {
        public static Stream getResourceStream(String filename)
        {
            //Assembly assembly = GetType().GetTypeInfo().Assembly;
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceID = assembly.GetManifestResourceNames()
                                .Where(name => name.Contains(filename))
                                .FirstOrDefault();
            return assembly.GetManifestResourceStream(resourceID);
        }
    }
}
