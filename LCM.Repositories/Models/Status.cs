﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace LCM.Repositories.Models;

public partial class Status
{
    public int ID { get; set; }

    public string Description { get; set; }

    public virtual ICollection<PnPrice> PnPrice { get; set; } = new List<PnPrice>();
}