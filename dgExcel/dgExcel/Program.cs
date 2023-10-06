// See https://aka.ms/new-console-template for more information
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

string excelFilePath = @"c:\temp\excelFile.xlsx";
string textFilePath = @"c:\temp\csvFile.csv";

ConvertExcelToText(excelFilePath, textFilePath);

void ConvertExcelToText(string excelFilePath, string textFilePath)
{
    // Carrega o arquivo do Excel
    using (FileStream file = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
    {
        XSSFWorkbook workbook = new XSSFWorkbook(file);

        ISheet sheet = workbook.GetSheetAt(0); // Obtem a primeira planilha do Excel

        // Cria um novo arquivo de texto
        using (StreamWriter writer = new StreamWriter(textFilePath))
        {
            // Percorre todas as linhas da planilha
            for (int rowNum = 0; rowNum <= sheet.LastRowNum; rowNum++)
            {
                IRow row = sheet.GetRow(rowNum);

                // Percorre todas as células da linha e escreve os valores no arquivo de texto separados por vírgula
                for (int cellNum = 0; cellNum < row.LastCellNum; cellNum++)
                {
                    ICell cell = row.GetCell(cellNum);
                    string value = cell?.ToString() ?? ""; // Se a célula for nula, escreve uma string vazia

                    if (cellNum > 0)
                    {
                        writer.Write(";"); // Separa os valores por ponto e vírgula
                    }

                    writer.Write(value);
                }

                writer.WriteLine(); // Pula uma linha após escrever os valores de uma linha do Excel no arquivo de texto
            }
        }
    }
}
