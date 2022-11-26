using System;
using System.Net;
using System.IO;
using System.Text;
using System.Text.Json;
using NLua;

namespace Scruffy
{
    internal class ScruffyServer
    {
        public static void Request(HttpListener server, string config)
        {
            HttpListenerContext context = server.GetContext();
            HttpListenerResponse response = context.Response;

            string text = "404";

            string url = context.Request.RawUrl;

            if (File.Exists(@"./public" + url))
            {
                response.StatusCode = 200;
                text = ParseFile(@"./public" + url);
            }
            else
            {
                bool found = false;
                string[] alias = config.Split("{alias")[1].Split(";");
                foreach (var ali in alias)
                {
                    if (ali.Split("->")[0].Trim() == url)
                    {
                        found = true;

                        url = ali.Split("->")[1].Trim();
                        response.StatusCode = 200;
                        text = ParseFile(@"./public" + url);
                        break;
                    }
                }
                if (!found)
                {
                    response.StatusCode = 404;
                    text = "404";
                }
            }

            byte[] buffer = Encoding.UTF8.GetBytes(text);

            Console.WriteLine("Requested " + context.Request.RawUrl);


            Console.WriteLine("Returned " + url + " : " + response.StatusCode);

            response.ContentLength64 = buffer.Length;
            Stream st = response.OutputStream;
            st.Write(buffer, 0, buffer.Length);

            context.Response.Close();
        }
        public static string ParseFile(string file)
        {
            string text = "ERROR: parse file failed";
            switch (file.Split(".").Last())
            {
                case "scru":
                    text = ParseScruffy(file);
                    break;
                default:
                    text = File.ReadAllText(file);
                    break;
            }
            return text;
        }
        public static string ParseScruffy(string file)
        {
            string read_buffer = File.ReadAllText(file);
            string buffer = "";

            bool lua = false;

            Lua state = new Lua ();

            LuaScruffy luaScruffy = new LuaScruffy();

            state["scruffy"] = luaScruffy;

            foreach (var segment in read_buffer.Split("{scruffy}"))
            {
                if(lua)
                {
                    state.DoString(segment);

                    luaScruffy = (LuaScruffy)state["scruffy"];

                    buffer += luaScruffy.standardOutput;
                }else
                {
                    buffer += segment;
                }

                lua = !lua;
            }


            return buffer;
        }
        public class LuaScruffy
        {
            public string standardOutput = "";
            public void echo(string a)
            {
                standardOutput += a;
            }
        }
    }
}