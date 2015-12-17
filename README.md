# CoupleSharp
Wrapper around the private Couple.me API

This is an incomplete wrapper around the private Couple.me API, which I unfortunatley relies on RestSharp and I never had time to finish.

# Example
```
if (client.Login("username", "password"))
{
  Console.WriteLine("Success!");
}
else
{
  Console.WriteLine("Unable to login");
}

if (client.SendMessage("I am testing"))
{
  Console.WriteLine("Success!");
}
else
{
  Console.WriteLine("Uuuh something went wrong there");
}

if (client.SendNudge())
{
  Console.WriteLine("Success!");
}
else
{
  Console.WriteLine("Uuuh something went wrong there");
}

if (client.SendMedia("image.png"))
{
  Console.WriteLine("Success");
}
else
{
  Console.WriteLine("Uuuh something went wrong there");
}

if (client.SendMedia("video.mp4"))
{
  Console.WriteLine("Success! MEDIA");
}
else
{
  Console.WriteLine("Uuuh something went wrong there");
}
```