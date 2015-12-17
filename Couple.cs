using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using RestSharp;
using RestSharp.Extensions;
using System.Drawing;
using System.IO;

namespace CoupleSharp
{
  public class Couple
  {

    private string s_baseUri = "https://app.couple.me";
    private string s_useragent = "CoupleSharp/0.1 (C#.NET)";
    private string csrf;
    private string userID;
    private string authToken;
    private bool setup;
    private RestClient client;

    /// <summary>
    /// Login to couple and set authentication token
    /// </summary>
    /// <param name="username">Couple username</param>
    /// <param name="password">Couple password</param>
    /// <returns>If login was successful</returns>
    public bool Login(string username, string password)
    {
      setup = true;
      List<Parameter> data = new List<Parameter>();
      data.Add(new Parameter() { Name = "userID", Value = username, Type = ParameterType.GetOrPost });
      data.Add(new Parameter() { Name = "secretKey", Value = password, Type = ParameterType.GetOrPost });
      data.Add(new Parameter() { Name = "set_cookie", Value = true, Type = ParameterType.GetOrPost });

      JSONSchemas.UserHead head = SimpleJson.DeserializeObject<JSONSchemas.UserHead>(PostRequest("/0/p/authenticate", data));
      if (head.error != null)
        return false;

      csrf = head.csrf;
      userID = username;

      return true;
    }

    /// <summary>
    /// Send a message to partner
    /// </summary>
    /// <param name="message">Message to send</param>
    /// <returns>If message was sent</returns>
    public bool SendMessage(string message)
    {
      List<Parameter> data = new List<Parameter>();
      data.Add(new Parameter() { Name = "text", Value = message, Type = ParameterType.GetOrPost });
      data.Add(new Parameter() { Name = "localID", Value = NewLocalID(), Type = ParameterType.GetOrPost });
      data.Add(new Parameter() { Name = "_jucsrf", Value = csrf, Type = ParameterType.GetOrPost });

      JSONSchemas.RecievedEvent head = SimpleJson.DeserializeObject<JSONSchemas.RecievedEvent>(PostRequest("/30/p/send/text", data));
      if (head.error != null)
        return false;

      return true;
    }

    /// <summary>
    /// Sends a nudge to partner
    /// </summary>
    /// <returns>If nudge was sent</returns>
    public bool SendNudge()
    {
      List<Parameter> data = new List<Parameter>();
      data.Add(new Parameter() { Name = "localID", Value = NewLocalID(), Type = ParameterType.GetOrPost });
      data.Add(new Parameter() { Name = "_jucsrf", Value = csrf, Type = ParameterType.GetOrPost });

      JSONSchemas.RecievedEvent head = SimpleJson.DeserializeObject<JSONSchemas.RecievedEvent>(PostRequest("/30/p/send/nudge", data));
      if (head.error != null)
        return false;

      return true;
    }

    /// <summary>
    /// Sends media to partner
    /// </summary>
    /// <param name="media">Full path to media</param>
    /// <returns>If media was sent</returns>
    public bool SendMedia(string media)
    {
      List<Parameter> data = new List<Parameter>();
      data.Add(new Parameter() { Name = "localID", Value = NewLocalID(), Type = ParameterType.GetOrPost });
      data.Add(new Parameter() { Name = "_jucsrf", Value = csrf, Type = ParameterType.GetOrPost });

      JSONSchemas.RecievedEvent head = SimpleJson.DeserializeObject<JSONSchemas.RecievedEvent>(PostRequest("/30/p/send/media", data, media));
      if (head.error != null)
        return false;

      return true;
    }

    /// <summary>
    /// Creates a request (also sets up client)
    /// </summary>
    /// <param name="uri">Resource address</param>
    /// <param name="parameters">Parameters to send</param>
    /// <param name="media">File path to media(if any)</param>
    /// <returns>A basic request</returns>
    private RestRequest CreateRequest(string uri, List<Parameter> parameters, string media = "")
    {
      if (client == null)
      {
        client = new RestClient();
        client.BaseUrl = new Uri(s_baseUri);
        client.CookieContainer = new CookieContainer();
        client.AddDefaultHeader("x-juliet-client-ver", "1.4");
        client.AddDefaultHeader("x-juliet-ver", "1.70");
        client.AddDefaultHeader("Accept-Encoding", "gzip, deflate");
        client.AddDefaultHeader("Accept-Language", "en-US,en;q=0.8,sv;q=0.6");
        client.AddDefaultHeader("Keep-Alive", "true");
        client.UserAgent = s_useragent;
      }
      RestRequest request = new RestRequest();

      request.Resource = uri;
      if (media != "")
      {
        string mediaType = "";
        byte[] mediaBytes = null;
        if (media.EndsWith(".jpg") || media.EndsWith(".jpeg") || media.EndsWith(".png"))
        {
          Image img = Image.FromFile(media);
          request.AddParameter("width", img.Width);
          request.AddParameter("height", img.Height);
          mediaType = (media.EndsWith(".jpg") || media.EndsWith(".jpeg")) ? "image/jpeg" : "image/png";
          mediaBytes = ImageToByteArray(img);
        }
        else if (media.EndsWith(".m4a"))
        {
          mediaType = "audio/mp4";
          mediaBytes = File.ReadAllBytes(media);
        }
        else if (media.EndsWith(".mp4"))
        {
          mediaType = "video/mp4";
          mediaBytes = File.ReadAllBytes(media);
        }
        request.AddFile("media-content", mediaBytes, Path.GetFileNameWithoutExtension(media), mediaType);
      }

      foreach(Parameter p in parameters){
        request.AddParameter(p);
      }

      return request;
    }

    /// <summary>
    /// Creates and posts request
    /// </summary>
    /// <param name="uri">URI to send to</param>
    /// <param name="data">Data to send</param>
    /// <returns>Returned content</returns>
    private string PostRequest(string uri, List<Parameter> data, string media = "")
    {
      RestRequest request = CreateRequest(uri, data, media);
      request.Method = Method.POST;
      request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
      IRestResponse response = client.Execute(request);

      if (setup)
      {
        foreach (RestResponseCookie c in response.Cookies)
        {
          if (c.Name == "authToken")
          {
            JSONSchemas.CookieToken ct = SimpleJson.DeserializeObject<JSONSchemas.CookieToken>(GetBetween(c.Value.UrlDecode(), "s:j:", "."));
            authToken = ct.token;
          }
        }
      }

      if (response.ErrorException != null)
      {
        throw new ApplicationException("Unable to retrieve response", response.ErrorException);
      }

      return response.Content;
    }

    /// <summary>
    /// Creates a GET request
    /// </summary>
    /// <param name="uri">URI to send to</param>
    /// <param name="data">Data to send</param>
    /// <returns></returns>
    private string GetRequest(string uri, List<Parameter> data = null)
    {
      RestRequest request = CreateRequest(uri, data);
      request.Method = Method.GET;

      IRestResponse response = client.Execute(request);

      if (response.ErrorException != null)
      {
        throw new ApplicationException("Unable to retrieve response", response.ErrorException);
      }

      return response.Content;
    }

    /// <summary>
    /// Generates a new Local timeline ID for message
    /// </summary>
    /// <returns>Local Timeline ID</returns>
    private long NewLocalID()
    {
      Int64 retval = 0;
      var st = new DateTime(1970, 1, 1);
      TimeSpan t = (DateTime.Now.ToUniversalTime() - st);
      retval = (Int64)(t.TotalMilliseconds + 0.5);
      return (long)Math.Round((retval - 13569984e5) / 100);
    }

    /// <summary>
    /// Simple get between function
    /// </summary>
    /// <param name="str">Full string</param>
    /// <param name="start">Start to find</param>
    /// <param name="end">End to find</param>
    /// <returns>String inbetween</returns>
    private string GetBetween(string str, string start, string end)
    {
      int spos = str.IndexOf(start) + start.Length;
      int epos = str.IndexOf(end);

      int length = (epos - spos);

      return str.Substring(spos, length);
    }

    /// <summary>
    /// Converts image to byte array
    /// </summary>
    /// <param name="imageIn">Image to turn to byte array</param>
    /// <returns>Byte array of image</returns>
    private byte[] ImageToByteArray(System.Drawing.Image imageIn)
    {
      MemoryStream ms = new MemoryStream();
      imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
      return ms.ToArray();
    }

  }
}
