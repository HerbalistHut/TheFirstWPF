using Engine.Factories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Location
    {
        public int XCoordinate { get; set; }
        public int YCoordinate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
        public List<Quest> QuestsAvailableHere { get; set; } = new List<Quest>();
        public List<MonsterEncounter> MonstersHere { get; set; } = new List<MonsterEncounter>();

        public void AddMonster(int monsterID, int chanceOfEncountering)
        {
            if (MonstersHere.Exists(m=>m.MonsterID == monsterID))
            {
                // Этот монстр уже был добавлен на локацию
                // Тогда перезапишем ChanceOfEncountering с новым значением.
                MonstersHere.First(m => m.MonsterID == monsterID).ChanceOfEncountering = chanceOfEncountering;
            }
            else
            {
                MonstersHere.Add(new MonsterEncounter(monsterID, chanceOfEncountering));
            }
        }

        public Monster GetMonster()
        {
            if (!MonstersHere.Any())
            {
                return null;
            }
            // Суммарный процент спавна монстров в этой локации.
            int totalChances = MonstersHere.Sum(m => m.ChanceOfEncountering);
            // Выберает случайное число между 1 и общим числом (в случае, если общее количество процентов не равно 100).
            int randomNumber = RandomNumberGenerator.NumberBetween(1, totalChances);
            // Перебираем список монстров, 
            // добавляем процент шанса спавна монстра в runningTotal.
            // Когда слуайное число будет меньше runningTotal,
            // возвращаем этого монстра.
            int runningTotal = 0;
            foreach (MonsterEncounter monsterEncounter in MonstersHere)
            {
                runningTotal += monsterEncounter.ChanceOfEncountering;
                if (runningTotal >= randomNumber)
                {
                    return MonsterFactory.GetMonster(monsterEncounter.MonsterID);
                }
            }
            // Если возникает проблема, на всякий выводим последнего монстра в листе
            return MonsterFactory.GetMonster(MonstersHere.Last().MonsterID);
        }
    }
}
