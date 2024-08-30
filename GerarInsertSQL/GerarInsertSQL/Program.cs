    StreamReader srDE;
    StreamWriter swPara; 

    string d = "C:\\temp\\credential\\credential.csv";
    string p = "C:\\temp\\credential\\credential.txt";     

    srDE = File.OpenText(d);
    swPara = File.CreateText(p);

    while (!srDE.EndOfStream)
    {
        string line = srDE.ReadLine();
        if (!string.IsNullOrEmpty(line))
        {
            var fields = line.Split(("\\"));
            var cnj = fields[0];
            var user = fields[1];
            var passwd = fields[2];
            var idRef = fields[3];
            var idMon = fields[4];
            string emp = idRef.Split("-")[0];
            if (!emp.Contains("_T") && !emp.Contains("_D"))
            {
                string saida = $@"insert into CNV_CREDENTIALS VALUES('{cnj.Trim()}','{user.Trim()}', '{passwd.Trim()}','{idRef.Trim()}',{idMon},'{emp}')";
                swPara.WriteLine(saida);

            }           
        }  
        
    }
    srDE.Close();
    swPara.Close();
    Console.WriteLine("Pressione uma tecla para finalizar");
    Console.ReadLine();

