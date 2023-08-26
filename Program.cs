using newHSBGcheck;

SQLRequest.ConnectToDB();

var bg = new BG();
bg.Check(0);
bg.Check(1);

while (0 == 0)
{
    if (DateTime.Now.Minute == 0)
    {
        bg.Check(0);
        bg.Check(1);
    }
    
    Task.Delay(60000).Wait();
}