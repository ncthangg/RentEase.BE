﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RentEase.Data.Models;

public partial class AccountVerification
{
    public int Id { get; set; }

    public string AccountId { get; set; }

    public string VerificationCode { get; set; }

    public bool? IsUsed { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Account Account { get; set; }
}