using FadeBot;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

public static class HttpUtils
{
    public static string HWID;
    public static string ClientVersion = "2021.2.4p3-1110--Release";

    public static string base64Encode(string data)
    {
        byte[] encData_byte = Encoding.UTF8.GetBytes(data);
        string encodedData = Convert.ToBase64String(encData_byte);
        return encodedData;
    }

    public static string GetCookie(string Account)
    {
        HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://vrchat.com/api/1/auth/user?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&organization=vrchat");
        Request.Headers.Clear();
        Request.Headers.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes(Account))}");
        Request.Headers.Add("X-Requested-With", "XMLHttpRequest");
        Request.Headers.Add("X-Platform", "android");
        Request.Headers.Add("X-MacAddress", HWID);
        Request.Headers.Add("X-Client-Version", ClientVersion);
        Request.Headers.Add("TE", "identity");
        Request.Headers.Add("User-Agent", "VRC.Core.BestHTTP");
        Request.Headers.Add("Origin", "vrchat.com");
        Request.Headers.Add("Host", "api.vrchat.cloud");
        Request.Headers.Add("Cookie", "twoFactorAuth=");
        Request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
        Request.Headers.Add("Content-Length", "0");
        Request.Headers.Add("Connection", "Keep-Alive, TE");
        Request.Headers.Add("Accept-Encoding", "identity");
        Request.Method = "GET";

        for (int i = 0; i < 2; i++)
        {
            try
            {
                HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
                if (Response.StatusCode == HttpStatusCode.OK || Response.StatusCode == HttpStatusCode.Accepted || Response.StatusCode == HttpStatusCode.Created || Response.StatusCode == HttpStatusCode.NotModified)
                {
                    var Cookie = Response.Headers.Get("Set-Cookie");
                    StreamReader webSource = new StreamReader(Response.GetResponseStream());
                    string Auth = JsonConvert.DeserializeObject<user>(webSource.ReadToEnd()).id;
                    if (Cookie.Contains("auth"))
                    {
                        var Token = Cookie.Split(Convert.ToChar("="))[1].Split(Convert.ToChar(";"))[0];
                        Auth = $"{Token}:{Auth}";
                        return Auth;
                    }
                }
            }
            catch { }
            Thread.Sleep(2500);
        }
        return "";
    }

    public static string ValidateAuthcookie(string auth)
    {
        string URL = $"https://api.vrchat.cloud/api/1/auth?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26";
        HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(URL);
        Request.Headers.Clear();
        Request.Method = "GET";
        Request.Headers.Add("Cookie", $"apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26; auth={auth}");
        Request.Headers.Add("Host", "api.vrchat.cloud");
        Request.Headers.Add("User-Agent", "Transmtn-Pipeline");
        for (int i = 0; i < 2; i++)
        {
            try
            {
                HttpWebResponse webResponse = (HttpWebResponse)Request.GetResponse();
                if (webResponse.StatusCode == HttpStatusCode.OK || webResponse.StatusCode == HttpStatusCode.Accepted || webResponse.StatusCode == HttpStatusCode.Created || webResponse.StatusCode == HttpStatusCode.NotModified)
                {
                    StreamReader webSource = new StreamReader(webResponse.GetResponseStream()); ;
                    string source = webSource.ReadToEnd();
                    webResponse.Close();
                    return source;
                }
            }
            catch { }
            Thread.Sleep(2500);
        }
        return null;
    }

    public static string ValidateInstance(string WorldID, string auth)
    {
        string URL = $"https://api.vrchat.cloud/api/1/instances/{WorldID}/shortName?permanentify=false&apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&organization=vrchat";
        HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(URL);
        Request.Method = "GET";
        Request.ContentLength = 0;
        Request.ContentType = "application/x-www-form-urlencoded";
        Request.Headers["Connection"] = "TE";
        Request.Headers["Cookie"] = $"apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26; auth={auth}; twoFactorAuth=";
        Request.Host = "api.vrchat.cloud";
        Request.Headers["Origin"] = "vrchat.com";
        Request.Headers["TE"] = "identity";
        Request.UserAgent = "VRC.Core.BestHTTP";
        Request.Headers["X-Client-Version"] = ClientVersion;
        Request.Headers["X-MacAddress"] = HWID;
        Request.Headers["X-Platform"] = "android";
        Request.Headers["Accept-Encoding"] = $"identity";
        Request.Headers["X-Requested-With"] = "XMLHttpRequest";

        for (int i = 0; i < 2; i++)
        {
            try
            {
                HttpWebResponse webResponse = (HttpWebResponse)Request.GetResponse();
                if (webResponse.StatusCode == HttpStatusCode.OK || webResponse.StatusCode == HttpStatusCode.Accepted || webResponse.StatusCode == HttpStatusCode.Created || webResponse.StatusCode == HttpStatusCode.NotModified)
                {
                    StreamReader webSource = new StreamReader(webResponse.GetResponseStream()); ;
                    string source = webSource.ReadToEnd();
                    webResponse.Close();
                    return source;
                }
            }
            catch { }
            Thread.Sleep(2500);
        }
        return null;
    }

    public static string HttpGetRoomtoken(string WorldID, string auth)
    {
        string URL = $"https://api.vrchat.cloud/api/1/instances/{WorldID}/join?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&organization=vrchat";
        HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(URL);
        Request.Headers.Clear();
        Request.Method = "GET";
        Request.Headers.Add("X-Requested-With", "XMLHttpRequest");
        Request.Headers.Add("X-Platform", "android");
        Request.Headers.Add("X-MacAddress", HWID);
        Request.Headers.Add("X-Client-Version", ClientVersion);
        Request.Headers.Add("TE", "identity");
        Request.Headers.Add("User-Agent", "VRC.Core.BestHTTP");
        Request.Headers.Add("Origin", "vrchat.com");
        Request.Headers.Add("Host", "api.vrchat.cloud");
        Request.Headers.Add("Cookie", $"apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26; auth={auth}; twoFactorAuth=");
        Request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
        Request.Headers.Add("Content-Length", "0");
        Request.Headers.Add("Connection", "Keep-Alive, TE");
        Request.Headers.Add("Accept-Encoding", "identity");

        for (int i = 0; i < 2; i++)
        {
            try
            {
                HttpWebResponse webResponse = (HttpWebResponse)Request.GetResponse();
                if (webResponse.StatusCode == HttpStatusCode.OK || webResponse.StatusCode == HttpStatusCode.Accepted || webResponse.StatusCode == HttpStatusCode.Created || webResponse.StatusCode == HttpStatusCode.NotModified)
                {
                    StreamReader webSource = new StreamReader(webResponse.GetResponseStream()); ;
                    string source = webSource.ReadToEnd();
                    webResponse.Close();
                    return source;
                }
            }
            catch { }
            Thread.Sleep(2500);
        }
        return null;
    }

    public static string HttpGetWorld(string wid, string auth)
    {
        string URL = $"https://api.vrchat.cloud/api/1/worlds/{wid}?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&organization=vrchat";
        HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(URL);
        Request.Headers.Clear();
        Request.Headers.Add("X-Requested-With", "XMLHttpRequest");
        Request.Headers.Add("X-Platform", "android");
        Request.Headers.Add("X-MacAddress", HWID);
        Request.Headers.Add("X-Client-Version", ClientVersion);
        Request.Headers.Add("User-Agent", "VRC.Core.BestHTTP");
        Request.Headers.Add("TE", "identity");
        Request.Headers.Add("Origin", "vrchat.com");
        Request.Headers.Add("Host", "api.vrchat.cloud");
        Request.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
        Request.Headers.Add("Content-Length", "0");
        Request.Headers.Add("Connection", "Keep-Alive, TE");
        Request.Headers.Add("Accept-Encoding", "identity");
        Request.Method = "GET";
        Request.Headers.Add("Cookie", $"apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26; auth={auth}; twoFactorAuth=");
        for (int i = 0; i < 2; i++)
        {
            try
            {
                HttpWebResponse webResponse = (HttpWebResponse)Request.GetResponse();
                if (webResponse.StatusCode == HttpStatusCode.OK || webResponse.StatusCode == HttpStatusCode.Accepted || webResponse.StatusCode == HttpStatusCode.Created || webResponse.StatusCode == HttpStatusCode.NotModified)
                {
                    StreamReader webSource = new StreamReader(webResponse.GetResponseStream()); ;
                    string source = webSource.ReadToEnd();
                    webResponse.Close();
                    return source;
                }
            }
            catch { }
            Thread.Sleep(2500);
        }
        return null;
    }

    public static void SetAvatar(string Token, string ID)
    {
        if (!ID.Contains("avtr_")) return;
        HttpWebRequest Request = (HttpWebRequest)WebRequest.Create($"https://api.vrchat.cloud/api/1/avatars/{ID}/select?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&organization=vrchat");
        Request.Headers["X-Requested-With"] = "XMLHttpRequest";
        Request.Headers["X-Platform"] = "android";
        Request.Headers["X-MacAddress"] = HWID;
        Request.Headers["X-Client-Version"] = ClientVersion;
        Request.UserAgent = "VRC.Core.BestHTTP";
        Request.Headers["TE"] = "identity";
        Request.Headers["Origin"] = "vrchat.com";
        Request.Host = "api.vrchat.cloud";
        Request.Headers["Cookie"] = $"auth={Token}; apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26; twoFactorAuth=";
        Request.ContentLength = 0;
        Request.Headers["Connection"] = "TE";
        Request.Headers["Accept-Encoding"] = "identity";
        Request.Method = "PUT";
        HttpWebResponse webResponse = (HttpWebResponse)Request.GetResponse();
        StreamReader webSource = new StreamReader(webResponse.GetResponseStream());
        webSource.ReadToEnd();
        webResponse.Close();
    }

    public static void HttpJoinVisit(string Type, string Token, string UserID, string WorldID)
    {
        string URL = $"https://api.vrchat.cloud/api/1/{Type}?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&organization=vrchat";
        WebRequest webRequest = WebRequest.Create(URL);
        webRequest.Method = "PUT";
        webRequest.Headers["Cookie"] = $"auth={Token}; apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26; twoFactorAuth=";
        webRequest.Headers["User-Agent"] = $"VRC.Core.BestHTTP";
        webRequest.Headers["X-Client-Version"] = ClientVersion;
        webRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
        webRequest.Headers["X-Platform"] = "android";
        webRequest.Headers["X-MacAddress"] = HWID;
        webRequest.Headers["TE"] = "identity";
        webRequest.Headers["Origin"] = "vrchat.com";
        webRequest.Headers["Host"] = "api.vrchat.cloud";
        webRequest.Headers["Connection"] = "TE";
        webRequest.Headers["Accept-Encoding"] = "identity";
        webRequest.ContentType = "application/json";
        webRequest.Headers["Cookie"] = $"auth={Token}; apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26; twoFactorAuth="; ;

        var MyObject = new
        {
            userId = UserID,
            worldId = WorldID
        };
        var json = JsonConvert.SerializeObject(MyObject, Formatting.Indented);
        byte[] bytes = Encoding.ASCII.GetBytes(json);
        Stream os = null;
        try
        {
            webRequest.ContentLength = bytes.Length;
            os = webRequest.GetRequestStream();
            os.Write(bytes, 0, bytes.Length);
        }
        catch { }
        finally
        {
            if (os != null) os.Close();
        }

        try
        {
            WebResponse webResponse = webRequest.GetResponse();
            StreamReader sr = new StreamReader(webResponse.GetResponseStream());
        }
        catch { }
    }

    public static void HttpModerate(string Type, string Token, string UserID)
    {
        string URL = $"https://api.vrchat.cloud/api/1/auth/user/playermoderations?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&organization=vrchat";
        WebRequest webRequest = WebRequest.Create(URL);
        webRequest.ContentType = "application/json";
        webRequest.Headers["Cookie"] = $"auth={Token}; apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26; twoFactorAuth=";
        webRequest.Headers["User-Agent"] = $"VRC.Core.BestHTTP";
        webRequest.Headers["X-Client-Version"] = ClientVersion;
        webRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
        webRequest.Headers["X-Platform"] = "android";
        webRequest.Headers["X-MacAddress"] = HWID;
        webRequest.Headers["TE"] = "identity";
        webRequest.Headers["Origin"] = "vrchat.com";
        webRequest.Headers["Host"] = "api.vrchat.cloud";
        webRequest.Headers["Connection"] = "TE";
        webRequest.Headers["Accept-Encoding"] = "identity";
        webRequest.Method = "POST";
        webRequest.Headers["Cookie"] = $"auth={Token}; apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26; twoFactorAuth="; ;

        var MyObject = new
        {
            created = "01/01/0001 00:00:00",
            moderated = UserID,
            type = Type
        };
        var json = JsonConvert.SerializeObject(MyObject, Formatting.Indented);
        byte[] bytes = Encoding.ASCII.GetBytes(json);
        Stream os = null;
        try
        {
            webRequest.ContentLength = bytes.Length;
            os = webRequest.GetRequestStream();
            os.Write(bytes, 0, bytes.Length);
        }
        catch { }
        finally
        {
            if (os != null) os.Close();
        }

        try
        {
            WebResponse webResponse = webRequest.GetResponse();
            StreamReader sr = new StreamReader(webResponse.GetResponseStream());
        }
        catch { }
    }

    public static void HttpUnmoderate(string Type, string Token, string UserID)
    {
        string URL = $"https://api.vrchat.cloud/api/1/auth/user/unplayermoderate?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&organization=vrchat";
        WebRequest webRequest = WebRequest.Create(URL);
        webRequest.ContentType = "application/json";
        webRequest.Headers["Cookie"] = $"auth={Token}; apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26; twoFactorAuth=";
        webRequest.Headers["User-Agent"] = $"VRC.Core.BestHTTP";
        webRequest.Headers["X-Client-Version"] = ClientVersion;
        webRequest.Headers["X-Requested-With"] = "XMLHttpRequest";
        webRequest.Headers["X-Platform"] = "android";
        webRequest.Headers["X-MacAddress"] = HWID;
        webRequest.Headers["TE"] = "identity";
        webRequest.Headers["Origin"] = "vrchat.com";
        webRequest.Headers["Host"] = "api.vrchat.cloud";
        webRequest.Headers["Connection"] = "TE";
        webRequest.Headers["Accept-Encoding"] = "identity";
        webRequest.Method = "PUT";
        webRequest.Headers["Cookie"] = $"auth={Token}; apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26; twoFactorAuth="; ;

        var MyObject = new
        {
            moderated = UserID,
            type = Type
        };
        var json = JsonConvert.SerializeObject(MyObject, Formatting.Indented);
        byte[] bytes = Encoding.ASCII.GetBytes(json);
        Stream os = null;
        try
        {
            webRequest.ContentLength = bytes.Length;
            os = webRequest.GetRequestStream();
            os.Write(bytes, 0, bytes.Length);
        }
        catch { }
        finally
        {
            if (os != null) os.Close();
        }

        try
        {
            WebResponse webResponse = webRequest.GetResponse();
            StreamReader sr = new StreamReader(webResponse.GetResponseStream());
        }
        catch { }
    }

    public static void FriendRequest(string Token, string userid)
    {
        try
        {
            if (!userid.Contains("usr_")) return;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create($"https://api.vrchat.cloud/api/1/user/{userid}/friendRequest?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26");
            webRequest.ContentType = "application/json";
            webRequest.Method = "POST";
            webRequest.UserAgent = "Transmtn-Pipeline";
            webRequest.Host = "api.vrchat.cloud";
            webRequest.Headers["X-Client-Version"] = ClientVersion;
            webRequest.Headers["X-Platform"] = "android";
            webRequest.Headers["Cookie"] = $"auth={Token}; apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26; twoFactorAuth=";
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            StreamReader webSource = new StreamReader(webResponse.GetResponseStream());
            webSource.ReadToEnd();
            webResponse.Close();
        }
        catch { }
    }

    public static string HttpGetUsers()
    {
        string URL = $"https://api.vrchat.cloud/api/1/visits";
        HttpWebRequest Request = (HttpWebRequest)WebRequest.Create(URL);
        Request.Method = "GET";
        Request.Headers["User-Agent"] = $"Mozilla/5.0";
        try
        {
            HttpWebResponse webResponse = (HttpWebResponse)Request.GetResponse();
            if (webResponse.StatusCode == HttpStatusCode.OK || webResponse.StatusCode == HttpStatusCode.Accepted || webResponse.StatusCode == HttpStatusCode.Created || webResponse.StatusCode == HttpStatusCode.NotModified)
            {
                StreamReader webSource = new StreamReader(webResponse.GetResponseStream()); ;
                string source = webSource.ReadToEnd();
                webResponse.Close();
                return source;
            }
        }
        catch { }
        return "";
    }
}