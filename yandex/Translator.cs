﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace yandex
{
    class Translator
    {
        public string Translate(string s)
        {
            string line = string.Empty;
            Translation a = null;
            const string api = "trnsl.1.1.20171217T153500Z.e982aab086711b47.9ad3b1d68ea1498dc93518918e939728b1afd999";
            const string link = "https://translate.yandex.net/api/v1.5/tr.json/translate";
            string url = $"{link}?key={api}&text={WebUtility.UrlEncode(s)}&lang=ru-en";

            if (!string.IsNullOrEmpty(s))
            {
                try
                {
                    WebRequest request = WebRequest.Create(url);
                    WebResponse response = request.GetResponse();
                    using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                    {
                        line = stream.ReadToEnd();
                    }
                }
                catch
                {
                    MessageBox.Show("Can't load result");
                    return null;
                }
                try
                {
                    a = JsonConvert.DeserializeObject<Translation>(line);
                }
                catch
                {
                    MessageBox.Show("Can't parse answer");
                    return null;
                }
            }
            return a.text[0];
        }
}

    class Translation
    {
        public string[] text { get; set; }
    }
}
