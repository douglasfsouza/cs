using System;

namespace dgInterfaces
{
    class Program
    {
        static void Main(string[] args)
        {
            ITV tv = new TV()
            {
                Manufacturer = "Samsung",
                Size = "58'"
            };

            tv.ChangeChannel(5);             

            Console.WriteLine("Channel: {0} - Manufacturer: {1}",tv.ActualChannel, tv.Manufacturer);

            IVideo video = (IVideo)tv;
            IVideo video1 = tv as IVideo;

            TV t = tv as TV;

            video.Manufacturer = "LG";
            
            Console.WriteLine("video.manufacturer: {0} - video1.manufacturer: {1} - t.Size: {2}", video.Manufacturer, video1.Manufacturer, t.Size);
        }
    }
}
