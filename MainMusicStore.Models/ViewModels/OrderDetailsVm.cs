using System.Collections.Generic;
using MainMusicStore.Models.DbModels;

namespace MainMusicStore.Models.ViewModels
{
    public class OrderDetailsVm
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}
