using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.IO;

namespace LanguagesSharp
{
    public class IdentWord
    {
        public String Word;
        public string Language;

        public IdentWord(String newWord, string newLanguage)
        {
            Word = newWord;
            Language = newLanguage;
        }
    }

    
    class Program
    {
        static void Main()
        {
            StreamReader input = File.OpenText("train.txt");
            String CurrentString = null;
            List<IdentWord> LanguagesCollection = new List<IdentWord>();    // Для каждого слова записывается слово и его язык (не уникальные элементы)
            // заполнение словаря
            while((CurrentString = input.ReadLine()) != null)
            {
                String currentLang = CurrentString.Substring(0, 2);     //Первые два символа - всегда язык
                int currentLength = 0;
                String tmpStr;      // Текущее слово в предложении
                for(int i = 3; i < CurrentString.Length; i+= currentLength+1)
                {
                    int tmp = CurrentString.IndexOfAny((" ,.0123456789/<>!-&?@").ToCharArray(), i);     // Находим символ, не принадлежащий слову
                    currentLength = tmp - i;
                    tmpStr = CurrentString.Substring(i, currentLength).ToLower();         // Извлекаем слово
                    if (tmpStr.Length != 0)
                    {
                        IdentWord CurrentWord = new IdentWord(tmpStr, currentLang);
                        LanguagesCollection.Add(CurrentWord);                   // Добавляем извлечённое слово в коллекцию

                    }
                }
            }
            input.Close();

            // Заполнение окончено
            
            // Преобразуем словарь так, чтобы слова не повторялись

            var AllWords = LanguagesCollection.GroupBy(dict => dict.Word, dict => dict.Language);
            List<List<string>> DistinctLanguages = AllWords.Select(d => d.Distinct().ToList()).ToList();
            List<string> BareWords = AllWords.Select(d => d.Key).ToList();
            List<string> PossibleLanguages = new List<string>();

            // Состовляем перечень встречающихся языков

            foreach(var t in DistinctLanguages)
            {
                foreach (var tt in t)
                {
                    if (!PossibleLanguages.Any(d => d == tt))
                    {
                        PossibleLanguages.Add(tt);
                    }
                }

            }
            
            // Обработка тестового файла

            StreamReader inTest = File.OpenText("test.txt");
            StreamWriter Result = new StreamWriter("newResult.txt");

            CurrentString = null;
            while ((CurrentString = inTest.ReadLine()) != null)
            {
                int currentLength = 0;
                String tmpStr;
                int[] PossibleArray = new int[PossibleLanguages.Count()];       // Массив для частот встречания различных языков
                List<string> LangsForCurrentStr = new List<string>();
                for (int i = 0; i < CurrentString.Length; i += currentLength + 1)   // Для каждого слова в предложении
                {
                    int tmp = CurrentString.IndexOfAny((" ,.0123456789/<>!-&?@№;%:()|\\+^#$`~").ToCharArray(), i);
                    currentLength = tmp - i;
                    tmpStr = CurrentString.Substring(i, currentLength).ToLower();
                    if (tmpStr.Length != 0)
                    {
                        int idOfWord = BareWords.TakeWhile(w => w != tmpStr).Count();   // Вычисляем позицию, в которой находится в словаре текущее слово
                        if (idOfWord != BareWords.Count())
                        {
                            for (int t = 0; t < DistinctLanguages[idOfWord].Count(); t++)
                            {
                                LangsForCurrentStr.Add((DistinctLanguages[idOfWord][t]));   // Добавляем языки,  в которых встречалось текущее слово, в общий для предложения лист
                            }
                        }
                    }
                }
                int max = 0;
                int maxIndex = 0;
                for(int i = 0; i < PossibleLanguages.Count(); i++)
                {
                    PossibleArray[i] = LangsForCurrentStr.Count(t => t == PossibleLanguages[i]);    // Записываем, сколько раз встречается каждый язык
                    if(PossibleArray[i] > max)      // Находим максимальную частоту встречания языка
                    {
                        max = PossibleArray[i];
                        maxIndex = i;
                    }
                }
                Result.WriteLine(PossibleLanguages[maxIndex]);  // Записываем язык с максимальной частотой
            }

            Result.Close();
            inTest.Close();
            input.Close();
        }
    }
}
