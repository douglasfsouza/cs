using System.Net;
 
    WebRequest w;
    string url = "https://pje.jfce.jus.br/pje/ConsultaPublica/listView.seam";

    try
    {
        w = WebRequest.Create(url);

    //se precisar usar proxy:
    //WebProxy myProxy = new WebProxy("myproxy", 80);
    //myProxy.BypassProxyOnLocal = true;

    //w.Proxy = myProxy;

    //ou pegar o proxy já definido no browser:
   // w.Proxy = WebProxy.GetDefaultProxy();

    Stream objStream;
        objStream = w.GetResponse().GetResponseStream();

        StreamReader objReader = new StreamReader(objStream);

        string sLine = "";
        int i = 0;

        while (sLine != null)
        {
            i++;
            sLine = objReader.ReadLine();
            if (sLine != null)
                Console.WriteLine("{0}:{1}", i, sLine);
        }
        Console.ReadLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

 
