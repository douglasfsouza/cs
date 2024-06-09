string version1 = "3.1.10";
string version2 = "3.2.3";

var v1 = version1.Split('.');
var v2 = version2.Split(".");

long r = 0; //0=igual , 1=aumentou , -1=diminuiu

long i = 0;
long d = 0;

if (v1.Length >= v2.Length)
{    
    while (i < v1.Length)
    { 
        if (i <= v2.Length -1)
        {
            d = Convert.ToInt64(v2[i]);             
        }
        else
        {
            d = 0;
        }

        if (Convert.ToInt64(v1[i]) > d)
        {
            r = -1;
            break;
        }
        else if (Convert.ToInt64(v1[i]) < d)
        {
            r = 1;
            break;
        }

        i++;
    }
}
else  
{
    while (i < v2.Length)
    {
        if ( i <= v1.Length -1)
        {
            d = Convert.ToInt64(v1[i]);
        }
        else
        {
            d = 0;
        }

        if (d > Convert.ToInt64(v2[i]))
        {
            r = -1;
            break;
        }
        else if (d < Convert.ToInt64(v2[i]))
        {
            r = 1;
            break;
        }

        i++;
    }   

}

Console.WriteLine($"version1={version1}");
Console.WriteLine($"version2={version2}");
if (r == 0)
{
    Console.WriteLine("são iguais");
}
else if(r == 1)
{
    Console.WriteLine("Aumentou");
}
else
{
    Console.WriteLine("Diminuiu");
}

