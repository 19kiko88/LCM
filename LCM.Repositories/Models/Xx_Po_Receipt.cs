﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace LCM.Repositories.Models;

public partial class Xx_Po_Receipt
{
    public string TransactionID { get; set; }

    public string PONo { get; set; }

    public int POLineNo { get; set; }

    public decimal POUnitPrice { get; set; }

    public string PartNo { get; set; }

    public DateOnly TransactionDate { get; set; }

    public DateOnly SystemUpdateDate { get; set; }

    public string Updater { get; set; }

    public int Quantity { get; set; }

    public DateTime UpdateTime { get; set; }

    public int? XxPor0001_Resell_ID { get; set; }

    public virtual XxPor0001_Resell XxPor0001_Resell { get; set; }
}