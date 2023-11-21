using System;

namespace Society.Logic
{
    public class PsevdoDateTime
    {
        // Метод для проверки валидности псевдодаты в формате "dd.MM.yyyy"
        public static bool IsValidPseudoDate(string pseudoDate)
        {
            if (DateTime.TryParseExact(pseudoDate, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime result))
            {
                // Если парсинг успешен, и дата валидна
                return true;
            }

            else
            {
                // Если парсинг не удался или дата не валидна
                return false;
            }
        }

        // Метод для проверки валидности псевдовремени в формате "HH:mm"
        public static bool IsValidPseudoTime(string pseudoTime)
        {
            if (DateTime.TryParseExact(pseudoTime, "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime result))
            {
                // Если парсинг успешен, и время валидно
                return true;
            }

            else
            {
                // Если парсинг не удался или время не валидно
                return false;
            }
        }
    }
}
