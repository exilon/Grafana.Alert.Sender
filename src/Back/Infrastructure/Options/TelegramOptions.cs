using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TelegramContact
{
    public string Name { get; set; }
    public string BotToken { get; set; }
    public string ChatId { get; set; }
}

public class TelegramOptions
{
    public Dictionary<string,TelegramContact> Contacts { get; set; }
}
