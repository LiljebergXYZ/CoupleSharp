using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoupleSharp.JSONSchemas
{
  public class RecievedEvent
  {
    public List<Event> events { get; set; }
    public string error { get; set; }
  }

  public class Event
  {
    public string pairingID { get; set; }
    public string itemID { get; set; }
    public string eventType { get; set; }
    public string from { get; set; }
    public string text { get; set; }
    public long timeStamp { get; set; }
    public string cver { get; set; }
  }

}
