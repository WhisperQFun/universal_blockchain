﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using universal_blockchain.Nodes;
using System.Text.RegularExpressions;

namespace universal_blockchain
{
    public class Settings
    {
        public Node node = new Node();
        string path = GetApplicationRoot() + "\\";

        public void save(Node node)
        {
            using (StreamWriter file = File.CreateText(path+"settings.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, node);
            }

        }

        public void load()
        {
            using (StreamReader file = File.OpenText(path+"settings.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                this.node = (Node)serializer.Deserialize(file, typeof(Node));
                
            }
        }

        public static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                              .Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }
    }


}