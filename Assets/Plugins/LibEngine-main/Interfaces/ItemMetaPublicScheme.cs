using LibEngine.Items;
using LibEngine.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibEngine.Meta
{
    public class ItemMetaPublicScheme : IItem, IRarity
    {
        [JsonProperty("id")]
        public int ItemKeyId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("rarity")]
        public string Rarity { get; set; }

        [JsonProperty("animation")]
        public string Animation { get; set; }

        [JsonProperty("content_ipfs")]
        public string ContentIpfs { get; set; }
    }

    public class ItemMetaPublicSchemeCollection : List<ItemMetaPublicScheme>, IRepositoryFull<ItemMetaPublicScheme>
    {
        public void AddItem(ItemMetaPublicScheme _item)
        {
            this.Add(_item);
        }

        public void AddItems(ICollection<ItemMetaPublicScheme> _items)
        {
            foreach (var item in _items)
            {
                this.Add(item);
            }
        }

        public ItemMetaPublicScheme GetItemByExpression(Func<ItemMetaPublicScheme, bool> _expressionPredicate)
        {
            return this.FirstOrDefault(_expressionPredicate);
        }

        public ICollection<ItemMetaPublicScheme> GetItems()
        {
            return this;
        }

        public ICollection<ItemMetaPublicScheme> GetItemsByExpression(Func<ItemMetaPublicScheme, bool> _expressionPredicate)
        {
            return this.Where(_expressionPredicate).ToList();
        }

        public void RemoveItem(ItemMetaPublicScheme _item)
        {
            this.Remove(_item);
        }
    }
}