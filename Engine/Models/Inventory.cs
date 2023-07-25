using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Inventory
    {
        private readonly List<GameItem> _backingInventory = new List<GameItem>();
        private readonly List<GroupedInventoryItem> _backingGroupedInventory = new List<GroupedInventoryItem>();

        public IReadOnlyList<GameItem> Items => _backingInventory.AsReadOnly();
        public IReadOnlyList<GroupedInventoryItem> GroupedInventory => _backingGroupedInventory.AsReadOnly();
        public IReadOnlyList<GameItem> Weapons => _backingInventory.Where(i => i.Category == GameItem.ItemCategory.Weapon).ToList().AsReadOnly();
        public IReadOnlyList<GameItem> Consumables => _backingInventory.Where(i => i.Category == GameItem.ItemCategory.Consumable).ToList().AsReadOnly();
        public bool HasConsumable => Consumables.Any();

        public Inventory(IEnumerable<GameItem> items = null)
        {
            if (items == null)
            {
                return;
            }

            foreach(GameItem item in items)
            {
                _backingInventory.Add(item);

                AddItemToGroupedInventory(item);
            }
        }

        public bool HasAllTheseItems(IEnumerable<ItemQuantity> items)
        {
            return items.All(item => Items.Count(i => i.Id == item.Id) >= item.Quantity);
        }

        private void AddItemToGroupedInventory(GameItem item)
        {
            if (item.IsUnique)
            {
                _backingGroupedInventory.Add(new GroupedInventoryItem(item, 1));
            }
            else
            {
                // If the item was not in the inventory before this moment, then first of all the item is added to the GroupedInventory in the amount of 0
                if (!_backingGroupedInventory.Any(gi => gi.Item.Id == item.Id))
                {
                    _backingGroupedInventory.Add(new GroupedInventoryItem(item, 0));
                }
                _backingGroupedInventory.First(gi => gi.Item.Id == item.Id).Quantity++;
            }
        }
    }
}
