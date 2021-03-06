/*
 * MIT License
 * 
 * Copyright (c) 2017 Michael VanOverbeek and ShiftOS devs
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShiftOS.Objects;
using NetSockets;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using Nancy.Hosting.Self;
using Nancy;
using Nancy.Authentication.Basic;
using Nancy.Security;
using Nancy.TinyIoc;
using Nancy.Bootstrapper;

namespace ShiftOS.Server
{
    public interface IUserMapper
    {
        /// <summary>
        /// Get the real username from an identifier
        /// </summary>
        /// <param name="identifier">User identifier</param>
        /// <param name="context">The current NancyFx context</param>
        /// <returns>Matching populated IUserIdentity object, or empty</returns>
        IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context);
    }

    public class MUDUserValidator : IUserValidator
    {
        public IUserIdentity Validate(string username, string password)
        {
            if(username == Program.AdminUsername && password == Program.AdminPassword)
            {
                return null;
            }
            else
            {
                return null;
            }
        }
    }

    public class MUDUserIdentity : IUserIdentity
    {
        public IEnumerable<string> Claims
        {
            get
            {
                return null;
            }
        }

        public string UserName
        {
            get
            {
                return uname;
            }
        }

        public string uname = "";

        public MUDUserIdentity(string username)
        {
            uname = username;
        }
    }

    public class AuthenticationBootstrapper : DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            pipelines.EnableBasicAuthentication(new BasicAuthenticationConfiguration(
                container.Resolve<MUDUserValidator>(),
                "MUD", UserPromptBehaviour.NonAjax));
        }
    }

    public class Program
    {
        public static string AdminUsername = "admin";
        public static string AdminPassword = "admin";



        public static NetObjectServer server;

        public delegate void StringEventHandler(string str);

        public static event StringEventHandler ServerStarted;

        public static void SaveChats()
        {
            List<Channel> saved = new List<Channel>();
            foreach(var chat in chats)
            {
                saved.Add(new Channel
                {
                    ID = chat.ID,
                    Name = chat.Name,
                    MaxUsers = chat.MaxUsers,
                    Topic = chat.Topic,
                    Users = new List<Save>()
                });
            }
            File.WriteAllText("chats.json", JsonConvert.SerializeObject(saved));
        }

        public static void LoadChats()
        {
            chats = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Channel>>(File.ReadAllText("chats.json"));
        }

        public static void Main(string[] args)
        {
            if (!Directory.Exists("saves"))
            {
                Directory.CreateDirectory("saves");
            }

            if(!File.Exists("chats.json"))
            {
                SaveChats();
            }
            else
            {
                LoadChats();
            }

            if(!Directory.Exists("scripts"))
            {
                Console.WriteLine("Creating scripts directory...");
                Directory.CreateDirectory("scripts");
                Console.WriteLine("NOTE: This MUD is not just gonna generate scripts for you. You're going to need to write them. YOU are DevX.");
            }

            Console.WriteLine("Starting server...");
            server = new NetObjectServer();

            server.OnStarted += (o, a) =>
            {
                Console.WriteLine($"Server started on address {server.Address}, port {server.Port}.");
                ServerStarted?.Invoke(server.Address.MapToIPv4().ToString());
            };

            server.OnStopped += (o, a) =>
            {
                Console.WriteLine("WARNING! Server stopped.");
            };

            server.OnError += (o, a) =>
            {
                Console.WriteLine("ERROR: " + a.Exception.Message);
            };

            server.OnClientAccepted += (o, a) =>
            {
                Console.WriteLine("Client connected.");
                server.DispatchTo(a.Guid, new NetObject("welcome", new ServerMessage { Name = "Welcome", Contents = a.Guid.ToString(), GUID = "Server" }));
            };

            server.OnReceived += (o, a) =>
            {
                var obj = a.Data.Object;

                var msg = obj as ServerMessage;

                if(msg != null)
                {
                    Interpret(msg);
                }
            };

            IPAddress defaultAddress = null;

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    defaultAddress = ip;
                }
            }

            try
            {
                server.Start(defaultAddress, 13370);
            }
            catch
            {
                Console.WriteLine("So we tried to bind the server to your IP address automatically, but your operating system or the .NET environment you are in (possibly Mono on Linux) is preventing us from doing so. We'll try to bind to the loopback IP address (127.0.0.1) and if that doesn't work, the multi-user domain software may not be compatible with this OS or .NET environment.");
                server.Stop();
                server.Start(IPAddress.Loopback, 13370);

            }

            var hConf = new HostConfiguration();
            hConf.UrlReservations.CreateAutomatically = true;

            var nancy = new NancyHost(hConf, new Uri("http://localhost:13371/"));

            server.OnStopped += (o, a) =>
            {
                nancy.Stop();
            };

            nancy.Start();
        }

        public static bool UserInChat(Channel chan, Save user)
        {
            foreach(var usr in chan.Users)
            {
                if(usr.Username == user.Username)
                {
                    return true;
                }
            }
            return false;
        }



        public static void Interpret(ServerMessage msg)
        {
            Dictionary<string, object> args = null;

            try
            {
                Console.WriteLine($@"Message received from {msg.GUID}: {msg.Name}

Contents:
{msg.Contents}");

                if (!string.IsNullOrWhiteSpace(msg.Contents))
                {
                    try
                    {
                        //It's gotta be JSON.
                        if (msg.Contents.StartsWith("{"))
                        {
                            args = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg.Contents);
                        }
                    }
                    catch
                    {
                        //Damnit, we were wrong.
                        args = null;
                    }
                }

                switch (msg.Name)
                {
                    case "mud_login":
                        if (args["username"] != null && args["password"] != null)
                        {
                            foreach(var savefile in Directory.GetFiles("saves"))
                            {
                                try
                                {
                                    var save = JsonConvert.DeserializeObject<Save>(File.ReadAllText(savefile));

                                    if(save.Username == args["username"].ToString() && save.Password == args["password"].ToString())
                                    {
                                        server.DispatchTo(new Guid(msg.GUID), new NetObject("mud_savefile", new ServerMessage
                                        {
                                            Name = "mud_savefile",
                                            GUID = "server",
                                            Contents = File.ReadAllText(savefile)
                                        }));
                                        return;
                                    }
                                }
                                catch { }
                            }
                            server.DispatchTo(new Guid(msg.GUID), new NetObject("auth_failed", new ServerMessage
                            {
                                Name = "mud_login_denied",
                                GUID = "server"
                            }));
                        }
                        else
                        {
                            server.DispatchTo(new Guid(msg.GUID), new NetObject("auth_failed", new ServerMessage
                            {
                                Name = "mud_login_denied",
                                GUID = "server"
                            }));
                        }
                        break;
                    case "legion_create":
                        List<Legion> legions = new List<Legion>();
                        if (File.Exists("legions.json"))
                            legions = JsonConvert.DeserializeObject<List<Legion>>(File.ReadAllText("legions.json"));

                        var l = JsonConvert.DeserializeObject<Legion>(msg.Contents);

                        legions.Add(l);

                        File.WriteAllText("legions.json", JsonConvert.SerializeObject(legions, Formatting.Indented));
                        break;
                    case "legion_get_all":
                        List<Legion> allLegions = new List<Legion>();

                        if (File.Exists("legions.json"))
                            allLegions = JsonConvert.DeserializeObject<List<Legion>>(File.ReadAllText("legions.json"));

                        server.DispatchTo(new Guid(msg.GUID), new NetObject("alllegions", new ServerMessage
                        {
                            Name = "legion_all",
                            GUID = "server",
                            Contents = JsonConvert.SerializeObject(allLegions)
                        }));
                        break;
                    case "legion_get_users":
                        var lgn = JsonConvert.DeserializeObject<Legion>(msg.Contents);

                        List<string> userIDs = new List<string>();

                        foreach (var savfile in Directory.GetFiles("saves"))
                        {
                            try
                            {
                                var savefilecontents = JsonConvert.DeserializeObject<Save>(File.ReadAllText(savfile));
                                if (savefilecontents.CurrentLegions.Contains(lgn.ShortName))
                                {
                                    userIDs.Add($"{savefilecontents.Username}@{savefilecontents.SystemName}");
                                }
                            }
                            catch { }
                        }

                        server.DispatchTo(new Guid(msg.GUID), new NetObject("userlist", new ServerMessage
                        {
                            Name = "legion_users_found",
                            GUID = "server",
                            Contents = JsonConvert.SerializeObject(userIDs)
                        }));
                        break;
                    case "user_get_legion":
                        var userSave = JsonConvert.DeserializeObject<Save>(msg.Contents);

                        if (File.Exists("legions.json"))
                        {
                            var legionList = JsonConvert.DeserializeObject<List<Legion>>(File.ReadAllText("legions.json"));
                            foreach (var legion in legionList)
                            {
                                if (userSave.CurrentLegions.Contains(legion.ShortName))
                                {
                                    server.DispatchTo(new Guid(msg.GUID), new NetObject("reply", new ServerMessage
                                    {
                                        Name = "user_legion",
                                        GUID = "server",
                                        Contents = JsonConvert.SerializeObject(legion)
                                    }));
                                    return;
                                }
                            }
                        }

                        server.DispatchTo(new Guid(msg.GUID), new NetObject("fuck", new ServerMessage
                        {
                            Name = "user_not_found_in_legion",
                            GUID = "server"
                        }));

                        break;
                    case "mud_save":
                        var sav = JsonConvert.DeserializeObject<Save>(msg.Contents);
                        File.WriteAllText("saves/" + sav.Username + ".save", JsonConvert.SerializeObject(sav, Formatting.Indented));

                        server.DispatchTo(new Guid(msg.GUID), new NetObject("auth_failed", new ServerMessage
                        {
                            Name = "mud_saved",
                            GUID = "server"
                        }));

                        break;
                    case "mud_checkuserexists":
                        if (args["username"] != null && args["password"] != null)
                        {
                            foreach (var savefile in Directory.GetFiles("saves"))
                            {
                                try
                                {
                                    var save = JsonConvert.DeserializeObject<Save>(File.ReadAllText(savefile));

                                    if (save.Username == args["username"].ToString() && save.Password == args["password"].ToString())
                                    {
                                        server.DispatchTo(new Guid(msg.GUID), new NetObject("mud_savefile", new ServerMessage
                                        {
                                            Name = "mud_found",
                                            GUID = "server",
                                        }));
                                        return;
                                    }
                                }
                                catch { }
                            }
                            server.DispatchTo(new Guid(msg.GUID), new NetObject("auth_failed", new ServerMessage
                            {
                                Name = "mud_notfound",
                                GUID = "server"
                            }));
                        }
                        else
                        {
                            server.DispatchTo(new Guid(msg.GUID), new NetObject("auth_failed", new ServerMessage
                            {
                                Name = "mud_notfound",
                                GUID = "server"
                            }));
                        }
                        break;
                        break;
                    case "pong_gethighscores":
                        if (File.Exists("pong_highscores.json"))
                        {
                            server.DispatchTo(new Guid(msg.GUID), new NetObject("pongstuff", new ServerMessage
                            {
                                Name = "pong_highscores",
                                GUID = "server",
                                Contents = File.ReadAllText("pong_highscores.json")
                            }));
                        }
                        break;
                    case "get_memos_for_user":
                        if(args["username"] != null)
                        {
                            string usrname = args["username"].ToString();

                            List<MUDMemo> mmos = new List<MUDMemo>();

                            if (File.Exists("memos.json"))
                            {
                                foreach(var mmo in JsonConvert.DeserializeObject<MUDMemo[]>(File.ReadAllText("memos.json")))
                                {
                                    if(mmo.UserTo == usrname)
                                    {
                                        mmos.Add(mmo);
                                    }
                                }
                            }

                            server.DispatchTo(new Guid(msg.GUID), new NetObject("mud_memos", new ServerMessage
                            {
                                Name = "mud_usermemos",
                                GUID = "server",
                                Contents = JsonConvert.SerializeObject(mmos)
                            }));
                        }
                        break;
                    case "mud_post_memo":
                        MUDMemo memo = JsonConvert.DeserializeObject<MUDMemo>(msg.Contents);
                        List<MUDMemo> memos = new List<MUDMemo>();

                        if (File.Exists("memos.json"))
                            memos = JsonConvert.DeserializeObject<List<MUDMemo>>(File.ReadAllText("memos.json"));

                        memos.Add(memo);
                        File.WriteAllText("memos.txt", JsonConvert.SerializeObject(memos));


                        break;
                    case "pong_sethighscore":
                        var hs = new List<PongHighscore>();
                        if (File.Exists("pong_highscores.json"))
                            hs = JsonConvert.DeserializeObject<List<PongHighscore>>(File.ReadAllText("ponghighscores.json"));

                        var newHS = JsonConvert.DeserializeObject<PongHighscore>(msg.Contents);
                        for (int i = 0; i <= hs.Count; i++)
                        {
                            try
                            {
                                if (hs[i].UserName == newHS.UserName)
                                {
                                    if (newHS.HighestLevel > hs[i].HighestLevel)
                                        hs[i].HighestLevel = newHS.HighestLevel;
                                    if (newHS.HighestCodepoints > hs[i].HighestCodepoints)
                                        hs[i].HighestCodepoints = newHS.HighestCodepoints;
                                    File.WriteAllText("pong_highscores.json", JsonConvert.SerializeObject(hs));
                                    return;

                                }
                            }
                            catch
                            {

                            }
                        }
                        hs.Add(newHS);
                        File.WriteAllText("pong_highscores.json", JsonConvert.SerializeObject(hs));
                        return;
                    case "getvirusdb":
                        if (!File.Exists("virus.db"))
                            File.WriteAllText("virus.db", "{}");

                        server.DispatchTo(new Guid(msg.GUID), new NetObject("vdb", new ServerMessage
                        {
                            Name = "virusdb",
                            GUID = "server",
                            Contents = File.ReadAllText("virus.db")
                        }));
                        break;
                    case "getvirus":
                        Dictionary<string, string> virusDB = new Dictionary<string, string>();

                        if (File.Exists("virus.db"))
                            virusDB = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("virus.db"));

                        foreach (var kv in virusDB)
                        {
                            if (kv.Key == msg.Contents)
                            {
                                server.DispatchTo(new Guid(msg.GUID), new NetObject("response", new ServerMessage
                                {
                                    Name = "mud_virus",
                                    GUID = "server",
                                    Contents = kv.Value,
                                }));
                                return;
                            }
                        }


                        break;
                    case "mud_scanvirus":
                        Dictionary<string, string> _virusDB = new Dictionary<string, string>();

                        bool addIfNotFound = true;

                        if (msg.Contents.Contains("||scanonly"))
                            addIfNotFound = false;

                        msg.Contents = msg.Contents.Replace("||scanonly", "");

                        if(File.Exists("virus.db"))
                            _virusDB = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("virus.db"));

                        foreach(var kv in _virusDB)
                        {
                            if(kv.Value == msg.Contents)
                            {
                                server.DispatchTo(new Guid(msg.GUID), new NetObject("response", new ServerMessage
                                {
                                    Name = "mud_virus_signature",
                                    GUID = "server",
                                    Contents = kv.Key,
                                }));
                                return;
                            }
                        }

                        if (addIfNotFound == true)
                        {
                            string newguid = Guid.NewGuid().ToString();
                            _virusDB.Add(newguid, msg.Contents);
                            File.WriteAllText("virus.db", JsonConvert.SerializeObject(_virusDB, Formatting.Indented));
                            server.DispatchTo(new Guid(msg.GUID), new NetObject("response", new ServerMessage
                            {
                                Name = "mud_virus_signature",
                                GUID = "server",
                                Contents = newguid,
                            }));
                        }
                        else
                        {
                            server.DispatchTo(new Guid(msg.GUID), new NetObject("response", new ServerMessage
                            {
                                Name = "mud_virus_signature",
                                GUID = "server",
                                Contents = "unknown",
                            }));
                        }
                        return;
                        
                    case "chat_join":
                        if (args.ContainsKey("id"))
                        {
                            var cuser = new Save();
                            if (args.ContainsKey("user"))
                            {
                                cuser = JsonConvert.DeserializeObject<Save>(JsonConvert.SerializeObject(args["user"]));


                            }
                            int index = -1;
                            string chat_id = args["id"] as string;
                            foreach(var chat in chats)
                            {
                                if(chat.ID == chat_id)
                                {
                                    if(chat.Users.Count < chat.MaxUsers || chat.MaxUsers == 0)
                                    {
                                        //user can join chat.
                                        if(cuser != null)
                                        {
                                            index = chats.IndexOf(chat);
                                        }
                                    }   
                                }
                            }
                            if(index > -1)
                            {
                                chats[index].Users.Add(cuser);
                                server.DispatchAll(new NetObject("broadcast", new ServerMessage
                                {
                                    Name = "cbroadcast",
                                    GUID = "server",
                                    Contents = $"{chat_id}: {cuser.Username} {{CHAT_HAS_JOINED}}"
                                }));
                            }
                            else
                            {
                                server.DispatchTo(new Guid(msg.GUID), new NetObject("broadcast", new ServerMessage
                                {
                                    Name = "cbroadcast",
                                    GUID = "server",
                                    Contents = $"{chat_id}: {{CHAT_NOT_FOUND_OR_TOO_MANY_MEMBERS}}"
                                }));
                            }
                        }
                        break;
                    case "chat":
                        if (args.ContainsKey("id"))
                        {
                            var cuser = new Save();
                            if (args.ContainsKey("user"))
                            {
                                cuser = JsonConvert.DeserializeObject<Save>(JsonConvert.SerializeObject(args["user"]));


                            }
                            string message = "";
                            if (args.ContainsKey("msg"))
                                message = args["msg"] as string;

                            int index = -1;
                            string chat_id = args["id"] as string;
                            foreach (var chat in chats)
                            {
                                if (chat.ID == chat_id)
                                {
                                    if (cuser != null && !string.IsNullOrWhiteSpace(message) && UserInChat(chat, cuser))
                                    {
                                        index = chats.IndexOf(chat);
                                    }
                                }
                            }
                            if (index > -1)
                            {
                                server.DispatchAll(new NetObject("broadcast", new ServerMessage
                                {
                                    Name = "cbroadcast",
                                    GUID = "server",
                                    Contents = $"{chat_id}/{cuser.Username}: {message}"
                                }));
                            }
                            else
                            {
                                server.DispatchTo(new Guid(msg.GUID), new NetObject("broadcast", new ServerMessage
                                {
                                    Name = "cbroadcast",
                                    GUID = "server",
                                    Contents = $"{chats[index].ID}: {{CHAT_NOT_FOUND_OR_NOT_IN_CHAT}}"
                                }));
                            }
                        }

                        break;
                    case "chat_leave":
                        if (args.ContainsKey("id"))
                        {
                            var cuser = new Save();
                            if (args.ContainsKey("user"))
                            {
                                cuser = JsonConvert.DeserializeObject<Save>(JsonConvert.SerializeObject(args["user"]));


                            }
                            int index = -1;
                            string chat_id = args["id"] as string;
                            foreach (var chat in chats)
                            {
                                if (chat.ID == chat_id)
                                {
                                    if (cuser != null && UserInChat(chat, cuser))
                                    {
                                        index = chats.IndexOf(chat);
                                    }
                                }
                            }
                            if (index > -1)
                            {
                                server.DispatchAll(new NetObject("broadcast", new ServerMessage
                                {
                                    Name = "cbroadcast",
                                    GUID = "server",
                                    Contents = $"{chats[index].ID}: {cuser.Username} {{HAS_LEFT_CHAT}}"
                                }));
                            }
                            else
                            {
                                server.DispatchTo(new Guid(msg.GUID), new NetObject("broadcast", new ServerMessage
                                {
                                    Name = "cbroadcast",
                                    GUID = "server",
                                    Contents = $"{chat_id}: {{CHAT_NOT_FOUND_OR_NOT_IN_CHAT}}"
                                }));
                            }
                        }
                        break;
                    case "chat_create":
                        string id = "";
                        string topic = "";
                        string name = "";
                        int max_users = 0;

                        if (args.ContainsKey("id"))
                            id = args["id"] as string;
                        if (args.ContainsKey("topic"))
                            name = args["topic"] as string;
                        if (args.ContainsKey("name"))
                            topic = args["name"] as string;
                        if (args.ContainsKey("max_users"))
                            max_users = Convert.ToInt32(args["max_users"].ToString());

                        bool id_taken = false;

                        foreach(var chat in chats)
                        {
                            if (chat.ID == id)
                                id_taken = true;
                        }

                        if (id_taken == false)
                        {
                            chats.Add(new Channel
                            {
                                ID = id,
                                Name = name,
                                Topic = topic,
                                MaxUsers = max_users,
                                Users = new List<Save>()
                            });
                            SaveChats();
                            server.DispatchTo(new Guid(msg.GUID), new NetObject("broadcast", new ServerMessage
                            {
                                Name = "cbroadcast",
                                GUID = "server",
                                Contents = $"{id}: {{SUCCESSFULLY_CREATED_CHAT}}"
                            }));
                        }
                        else
                        {
                            server.DispatchTo(new Guid(msg.GUID), new NetObject("broadcast", new ServerMessage
                            {
                                Name = "cbroadcast",
                                GUID = "server",
                                Contents = $"{id}: {{ID_TAKEN}}"
                            }));
                        }

                        break;
                    case "broadcast":
                        string text = msg.Contents;
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            server.DispatchTo(new Guid(msg.GUID), new NetObject("runme", new ServerMessage
                            {
                                Name = "broadcast",
                                GUID = "Server",
                                Contents = text

                            }));
                        }
                        break;
                    case "lua_up":
                        string lua = msg.Contents;
                        string firstLine = lua.Split(new[] { Environment.NewLine }, StringSplitOptions.None)[0];
                        firstLine = firstLine.Remove(0, 3); //delete the comment
                        string[] a = firstLine.Split('.');
                        if(!Directory.Exists("scripts/" + a[0]))
                        {
                            Directory.CreateDirectory($"scripts/{a[0]}");
                        }
                        File.WriteAllText($"scripts/{a[0]}/{a[1]}.lua", lua);
                        break;
                    case "mudhack_init":
                        if (MUDHackPasswords.ContainsKey(msg.GUID))
                            MUDHackPasswords.Remove(msg.GUID);

                            MUDHackPasswords.Add(msg.GUID, GenerateRandomPassword());
                        

                        server.DispatchTo(new Guid(msg.GUID), new NetObject("mudhack_init", new ServerMessage
                        {
                            Name = "mudhack_init",
                            GUID = "SERVER",
                            Contents = MUDHackPasswords[msg.GUID],
                        }));

                        break;
                    case "mudhack_verify":
                        if (!MUDHackPasswords.ContainsKey(msg.GUID))
                        {

                            server.DispatchTo(new Guid(msg.GUID), new NetObject("mudhack_init", new ServerMessage
                            {
                                Name = "server_error",
                                GUID = "SERVER",
                                Contents = "{SRV_HACK_NOT_INITIATED}",
                            }));
                            return;
                        }

                        string pass = "";
                        if (args.ContainsKey("pass"))
                            pass = args["pass"] as string;

                        if(pass == MUDHackPasswords[msg.GUID])
                        {
                            server.DispatchTo(new Guid(msg.GUID), new NetObject("mudhack_init", new ServerMessage
                            {
                                Name = "mudhack_granted",
                                GUID = "SERVER",
                            }));

                        }
                        else
                        {

                            server.DispatchTo(new Guid(msg.GUID), new NetObject("mudhack_init", new ServerMessage
                            {
                                Name = "mudhack_denied",
                                GUID = "SERVER",
                            }));
                        }
                        break;
                    case "mudhack_killpass":
                        if (MUDHackPasswords.ContainsKey(msg.GUID))
                            MUDHackPasswords.Remove(msg.GUID);
                        break;
                    case "mudhack_getallusers":
                        List<OnlineUser> users = new List<OnlineUser>();

                        foreach (var chat in chats)
                        {
                            foreach(var usr in chat.Users)
                            {
                                var ousr = new OnlineUser();
                                ousr.Username = usr.Username;
                                ousr.OnlineChat = chat.ID;
                                users.Add(ousr);
                            }
                        }

                        server.DispatchTo(new Guid(msg.GUID), new NetObject("mudhack_users", new ServerMessage
                        {
                            Name = "mudhack_users",
                            GUID = "SERVER",
                            Contents = JsonConvert.SerializeObject(users),
                        }));
                        break;
                    case "getguid_reply":
                        msg.GUID = "server";
                        //The message's GUID was manipulated by the client to send to another client.
                        //So we can just bounce back the message to the other client.
                        server.DispatchTo(new Guid(msg.GUID), new NetObject("bounce", msg));
                        break;
                    case "getguid_send":
                        string username = msg.Contents;
                        string guid = msg.GUID;
                        server.DispatchAll(new NetObject("are_you_this_guy", new ServerMessage
                        {
                            Name = "getguid_fromserver",
                            GUID = guid,
                            Contents = username,
                        }));
                        break;
                    case "script":
                        string user = "";
                        string script = "";
                        string sArgs = "";

                        if (!args.ContainsKey("user"))
                            throw new Exception("No 'user' arg specified in message to server");

                        if (!args.ContainsKey("script"))
                            throw new Exception("No 'script' arg specified in message to server");

                        if (!args.ContainsKey("args"))
                            throw new Exception("No 'args' arg specified in message to server");

                        user = args["user"] as string;
                        script = args["script"] as string;
                        sArgs = args["args"] as string;

                        if(File.Exists($"scripts/{user}/{script}.lua"))
                        {
                            var script_arguments = JsonConvert.DeserializeObject<Dictionary<string, object>>(sArgs);
                            server.DispatchTo(new Guid(msg.GUID), new NetObject("runme", new ServerMessage {
                                Name="run",
                                GUID="Server",
                                Contents = $@"{{
    script:""{File.ReadAllText($"scripts/{user}/{script}.lua").Replace("\"", "\\\"")}"",
    args:""{sArgs}""
}}"
                            }));
                        }
                        else
                        {
                            throw new Exception($"{user}.{script}: Script not found.");
                        }
                        break;
                    default:
                        throw new Exception($"Server couldn't decipher this message:\n\n{JsonConvert.SerializeObject(msg)}");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occurred with that one.");
                Console.WriteLine(ex);

                server.DispatchTo(new Guid(msg.GUID), new NetObject("error", new ServerMessage { Name = "Error", GUID = "Server", Contents = JsonConvert.SerializeObject(ex) }));
            }
        }

        public static string GenerateRandomPassword()
        {
            return Guid.NewGuid().ToString();
        }

        public static Dictionary<string, string> MUDHackPasswords = new Dictionary<string, string>();

        public static void Stop()
        {
            try
            {
                if (server.IsOnline)
                {
                    try
                    {
                        server.Stop();
                    }
                    catch
                    {

                    }
                }
                server = null;
            }
            catch { }
        }

        public static List<Channel> chats = new List<Channel>();
    }
}
