using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoupleSharp.JSONSchemas
{

  public class UserHead
  {
    public User user { get; set; }
    public Poll poll { get; set; }
    public string @base { get; set; }
    public Db db { get; set; }
    public string csrf { get; set; }
    public string error { get; set; }
  }

  public class AppSettings
  {
    public bool forceVideo { get; set; }
    public int videoLimit { get; set; }
    public int reviewThreshold { get; set; }
    public double stickersVersion { get; set; }
    public double whatsNewVersion { get; set; }
    public string tut { get; set; }
  }

  public class Location
  {
    public string state { get; set; }
    public string country { get; set; }
    public string city { get; set; }
  }

  public class AppSettings2
  {
    public bool forceVideo { get; set; }
    public int videoLimit { get; set; }
    public int reviewThreshold { get; set; }
    public double stickersVersion { get; set; }
    public double whatsNewVersion { get; set; }
    public string tut { get; set; }
  }

  public class Settings
  {
    public string tz { get; set; }
    public bool hide_expired_events { get; set; }
  }

  public class GcmData
  {
    public string t { get; set; }
  }

  public class Other
  {
    public string userID { get; set; }
    public string uuid { get; set; }
    public string pairID { get; set; }
    public string userName { get; set; }
    public bool emailConfirmed { get; set; }
    public AppSettings2 appSettings { get; set; }
    public string lastAck { get; set; }
    public Settings settings { get; set; }
    public string photo { get; set; }
    public string phoneNumber { get; set; }
    public string gender { get; set; }
    public string email { get; set; }
    public List<GcmData> gcmData { get; set; }
    public long ts { get; set; }
  }

  public class Settings2
  {
    public string tz { get; set; }
  }

  public class GcmData2
  {
    public string t { get; set; }
  }

  public class User
  {
    public string userID { get; set; }
    public string uuid { get; set; }
    public string pairID { get; set; }
    public string userName { get; set; }
    public bool emailConfirmed { get; set; }
    public AppSettings appSettings { get; set; }
    public Location location { get; set; }
    public string lastAck { get; set; }
    public Other other { get; set; }
    public Settings2 settings { get; set; }
    public string photo { get; set; }
    public string phoneNumber { get; set; }
    public string gender { get; set; }
    public string email { get; set; }
    public List<GcmData2> gcmData { get; set; }
    public long ts { get; set; }
  }

  public class Params
  {
    public string p { get; set; }
    public string sig { get; set; }
  }

  public class Poll
  {
    public string host { get; set; }
    public int port { get; set; }
    public bool ssl { get; set; }
    public Params @params { get; set; }
  }

  public class Db
  {
    public string shared { get; set; }
    public string user { get; set; }
  }

}
