using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Engine.Factories;
using System.Runtime.CompilerServices;
using System.Data.SqlTypes;

namespace Engine.Services
{
    public static class InventoryService
    {
        /// <summary>
        /// Adds list of GameItems to inventory
        /// </summary>
        /// <returns>Whole new copy of inventory (unreasonable and costly asf)</returns>
        public static Inventory AddItem(this Inventory inventory, IEnumerable<GameItem> items)
        {
            return new Inventory(inventory.Items.Concat(items));
        }

        /// <summary>
        /// Adds single GameItem to inventory
        /// </summary>
        /// <returns>Whole new copy of inventory (unreasonable and costly asf)</returns>
        public static Inventory AddItem(this Inventory inventory, GameItem item)
        {
            return inventory.AddItem(new List<GameItem> { item });
        }

        /// <summary>
        /// Adds GameItem to inventory by it's ID
        /// </summary>
        /// <returns>Whole new copy of inventory (unreasonable and costly asf)</returns>
        public static Inventory AddItem(this Inventory inventory, int itemID)
        {
            return inventory.AddItem(new List<GameItem> { ItemFactory.CreateGameItem(itemID) });
        }

        /// <summary>
        /// Adds ItemQuantity to inventory
        /// </summary>
        /// <returns>Whole new copy of inventory (unreasonable and costly asf)</returns>
        public static Inventory AddItem(this Inventory inventory, IEnumerable<ItemQuantity> itemQuantityes)
        {
            List<GameItem> itemsToAdd = new List<GameItem>();

            foreach (ItemQuantity itemQuantity in itemQuantityes)
            {
                for (int i = 0; i < itemQuantity.Quantity; i++)
                {
                    itemsToAdd.Add(ItemFactory.CreateGameItem(itemQuantity.Id));
                }
            }

            return inventory.AddItem(itemsToAdd);
        }

        /// <summary>
        /// Removes list of GameItems
        /// </summary>
        /// <returns>Whole new copy of inventory (unreasonable and costly asf)</returns>
        public static Inventory RemoveItem(this Inventory inventory, IEnumerable<GameItem> items)
        {
            return new Inventory(inventory.Items.Except(items));
        }

        /// <summary>
        /// Removes single GameItem 
        /// </summary>
        /// <returns>Whole new copy of inventory (unreasonable and costly asf)</returns>
        public static Inventory RemoveItem(this Inventory inventory, GameItem item) 
        {
            return inventory.RemoveItem(new List<GameItem> { item });
        }

        /// <summary>
        /// Removes list of ItemQuantity from inventory
        /// </summary>
        /// <returns>Whole new copy of inventory (unreasonable and costly asf)</returns>
        public static Inventory RemoveItem(this Inventory inventory,
                                         IEnumerable<ItemQuantity> itemQuantities)
        {
            // REFACTOR
            Inventory workingInventory = inventory;
            foreach (ItemQuantity itemQuantity in itemQuantities)
            {
                for (int i = 0; i < itemQuantity.Quantity; i++)
                {
                    workingInventory =
                        workingInventory
                            .RemoveItem(workingInventory
                                        .Items
                                        .First(item => item.Id == itemQuantity.Id));
                }
            }
            return workingInventory;
        }
    }
}
