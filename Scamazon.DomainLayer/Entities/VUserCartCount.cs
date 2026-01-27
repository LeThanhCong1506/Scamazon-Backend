using System;
using System.Collections.Generic;

namespace MV.DomainLayer.Entities;

public partial class VUserCartCount
{
    public int? UserId { get; set; }

    public long? CartItemCount { get; set; }
}
