﻿using Entities.Models;

namespace Entities.ViewModels.Orders
{
    public class OrderDetailResponseModel
    {
        public OrderDetailMongoDbModel data_order { get; set; }
        public OrderESModel data { get; set; }
        public Province province { get; set; }
        public District district { get; set; }
        public Ward ward { get; set; }
        public bool has_raiting { get; set; }
    }
}
