using System;
using System.Text;
using System.Xml.Linq;
using CurrencyRates;
class Program
{
    static async Task Main()
    {
        var valuteCode = GetValuteCode();
        var valuteValue = await GetValuteValueByCode(valuteCode);

        if (valuteValue == null)
            Console.WriteLine("Валюта не найдена.");
        else
            Console.WriteLine($"Сегодняшний курс {valuteCode}: {valuteValue}.");

        Console.ReadLine();
    }

    /// <summary>
    /// Метод считывает код валюты для получения значения.
    /// </summary>
    /// <returns>Код валюты.</returns>
    static string GetValuteCode()
    {
        Console.WriteLine("Введите код валюты");
        var input = Console.ReadLine().Trim();
        while (string.IsNullOrWhiteSpace(input))
        {
            Console.Write("Некорректный ввод. Повторите ввод валюты: ");
            input = Console.ReadLine()?.Trim().ToUpper();
        }

        return input.ToUpper();
    }

    /// <summary>
    /// Метод считывает значение валюты по коду валюты.
    /// </summary>
    /// <param name="valuteCode">Код валюты.</param>
    /// <returns>Значение валюты в текущий день.</returns>
    static async Task<string> GetValuteValueByCode(string valuteCode)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var result = string.Empty;
        try
        {
            using HttpClient client = new HttpClient();
            byte[] responseBytes = await client.GetByteArrayAsync(Constants.PARSING_URL);
            string response = Encoding.GetEncoding(1251).GetString(responseBytes);

            XDocument xml = XDocument.Parse(response);
            var valuteValue = xml.Descendants("Valute")
                .FirstOrDefault(x => (string)x.Element("CharCode") == valuteCode);

            return valuteValue?.Element("Value")?.Value;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке данных: {ex.Message}");
            return null;
        }
    }
}