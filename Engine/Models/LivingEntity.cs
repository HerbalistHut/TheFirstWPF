using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public abstract class LivingEntity : BaseNotification
    {
        private string _name;
        private int _currentHitPoints;
        private int _maximumHitPoints;
        private int _gold;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public int CurrentHitPoints
        {
            get => _currentHitPoints;
            set
            {
                _currentHitPoints = value;
                OnPropertyChanged(nameof(CurrentHitPoints));
            }
        }
        public int MaximumHitPoints
        {
            get => _maximumHitPoints;
            set
            {
                _maximumHitPoints = value;
                OnPropertyChanged(nameof(MaximumHitPoints));
            }
        }
        public int Gold
        {
            get => _gold;
            set
            {
                _gold = value;
                OnPropertyChanged(nameof(Gold));
            }
        }
        // На данный момент надо сделать ещё несколько вещей, прежде чем убирать свойство Inventory, поэтому сейчас тут существует кое-какое дублирование кода
        public ObservableCollection<GameItem> Inventory { get; set; }
        public ObservableCollection<GroupedInventoryItem> GroupedInventory { get; set; }
        public List<GameItem> Weapons =>
            Inventory.Where(i => i is Weapon).ToList();

        protected LivingEntity()
        {
            Inventory = new ObservableCollection<GameItem>();
            GroupedInventory = new ObservableCollection<GroupedInventoryItem>();
        }

        //Здесь ОРС эвент вызывается для Weapons, чтобы UI при добавлении предмета в инвентарь обновлял и интерфейс с оружием. 
        //OPC вызывается только для Weapons, так как Inventory и так сам обновляет свой UI при изменение в своей коллексии из-за ObservableCollection
        public void AddItemToInventory(GameItem item)
        {
            if(item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Inventory.Add(item);

            if (item.IsUnique)
            {
                GroupedInventory.Add(new GroupedInventoryItem(item, 1));
            }
            else
            {
                // Если предмета не было в инвентаре до этого момента,то в сгруппированный инвентарь сначало добавляется предмет в количестве 0 штук,
                // это сделанлано потому, что прибавление количества(Quantity) предмета выполнится в любом случае
                if (!GroupedInventory.Any(gi=>gi.Item.Id == item.Id))
                {
                    GroupedInventory.Add(new GroupedInventoryItem(item, 0));
                }
                GroupedInventory.First(gi => gi.Item.Id == item.Id).Quantity++;
            }

            OnPropertyChanged(nameof(Weapons));
        }

        public void RemoveItemFromInventory(GameItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (Inventory.FirstOrDefault(i=>i.Id == item.Id) == null)
            {
                throw new ArgumentException(nameof(item),"This item doesn't exist in Inventory");
            }

            Inventory.Remove(item);

            GroupedInventoryItem groupedInventoryItem = GroupedInventory.FirstOrDefault(gi => gi.Item == item);

            if (groupedInventoryItem == null)
            {
                throw new ArgumentException($"{nameof(item)} doesn't exist in GroupedInventory");
            }

            if (groupedInventoryItem.Quantity == 1)
            {
                GroupedInventory.Remove(groupedInventoryItem);
            }
            else
            {
                groupedInventoryItem.Quantity--;
            }
        }
    }
}
